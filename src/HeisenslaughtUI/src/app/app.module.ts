import 'hammerjs';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { MaterialModule } from '@angular/material';
import { MdSidenavModule } from '@angular/material/sidenav';
import { CovalentCoreModule } from '@covalent/core';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

import { HomeScreenComponent } from './screens/home-screen/home-screen.component';
import { NotFoundScreenComponent } from './screens/not-found-screen/not-found-screen.component';
import { AppCommonModule } from './modules/app-common/app-common.module';

import { LoginService } from './modules/users/shared/services/login.service';

@NgModule({
  declarations: [
    AppComponent,
    HomeScreenComponent,
    NotFoundScreenComponent
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
  providers: [LoginService],
  bootstrap: [AppComponent]
})
export class AppModule { }
