import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  http = inject(HttpClient)

  getAllOrderByUserId(userId: number){
    return this.http.get(`https://localhost:7238/api/Order/get-orders-by-userId/${userId}`);
  }

}
