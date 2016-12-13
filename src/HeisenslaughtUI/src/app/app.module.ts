import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { AppComponent } from './app.component';

import { ChannelService, ChannelConfig } from './signalr/channel.service';
import { HeroIconComponent } from './hero-icon/hero-icon.component';
import { HeroesService } from './services/heroes.service';
import { DraftService } from './services/draft.service';

import { HeroSearchComponent } from './hero-search/hero-search.component';
import { HeroFilter } from './hero-search/hero-filter.pipe';
import { DraftComponent } from './draft/draft.component';
import { HeroPickComponent } from './hero-pick/hero-pick.component';


let channelConfig = new ChannelConfig();
channelConfig.url = 'http://34.194.9.34/signalr'; //'http://heisenslaught.com/signalr';
channelConfig.hub = 'DraftHub';


@NgModule({
  declarations: [
    AppComponent,
    HeroIconComponent,
    HeroSearchComponent,
    HeroFilter,
    DraftComponent,
    HeroPickComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule
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
