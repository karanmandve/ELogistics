import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  ReactiveFormsModule,
  ValidationErrors,
} from '@angular/forms';
import { UserServiceService } from '../../services/user/user-service.service';
import { PaymentService } from '../../services/razorpay-payment/payment.service';
import { AppointmentService } from '../../services/appointment/appointment.service';
import { SweetAlertToasterService } from '../../services/toaster/sweet-alert-toaster.service';
import { RouterTestingHarness } from '@angular/router/testing';
import { Router } from '@angular/router';

@Component({
  selector: 'app-appointment',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './appointment.component.html',
  styleUrl: './appointment.component.css',
})
export class AppointmentComponent {
  
  todayDate = new Date().toISOString().split('T')[0];
  maxDate: any;
  minTime: any;
  sixMonthsLater = new Date();
  allSpecialisations: any[] = [];
  allDistributors: any[] = [];
  allCustomer: any[] = [];
  fees = 0;
  userDetails: any;
  allAppointments: any[] = [];
  counter: any = 1;
  timer: any;
  isTimeValid = true; 
  isTimeInRange = true; 
  isTimeValidForDistributor = true; 
  isTimeInRangeForDistributor = true; 

  userService = inject(UserServiceService);
  paymentService = inject(PaymentService);
  appointmentService = inject(AppointmentService);
  toaster = inject(SweetAlertToasterService);
  router = inject(Router);
  changeDetector = inject(ChangeDetectorRef);

  ngOnInit() {
    this.getAllSpecialization();
    this.userService.user$.subscribe((user: any) => {
      if (user !== null) {
        this.userDetails = user;
      }
    });


    this.sixMonthsLater.setMonth(this.sixMonthsLater.getMonth() + 6);
    this.maxDate = this.sixMonthsLater.toISOString().split('T')[0];

    //     this.updateMinTime();

    // // Watch for changes in the date field to update the minTime dynamically
    // this.customerAppointmentForm.get('date')?.valueChanges.subscribe(() => {
    //   this.updateMinTime();
    // });
  }

  onChangeTime(event: Event): void {
    const selectedTime = (event.target as HTMLInputElement).value;
    const currentTime = new Date();
    
    // Get the time 1 hour later
    const minAllowedTime = new Date(currentTime.getTime() + 60 * 60 * 1000); // 1 hour from now
    const formattedMinTime = this.formatTime(minAllowedTime);
    
    // Max time allowed is 20:00
    const maxTime = '20:00';
    // Min time allowed is 08:00
    const minTime = '08:00';
  
    // Validate if the selected time is within the valid range
    if (selectedTime < minTime || selectedTime > maxTime) {
      this.isTimeInRange = false;
    } else {
      this.isTimeInRange = true;
    }
  
    // Check if selected time is at least 1 hour after the current time
    if (selectedTime < formattedMinTime) {
      this.isTimeValid = false;
      console.log("Time must be at least 1 hour later than the current time.");
    } else {
      this.isTimeValid = true;
      console.log("Time is valid.");
    }
  }
  formatTime(date: Date): string {
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  }

  // Function to handle the input change and validate date
  onChangeDate(): void {
    const time = this.customerAppointmentForm.get('time')?.value;
    if (time) {
      const date = this.customerAppointmentForm.get('date')?.value;
      // Ensure date is parsed correctly
      const selectedDate = date ? new Date(date) : new Date();
      const today = new Date();
  
      // Normalize both the selected date and today's date
      selectedDate.setHours(0, 0, 0, 0);
      today.setHours(0, 0, 0, 0);
  
      if (selectedDate.getTime() === today.getTime()) {
        const selectedTime = time;
  
        // Get current time and add one hour
        const now = new Date();
        now.setHours(now.getHours() + 1);
        const nextHour = now.getHours().toString().padStart(2, '0');
        const nextMinutes = now.getMinutes().toString().padStart(2, '0');
        const nextTime = `${nextHour}:${nextMinutes}`;
  
        // Convert selected time and next time to Date objects for comparison
        const selectedDateTime = new Date(`${today.toDateString()} ${selectedTime}`);
        const nextDateTime = new Date(`${today.toDateString()} ${nextTime}`);
  
        // Validate the selected time (should be at least 1 hour after current time)
        if (selectedDateTime < nextDateTime) {
          this.isTimeValid = false;
          console.log("Selected time is less than the next hour time");
        } else {
          this.isTimeValid = true;
          console.log("Selected time is valid");
        }
      }
    }
  }



