import { CommonModule } from '@angular/common';
import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { ToastContainerDirective, ToastrService } from 'ngx-toastr';
import { Router, RouterLink } from '@angular/router';
import { UserServiceService } from '../../../services/user/user-service.service';
import { FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { LoaderService } from '../../../services/loader/loader.service';
import { CountryStateServiceService } from '../../../services/country-state-service/country-state-service.service';
import { SweetAlertToasterService } from '../../../services/toaster/sweet-alert-toaster.service';
import { DistributorRegistrationComponent } from "../distributor-registration/distributor-registration.component";
import { CustomerRegistrationComponent } from "../customer-registration/customer-registration.component";
declare var bootstrap: any;

@Component({
  selector: 'app-register-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CustomerRegistrationComponent, DistributorRegistrationComponent],
  templateUrl: './register-login.component.html',
  styleUrl: './register-login.component.css'
})
export class RegisterLoginComponent {

  password: string = '';
  confirmPassword: string = '';
  loginData : any;
  registerData: any;
  otpSent: boolean = false;
  forgetOtpSent: boolean = false;
  passwordRgx: RegExp = /^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[!@#$%^&*]).{8,}$/
  isResendDisabled: boolean = false;  // To control the resend button
  forgetOtpIsResendDisabled: boolean = false;  // To control the resend button
  forgetOtpCountdown: number = 30; // Countdown timer for resend OTP
  countdown: number = 30; // Countdown timer for resend OTP
  resendTimeout: any;
  resendOtpTimeout: any;
  todayDate = new Date().toISOString().split('T')[0];
  pastDate = new Date('1901-01-01').toISOString().split('T')[0];
  fileSizeError = false;
  imagePreview: string | ArrayBuffer | null = null;
  isLogin: boolean = true;
  distributorRegistration: boolean = false;
  customerRegistration: boolean = false;
  activeForm: string = 'login';
  allSpecialisations: any[] = [];

  @ViewChild('forgotPasswordModal') forgotPasswordModal!: ElementRef;
  
  userService = inject(UserServiceService)
  router = inject(Router)
  toaster = inject(SweetAlertToasterService)
  loader = inject(LoaderService)
  countryStateService: any = inject(CountryStateServiceService)

  ngonInit() {
  }



  setActiveForm(form: string): void {
    if (form === 'login') {
      // this.loginForm.reset();
      this.isLogin = true;
      this.customerRegistration = false;
      this.distributorRegistration = false;
    }else if (form === 'distributorRegister') {
      this.distributorRegistration = true;
      this.isLogin = false;
      this.customerRegistration = false;
    } else if (form === 'customerRegister') {
      this.customerRegistration = true;
      this.isLogin = false;
      this.distributorRegistration = false;
    }

    this.activeForm = form;

  }


}
