import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeScreenComponent } from './screens/home-screen/home-screen.component';
import { NotFoundScreenComponent } from './screens/not-found-screen/not-found-screen.component';
import { LoginScreenComponent } from './screens/login-screen/login-screen.component';
import { AuthGuard } from './modules/users/shared/guards/auth-guard.service';

const appRoutes: Routes = [
    { path: '', component: HomeScreenComponent },
    { path: 'login', component: LoginScreenComponent },
    {
        path: 'draft',
        canLoad: [AuthGuard],
        loadChildren: 'app/modules/heroes-drafting/heroes-drafting.module#HeroesDraftingModule'
    },
    { path: '**', component: NotFoundScreenComponent }
];

@NgModule({
    imports: [
        RouterModule.forRoot(appRoutes)
    ],
    exports: [
        RouterModule
    ]
})
export class AppRoutingModule { }
