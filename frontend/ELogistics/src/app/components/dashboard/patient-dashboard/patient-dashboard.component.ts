import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { UserServiceService } from '../../../services/user/user-service.service';
import { AppointmentService } from '../../../services/appointment/appointment.service';
import { CommonModule } from '@angular/common';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';
import { SweetAlertToasterService } from '../../../services/toaster/sweet-alert-toaster.service';
import Swal from 'sweetalert2';
declare var bootstrap: any;
@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './customer-dashboard.component.html',
  styleUrl: './customer-dashboard.component.css'
})
export class CustomerDashboardComponent {

  allAppointments: any[] = [];
  filterAppointments: any[] = [];
  userDetails: any;
  sixMonthsLater = new Date();
  maxDate: any;
  todayDate = new Date().toISOString().split('T')[0];
  minTime: any;
  isTimeValid = true; 
  isTimeInRange = true; 


  appointmentService = inject(AppointmentService);
  userService = inject(UserServiceService);
  toaster  = inject(SweetAlertToasterService);


  ngOnInit() {
    this.userService.user$.subscribe((user: any) => {
      if (user !== null){
        this.userDetails = user;
        this.getAllAppointments();
      }
    });

    this.sixMonthsLater.setMonth(this.sixMonthsLater.getMonth() + 6);
    this.maxDate = this.sixMonthsLater.toISOString().split('T')[0];
  }

  appointmentForm = new FormGroup({
    id : new FormControl(''),
    appointmentDate : new FormControl(''),
    appointmentTime : new FormControl(''),
    chiefComplaint : new FormControl(''),
});



  getAllAppointments() {
    this.appointmentService.getAppointmentsByCustomerId(this.userDetails.id).subscribe({
      next: (res: any) => {
        this.allAppointments = res.data
        this.filterAppointments = this.allAppointments.filter((appointment: any) => appointment.appointmentStatus === "Scheduled");
      },
      error: (error: any) => {
        console.log(error);
      }
    })
  }

  openModal(): void {
    const modalElement = document.getElementById('appointmentModal') as HTMLElement;
    const modal = new bootstrap.Modal(modalElement); 
    modal.show(); 
  }

  closeModal(): void {
    const modalElement = document.getElementById('appointmentModal') as HTMLElement;
    const modal = bootstrap.Modal.getInstance(modalElement); 
    modal.hide(); 
  }

  onEditAppointment(appointment: any) {
    this.openModal();
    this.appointmentForm.patchValue(appointment);
  }


  confirmDeletion(appointment: any): void {
    Swal.fire({
      title: 'Are you sure?',
      text: 'You will not be able to recover this item!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, cancel it!',
      cancelButtonText: 'No!',
    }).then((result) => {
      if (result.isConfirmed) {
        // Perform the delete operation
        this.onCancelAppointment(appointment);
        
      } else if (result.dismiss === Swal.DismissReason.cancel) {
        // Swal.fire('Cancelled', 'Your item is safe.', 'info');
      }
    });
  }
  
  onCancelAppointment(appointment: any) {
    this.appointmentService.cancelAppointment(appointment.id).subscribe({
      next: (res: any) => {
        this.getAllAppointments();
        this.toaster.success('Appointment cancelled successfully');
      },
      error: (error: any) => {
        console.log(error);
      }
    })
  }

  onSubmit() {

    this.appointmentService.updateAppointment(this.appointmentForm.value).subscribe({
      next: (res: any) => {
        this.closeModal();
        this.getAllAppointments();
        this.toaster.success('Appointment updated successfully');
      },
      error: (error: any) => {
        this.toaster.success('Server Error');
        console.log(error);
      }
    })
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
    const time = this.appointmentForm.get('time')?.value;
    if (time) {
      const date = this.appointmentForm.get('date')?.value;
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


}
