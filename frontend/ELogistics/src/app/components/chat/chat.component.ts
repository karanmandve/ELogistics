import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { UserServiceService } from '../../services/user/user-service.service';
import { getDatabase, ref, set, onValue, child, push } from 'firebase/database';
import { initializeApp } from 'firebase/app';
import { firebaseConfig } from '../../../environments/environment';

const app = initializeApp(firebaseConfig);
const db = getDatabase(app);

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent {


  senderId: string = '';
  receiverId: string = '';
  message: string = '';
  messages: any[] = [];
  userData: any;

  constructor(private route: ActivatedRoute) {}
  userServive = inject(UserServiceService)

  ngOnInit(): void {
    this.userServive.user$.subscribe((user: any) => {
      if (user !== null) {
        this.userData = user;
        this.senderId = this.userData.id;
        
      }
    })

    // Fetching receiverId from the route parameters
    // If the user is a customer, receiverId will be distributorId, otherwise it will be customerId for distributor
    if (this.userData.userTypeId === 1) { // If user is a customer (assuming 1 means customer)
      this.receiverId = this.route.snapshot.paramMap.get('customerId')!;
    } else { // If user is a distributor
      this.receiverId = this.route.snapshot.paramMap.get('distributorId')!;
    }

    console.log('Sender ID: ', this.senderId);
    console.log('Receiver ID: ', this.receiverId);

    // Initialize chat session by listening for messages
    this.listenForMessages(this.senderId, this.receiverId);
  }




  // Send message to both sender-receiver and receiver-sender paths
  sendMessage(receiverId: string, message: string): void {
    const messageId = push(ref(db, `messages/${this.senderId}_${receiverId}`)).key;
    if (messageId) {
      const senderName = `${this.userData.firstName} ${this.userData.lastName}`; // Full name of the sender
    const messageObj = {
      senderId: this.senderId,
      senderName: senderName,  // Add sender full name
      receiverId: receiverId,
      message: message,
      timestamp: Date.now(),
    };
      const messageRefSenderReceiver = ref(db, `messages/${this.senderId}_${receiverId}/${messageId}`);
      set(messageRefSenderReceiver, messageObj);

      const messageRefReceiverSender = ref(db, `messages/${receiverId}_${this.senderId}/${messageId}`);
      set(messageRefReceiverSender, messageObj);
    }
    this.message = '';
  }

  // Listen for messages from both sender-receiver and receiver-sender paths
  listenForMessages(senderId: string, receiverId: string): void {
    const messagesRefSenderReceiver = ref(db, `messages/${senderId}_${receiverId}`);
    const messagesRefReceiverSender = ref(db, `messages/${receiverId}_${senderId}`);
    
    // Listen for messages from the sender-receiver path
    onValue(messagesRefSenderReceiver, (snapshot) => {
      const data = snapshot.val();
      if (data) {
        // Combine the messages and sort them by timestamp
        this.messages = Object.values(data).sort(
          (a: any, b: any) => a.timestamp - b.timestamp
        );
        console.log('Messages (Sender -> Receiver): ', this.messages);
      }
    });

    // Listen for messages from the receiver-sender path
    onValue(messagesRefReceiverSender, (snapshot) => {
      const data = snapshot.val();
      if (data) {
        // Combine the messages and sort them by timestamp
        this.messages = Object.values(data).sort(
          (a: any, b: any) => a.timestamp - b.timestamp
        );
        console.log('Messages (Receiver -> Sender): ', this.messages);
      }
    });
  }
  

}
