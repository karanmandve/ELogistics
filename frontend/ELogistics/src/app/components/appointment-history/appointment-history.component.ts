import { Component, inject } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';
import { UserServiceService } from '../../services/user/user-service.service';
import { AppointmentService } from '../../services/appointment/appointment.service';
import { CommonModule } from '@angular/common';
declare const bootstrap: any;

@Component({
  selector: 'app-appointment-history',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './appointment-history.component.html',
  styleUrl: './appointment-history.component.css'
})
export class AppointmentHistoryComponent {

  allAppointments: any[] = [];
  filterAppointments: any[] = [];
  userDetails: any;
  soapNoteData: any;

  userService = inject(UserServiceService);
  appointmentService = inject(AppointmentService);


  ngOnInit() {
    this.userService.user$.subscribe((user: any) => {
      if (user !== null){
        this.userDetails = user;
        this.getAllAppointments();
      }
    });
  }


  appointmentStatusform = new FormGroup({
    appointmentStatus: new FormControl(''),
  });
  
  appointmentStatusFiltered(appointmentStatus: any){
  
    if (appointmentStatus === '') {
      this.filterAppointments = this.allAppointments.filter((appointment: any) => appointment.appointmentStatus === "Cancelled" || appointment.appointmentStatus === "Completed");
    } else {
      this.filterAppointments = this.allAppointments.filter((appointment: any) => appointment.appointmentStatus === appointmentStatus);
    }
  }

  getAllAppointments() {

    if (this.userDetails.userTypeId == 1){
            
          this.appointmentService.getAppointmentsByDistributorId(this.userDetails.id).subscribe({
            next: (res: any) => {
              this.allAppointments = res.data

              this.allAppointments.sort((a: any, b: any) => {
                const dateA = new Date(`${a.appointmentDate}T${a.appointmentTime}`);
                const dateB = new Date(`${b.appointmentDate}T${b.appointmentTime}`);
                return dateB.getTime() - dateA.getTime(); // Latest first
              });

              this.filterAppointments = this.allAppointments.filter((appointment: any) => appointment.appointmentStatus === "Cancelled" || appointment.appointmentStatus === "Completed");
            },
            error: (error: any) => {
              console.log(error);
            }
          })
    }else{
      
          this.appointmentService.getAppointmentsByCustomerId(this.userDetails.id).subscribe({
            next: (res: any) => {
              this.allAppointments = res.data
              this.filterAppointments = this.allAppointments.filter((appointment: any) => appointment.appointmentStatus === "Cancelled" || appointment.appointmentStatus === "Completed");
            },
            error: (error: any) => {
              console.log(error);
            }
          })
        }
    }


    viewSoapNote(appointmentId: any) {
      this.appointmentService.getSoapNote(appointmentId).subscribe({
        next: (res: any) => {
          this.soapNoteData = res.data;
    
          // Show the modal after data is fetched
          const modalElement = document.getElementById('soapNoteModal');
          if (modalElement) {
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
          }
        },
        error: (error: any) => {
          console.error('Error fetching SOAP note:', error);
        },
      });
    }

}
