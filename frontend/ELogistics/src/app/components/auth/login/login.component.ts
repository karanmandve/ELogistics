import { CommonModule } from '@angular/common';
import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SweetAlertToasterService } from '../../../services/toaster/sweet-alert-toaster.service';
import { Router } from '@angular/router';
import { UserServiceService } from '../../../services/user/user-service.service';
import { CountryStateServiceService } from '../../../services/country-state-service/country-state-service.service';
declare var bootstrap: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  loginData : any;
  allState: any;
  passwordRgx: RegExp = /^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[!@#$%^&*]).{8,}$/
  isResendDisabled: boolean = false;  // To control the resend button
  forgetOtpIsResendDisabled: boolean = false;  // To control the resend button
  forgetOtpCountdown: number = 30; // Countdown timer for resend OTP
  countdown: number = 30; // Countdown timer for resend OTP
  resendTimeout: any;
  resendOtpTimeout: any;
  otpSent: boolean = false;
  forgetOtpSent: boolean = false;
  isOtpModalOpen: boolean = false;


  toaster = inject(SweetAlertToasterService)
  router = inject(Router)
  userService = inject(UserServiceService)
  countryStateService = inject(CountryStateServiceService)


  @ViewChild('forgotPasswordModal') forgotPasswordModal!: ElementRef;



 loginForm: FormGroup = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    password: new FormControl('', [Validators.required, Validators.pattern(this.passwordRgx)]),
    
  });

  forgetPasswordForm = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    otp: new FormControl('', [Validators.required, Validators.minLength(6), Validators.maxLength(6)])
  })

  otpForm = new FormGroup({
    otp: new FormControl('', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]),
  });

  onKeyPress(event: KeyboardEvent) {
    const charCode = event.which ? event.which : event.keyCode;

    if (charCode < 48 || charCode > 57) {
      event.preventDefault();
    }
  }

  onLoginSubmit() {
    this.loginData = {
      ...this.loginForm.value,
      ...this.otpForm.value
    }

    this.userService.loginUser(this.loginData).subscribe({
      next: (res: any) => {
        this.loginForm.reset();
        if (res.statusCode == 200) {
          this.closeOtpModal();
          localStorage.setItem("token", res.data.token);
          localStorage.setItem("username", this.loginData.username);

          this.userService.updateUserDetails(this.loginData.username);

          const expiry = new Date(res.data.expiration)
          localStorage.setItem('expiry', expiry.toISOString());
          
          this.toaster.success("Login successful");
          localStorage.setItem("session", "true")
          this.router.navigateByUrl('/home');
          
        } else {
          console.log(res);
          this.toaster.error("Server Error Occur");
        }
      },
      error: (error: any) => {
        this.loginForm.reset(0);
        if (error.error.statusCode == 401) {
          this.toaster.error("Invalid Credential");
        } else {
          this.toaster.error("Error Occurred From Our Side");
          console.log(error);
        }
      }
    });
  }

  openForgotPasswordModal() {
    this.loginForm.reset();
    const modal = new bootstrap.Modal(this.forgotPasswordModal.nativeElement, {
      backdrop: 'static', // Prevent closing when clicking outside
      keyboard: false, // Prevent closing with Esc
    });
    modal.show();
  }

  closeForgotPasswordModal() {
    this.forgetPasswordForm.reset();
    const modalInstance = bootstrap.Modal.getInstance(this.forgotPasswordModal.nativeElement);
    modalInstance.hide();
  }


  private otpModal: any;

  // Open the OTP modal
  openOtpModal(): void {
    this.isOtpModalOpen = true;
    this.otpModal = new bootstrap.Modal(document.getElementById('otpModal'), {
      backdrop: 'static', // Prevent closing when clicking outside
      keyboard: false, // Prevent closing with Esc
    });
    this.otpModal.show();
  }

  // Close the OTP modal
  closeOtpModal(): void {
    this.otpForm.reset();
    this.isOtpModalOpen = false;
    if (this.otpModal) {
      this.otpModal.hide();
    }
  }

  sendOtpWithPasswordCheck() {
    // this.loader.show();

    
    var data = {
      username: this.loginForm.get('username')?.value,
      password: this.loginForm.get('password')?.value
    }

    this.startResendCountdown();
    
    this.userService.sendOtpWithPasswordCheck(data).subscribe({
      next : (res: any) => {
        if(res.statusCode == 200){
          if (!this.isOtpModalOpen){
            this.openOtpModal();
          }
          // this.loader.hide();
          this.otpSent = true;  // Mark OTP as sent
          console.log("OTP Sent!");
          this.toaster.success("Send Otp Successfully")
        }else{
          this.toaster.error("Otp Not Send")
        }
      },
      error : (error: any) => {
        if(error.error.statusCode == 404){
          this.toaster.error("Invalid Credential, Otp Not Send")
        }else{
          this.toaster.error("Some Error Occurred, Otp Not Send")
          console.log(error);
          
          // alert("i am in register error")
          // alert(JSON.stringify(error))
        }
      }
    })
  }

  sendOtp() {
    // this.loader.show();
    
    const username = this.loginForm.get('username')?.value;
    // alert("Otp send take time 6 second. Don't click again")
    this.startResendCountdown();
    
    this.userService.sendOtpWithPasswordCheck(username).subscribe({
      next : (res: any) => {
        if(res.statusCode == 200){
          // this.loader.hide();
          this.otpSent = true;  // Mark OTP as sent
          console.log("OTP Sent!");
          this.toaster.success("Send Otp Successfully")
        }else{
          this.toaster.error("Otp Not Send")
        }
      },
      error : (error: any) => {
        if(error.error.statusCode == 404){
          this.toaster.error("Invalid Credential, Otp Not Send")
        }else{
          this.toaster.error("Some Error Occurred, Otp Not Send")
          console.log(error);
          
          // alert("i am in register error")
          // alert(JSON.stringify(error))
        }
      }
    })
  }

