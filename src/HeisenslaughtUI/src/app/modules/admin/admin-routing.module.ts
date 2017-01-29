import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../users/shared/guards/auth-guard.service';
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
                canLoad: [AuthGuard],
                loadChildren: 'app/modules/heroes-drafting/modules/draft-admin/draft-admin.module#DraftAdminModule',
                data: {
                    navigation: {
                        label: 'Drafts',
                        showChildren: 1,
                        children: [
                            {
                                label: 'Active Drafts',
                                path: 'active'
                            },
                            {
                                label: 'All Drafts',
                                path: 'all'
                            }
                        ]
                    },
                    checkRoles: ['SU', 'Admin']
                }
            },
            {
                path: 'heroesdata',
                canLoad: [AuthGuard],
                loadChildren: 'app/modules/heroes-data-service/modules/heroes-data-admin/heroes-data-admin.module#HeroesDataAdminModule',
                data: {
                    navigation: {
                        label: 'Heroes Data',
                        showChildren: 1,
                        children: [
                            {
                                label: 'Heroes',
                                path: 'heroes'
                            },
                            {
                                label: 'Maps',
                                path: 'maps'
                            }
                        ]
                    },
                    checkRoles: ['SU', 'Admin']
                }
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
