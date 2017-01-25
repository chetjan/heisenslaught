import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '@angular/material';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminComponent } from './admin.component';
import { AppCommonModule } from '../app-common/app-common.module';
import { AdminDashboardComponent } from './screens/admin-dashboard/admin-dashboard.component';


@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    AppCommonModule,
    AdminRoutingModule
  ],
  declarations: [AdminComponent, AdminDashboardComponent]
})
export class AdminModule { }
