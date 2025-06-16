import { Component, EventEmitter, inject, Output } from '@angular/core';
import { FormGroup, FormControl, Validators, ValidationErrors, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CountryStateServiceService } from '../../../services/country-state-service/country-state-service.service';
import { SweetAlertToasterService } from '../../../services/toaster/sweet-alert-toaster.service';
import { UserServiceService } from '../../../services/user/user-service.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-distributor-registration',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './distributor-registration.component.html',
  styleUrl: './distributor-registration.component.css'
})
export class DistributorRegistrationComponent {

    todayDate = new Date().toISOString().split('T')[0];
    pastDate = new Date('1901-01-01').toISOString().split('T')[0];
    allState: any;
    allCountry : any [] = []
    fileSizeError = false;
    allSpecialisations: any[] = [];
    @Output() registered = new EventEmitter<void>();
  
    toaster = inject(SweetAlertToasterService);
    router = inject(Router);
    userService = inject(UserServiceService);
    countryStateService = inject(CountryStateServiceService);


    ngOnInit() {
      this.getAllStates();
      this.sanitizeField('firstName');
      this.sanitizeField('lastName');
      this.sanitizeFieldForEmail("email");
    }


    distributorRegisterForm = new FormGroup({
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
      GSTNumber: new FormControl('', [
        Validators.pattern(/^\d{2}[A-Z]{5}\d{4}[A-Z]{1}[A-Z\d]{1}[Z]{1}[A-Z\d]{1}$/)
      ]), // GSTIN format validation, optional
    })


    sanitizeField(fieldName: string): void {
      this.distributorRegisterForm.get(fieldName)?.valueChanges.subscribe((value) => {
        if (value) {
          
          const sanitizedValue = value
            .replace(/[^A-Za-z\s]/g, '') 
            .replace(/\s{2,}/g, ' '); 
          if (value !== sanitizedValue) {
            this.distributorRegisterForm.get(fieldName)?.setValue(sanitizedValue, {
              emitEvent: false, 
            });
          }
        }
      });
    }


    sanitizeFieldForEmail(fieldName: string): void {
      this.distributorRegisterForm.get(fieldName)?.valueChanges.subscribe((value) => {
        if (value) {
          const sanitizedValue = value
            .replace(/[^A-Za-z0-9@._-]/g, '')
            .replace(/\s{2,}/g, '') 
            .trim(); 
          if (value !== sanitizedValue) {
            this.distributorRegisterForm.get(fieldName)?.setValue(sanitizedValue, {
              emitEvent: false, // Prevent triggering valueChanges again
            });
          }
        }
      });
    }


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
        if (this.distributorRegisterForm.invalid) {
          this.distributorRegisterForm.markAllAsTouched();
          this.toaster.error('Please Fill And Correct All The fields');
          return;
        }

        // Clone the form value and add userTypeId
        const payload = { ...this.distributorRegisterForm.value, userTypeId: 1 };

        this.userService.addUser(payload).subscribe({
          next: (res: any) => {
            this.distributorRegisterForm.reset();
            if (res.statusCode == 201) {
              this.toaster.success('Distributor Register Successfully');
              this.registered.emit();
            } else {
              this.toaster.error('Distributor Not Register');
            }
          },
          error: (error: any) => {
            if (error.error.statusCode == 409) {
              this.toaster.error('Distributor Already Exist');
              this.distributorRegisterForm.reset();
            } else {
              this.toaster.error('Error From Our Side');
              console.log(error);
            }
          },
        });
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
    
      getAllStates() {
        this.countryStateService.getAllState().subscribe({
          next: (res: any) => {
            this.allState = res.data || res;
          },
          error: (error: any) => {
            console.log(error);
          }
        });
      }
}
