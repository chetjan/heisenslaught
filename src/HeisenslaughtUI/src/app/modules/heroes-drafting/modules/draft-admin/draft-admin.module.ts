import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';
import { DraftAdminComponent } from './draft-admin.component';
import { HeroesDrafAdminRoutingModule } from './draft-admin-routing.module';
import { DraftAdminService } from './services/draft-admin.service';
import { AppCommonModule } from '../../../app-common/app-common.module';
import { ActiveDraftsComponent } from './active-drafts/active-drafts.component';
import { AllDraftsComponent } from './all-drafts/all-drafts.component';


@NgModule({
  imports: [
    CommonModule,
    HttpModule,
    AppCommonModule,
    HeroesDrafAdminRoutingModule
  ],
  declarations: [
    DraftAdminComponent,
    ActiveDraftsComponent,
    AllDraftsComponent
  ],
  providers: [DraftAdminService]
})
export class DraftAdminModule { }
