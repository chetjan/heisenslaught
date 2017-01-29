import 'hammerjs';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { MaterialModule } from '@angular/material';
import { MdSidenavModule } from '@angular/material/sidenav';
import { FlexLayoutModule } from '@angular/flex-layout';

import { SignalRConnectionService } from './services/signalr/signalr-connection';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

import { HomeScreenComponent } from './screens/home-screen/home-screen.component';
import { NotFoundScreenComponent } from './screens/not-found-screen/not-found-screen.component';
import { AppCommonModule } from './modules/app-common/app-common.module';
import { HeroesCommonModule } from './modules/heroes-common/heroes-common.module';

import { LoginService } from './modules/users/shared/services/login.service';
import { LoginScreenComponent } from './screens/login-screen/login-screen.component';
import { PermissionErrorScreenComponent } from './screens/permission-error-screen/permission-error-screen.component';
import { AuthGuard } from './modules/users/shared/guards/auth-guard.service';
import { LoginBarComponent } from './components/login-bar/login-bar.component';

import { HeroesService } from './modules/heroes-data-service/services/heroes.service';
import { DraftHubService } from './modules/heroes-draft-service/services/draft.service';
import { DraftService } from './modules/heroes-draft-service/services/draft-api.service';
import { ServerEventService } from './services/signalr/signalr-server-event.service';

@NgModule({
  declarations: [
    AppComponent,
    HomeScreenComponent,
    NotFoundScreenComponent,
    LoginScreenComponent,
    LoginBarComponent,
    PermissionErrorScreenComponent
  ],
  imports: [
    BrowserModule,
    HttpModule,
    FormsModule,
    MaterialModule.forRoot(),
    MdSidenavModule.forRoot(),
    FlexLayoutModule.forRoot(),
    AppCommonModule,
    HeroesCommonModule,
    AppRoutingModule
  ],
  exports: [],
  providers: [
    LoginService,
    AuthGuard,
    SignalRConnectionService,
    HeroesService,
    DraftHubService,
    DraftService,
    ServerEventService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
