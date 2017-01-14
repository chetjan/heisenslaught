import { Component, OnDestroy } from '@angular/core';
import { LoginService } from '../../modules/users/shared/services/login.service';

import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login-screen',
  templateUrl: './login-screen.component.html',
  styleUrls: ['./login-screen.component.scss']
})
export class LoginScreenComponent implements OnDestroy {

  private _userSub: Subscription;

  constructor(
    private loginService: LoginService
  ) {
    this._userSub = loginService.user.subscribe((user) => {
      if (user) {
        this._userSub.unsubscribe();
        this._userSub = null;
        loginService.loginRedirect();
      }
    });
  }

  public battleNetLogin() {
    this.loginService.battleNetLogin();
  }

  public ngOnDestroy() {
    if (this._userSub) {
      this._userSub.unsubscribe();
      this._userSub = null;
    }
  }

}
