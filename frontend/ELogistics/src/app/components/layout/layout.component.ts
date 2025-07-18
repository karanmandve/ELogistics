import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CartService } from '../../services/cart/cart.service';
import { CommonModule } from '@angular/common';
import { UserServiceService } from '../../services/user/user-service.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent {
  imageUrl: string = '';
  cartCount: number = 0;
  userDetails: any;
  
  
  toaster: any = inject(ToastrService);
  router: any = inject(Router);
  cartService: any = inject(CartService);
  userServices: any = inject(UserServiceService);
  
  
  ngOnInit() {
    const email = localStorage.getItem('email') || '';
    this.userServices.updateUserDetails(email);
    this.userServices.user$.subscribe((user: any) => {
      this.userDetails = user;
      this.cartService.updateCartCount(this.userDetails.id);
    });
    

    // Subscribe to cart count updates
    this.cartService.cartCount$.subscribe((count: any) => {
      this.cartCount = count;
      console.log('Cart count updated:', this.cartCount);
    });
  }



  logout(type: 'manual' | 'auto') {
    localStorage.clear();
    this.router.navigate(['/']);

    if (type === 'auto') {
      this.toaster.info(
        'Your session has expired. You have been logged out automatically.',
        'Session Expired'
      );
    } else if (type === 'manual') {
      this.toaster.success(
        'You have successfully logged out.',
        'Logout Successful'
      );
    }
  }
}
