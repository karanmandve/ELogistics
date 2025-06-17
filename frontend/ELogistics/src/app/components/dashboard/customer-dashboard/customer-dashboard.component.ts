import { Component, inject } from '@angular/core';
import { ProductService } from '../../services/product/product.service';
import { CommonModule } from '@angular/common';
import { CartService } from '../../services/cart/cart.service';
import { ToastrService } from 'ngx-toastr';
declare var bootstrap: any;

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './customer-dashboard.component.html',
  styleUrl: './customer-dashboard.component.css'
})
export class CustomerDashboardComponent {
  products: any[] = [];  // Store the products
  selectedProduct: any;  // To store selected product details for the modal
  userDetails = JSON.parse(localStorage.getItem("userDetails") || "{}");

  // Injecting the ProductService
  productService = inject(ProductService);
  cartService = inject(CartService);
  toaster = inject(ToastrService)

  ngOnInit() {
    this.loadCartFromLocalStorage();
    this.getAllProducts();
  }


  getAllProducts() {
    this.productService.getAllProduct().subscribe({
      next: (data: any) => {
        console.log('Products:', data);
        this.products = data;  // Store the products data in the array
      },
      error: (error: any) => {
        console.error('Error fetching products:', error);
      }
    });
  }


  openModal(product: any) {
    this.selectedProduct = product;  // Assign the selected product data
    const modal = new bootstrap.Modal(document.getElementById('productDetailsModal') as HTMLElement);
    modal.show();  // Show the modal
  }


  addToCart(product: any) {
    // product.isAddedToCart = true;
    // this.cartService.incrementCartCount();
    this.cart.add(product.id);
    const productData = {
      userId : this.userDetails.id,
      productId : product.id,
      quantity : 1
    }

    this.cartService.addToCart(productData).subscribe({
      next: (res: any) => {
        // this.toaster.success("added to cart", "Success", {
        //   progressBar: true,
        //   timeOut: 1000
        // })
        this.updateCartInLocalStorage();
        this.cartService.updateCartCount(this.userDetails.id);

      },
      error: (error: any) => {
        console.log(error)
      }
    })
  }

  cart = new Set<number>();
  
  isProductInCart(productId: number): boolean {
    return this.cart.has(productId); 
  }

  
  updateCartInLocalStorage() {
    localStorage.setItem('cart', JSON.stringify(Array.from(this.cart)));
  }

  
  loadCartFromLocalStorage() {
    const storedCart = localStorage.getItem('cart');
    if (storedCart) {
      this.cart = new Set(JSON.parse(storedCart));  
    }
  }

}