sendForgetOtp() {
  // this.loader.show();
  const username = this.forgetPasswordForm.get('username')?.value;
  console.log(username);
  console.log(typeof(username));
  
  // alert("Otp send take time 6 second. Don't click again")
  this.startForgetOtpResendCountdown();
  
  this.userService.sendOtpWithPasswordCheck(username).subscribe({
    next : (res: any) => {
      if(res.statusCode == 200){
        // this.loader.hide();
        this.forgetOtpSent = true;  // Mark OTP as sent
        console.log("OTP Sent!");
        this.toaster.success("Send Otp Successfully")
      }else{
        this.toaster.error("Otp Not Send")
      }
    },
    error : (error: any) => {
      if(error.error.statusCode == 404){
        this.toaster.error("Invalid Credential, Otp Not Send")
      }else{
        this.toaster.error("Some Error Occurs, Otp Not Send")
        console.log(error);
        

        // alert("i am in register error")
        // alert(JSON.stringify(error))
      }
    }
  })
}

  // resetPassword() {
  //   const username = this.forgetPasswordForm.get('username')?.value;
  //   const otp = this.forgetPasswordForm.get('otp')?.value;

  //   this.userService.forgotPassword(username, otp).subscribe({
  //     next : (res: any) => {
  //       this.forgetPasswordForm.reset();
  //       if(res.statusCode == 200){
  //         this.toaster.success("New Password Send To Your Registered Email")
  //         this.closeForgotPasswordModal();
  //       }else{
  //         this.toaster.error("Password Not Send")
  //       }
  //     },
  //     error : (error: any) => {
  //       this.forgetPasswordForm.reset();
  //       if(error.error.statusCode == 401){
  //         this.toaster.error("Invalid Credential, Password Not Send")
  //       }else{
  //         this.toaster.error("Password Not Send, Error Occur")
  //         console.log(error);
          
  //         // alert(JSON.stringify(error))
  //     }
  // }})
  // }

    // Start countdown for resend OTP button
    startResendCountdown() {
      this.isResendDisabled = true;  // Disable the resend button
      this.countdown = 30;  // Set initial countdown time
      this.resendTimeout = setInterval(() => {
        this.countdown--;  // Decrease the countdown by 1 each second
        if (this.countdown <= 0) {
          clearInterval(this.resendTimeout);  // Stop the countdown when it reaches 0
          this.isResendDisabled = false;  // Enable the resend button again
        }
      }, 1000);  // Run every second
    }

    startForgetOtpResendCountdown() {
      this.forgetOtpIsResendDisabled = true;  // Disable the resend button
      this.forgetOtpCountdown = 30;  // Set initial countdown time
      this.resendOtpTimeout = setInterval(() => {
        this.forgetOtpCountdown--;  // Decrease the countdown by 1 each second
        if (this.forgetOtpCountdown <= 0) {
          clearInterval(this.resendOtpTimeout);  // Stop the countdown when it reaches 0
          this.forgetOtpIsResendDisabled = false;  // Enable the resend button again
        }
      }, 1000);  // Run every second
    }


  // getState(countrId : any, stateId: any){
  //   this.countryStateService.getStateByCountryId(countrId).subscribe({
  //     next : (res:any) => {
  //       this.allState = res.data
  //       const state = this.allState.find((s: any) => s.stateId === stateId);
  //       localStorage.setItem("stateName", state.name)

  //     },
  //     error : (error: any) => {
  //       console.log(error);
        
  //       // alert("I am in error")
  //     }
  //   })
  // }

}
