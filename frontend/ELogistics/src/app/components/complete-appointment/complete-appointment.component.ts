import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppointmentService } from '../../services/appointment/appointment.service';
import { UserServiceService } from '../../services/user/user-service.service';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SweetAlertToasterService } from '../../services/toaster/sweet-alert-toaster.service';

@Component({
  selector: 'app-complete-appointment',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './complete-appointment.component.html',
  styleUrl: './complete-appointment.component.css'
})
export class CompleteAppointmentComponent {

  appointmentId: any;
  userDetails: any;
  allAppointments: any[] = [];
  appointment: any;

  route = inject(ActivatedRoute)
  router = inject(Router)
  appointmentService = inject(AppointmentService);
  userService = inject(UserServiceService);
  toaster = inject(SweetAlertToasterService);
  
  getAllAppointments(distributorId: any) {
  
    this.appointmentService.getAppointmentsByDistributorId(distributorId).subscribe({
      next: (res: any) => {
        this.allAppointments = res.data
        this.appointmentId = this.route.snapshot.paramMap.get('appointmentId')!;
        this.appointment = this.allAppointments.filter((appointment: any) => appointment.id == this
        .appointmentId)[0];
        // console.log(this.filterAppointment[0]);
        
      },
      error: (error: any) => {
        console.log(error);
      }
    })
  
  }



  ngOnInit(): void {
    this.userService.user$.subscribe((user: any) => {
      if (user !== null){
        this.userDetails = user;
        this.getAllAppointments(this.userDetails.id);
      }
    });
  }


  soapForm = new FormGroup({
    appointmentId: new FormControl(''),
    subjective: new FormControl('', [Validators.required, Validators.minLength(10), Validators.maxLength(400)]),
    objective: new FormControl('', [Validators.required, Validators.minLength(10), Validators.maxLength(400)]),
    assessment: new FormControl('', [Validators.required, Validators.minLength(10), Validators.maxLength(400)]),
    plan: new FormControl('', [Validators.required, Validators.minLength(10), Validators.maxLength(400)]),
  });


  onSubmit() {
    var soapNoteData = this.soapForm.value;
    soapNoteData.appointmentId = this.appointmentId;
    this.appointmentService.addSoapNotes(soapNoteData).subscribe({
      next: (res: any) => {
        this.toaster.success('SOAP Note added successfully');
        this.router.navigateByUrl("/home");
      },
      error: (error: any) => {
        this.toaster.error('SOAP Note not added');
      }});
    }


}

