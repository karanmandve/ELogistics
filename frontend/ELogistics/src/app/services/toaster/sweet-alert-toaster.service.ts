import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class SweetAlertToasterService {

  toastMixin = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000,
    didOpen: (toast) => {
      toast.onmouseenter = Swal.stopTimer;
      toast.onmouseleave = Swal.resumeTimer;

      const closeButton = document.createElement('button');
      closeButton.innerHTML = '×';  
      closeButton.classList.add('close-btn');  
      closeButton.onclick = () => Swal.close();  
      toast.appendChild(closeButton);  
    }
  });

  
  success(message: string): void {
    this.toastMixin.fire({
      icon: 'success',
      title: message
    });
  }

  
  error(message: string): void {
    this.toastMixin.fire({
      icon: 'error',
      title: message
    });
  }

  
  warning(message: string): void {
    this.toastMixin.fire({
      icon: 'warning',
      title: message
    });
  }

  
  info(message: string): void {
    this.toastMixin.fire({
      icon: 'info',
      title: message
    });
  }

}
