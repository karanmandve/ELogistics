import { Component, inject } from '@angular/core';
import { AppointmentService } from '../../../services/appointment/appointment.service';
import { UserServiceService } from '../../../services/user/user-service.service';
import { CommonModule } from '@angular/common';
import { SweetAlertToasterService } from '../../../services/toaster/sweet-alert-toaster.service';
import Swal from 'sweetalert2';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-distributor-dashboard',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule, RouterLink],
  templateUrl: './distributor-dashboard.component.html',
  styleUrl: './distributor-dashboard.component.css'
})
export class DistributorDashboardComponent {

  allAppointments: any[] = [];
  filterAppointments: any[] = [];
  userDetails: any;


  appointmentService = inject(AppointmentService);
  userService = inject(UserServiceService);
  toaster = inject(SweetAlertToasterService);


  ngOnInit() {
    this.userService.user$.subscribe((user: any) => {
      if (user !== null){
        this.userDetails = user;
        this.getAllAppointments();
      }
    });
  }



  getAllAppointments() {
    this.appointmentService.getAppointmentsByDistributorId(this.userDetails.id).subscribe({
      next: (res: any) => {
        this.allAppointments = res.data
        this.filterAppointments = this.allAppointments.filter((appointment: any) => appointment.appointmentStatus === "Scheduled");
        // check for empty array
      },
      error: (error: any) => {
        console.log(error);
      }
    })
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
}
