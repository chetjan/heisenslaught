import { Component, ElementRef } from '@angular/core';
import { LoginService } from './modules/users/shared/services/login.service';
import { Observable } from 'rxjs';


import { SignalRConnection, SignalRConnectionState } from './services/signalr/signalr-connection';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {


  constructor(
    private loginService: LoginService,
    private elm: ElementRef,
    signalR: SignalRConnection
  ) {
    let user = JSON.parse((<HTMLElement>elm.nativeElement).getAttribute('authenticatedUser'));
    (<HTMLElement>elm.nativeElement).removeAttribute('authenticatedUser');
    loginService.initialize(user);


    let sub = signalR.subscribe((state: SignalRConnectionState) => {
      console.log('state', SignalRConnectionState[state]);
    });

    setTimeout(() => {
     // signalR.reconnect();
    }, 5000);

  }

  public get user(): Observable<any> {
    return this.loginService.user;
  }

  public login() {
    this.loginService.battleNetLogin();
  }

  public logout() {
    this.loginService.logOut();
  }

}
