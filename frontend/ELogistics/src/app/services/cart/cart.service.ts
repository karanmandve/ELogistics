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



  getCartCount(userId: number) {
    return this.http.get(`https://localhost:7238/api/Cart/get-cart-count/${userId}`);
  } 

  updateCartCount(userId: number) {
    this.getCartCount(userId).subscribe((count: any) => {
      this.cartCountSubject.next(count);
    });
  }

  addToCart(product: any){
    return this.http.post("https://localhost:7238/api/Cart/add-to-cart", product);
  }

  getCartProduct(userId: number){
    return this.http.get(`https://localhost:7238/api/Cart/get-cart-products/${userId}`);
  }

  updateCartQuantity(cartQuantityData: any){
    return this.http.put("https://localhost:7238/api/Cart/update-cart-quantity", cartQuantityData);
  }

  makePayment(paymentData: any){
    return this.http.post("https://localhost:7238/api/Cart/pay", paymentData);
  }

  makeFakePayment(paymentData: any){
    return this.http.post("https://localhost:7238/api/Cart/fake-pay", paymentData);
  }

  removeProductFromCart(productId: any, userId: any): Observable<any>{
    return this.http.delete(`https://localhost:7238/api/Cart/remove-product-from-cart/${productId}/${userId}`)
  }
  
}
