import { Component, EventEmitter, inject, Output } from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  ValidationErrors,
  ReactiveFormsModule,
} from '@angular/forms';
import { SweetAlertToasterService } from '../../../services/toaster/sweet-alert-toaster.service';
import { Router } from '@angular/router';
import { CountryStateServiceService } from '../../../services/country-state-service/country-state-service.service';
import { UserServiceService } from '../../../services/user/user-service.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-customer-registration',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './customer-registration.component.html',
  styleUrl: './customer-registration.component.css',
})
export class CustomerRegistrationComponent {
  allDistributors: any[] = [];
  todayDate = new Date().toISOString().split('T')[0];
  pastDate = new Date('1901-01-01').toISOString().split('T')[0];
  allState: any;
  allCountry: any[] = [];
  fileSizeError = false;
  @Output() registered = new EventEmitter<void>();

  toaster = inject(SweetAlertToasterService);
  router = inject(Router);
  userService = inject(UserServiceService);
  countryStateService = inject(CountryStateServiceService);

  ngOnInit() {
    this.getAllStates();
    this.sanitizeField('lastName');
    // this.sanitizeFieldForEmail('email'); 
    this.getAllDistributors();
  }

  customerRegisterForm = new FormGroup({
    firstName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(25),
      Validators.pattern(/^[A-Za-z]+(?: [A-Za-z]+)*\s*$/),
    ]),
    lastName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(25),
      Validators.pattern(/^[A-Za-z]+(?: [A-Za-z]+)*\s*$/),
    ]),
    email: new FormControl('', [
      Validators.required,
      Validators.email,
      Validators.maxLength(50),
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(20),
      Validators.pattern(/^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*#?&]{6,}$/), // at least 1 letter, 1 number
    ]),
    phoneNumber: new FormControl('', [
      Validators.required,
      Validators.pattern(/^\d{10}$/),
    ]),
    stateId: new FormControl('', [Validators.required]),
    city: new FormControl('', [
      Validators.required,
      Validators.maxLength(30),
      Validators.pattern(/^[A-Za-z ]+$/),
    ]),
    zip: new FormControl('', [
      Validators.required,
      Validators.pattern(/^\d{6}$/),
      Validators.minLength(6),
      Validators.maxLength(6),
    ]),
    line1: new FormControl('', [
      Validators.required,
      Validators.minLength(5),
      Validators.maxLength(100),
      Validators.pattern(/^[a-zA-Z0-9\s,.-]+$/),
    ]),
    line2: new FormControl('', [
      Validators.maxLength(100),
      Validators.pattern(/^[a-zA-Z0-9\s,.-]*$/),
    ]),
    distributorId: new FormControl('', [Validators.required]),
    GSTNumber: new FormControl('', [
      Validators.pattern(/^\d{2}[A-Z]{5}\d{4}[A-Z]{1}[A-Z\d]{1}[Z]{1}[A-Z\d]{1}$/),
    ]), // GSTIN format validation, optional
  });

  futureDateValidator(control: FormControl): ValidationErrors | null {
    const today = new Date();
    const pastDate = new Date('1901-01-01');
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

  onRegisterSubmit() {
    if (this.customerRegisterForm.invalid) {
      this.customerRegisterForm.markAllAsTouched();
      this.toaster.error('Please Fill And Correct All The fields');
      return;
    }

    // Clone the form value and add userTypeId
    const payload = { ...this.customerRegisterForm.value, userTypeId: 2 };

    this.userService.addUser(payload).subscribe({
      next: (res: any) => {
        this.customerRegisterForm.reset();
        if (res.statusCode == 201) {
          this.toaster.success('Customer Register Successfully');
          this.registered.emit();
        } else {
          this.toaster.error('Customer Not Register');
        }
      },
      error: (error: any) => {
        if (error.error.statusCode == 409) {
          this.toaster.error('Customer Already Exist');
          this.customerRegisterForm.reset();
        } else {
          this.toaster.error('Error From Our Side');
          console.log(error);
        }
      },
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

  onKeyPress(event: KeyboardEvent) {
    const charCode = event.which ? event.which : event.keyCode;

    if (charCode < 48 || charCode > 57) {
      event.preventDefault();
    }
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

  getAllCountry() {
    this.countryStateService.getAllCountry().subscribe({
      next: (res: any) => {
        this.allCountry = res.data;
      },
      error: (error: any) => {
        console.log(error);
        // alert("I am in error")
      },
    });
  }

  getAllDistributors() {
    this.userService.getAllDistributors().subscribe({
      next: (res: any) => {
        this.allDistributors = res.data;
      },
      error: (error: any) => {
        console.log(error);
      },
    });
  }

  getAllStates() {
    this.countryStateService.getAllState().subscribe({
      next: (res: any) => {
        this.allState = res.data || res;
      },
      error: (error: any) => {
        console.log(error);
      },
    });
  }

  sanitizeField(fieldName: string): void {
    this.customerRegisterForm.get(fieldName)?.valueChanges.subscribe((value) => {
      if (value) {
        const sanitizedValue = value
          .replace(/[^A-Za-z\s]/g, '')
          .replace(/\s{2,}/g, ' ');
        if (value !== sanitizedValue) {
          this.customerRegisterForm.get(fieldName)?.setValue(sanitizedValue, {
            emitEvent: false, // Prevent triggering valueChanges again
          });
        }
      }
    });
  }

  sanitizeFieldForEmail(fieldName: string): void {
    this.customerRegisterForm.get(fieldName)?.valueChanges.subscribe((value) => {
      if (value) {
        const sanitizedValue = value
          .replace(/[^A-ZaZ0-9@._-]/g, '')
          .replace(/\s{2,}/g, '')
          .trim();
        if (value !== sanitizedValue) {
          this.customerRegisterForm.get(fieldName)?.setValue(sanitizedValue, {
            emitEvent: false, // Prevent triggering valueChanges again
          });
        }
      }
    });
  }
}
