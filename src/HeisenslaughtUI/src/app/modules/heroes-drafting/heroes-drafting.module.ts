import { NgModule, ValueProvider } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MdCardModule } from '@angular/material/card';
import { MdCheckboxModule } from '@angular/material/checkbox';
import { MdInputModule } from '@angular/material/input';
import { MdSelectModule } from '@angular/material/select';
import { MdRadioModule } from '@angular/material/radio';
import { MdListModule } from '@angular/material/list';
import { MdIconModule } from '@angular/material/icon';
import { MdButtonToggleModule } from '@angular/material/button-toggle';

import { HeroesDraftingRoutingModule } from './heroes-drafting-routing.module';

import { HeroIconComponent, HeroPickComponent, HeroSearchComponent, TeamHeroPicksComponent, HeroFilter } from './components/components';
import { HeroesCommonModule } from '../heroes-common/heroes-common.module';
import { AppCommonModule } from '../app-common/app-common.module';
import { HeroesDraftingComponent } from './heroes-drafting.component';
import { DraftConfigScreenComponent } from './screens/draft-config-screen/draft-config-screen.component';
import { DraftScreenComponent } from './screens/draft-screen/draft-screen.component';
import { DraftConnectionStatusComponent } from './components/draft-connection-status/draft-connection-status.component';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    MdCardModule,
    MdCheckboxModule,
    MdInputModule,
    MdSelectModule,
    MdRadioModule,
    MdListModule,
    MdIconModule,
    MdButtonToggleModule,
    HeroesCommonModule,
    AppCommonModule,
    HeroesDraftingRoutingModule
  ],
  declarations: [
    HeroFilter,
    HeroIconComponent,
    HeroPickComponent,
    HeroSearchComponent,
    TeamHeroPicksComponent,
    HeroesDraftingComponent,
    DraftConfigScreenComponent,
    DraftScreenComponent,
    DraftConnectionStatusComponent
  ],
  providers: [

  ]
})
export class HeroesDraftingModule { }
