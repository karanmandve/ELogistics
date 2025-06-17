import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CartService } from '../../services/cart/cart.service';
import { CommonModule } from '@angular/common';
import { UserServiceService } from '../../services/user/user-service.service';
import { SweetAlertToasterService } from '../../services/toaster/sweet-alert-toaster.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
})
export class LayoutComponent {
  imageUrl: string = '';
  cartCount: number = 0;
  userDetails: any;
  username = localStorage.getItem('username');

  toaster: any = inject(SweetAlertToasterService);
  router: any = inject(Router);
  cartService: any = inject(CartService);
  userService = inject(UserServiceService);

  ngOnInit() {
    this.userService.updateUserDetails(this.username);

    this.userService.user$.subscribe((user: any) => {
      if (user !== null){
        this.userDetails = user;
        this.imageUrl = this.userDetails.profileImageUrl;
      }
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
