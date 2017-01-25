import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AdminComponent } from './admin.component';
import { AdminDashboardComponent } from './screens/admin-dashboard/admin-dashboard.component';

const routes: Routes = [
    {
        path: '',
        component: AdminComponent,
        children: [
            {
                path: '',
                component: AdminDashboardComponent,
                data: {
                    navigation: { label: 'Dashboard', order: 0 }
                },
            },
            {
                path: 'draft',
                component: AdminDashboardComponent,
                data: {
                    navigation: { label: 'Drafts' }
                },
            },
            {
                path: 'users',
                component: AdminDashboardComponent,
                data: {
                    navigation: { label: 'Users', order: 1 }
                },
            },
        ]

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
export class AdminRoutingModule { }
