import { Component, Input, ElementRef } from '@angular/core';
import { LoginService } from './modules/users/shared/services/login.service';

interface IDraftState {
  TimeTeam0: number;
  TimeTeam1: number;
  TimeBonus: number;
  CurrentState: string;
  CurrentAction: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {



  constructor(private loginService: LoginService, private elm: ElementRef) {
    let user = JSON.parse((<HTMLElement>elm.nativeElement).getAttribute('authenticatedUser'));
    (<HTMLElement>elm.nativeElement).removeAttribute('authenticatedUser');
    loginService.initialize(user);
  }



  public login() {
    this.loginService.battleNetLogin();
  }

}
