import { Component, inject } from '@angular/core';
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
  
    toaster = inject(SweetAlertToasterService);
    router = inject(Router);
    userService = inject(UserServiceService);
    countryStateService = inject(CountryStateServiceService);


    ngOnInit() {
      this.getAllCountry();
      this.getAllSpecialization();
      this.sanitizeField('firstName');
      this.sanitizeField('lastName');
      this.sanitizeFieldForEmail("email");
    }


   distributorRegisterForm = new FormGroup({
      firstName: new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(20),Validators.pattern(/^[A-Za-z]+(?: [A-Za-z]+)*\s*$/)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(25),Validators.pattern(/^[A-Za-z]+(?: [A-Za-z]+)*\s*$/)]),
      email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(50)]),
      mobile: new FormControl('', [Validators.required, Validators.pattern(/^\d{10}$/)]),
      dateOfBirth: new FormControl('', [Validators.required, this.futureDateValidator]),
      gender: new FormControl('', Validators.required),
      bloodGroup: new FormControl('', Validators.required),
      city: new FormControl('', [Validators.required]),
      userTypeId: new FormControl(1, Validators.required),
      file: new FormControl<File | null>(null, Validators.required),
      address: new FormControl('', [Validators.required, Validators.minLength(10), Validators.maxLength(150), Validators.pattern(/^[a-zA-Z0-9\s,.-]+$/)]),
      pincode: new FormControl("", [Validators.required, Validators.pattern(/^\d{6}$/), Validators.minLength(6), Validators.maxLength(6)]),
      countryId: new FormControl("", Validators.required),
      stateId: new FormControl("", Validators.required),
      qualification: new FormControl("", Validators.required),
      specialisationId : new FormControl("", Validators.required),
      registrationNumber : new FormControl("", Validators.required),
      visitingCharge: new FormControl("", Validators.required)
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

    getAllSpecialization(){
      this.userService.getSpecialization().subscribe({
        next : (res: any) => {
          this.allSpecialisations = res.data
          
        },
        error : (error: any) => {
          console.log(error);
        }
      })
    }
  
    getAllCountry(){
      this.countryStateService.getAllCountry().subscribe({
        next : (res: any) => {
          this.allCountry = res.data
        },
        error : (error: any) =>{
          console.log(error);
          // alert("I am in error")
        }
        
      })
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
    
        const formData = new FormData();
        Object.keys(this.distributorRegisterForm.controls).forEach((field) => {
          const value = this.distributorRegisterForm.get(field)?.value;
          formData.append(field, value);
        });
    
        this.userService.addUser(formData).subscribe({
          next: (res: any) => {
            this.distributorRegisterForm.reset();
            if (res.statusCode == 201) {
              window.location.reload();
              this.toaster.success('Distributor Register Successfully');
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
    
      onChange(countrId : any){
        this.countryStateService.getStateByCountryId(countrId).subscribe({
          next : (res:any) => {
            this.allState = res.data
          },
          error : (error: any) => {
            console.log(error);
            // alert("I am in error")
          }
        })
      }
    
      onFileSelected(event: Event) {
        const file = (event.target as HTMLInputElement).files?.[0];
        const maxSize = 5 * 1024 * 1024;
      
        if (file) {
          const validTypes = ['image/jpeg', 'image/jpg', 'image/png'];
          // Validate file type
          if (!validTypes.includes(file.type)) {
            this.distributorRegisterForm.get('file')?.setErrors({ invalidType: true });
            if (event.target instanceof HTMLInputElement) {
              event.target.value = ''; // Clear invalid input
            }
            // return; // Exit if invalid type
          } else {
            this.distributorRegisterForm.get('file')?.setErrors(null);
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
          this.distributorRegisterForm.patchValue({ file });
          this.distributorRegisterForm.get('file')?.updateValueAndValidity();
      
          // File is valid, read and preview it
          // const reader = new FileReader();
          // reader.onload = () => {
          //   this.imagePreview = reader.result as string; // Set the preview
          // };
          // reader.readAsDataURL(file);
        }
      }
}
