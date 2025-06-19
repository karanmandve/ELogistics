import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { CartService } from '../../services/cart/cart.service';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { CountryStateServiceService } from '../../services/country-state-service/country-state-service.service';
import { UserServiceService } from '../../services/user/user-service.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
  cartProducts: any[] = [];
  totalPrice: number = 0;
  userDetails: any;
  addressData: any;
  invoiceUrl: any;
  counter = 1;
  timer: any;
  sgstTotal: number = 0;
  cgstTotal: number = 0;

  cartService = inject(CartService);
  toaster = inject(ToastrService);
  countryStateService: any = inject(CountryStateServiceService);
  changeDetector = inject(ChangeDetectorRef);
  userServices = inject(UserServiceService);

  ngOnInit(): void {
    // this.getAllCountry();
    // this.loadState();
    this.userServices.user$.subscribe((user: any) => {
      this.userDetails = user;
      this.loadCart();
    });
  }

  makePayment() {
    alert("Payment Method is not implemented yet");
  }

  addressForm: FormGroup = new FormGroup({
    address: new FormControl('', [Validators.required, Validators.minLength(10), Validators.maxLength(150), Validators.pattern(/^[a-zA-Z0-9\s,.-]+$/)]),
    zipcode: new FormControl('', [Validators.required, Validators.pattern(/^\d{6}$/), Validators.minLength(6), Validators.maxLength(6)]),
    state: new FormControl(0, [Validators.required]),
    country: new FormControl(0, [Validators.required])
  });

  paymentForm: FormGroup = new FormGroup({
    cardNumber: new FormControl('', [Validators.required, Validators.pattern('^[0-9]{16}$')]),
    expiryYear: new FormControl('', [Validators.required]),
    expiryMonth: new FormControl('', [Validators.required]),
    cvv: new FormControl('', [Validators.required, Validators.pattern('^[0-9]{3}$')])
  });

  months = [
    { value: '01', label: '01' },
    { value: '02', label: '02' },
    { value: '03', label: '03' },
    { value: '04', label: '04' },
    { value: '05', label: '05' },
    { value: '06', label: '06' },
    { value: '07', label: '07' },
    { value: '08', label: '08' },
    { value: '09', label: '09' },
    { value: '10', label: '10' },
    { value: '11', label: '11' },
    { value: '12', label: '12' }
  ];


  currentYear = new Date().getFullYear();
  years = Array.from({ length: 20 }, (_, i) => this.currentYear + i);

  onKeyPress(event: KeyboardEvent) {
    const charCode = event.which ? event.which : event.keyCode;

    if (charCode < 48 || charCode > 57) {
      event.preventDefault();
    }
  }

  loadCart() {
    const customerId = this.userDetails.id;
    this.cartService.getCartProduct(customerId).subscribe({
      next: (res: any) => {
        this.cartProducts = res.data;
        this.calculateTotalPrice();
        this.calculateGSTTotals();
        localStorage.removeItem('cart');
        var cart = new Set<number>();
        this.cartProducts.forEach((product) => {
          cart.add(product.productId);
        });
        localStorage.setItem('cart', JSON.stringify(Array.from(cart)));
      },
      error: (error: any) => {
        this.cartProducts = [];
        this.sgstTotal = 0;
        this.cgstTotal = 0;
        this.changeDetector.detectChanges();
        localStorage.setItem('cart', JSON.stringify([]));
      }
    });
  }

  calculateGSTTotals() {
    this.sgstTotal = this.cartProducts.reduce((sum, product) => sum + ((product.productRate * product.quantity) * (product.sgst || 0) / 100), 0);
    this.cgstTotal = this.cartProducts.reduce((sum, product) => sum + ((product.productRate * product.quantity) * (product.cgst || 0) / 100), 0);
  }

  removeProductFromCart(product: any) {
    this.cartService.removeProductFromCart(product.productId, this.userDetails.id).subscribe(() => {
      this.loadCart();
      this.cartService.updateCartCount(this.userDetails.id);
    });
  }

  incrementQuantity(product: any) {
    this.updateCartQuantity(product, 1);
  }

  decrementQuantity(product: any) {
    this.updateCartQuantity(product, -1);
  }

  updateCartQuantity(product: any, quantity: any) {
    const cartQuantityData = {
      customerId: this.userDetails.id,
      productId: product.productId,
      quantityChange: quantity  // Increment by 1 or decrement by 1
    };

    this.cartService.updateCartQuantity(cartQuantityData).subscribe({
      next: (response) => {
        this.loadCart()
        this.cartService.updateCartCount(this.userDetails.id);
      },
      error: (error) => {
        this.toaster.warning("Stock Is Over", "Warning");
      }
    });
  }

  calculateTotalPrice() {
    // Total should include GST (rate * qty + sgst + cgst for all products)
    this.totalPrice = this.cartProducts.reduce((sum, product) => {
      const base = product.productRate * product.quantity;
      const sgst = base * (product.sgst || 0) / 100;
      const cgst = base * (product.cgst || 0) / 100;
      return sum + base + sgst + cgst;
    }, 0);
  }


  checkout() {
    this.cartService.checkout(this.userDetails.id).subscribe({
      next: (res: any) => {
        this.loadCart();
        this.invoiceUrl = res.data?.invoiceUrl;
        this.cartService.updateCartCount(this.userDetails.id);
        this.openSuccessPopup();
      },
      error: (error: any) => {
        this.toaster.error('Error fetching checkout details', 'Error');
      }
    });
  }

  downloadInvoice() {
    if (this.invoiceUrl) {
      window.open(this.invoiceUrl, '_blank');
    } else {
      this.toaster.error('No invoice available to download', 'Error');
    }
  }


  openAddressPopup() {

    if (this.userDetails.address) {
      this.addressForm.patchValue({
        address: this.userDetails.address,
        zipcode: this.userDetails.zipcode,
        country: this.userDetails.countryId,
        state: this.userDetails.stateId
      });
    }

    this.getState(this.userDetails.countryId);
    // Show the modal by adding the class
    const modal = document.getElementById('addressModal');
    if (modal) {
      modal.classList.add('show');
      modal.style.display = 'block';
    }
  }


  saveAddress() {

    this.addressData = this.addressForm.value;

    this.closeModal('addressModal');
    this.openChoosePaymentPopup();
  }

  // choosePaymentPopup(){
  //   const modal = document.getElementById('choosePaymentModal');
  //   if (modal) {
  //     modal.classList.add('show');
  //     modal.style.display = 'block';
  //   }
  // }

  openFakePaymentPopup() {
    // Show the payment modal
    this.closeModal('choosePaymentModal');
    const modal = document.getElementById('fakePaymentModal');
    if (modal) {
      modal.classList.add('show');
      modal.style.display = 'block';
    }
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
    }, 900);

  }

  openChoosePaymentPopup() {
    // Show the payment modal
    const modal = document.getElementById('choosePaymentModal');
    if (modal) {
      modal.classList.add('show');
      modal.style.display = 'block';
    }
  }

  closeModal(modalId: string) {
    const modal = document.getElementById(modalId);
    if (modal) {
      modal.classList.remove('show');
      modal.style.display = 'none';
    }
  }

  makeFakePayment() {
    alert("Payment Method is not implemented yet");
  }

  // makeFakePayment() {

  //     var paymentFormData = this.paymentForm.value;

  //     var expiryDate = `${paymentFormData.expiryMonth}/${paymentFormData.expiryYear}`;

  //     const paymentData = {
  //       cardNumber : paymentFormData.cardNumber,
  //       cvv : paymentFormData.cvv,
  //       expiryDate: expiryDate,
  //       customerId: this.userDetails.id,
  //       deliveryAddress: this.addressData.address,
  //       deliveryZipcode: this.addressData.zipcode,
  //       deliveryStateId: this.addressData.state,
  //       deliveryCountryId: this.addressData.country
  //     };

  //     this.cartService.makeFakePayment(paymentData).subscribe((res: any) => {

  //       this.loadCart();
  //       this.invoiceUrl = res.invoiceUrl;
  //       this.cartService.updateCartCount(this.userDetails.id);
  //       this.closeModal('fakePaymentModal');
  //       this.openSuccessPopup();
  //     }, (error) => {
  //       this.toaster.error('Invalid Payment Details', 'Error');

  //     });


  // }


  // makePayment() {
  //   this.closeModal('choosePaymentModal');

  //   this.paymentService.createOrder(this.totalPrice).subscribe((order: any) => {
  //     console.log(order.id, order.amount);

  //     const options = {
  //       key: 'rzp_test_j1n3HfglIVc3GS',  
  //       amount: order.amount,
  //       currency: order.currency,
  //       order_id: order.id,
  //       handler: (response: any) => {
  //         this.verifyPayment(response.razorpay_payment_id, order.id);
  //       },
  //     };
  //     if (window.Razorpay) {
  //       const razorpay = new Razorpay(options);
  //       razorpay.open();
  //     } else {
  //       console.error('Razorpay SDK is not loaded properly');
  //     }
  //   });

  // }


  // verifyPayment(paymentId: string, orderId: string) {
  //   this.paymentService.verifyPayment(paymentId, orderId).subscribe(
  //     (response: any) => {
  //       this.openWaitingPopup();

  //       console.log('Payment Verified', response);

  //     const paymentData = {
  //       customerId: this.userDetails.id,
  //       deliveryAddress: this.addressData.address,
  //       deliveryZipcode: this.addressData.zipcode,
  //       deliveryStateId: this.addressData.state,
  //       deliveryCountryId: this.addressData.country
  //     };

  //     this.cartService.makePayment(paymentData).subscribe((res: any) => {
  //       this.loadCart();
  //       this.closeModal("waitingModal");
  //       clearInterval(this.timer);  // Stop the timer
  //       this.counter = 0
  //       this.invoiceUrl = res.invoiceUrl;
  //       this.cartService.updateCartCount(this.userDetails.id);
  //       this.openSuccessPopup();
  //     }, (error : any) => {
  //       this.toaster.error('Invalid Payment Details', 'Error');
  //     });

  //     },
  //     (error : any) => {
  //       console.error('Payment verification failed', error);
  //     }
  //   );
  // }


  openSuccessPopup() {
    const modal = document.getElementById('successModal');
    if (modal) {
      modal.classList.add('show');
      modal.style.display = 'block';
    }

    const link = document.createElement('a');
    link.href = this.invoiceUrl;
    link.click();
    link.remove();
    this.loadCart();
    this.downloadInvoice();


    // const iframe = document.createElement('iframe');
    // iframe.style.display = 'none';  // Make the iframe invisible
    // iframe.src = this.invoiceUrl;
    // document.body.appendChild(iframe);  // Append it to the body to load the URL in the background

    // // Optional: Remove iframe after loading
    // iframe.onload = () => {
    //     document.body.removeChild(iframe);
    // };
    // this.loadCart();
  }




  allCountry: any[] = []


  getAllCountry() {
    this.countryStateService.getAllCountry().subscribe({
      next: (res: any) => {
        this.allCountry = res
      },
      error: (error: any) => {
        alert("I am in error")
      }

    })
  }


  allState: any[] = []


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
        this.allState = res
      },
      error: (error: any) => {
        alert("I am in error")
      }
    })
  }




  getCountryName(countryId: number): string {
    const country = this.allCountry.find(c => c.countryId === countryId);
    return country ? country.name : 'Not Found';
  }

  getStateName(stateId: number): string {
    const state = this.allState.find(s => s.stateId === stateId);
    return state ? state.name : 'Not Found';
  }


}
