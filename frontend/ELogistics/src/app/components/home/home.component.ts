import { Component, inject, OnInit } from '@angular/core';
import { CommonModule, TitleCasePipe } from '@angular/common';
import { UserServiceService } from '../../services/user/user-service.service';
import { Router } from '@angular/router';
import { AdminDashboardComponent } from "../dashboard/distributor-dashboard/distributor-dashboard.component";
import { CustomerDashboardComponent } from '../dashboard/customer-dashboard/customer-dashboard.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, AdminDashboardComponent, CustomerDashboardComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{
  email = localStorage.getItem('email');
  userDetails: any;

  userServices = inject(UserServiceService);
  router = inject(Router)

  ngOnInit(): void {
    this.userServices.user$.subscribe((user: any) => {
      this.userDetails = user;
    });
    
  }
  
}
