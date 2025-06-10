import { Routes } from '@angular/router';
import { RegisterLoginComponent } from './components/auth/register-login/register-login.component';
import { LayoutComponent } from './components/layout/layout.component';
import { ProfileComponent } from './components/profile/customer-profile/profile.component';
import { HomeComponent } from './components/home/home.component';
import { authGuardGuard } from './guards/auth-guard.guard';
import { ErrorComponent } from './components/error/error.component';
import { CustomerRegistrationComponent } from './components/auth/customer-registration/customer-registration.component';
import { AppointmentComponent } from './components/appointment/appointment.component';
import { AppointmentHistoryComponent } from './components/appointment-history/appointment-history.component';
import { CompleteAppointmentComponent } from './components/complete-appointment/complete-appointment.component';
import { DistributorProfileComponent } from './components/profile/distributor-profile/distributor-profile.component';
import { ChatComponent } from './components/chat/chat.component';

export const routes: Routes = [
    {path:"", redirectTo:"login-register", pathMatch:"full"},
    {path:"login-register", component:RegisterLoginComponent},
    {path: "", component: LayoutComponent, children : [
        {path: "customer-profile", component: ProfileComponent, canActivate : [authGuardGuard]},
        {path: "customer-chat/:distributorId", component: ChatComponent, canActivate : [authGuardGuard]},
        {path: "distributor-chat/:customerId", component: ChatComponent, canActivate : [authGuardGuard]},
        {path: "distributor-profile", component: DistributorProfileComponent, canActivate : [authGuardGuard]},
        {path: "home", component: HomeComponent, canActivate : [authGuardGuard]},
        {path: "appointment", component: AppointmentComponent, canActivate : [authGuardGuard]},
        {path: "complete-appointment/:appointmentId", component: CompleteAppointmentComponent, canActivate : [authGuardGuard]},
        {path: "appointment-history", component: AppointmentHistoryComponent, canActivate : [authGuardGuard]},
    ]},
    {path: "**", component: ErrorComponent}
];
