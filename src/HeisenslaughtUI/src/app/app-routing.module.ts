import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeScreenComponent } from './screens/home-screen/home-screen.component';
import { NotFoundScreenComponent } from './screens/not-found-screen/not-found-screen.component';

const appRoutes: Routes = [
    { path: '', component: HomeScreenComponent },
    { path: 'draft', loadChildren: 'app/modules/heroes-drafting/heroes-drafting.module#HeroesDraftingModule'},


    /*{ path: 'admin', component: DraftConfigScreenComponent },
    // specific draft management screen
    { path: 'draft/config/:id/:adminToken', component: DraftConfigScreenComponent },
    // team drafter screen
    { path: 'draft/:id/:team', component: DraftComponent },
    // draft observer screen
    { path: 'draft/:id', component: DraftComponent },
    // Draft creation screen
    { path: 'draft', component: DraftConfigScreenComponent }*/
    {path: '**', component: NotFoundScreenComponent}
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