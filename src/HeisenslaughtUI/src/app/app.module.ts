import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';


import { AppComponent } from './app.component';

import { ChannelService, ChannelConfig } from './signalr/channel.service';
import { HeroesService } from './services/heroes.service';
import { DraftService } from './services/draft.service';

import { HeroIconComponent } from './hero-icon/hero-icon.component';
import { HeroSearchComponent } from './hero-search/hero-search.component';
import { HeroFilter } from './hero-search/hero-filter.pipe';
import { DraftComponent } from './draft/draft.component';
import { HeroPickComponent } from './hero-pick/hero-pick.component';
import { HomeComponent } from './home/home.component';
import { DraftConfigScreenComponent } from './draft-config-screen/draft-config-screen.component';


let channelConfig = new ChannelConfig();
channelConfig.url = 'http://34.194.9.34/signalr'; //'http://heisenslaught.com/signalr';
channelConfig.hub = 'DraftHub';


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
    DraftConfigScreenComponent
  ],
  imports: [
    BrowserModule,
    HttpModule,
    RouterModule.forRoot(appRoutes)
  ],
  providers: [
    HeroesService,
    DraftService,
    ChannelService,
    { provide: 'channel.config', useValue: channelConfig }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
