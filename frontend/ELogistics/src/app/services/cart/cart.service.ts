import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  http = inject(HttpClient);

  private cartCountSubject = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCountSubject.asObservable();

  // incrementCartCount() {
  //   const currentCount = this.cartCountSubject.value;
  //   this.cartCountSubject.next(currentCount + 1);
  // }



  getCartCount(customerId: number) {
    return this.http.get(`https://localhost:7228/api/Cart/get-cart-count/${customerId}`);
  } 

  updateCartCount(customerId: number) {
    this.getCartCount(customerId).subscribe((count: any) => {
      this.cartCountSubject.next(count);
    });
  }

  addToCart(product: any){
    return this.http.post("https://localhost:7228/api/Cart/add-to-cart", product);
  }

  getCartProduct(customerId: number){
    return this.http.get(`https://localhost:7228/api/Cart/get-cart-products/${customerId}`);
  }

  updateCartQuantity(cartQuantityData: any){
    return this.http.put("https://localhost:7228/api/Cart/update-cart-quantity", cartQuantityData);
  }

  // makePayment(paymentData: any){
  //   return this.http.post("https://localhost:7238/api/Cart/pay", paymentData);
  // }

  // makeFakePayment(paymentData: any){
  //   return this.http.post("https://localhost:7238/api/Cart/fake-pay", paymentData);
  // }

  removeProductFromCart(productId: any, customerId: any): Observable<any>{
    return this.http.delete(`https://localhost:7228/api/Cart/remove-product-from-cart/${productId}/${customerId}`)
  }

  makePayment(customerId: any){
    return this.http.get(`https://localhost:7228/api/Cart/checkout/${customerId}`);
  }
  
}
