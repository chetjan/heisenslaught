import { Component, ElementRef } from '@angular/core';
import { LoginService } from './modules/users/shared/services/login.service';
import { Observable } from 'rxjs';
import { ServerEventService } from './services/signalr/signalr-server-event.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {


  constructor(
    private loginService: LoginService,
    private elm: ElementRef,
    serverEventService: ServerEventService
  ) {
    let user = JSON.parse((<HTMLElement>elm.nativeElement).getAttribute('authenticatedUser'));
    (<HTMLElement>elm.nativeElement).removeAttribute('authenticatedUser');
    loginService.initialize(user);
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
