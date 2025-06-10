import { HttpHandler, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { LoaderService } from '../services/loader/loader.service';
import { inject } from '@angular/core';
import { finalize, Observable, takeUntil } from 'rxjs';
import { AppointmentService } from '../services/appointment/appointment.service';

export const customInterceptor: HttpInterceptorFn = (req, next) => {

 const loaderService = inject(LoaderService);
 const appointmentService = inject(AppointmentService);

 loaderService.show();

 const token = sessionStorage.getItem('token');
 const authReq = token
   ? req.clone({
       setHeaders: {
         Authorization: `Bearer ${token}`,
       },
     })
   : req;
   
   return next(authReq).pipe(
    takeUntil(appointmentService.onCancelPendingRequests()),  // Cancels request if cancellation is triggered
    finalize(() => loaderService.hide())  // Hide loader once the request is complete or canceled
  );
};
