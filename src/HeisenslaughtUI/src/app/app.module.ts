import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { HeroesService } from './services/heroes.service';
import { DraftService } from './services/draft.service';

import { HeroIconComponent } from './hero-icon/hero-icon.component';
import { HeroSearchComponent } from './hero-search/hero-search.component';
import { HeroFilter } from './hero-search/hero-filter.pipe';
import { DraftComponent } from './draft/draft.component';
import { HeroPickComponent } from './hero-pick/hero-pick.component';
import { HomeComponent } from './home/home.component';
import { DraftConfigScreenComponent } from './draft-config-screen/draft-config-screen.component';
import { TeamHeroPicksComponent } from './team-hero-picks/team-hero-picks.component';
import { HeroesButtonComponent } from './heroes-button/heroes-button.component';


const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'admin', component: DraftConfigScreenComponent },
  { path: 'draft/:id/:team', component: DraftComponent },
  { path: 'draft/:id', component: DraftComponent },
  { path: 'draft', component: DraftComponent }
];


@NgModule({
  declarations: [
    AppComponent,
    HeroIconComponent,
    HeroSearchComponent,
    HeroFilter,
    DraftComponent,
    HeroPickComponent,
    HomeComponent,
    DraftConfigScreenComponent,
    TeamHeroPicksComponent,
    HeroesButtonComponent
  ],
  imports: [
    BrowserModule,
    HttpModule,
    FormsModule,
    RouterModule.forRoot(appRoutes)
  ],
  providers: [
    HeroesService,
    DraftService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
