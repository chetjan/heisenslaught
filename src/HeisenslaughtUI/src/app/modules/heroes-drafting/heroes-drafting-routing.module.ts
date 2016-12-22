import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HeroesDraftingComponent } from './heroes-drafting.component';
import { DraftConfigScreenComponent } from './screens/draft-config-screen/draft-config-screen.component';
import { DraftScreenComponent } from './screens/draft-screen/draft-screen.component';

const draftRoutes: Routes = [
    {
        path: '',
        component: HeroesDraftingComponent,
        children: [
            { path: '', component: DraftConfigScreenComponent },
            { path: 'config/:id/:adminToken', component: DraftConfigScreenComponent },
            { path: ':id/:team', component: DraftScreenComponent },
            { path: ':id', component: DraftScreenComponent }
        ]

    }
];

@NgModule({
    imports: [
        RouterModule.forChild(draftRoutes)
    ],
    exports: [
        RouterModule
    ]
})
export class HeroesDraftingRoutingModule { }
