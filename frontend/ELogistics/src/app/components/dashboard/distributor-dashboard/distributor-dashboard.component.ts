import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import { ProductService } from '../../../services/product/product.service';
import { UserServiceService } from '../../../services/user/user-service.service';
declare var bootstrap: any;

@Component({
  selector: 'app-distributor-dashboard',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './distributor-dashboard.component.html',
  styleUrl: './distributor-dashboard.component.css'
})
export class AdminDashboardComponent {
  userDetails: any;
  products: any[] = [];
  isEditing = false;
  selectedProduct: any = null;
  fileSizeError = false;
  sellingPrice: number = 0;
  purchasePrice: number = 0;
  todayDate = new Date().toISOString().slice(0, 16);
  pastDate = new Date('1900-01-01').toISOString().slice(0, 16);

  @ViewChild('productModel') productModel!: ElementRef;
  toaster = inject(ToastrService)

  productService = inject(ProductService)
  userServices = inject(UserServiceService);

  ngOnInit(): void {
    this.userServices.user$.subscribe((user: any) => {
      this.userDetails = user;
      this.loadProducts();
    });
    this.sanitizeField('sellingPrice');
    this.sanitizeField('purchasePrice');
    this.sanitizeField('stock');
  }

  productForm: FormGroup = new FormGroup({
    productName: new FormControl('', [Validators.required]),
    productCategory: new FormControl('', [Validators.required]),
    productMRP: new FormControl('', [Validators.required, Validators.min(0)]),
    productRate: new FormControl('', [Validators.required, Validators.min(0)]),
    availableStocks: new FormControl('', [Validators.required, Validators.min(0)]),
    imageFile: new FormControl<File | null>(null, [Validators.required]),
  });


  sanitizeField(fieldName: string): void {
    this.productForm.get(fieldName)?.valueChanges.subscribe((value) => {
      if (value) {
        // Remove all non-numeric characters
        let sanitizedValue = value.replace(/[^0-9]/g, '');

        // Limit the length to 6 digits
        sanitizedValue = sanitizedValue.slice(0, 6);

        // If the sanitized value is different, update the form control
        if (value !== sanitizedValue) {
          this.productForm.get(fieldName)?.setValue(sanitizedValue, {
            emitEvent: false,
          });
        }
      }
    });
  }

  futureDateValidator(control: FormControl): ValidationErrors | null {
    const today = new Date();
    const pastDate = new Date('1900-01-01');
    const selectedDate = new Date(control.value);

    // Reset time to the start of the day (to avoid time comparisons)
    // today.setHours(0, 0, 0, 0);

    if (selectedDate > today) {
      return { futureDate: true };  // Return error if date is in the future
    } else if (selectedDate < pastDate) {
      return { pastDate: true };
    }  // Return error if date is in the future
    return null;  // Return null if date is valid
  }

  categories: string[] = [
    'Electronics',
    'Fashion',
    'Home & Living',
    'Beauty & Health',
    'Sports & Outdoors',
    'Books & Stationery',
    'Toys & Games',
    'Groceries & Food'
  ];



  // Fetch all products from API
  loadProducts() {
    this.productService.getAllProductByDistributorId(this.userDetails.id).subscribe(
      (res: any) => {
        // Map API response to match UI expectations if needed
        this.products = (res.data || []).map((p: any) => ({
          id: p.id,
          distributorId: p.distributorId,
          productImageUrl: p.productImageUrl,
          productName: p.productName,
          productCategory: p.productCategory,
          productMRP: p.productMRP,
          productRate: p.productRate,
          availableStocks: p.availableStocks
        }));
        console.log('Products:', this.products);
      },
      (error: any) => {
        console.error('Error fetching products:', error);
      }
    );
  }

  // Show details of a product
  showDetails(product: any) {
    this.selectedProduct = product;
    const detailsModal = new bootstrap.Modal(document.getElementById('detailsModal')!);
    detailsModal.show();
  }

