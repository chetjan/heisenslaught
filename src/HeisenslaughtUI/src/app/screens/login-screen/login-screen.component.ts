import { Component } from '@angular/core';
import { LoginService } from '../../modules/users/shared/services/login.service';
@Component({
  selector: 'app-login-screen',
  templateUrl: './login-screen.component.html',
  styleUrls: ['./login-screen.component.scss']
})
export class LoginScreenComponent {

  constructor(
    private loginService: LoginService
  ) { }

  public battleNetLogin() {
    this.loginService.battleNetLogin().then((user) => {
      if (user) {
        this.onLoggedIn();
      }
    });
  }

  private onLoggedIn() {
    this.loginService.loginRedirect();
  }

}
