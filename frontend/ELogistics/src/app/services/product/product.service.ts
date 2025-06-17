import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  http = inject(HttpClient);

  // getAllProduct(){
  //   return this.http.get("https://localhost:7238/api/Product/get-all-product");
  // }

  getAllProductByDistributorId(distributorId: any){
    return this.http.get(`https://localhost:7228/api/Product/get-product-by-distributor/${distributorId}`);
  }
  
  // addProduct(productObj: any): Observable<any>{
    
  //   return this.http.post("https://localhost:7238/api/Product/add-product", productObj, { responseType: 'json' });
  // }

  // updateProduct(productObj: any): Observable<any>{
  //   return this.http.put(`https://localhost:7238/api/Product/update-product`, productObj, { responseType: 'json' })
  // }

  // deleteProductById(productId: any): Observable<any>{
  //   return this.http.delete(`https://localhost:7238/api/Product/delete-product/${productId}`)
  // }



}