  // Open Add/Edit Modal
  openModal(product: any = null) {
    this.isEditing = !!product;
    this.selectedProduct = product;
    this.productForm.reset();
    if (this.isEditing && product) {
      this.productForm.patchValue({
        productName: product.productName || '',
        productCategory: product.productCategory || '',
        productMRP: product.productMRP || '',
        productRate: product.productRate || '',
        availableStocks: product.availableStocks || ''
      });
      this.productForm.get('imageFile')?.clearValidators();
      this.productForm.get('imageFile')?.updateValueAndValidity();
    } else {
      this.productForm.get('imageFile')?.setValidators([Validators.required]);
      this.productForm.get('imageFile')?.updateValueAndValidity();
    }
    const modal = document.getElementById('productModal');
    if (modal) {
      modal.classList.add('show');
      modal.style.display = 'block';
    }
  }

  // Close Modal
  closeModal() {
    const modal = document.getElementById('productModal');
    if (modal) {
      modal.classList.remove('show');
      modal.style.display = 'none';
    }
    this.productForm.reset();
    this.isEditing = false;
    this.selectedProduct = null;
  }


  onFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    const maxSize = 5 * 1024 * 1024;

    if (file) {
      const validTypes = ['image/jpeg', 'image/jpg', 'image/png'];
      if (!validTypes.includes(file.type)) {
        // Set the custom error for invalid file type
        this.productForm.get('imageFile')?.setErrors({ invalidType: true });
        if (event.target instanceof HTMLInputElement) {
          event.target.value = '';
        }
      } else {
        // Clear the error if the file type is valid
        this.productForm.get('imageFile')?.setErrors(null);
      }
    }
    if (file) {
      // Check if the file size exceeds the 5 MB limit
      if (file.size > maxSize) {
        this.fileSizeError = true;
        // Clear the file input if it exceeds the limit
        if (event.target instanceof HTMLInputElement) {
          event.target.value = '';
        }
      } else {
        this.fileSizeError = false;
      }
    }
    this.productForm.patchValue({ imageFile: file });
    this.productForm.get('imageFile')?.updateValueAndValidity();
  }

  saveProduct() {
    if (this.productForm.invalid) {
      this.toaster.error('Please fill all required fields', 'Error');
      return;
    }
    const formData = new FormData();
    if (this.isEditing && this.selectedProduct?.id) {
      formData.append('Id', this.selectedProduct.id);
    }
    formData.append('ProductName', this.productForm.get('productName')?.value);
    formData.append('ProductCategory', this.productForm.get('productCategory')?.value);
    formData.append('ProductMRP', this.productForm.get('productMRP')?.value);
    formData.append('ProductRate', this.productForm.get('productRate')?.value);
    formData.append('AvailableStocks', this.productForm.get('availableStocks')?.value);
    formData.append('DistributorId', this.userDetails.id);
    const file = this.productForm.get('imageFile')?.value;
    if (file) {
      formData.append('ImageFile', file);
    }
    if (this.isEditing) {
      this.productService.updateProduct(formData).subscribe(
        () => {
          this.toaster.success('Product updated successfully', 'Success');
          this.loadProducts();
          this.closeModal();
        },
        (error) => {
          this.toaster.error('Error updating product', 'Error');
          console.error('Error updating product:', error)
        }
      );
    } else {
      this.productService.addProduct(formData).subscribe(
        () => {
          this.toaster.success('Product added successfully', 'Success');
          this.loadProducts();
          this.closeModal();
        },
        (error) => {
          this.toaster.error('Error adding product', 'Error');
          console.error('Error adding product:', error)
        }
      );
    }
  }


  confirmDeletion(productId: any): void {
    Swal.fire({
      title: 'Are you sure?',
      text: 'You will not be able to recover this item!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'No, cancel!',
    }).then((result) => {
      if (result.isConfirmed) {
        // Perform the delete operation
        this.deleteProduct(productId);

      } else if (result.dismiss === Swal.DismissReason.cancel) {
        // Swal.fire('Cancelled', 'Your item is safe.', 'info');
      }
    });
  }

  // Delete Product
  deleteProduct(productId: number) {
    this.productService.deleteProductById(productId).subscribe(
      () => {
        Swal.fire('Deleted!', 'Your item has been deleted.', 'success');
        this.toaster.success('Product deleted successfully', 'Success');
        this.loadProducts()
      },
      (error) => {
        this.toaster.error('Error deleting product', 'Error');
        console.error('Error deleting product:', error)
      }
    );
  }



}
