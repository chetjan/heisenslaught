import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '@angular/material';
import { HeroesButtonComponent } from './components/heroes-button/heroes-button.component';
import { HeroInfoCardComponent } from './components/hero-info-card/hero-info-card.component';
import { MyJoinedDraftsPodComponent } from './components/my-joined-drafts-pod/my-joined-drafts-pod.component';
import { AppCommonModule } from '../app-common/app-common.module';
import { MyCreatedDraftsPodComponent } from './components/my-created-drafts-pod/my-created-drafts-pod.component';

@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    RouterModule,
    AppCommonModule
  ],
  declarations: [
    HeroesButtonComponent,
    HeroInfoCardComponent,
    MyJoinedDraftsPodComponent,
    MyCreatedDraftsPodComponent
  ],
  exports: [
    HeroesButtonComponent,
    HeroInfoCardComponent,
    MyJoinedDraftsPodComponent,
    MyCreatedDraftsPodComponent
  ]
})
export class HeroesCommonModule { }
