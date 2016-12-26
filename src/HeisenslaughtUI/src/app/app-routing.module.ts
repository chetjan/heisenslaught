import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeScreenComponent } from './screens/home-screen/home-screen.component';
import { NotFoundScreenComponent } from './screens/not-found-screen/not-found-screen.component';


const appRoutes: Routes = [
    { path: '', component: HomeScreenComponent },
    { path: 'draft', loadChildren: 'app/modules/heroes-drafting/heroes-drafting.module#HeroesDraftingModule' },
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
