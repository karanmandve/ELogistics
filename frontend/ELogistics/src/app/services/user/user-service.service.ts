import { HttpClient } from '@angular/common/http';
import { HtmlParser } from '@angular/compiler';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, catchError, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserServiceService {
  
  private userSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  public user$: Observable<any> = this.userSubject.asObservable();
  email: string | null = "";
  
  
  http = inject(HttpClient);
  
  ngOnInit() {
    this.email = localStorage.getItem('email');
    this.updateUserDetails(this.email);
  }

  fetchUserData(email: any): Observable<any> {
    return this.http.get(`https://localhost:7228/api/User/get-user-details/${email}`)
  }

  updateUserDetails(email: any) {
    this.fetchUserData(email).subscribe({
      next: (res: any) => {
        this.setUserData(res.data);
      }
    })
  }

  setUserData(user: any) {
    this.userSubject.next(user);
  }

  getUserData() {
    return this.userSubject.value;
  }



  addUser(userObj: any): Observable<any>{
    return this.http.post("https://localhost:7228/api/User/register", userObj)
  }

  loginUser(loginData: any){
    return this.http.post("https://localhost:7228/api/User/Login", loginData)
  }
  
  // sendOtp(email: any){
  //   return this.http.get(`https://localhost:7228/api/User/sendotp/${email}`)
  // }

  sendOtpWithPasswordCheck(userData: any){
    return this.http.post(`https://localhost:7228/api/User/sendotp-with-password-check`, userData)
  }

  // forgotPassword(email: any, otp: any){
  //   return this.http.get(`https://localhost:7228/api/User/forgot-password/${email}/${otp}`)
  // }

  // getSpecialization(){
  //   return this.http.get("https://localhost:7228/get-all-specialisation")
  // }

  // getDistributorBySpecialization(specialisationId: any){
  //   return this.http.get(`https://localhost:7228/api/User/get-all-distributor-by-specialisationId/${specialisationId}`)
  // }
  
  // getAllCustomer(){
  //   return this.http.get("https://localhost:7228/api/User/get-all-customer");
  // }
  
  // updateCustomer(customerData: any){
  //   return this.http.put("https://localhost:7228/api/User/update-customer", customerData)
  // }

  // updateDistributor(distributorData: any){
  //   return this.http.put("https://localhost:7228/api/User/update-distributor", distributorData)
  // }

  // updateUser(userObj: any): Observable<any>{
  //   return this.http.put("https://localhost:7228/api/User/update-user", userObj)
  // }

  // changePassword(data: any){
  //   return this.http.post("https://localhost:7228/api/User/change-password", data)
  // }

  getAllDistributors(): Observable<any[]> {
    return this.http.get<any[]>("https://localhost:7228/api/User/get-all-distributor");
  }

}
