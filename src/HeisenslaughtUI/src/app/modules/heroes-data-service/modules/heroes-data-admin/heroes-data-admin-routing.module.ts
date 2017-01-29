import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HeroesComponent } from './heroes/heroes.component';
import { MapsComponent } from './maps/maps.component';

const routes: Routes = [
    {
        path: '',
        component: DashboardComponent,
    },
    {
        path: 'heroes',
        component: HeroesComponent
    },
    {
        path: 'maps',
        component: MapsComponent
    }
];


@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class HeroesDataAdminRoutingModule { }