  onChangeTimeForDistributor(event: Event): void {
    const selectedTime = (event.target as HTMLInputElement).value;
    const currentTime = new Date();
    
    // Get the time 1 hour later
    const minAllowedTime = new Date(currentTime.getTime() + 60 * 60 * 1000); // 1 hour from now
    const formattedMinTime = this.formatTime(minAllowedTime);
    
    // Max time allowed is 20:00
    const maxTime = '20:00';
    // Min time allowed is 08:00
    const minTime = '08:00';
  
    // Validate if the selected time is within the valid range
    if (selectedTime < minTime || selectedTime > maxTime) {
      this.isTimeInRange = false;
    } else {
      this.isTimeInRange = true;
    }
  
    // Check if selected time is at least 1 hour after the current time
    if (selectedTime < formattedMinTime) {
      this.isTimeValid = false;
      console.log("Time must be at least 1 hour later than the current time.");
    } else {
      this.isTimeValid = true;
      console.log("Time is valid.");
    }
  }

  // Function to handle the input change and validate date
  onChangeDateForDistributor(): void {
    const time = this.distributorAppointmentForm.get('time')?.value;
    if (time) {
      const date = this.distributorAppointmentForm.get('date')?.value;
      // Ensure date is parsed correctly
      const selectedDate = date ? new Date(date) : new Date();
      const today = new Date();
  
      // Normalize both the selected date and today's date
      selectedDate.setHours(0, 0, 0, 0);
      today.setHours(0, 0, 0, 0);
  
      if (selectedDate.getTime() === today.getTime()) {
        const selectedTime = time;
  
        // Get current time and add one hour
        const now = new Date();
        now.setHours(now.getHours() + 1);
        const nextHour = now.getHours().toString().padStart(2, '0');
        const nextMinutes = now.getMinutes().toString().padStart(2, '0');
        const nextTime = `${nextHour}:${nextMinutes}`;
  
        // Convert selected time and next time to Date objects for comparison
        const selectedDateTime = new Date(`${today.toDateString()} ${selectedTime}`);
        const nextDateTime = new Date(`${today.toDateString()} ${nextTime}`);
  
        // Validate the selected time (should be at least 1 hour after current time)
        if (selectedDateTime < nextDateTime) {
          this.isTimeValidForDistributor = false;
          console.log("Selected time is less than the next hour time");
        } else {
          this.isTimeValidForDistributor = true;
          console.log("Selected time is valid");
        }
      }
    }
  }





  // updateMinTime(): void {
  //   const today = new Date();
  //   const selectedDate = this.customerAppointmentForm.get('date')?.value;
  //   const formattedDate = selectedDate ? new Date(selectedDate).toLocaleDateString() : null;
  //   const todayDateString = today.toLocaleDateString();

  //   if (formattedDate === todayDateString) {
  //     // If selected date is today, set minTime to 1 hour after the current time
  //     this.minTime = this.formatTime(new Date(today.getTime() + 60 * 60 * 1000)); // Adding 1 hour
  //   } else {
  //     // If selected date is not today, set minTime to 08:00
  //     this.minTime = '08:00';
  //   }
  //   this.changeDetector.detectChanges();
  // }

  // formatTime(date: Date): string {
  //   const hours = date.getHours().toString().padStart(2, '0');
  //   const minutes = date.getMinutes().toString().padStart(2, '0');
  //   return `${hours}:${minutes}`;
  // }



  // timeRangeValidator(control: any): { [key: string]: boolean } | null {
  //   const time = control.value;
  //   if (!time) return null;

  //   const minTime = '08:00';
  //   const maxTime = '20:00';

  //   // Compare selected time with min and max time
  //   if (time < minTime || time > maxTime) {
  //     return { 'invalidTime': true }; // Error if time is outside the range
  //   }
  //   return null; // Valid time
  // }

  customerAppointmentForm = new FormGroup({
    specialisation: new FormControl('', Validators.required),
    distributor: new FormControl('', Validators.required),
    date: new FormControl('', [Validators.required]),
    time: new FormControl('', [Validators.required]),
    chiefComplaint: new FormControl('', [
      Validators.required,
      Validators.minLength(5),
    ]),
  });

  distributorAppointmentForm = new FormGroup({
    customer: new FormControl('', Validators.required),
    specialisation: new FormControl('', Validators.required),
    distributor: new FormControl('', Validators.required),
    date: new FormControl('', [Validators.required]),
    time: new FormControl('', [Validators.required]),
    chiefComplaint: new FormControl('', [
      Validators.required,
      Validators.minLength(5),
    ]),
  });

