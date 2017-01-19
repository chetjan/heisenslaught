import 'hammerjs';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { MaterialModule } from '@angular/material';
import { MdSidenavModule } from '@angular/material/sidenav';
import { CovalentCoreModule } from '@covalent/core';

import { SignalRConnectionService } from './services/signalr/signalr-connection';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

import { HomeScreenComponent } from './screens/home-screen/home-screen.component';
import { NotFoundScreenComponent } from './screens/not-found-screen/not-found-screen.component';
import { AppCommonModule } from './modules/app-common/app-common.module';

import { LoginService } from './modules/users/shared/services/login.service';
import { LoginScreenComponent } from './screens/login-screen/login-screen.component';
import { AuthGuard } from './modules/users/shared/guards/auth-guard.service';
import { LoginBarComponent } from './components/login-bar/login-bar.component';

import { HeroesService } from './modules/heroes-data-service/services/heroes.service';
import { DraftService } from './modules/heroes-draft-service/services/draft.service';

import { ServerEventService } from './services/signalr/signalr-server-event.service';

@NgModule({
  declarations: [
    AppComponent,
    HomeScreenComponent,
    NotFoundScreenComponent,
    LoginScreenComponent,
    LoginBarComponent
  ],
  imports: [
    BrowserModule,
    HttpModule,
    FormsModule,
    MaterialModule.forRoot(),
    MdSidenavModule.forRoot(),
    CovalentCoreModule.forRoot(),
    AppCommonModule,
    AppRoutingModule
  ],
  exports: [],
  providers: [
    LoginService,
    AuthGuard,
    SignalRConnectionService,
    HeroesService,
    DraftService,
    ServerEventService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
