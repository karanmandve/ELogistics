import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

export const authGuardGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const router = inject(Router);
  
  const isSessionActive = localStorage.getItem("session") === "true";
  const protectedRoutes = ["/home", "/profile", "/cart", "/orders", "/appointment"
    , "/appointment-history", "/customer-profile" ,"/distributor-profile"];
  
  if ((protectedRoutes.includes(state.url) || state.url.startsWith("/complete-appointment/") || state.url.startsWith("/distributor-chat/") || state.url.startsWith("/customer-chat/") ) && isSessionActive) {
    return true;
  } else {
    router.navigate(["/"]);
    return false;
  }
};