  // futureDateValidator(control: FormControl): ValidationErrors | null {
  //   const today = new Date();
  //   const selectedDate = new Date(control.value);

  //   // Reset time to midnight for accurate comparison
  //   today.setHours(0, 0, 0, 0);
  //   selectedDate.setHours(0, 0, 0, 0);

  //   if (selectedDate < today) {
  //     return { pastDate: true }; // Return error if the date is in the past
  //   }

  //   return null; // Return null if the date is valid (today or future)
  // }

  onPay() {
    this.paymentService.createOrder(this.fees).subscribe((order: any) => {
      console.log(order.id, order.amount);

      const options = {
        key: 'rzp_test_j1n3HfglIVc3GS',
        amount: order.amount,
        currency: order.currency,
        order_id: order.id,
        handler: (response: any) => {
          this.bookAppointment(response.razorpay_payment_id, order.id);
          // this.verifyPayment(response.razorpay_payment_id, order.id)
        },
      };
      if (window.Razorpay) {
        const razorpay = new Razorpay(options);
        razorpay.open();
      } else {
        console.error('Razorpay SDK is not loaded properly');
      }
    });
  }

  verifyPayment(paymentId: any, orderId: any) {
    this.paymentService.verifyPayment(paymentId, orderId).subscribe({
      next: (res: any) => {
        console.log(res);
      },
      error: (error: any) => {
        console.log(error);
      },
    });
  }

  openWaitingPopup() {
    const modal = document.getElementById('waitingModal');
    if (modal) {
      modal.classList.add('show');
      modal.style.display = 'block';
    }

    this.timer = setInterval(() => {
      this.counter = this.counter + 1;
      this.changeDetector.detectChanges();
    }, 1000);
  }

  async bookAppointment(paymentId: any, orderId: any) {
    this.openWaitingPopup();

    const appointmentData = this.customerAppointmentForm.value;

    const data = {
      UserTypeId: 2,
      customerId: this.userDetails.id,
      distributorId: appointmentData.distributor,
      specialisationId: appointmentData.specialisation,
      appointmentDate: appointmentData.date,
      AppointmentTime: appointmentData.time,
      chiefComplaint: appointmentData.chiefComplaint,
      fee: this.fees,
      paymentId: paymentId,
      orderId: orderId,
    };

    this.appointmentService.addAppointment(data).subscribe({
      next: (res: any) => {
        this.router.navigateByUrl('/home');
        this.toaster.success('Appointment Booked Successfully');
        // console.log(res);
        setInterval(() => {
          window.location.reload();
        }, 1000);
      },
      error: (error: any) => {
        this.toaster.error('Server Error Occur');
        // console.log(error);
      },
    });
  }

  async bookAppointmentByDistributor() {
    const appointmentData = this.distributorAppointmentForm.value;

    const data = {
      UserTypeId: 1,
      customerId: appointmentData.customer,
      distributorId: this.userDetails.id,
      specialisationId: appointmentData.specialisation,
      appointmentDate: appointmentData.date,
      AppointmentTime: appointmentData.time,
      chiefComplaint: appointmentData.chiefComplaint,
      fee: this.fees,
      paymentId: '0',
      orderId: '0',
    };

    this.appointmentService.addAppointment(data).subscribe({
      next: (res: any) => {
        this.router.navigateByUrl('/home');
        this.toaster.success('Appointment Booked Successfully');
        // console.log(res);
        // setInterval(() => {
        //   window.location.reload();
        // }, 1000);
      },
      error: (error: any) => {
        this.toaster.error('Server Error Occur');
        // console.log(error);
      },
    });
  }

  getAllCustomer() {
    this.userService.getAllCustomer().subscribe({
      next: (res: any) => {
        this.allCustomer = res.data;
      },
      error: (err: any) => {
        this.toaster.error('Server Error Occured');
      },
    });
  }

  getAllSpecialization() {
    this.userService.getSpecialization().subscribe({
      next: (res: any) => {
        this.allSpecialisations = res.data;
      },
      error: (error: any) => {
        console.log(error);
      },
    });
  }

  onChange(specialisationId: any) {
    this.userService.getDistributorBySpecialization(specialisationId).subscribe({
      next: (res: any) => {
        this.allDistributors = res.data;
      },
      error: (error: any) => {
        console.log(error);
      },
    });
  }

  onChangeDistributor(distributorId: any) {
    this.allDistributors.map((distributor: any) => {
      if (distributor.id == distributorId) {
        console.log(distributor.visitingCharge);

        this.fees = distributor.visitingCharge;
      }
    });
  }
}
