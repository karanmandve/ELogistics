import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CountryStateServiceService } from '../../../services/country-state-service/country-state-service.service';
import { UserServiceService } from '../../../services/user/user-service.service';
import { CommonModule, DatePipe, TitleCasePipe } from '@angular/common';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { SweetAlertToasterService } from '../../../services/toaster/sweet-alert-toaster.service';
declare var bootstrap: any;

@Component({
  selector: 'app-distributor-profile',
  standalone: true,
  imports: [TitleCasePipe, DatePipe, CommonModule, ReactiveFormsModule],
  templateUrl: './distributor-profile.component.html',
  styleUrl: './distributor-profile.component.css',
})
export class DistributorProfileComponent {
  userDetails: any = null;
  newPasswordMismatch: boolean = false;
  isStateLoaded: boolean = false;
  passwordRgx: RegExp =
    /^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[!@#$%^&*]).{8,}$/;
  todayDate = new Date().toISOString().split('T')[0];
  pastDate = new Date('1900-01-01').toISOString().split('T')[0];
  fileSizeError = false;
  allSpecialisations: any[] = [];
  specialisationName: any;

  userService = inject(UserServiceService);
  toaster = inject(SweetAlertToasterService);
  countryStateService: any = inject(CountryStateServiceService);
  router = inject(Router);
  detectChange = inject(ChangeDetectorRef);

  updateDistributorForm = new FormGroup({
    id: new FormControl(''),
    firstName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(20),
      Validators.pattern(/^[A-Za-z]+(?: [A-Za-z]+)*\s*$/),
    ]),
    lastName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(25),
      Validators.pattern(/^[A-Za-z]+(?: [A-Za-z]+)*\s*$/),
    ]),
    mobile: new FormControl('', [
      Validators.required,
      Validators.pattern(/^\d{10}$/),
    ]),
    dateOfBirth: new FormControl('', [
      Validators.required,
      this.futureDateValidator,
    ]),
    gender: new FormControl('', Validators.required),
    bloodGroup: new FormControl('', Validators.required),
    city: new FormControl('', [Validators.required]),
    imageFile: new FormControl<File | null>(null),
    address: new FormControl('', [
      Validators.required,
      Validators.minLength(10),
      Validators.maxLength(150),
      Validators.pattern(/^[a-zA-Z0-9\s,.-]+$/),
    ]),
    pincode: new FormControl('', [
      Validators.required,
      Validators.pattern(/^\d{6}$/),
      Validators.minLength(6),
      Validators.maxLength(6),
    ]),
    countryId: new FormControl('', Validators.required),
    stateId: new FormControl('', Validators.required),
    qualification: new FormControl('', Validators.required),
    specialisationId: new FormControl('', Validators.required),
    visitingCharge: new FormControl('', Validators.required),
  });

  changePasswordForm = new FormGroup({
    newPassword: new FormControl('', [
      Validators.required,
      Validators.pattern(this.passwordRgx),
    ]),
    confirmNewPassword: new FormControl('', [Validators.required]),
  });

  ngOnInit() {
    this.sanitizeField('firstName');
    this.sanitizeField('lastName');
    this.getAllCountry();
    this.userService.user$.subscribe((user: any) => {
      if (user !== null) {
        this.userDetails = user;
        this.getAllSpecialization();
      }
    });
    this.changePasswordForm.valueChanges.subscribe(() => {
      const newPassword = this.changePasswordForm.get('newPassword')?.value;
      const confirmPassword =
        this.changePasswordForm.get('confirmNewPassword')?.value;
      this.newPasswordMismatch = newPassword !== confirmPassword;
    });
  }

  onKeyPressCity(event: KeyboardEvent) {
    const charCode = event.which ? event.which : event.keyCode;
    const charStr = String.fromCharCode(charCode);

    // Allow only alphabets (a-z, A-Z) using a regex check
    if (!/^[a-zA-Z]+$/.test(charStr)) {
      event.preventDefault();
    }
  }

  sanitizeField(fieldName: string): void {
    this.updateDistributorForm.get(fieldName)?.valueChanges.subscribe((value) => {
      if (value) {
        const sanitizedValue = value
          .replace(/[^A-Za-z\s]/g, '')
          .replace(/\s{2,}/g, ' ');
        if (value !== sanitizedValue) {
          this.updateDistributorForm.get(fieldName)?.setValue(sanitizedValue, {
            emitEvent: false,
          });
        }
      }
    });
  }

  onKeyPress(event: KeyboardEvent) {
    const charCode = event.which ? event.which : event.keyCode;

    if (charCode < 48 || charCode > 57) {
      event.preventDefault();
    }
  }

  futureDateValidator(control: FormControl): ValidationErrors | null {
    const today = new Date();
    const pastDate = new Date('1900-01-01');
    const selectedDate = new Date(control.value);

    // Reset time to the start of the day (to avoid time comparisons)
    // today.setHours(0, 0, 0, 0);

    if (selectedDate > today) {
      return { futureDate: true }; // Return error if date is in the future
    } else if (selectedDate < pastDate) {
      return { pastDate: true };
    } // Return error if date is in the future
    return null; // Return null if date is valid
  }

  onChange(countrId: any) {
    this.countryStateService.getStateByCountryId(countrId).subscribe({
      next: (res: any) => {
        this.allState = res.data;
      },
      error: (error: any) => {
        console.log(error);
        // alert("I am in error")
      },
    });
  }

  onFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    const maxSize = 5 * 1024 * 1024;

    if (file) {
      const validTypes = ['image/jpeg', 'image/jpg', 'image/png'];
      // Validate file type
      if (!validTypes.includes(file.type)) {
        this.updateDistributorForm
          .get('imageFile')
          ?.setErrors({ invalidType: true });
        if (event.target instanceof HTMLInputElement) {
          event.target.value = ''; // Clear invalid input
        }
        // return; // Exit if invalid type
      } else {
        this.updateDistributorForm.get('imageFile')?.setErrors(null);
      }

      // Validate file size
      if (file.size > maxSize) {
        this.fileSizeError = true;
        if (event.target instanceof HTMLInputElement) {
          event.target.value = ''; // Clear input if size is too large
        }
        return; // Exit if size exceeds limit
      } else {
        this.fileSizeError = false;
      }

      // Set the file value in the form
      this.updateDistributorForm.patchValue({ imageFile: file });
      this.updateDistributorForm.get('imageFile')?.updateValueAndValidity();

      // File is valid, read and preview it
      // const reader = new FileReader();
      // reader.onload = () => {
      //   this.imagePreview = reader.result as string; // Set the preview
      // };
      // reader.readAsDataURL(file);
    }
  }

  acceptCharacterOnly(event: KeyboardEvent) {
    const charCode = event.which ? event.which : event.keyCode;
    const charStr = String.fromCharCode(charCode);

    // Allow only alphabets (a-z, A-Z) using a regex check
    if (!/^[a-zA-Z]+$/.test(charStr)) {
      event.preventDefault();
    }
  }

  acceptNumberOnly(event: KeyboardEvent) {
    const charCode = event.which ? event.which : event.keyCode;

    if (charCode < 48 || charCode > 57) {
      event.preventDefault();
    }
  }

  openDistributorUpdateModal(): void {
    const modalElement = document.getElementById('distributorUpdateModal');
    const modal = new bootstrap.Modal(modalElement);
    modal.show();
  }

  closeDistributorUpdateModal(): void {
    const modalElement = document.getElementById('distributorUpdateModal');
    const modal = bootstrap.Modal.getInstance(modalElement);
    modal.hide();
  }

  openChangePasswordModal(): void {
    const modal = new bootstrap.Modal(
      document.getElementById('changePasswordModal') as Element
    );
    modal.show();
  }

  closeChangePasswordModal(): void {
    const modal = bootstrap.Modal.getInstance(
      document.getElementById('changePasswordModal') as Element
    );
    modal.hide();
  }

  onEditProfile() {
    this.openDistributorUpdateModal();
    this.updateDistributorForm.patchValue(this.userDetails);
    this.getState(this.userDetails.countryId);
  }

  onUpdateSubmit() {
    const formData = new FormData();
    Object.keys(this.updateDistributorForm.controls).forEach((field) => {
      if (field === 'id') {
        const value = this.userDetails.id;
        formData.append(field, value);
      } else {
        const value = this.updateDistributorForm.get(field)?.value;
        formData.append(field, value);
      }
    });

    this.userService.updateDistributor(formData).subscribe({
      next: (res: any) => {
        if (res.statusCode == 200) {
          this.toaster.success('Profile Updated Successfully');
          this.userService.setUserData(res.data);
          this.closeDistributorUpdateModal();
        } else {
          this.toaster.error('Profile Not Updated');
        }
      },
      error: (error: any) => {
        this.toaster.error('Error From Our Side');
        console.log(error);
      },
    });
  }

  onChangePasswordSubmit() {
    const newPassword = this.changePasswordForm.get('newPassword')?.value;
    const username = this.userDetails.username;

    const changePasswordData = {
      username: username,
      newPassword: newPassword,
    };

    this.userService.changePassword(changePasswordData).subscribe({
      next: (res: any) => {
        this.closeChangePasswordModal();
        this.toaster.success('Password Changed Successfully');
      },
      error: (error: any) => {
        this.toaster.error('Password Change Failed');
      },
    });
  }

  getAllSpecialization() {
    this.userService.getSpecialization().subscribe({
      next: (res: any) => {
        this.allSpecialisations = res.data;
        this.specialisationName = this.allSpecialisations.filter(
          (specialisation: any) => {
            if (specialisation.id == this.userDetails.id) {
              return specialisation;
            }
          }
        )[0].specialisationName;
      },
      error: (error: any) => {
        console.log(error);
      },
    });
  }

  allCountry: any[] = [];

  getAllCountry() {
    this.countryStateService.getAllCountry().subscribe({
      next: (res: any) => {
        this.allCountry = res.data;
        // console.log(this.allCountry)
        // this.detectChange.detectChanges();
      },
      error: (error: any) => {
        alert('I am in error');
      },
    });
  }

  allState: any[] = [];

  // loadState(){
  //   this.countryStateService.getAllState().subscribe({
  //     next : (res:any) => {
  //       this.allState = res
  //     },
  //     error : (error: any) => {
  //       alert("I am in error")
  //     }
  //   })
  // }

  getState(countrId: any) {
    this.countryStateService.getStateByCountryId(countrId).subscribe({
      next: (res: any) => {
        this.allState = res.data;
        console.log(this.allState);
        this.isStateLoaded = true;
      },
      error: (error: any) => {
        alert('I am in error');
      },
    });
  }

  getCountryName(countryId: number): string {
    const country = this.allCountry.find((c) => c.countryId === countryId);
    return country ? country.name : 'Not Found';
  }

  getStateName(stateId: number): string {
    const state = this.allState.find((s) => s.stateId === stateId);
    return state ? state.name : 'Not Found';
  }
}
