import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';
import { MaterialModule } from '@angular/material';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HeroesComponent } from './heroes/heroes.component';
import { MapsComponent } from './maps/maps.component';
import { HeroesDataAdminRoutingModule } from './heroes-data-admin-routing.module';
import { AppCommonModule } from '../../../app-common/app-common.module';
import { HeroesDataAdminService } from './services/heroes-data.service';
import { HeroesDataImporterService } from './services/heroes-data-importer.service';

@NgModule({
  imports: [
    CommonModule,
    HttpModule,
    MaterialModule,
    AppCommonModule,
    HeroesDataAdminRoutingModule
  ],
  declarations: [
    DashboardComponent,
    HeroesComponent,
    MapsComponent
  ],
  providers: [
    HeroesDataAdminService,
    HeroesDataImporterService
  ]
})
export class HeroesDataAdminModule { }
