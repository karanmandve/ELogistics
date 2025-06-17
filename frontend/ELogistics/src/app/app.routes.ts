import { Routes } from '@angular/router';
import { RegisterLoginComponent } from './components/auth/register-login/register-login.component';
import { ErrorComponent } from './components/error/error.component';
import { LayoutComponent } from './components/layout/layout.component';
import { authGuardGuard } from './guards/auth-guard.guard';
import { HomeComponent } from './components/home/home.component';

export const routes: Routes = [
    {path:"", redirectTo:"login-register", pathMatch:"full"},
    {path:"login-register", component:RegisterLoginComponent},
    {path: "", component: LayoutComponent, children : [
        {path: "home", component: HomeComponent, canActivate : [authGuardGuard]},
    ]},
    {path: "**", component: ErrorComponent}
];
