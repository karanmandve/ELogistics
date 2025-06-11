import { Routes } from '@angular/router';
import { RegisterLoginComponent } from './components/auth/register-login/register-login.component';
import { ErrorComponent } from './components/error/error.component';

export const routes: Routes = [
    {path:"", redirectTo:"login-register", pathMatch:"full"},
    {path:"login-register", component:RegisterLoginComponent},
    {path: "**", component: ErrorComponent}
];
