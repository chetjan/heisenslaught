import { Component, OnDestroy } from '@angular/core';
import { LoginService, AuthenticatedUser } from '../../modules/users/shared/services/login.service';
import { Subscription } from 'rxjs';
@Component({
  selector: 'login-bar',
  templateUrl: './login-bar.component.html',
  styleUrls: ['./login-bar.component.scss']
})
export class LoginBarComponent implements OnDestroy {

  private _userSub: Subscription;

  public user: AuthenticatedUser;

  constructor(private loginService: LoginService) {
    this._userSub = this.loginService.user.subscribe((user) => {
      this.user = user;
    });
  }

  public login() {
    this.loginService.battleNetLogin();
  }

  public logout() {
    this.loginService.logOut();
  }

  ngOnDestroy() {
    this._userSub.unsubscribe();
  }

}
