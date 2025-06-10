import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  http = inject(HttpClient);

  createOrder(amount: number) {
    return this.http.post('https://localhost:7228/api/Razorpay/create-order', amount);
  }

  verifyPayment(paymentId: string, orderId: string) {
    return this.http.post('https://localhost:7228/api/Razorpay/verify-payment', { paymentId, orderId });
  }
}
