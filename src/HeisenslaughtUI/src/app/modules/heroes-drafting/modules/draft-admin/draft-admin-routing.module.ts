import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DraftAdminComponent } from './draft-admin.component';
import { ActiveDraftsComponent } from './active-drafts/active-drafts.component';
import { AllDraftsComponent } from './all-drafts/all-drafts.component';

const routes: Routes = [
    {
        path: '',
        component: DraftAdminComponent,
    },
    {
        path: 'active',
        component: ActiveDraftsComponent
    },
    {
        path: 'all',
        component: AllDraftsComponent
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
export class HeroesDrafAdminRoutingModule { }
