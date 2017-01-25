import { Component } from '@angular/core';
import { LoginService, AuthenticatedUser } from '../../modules/users/shared/services/login.service';

@Component({
  selector: 'home-screen',
  templateUrl: './home-screen.component.html',
  styleUrls: ['./home-screen.component.css']
})
export class HomeScreenComponent {
  public user: AuthenticatedUser;
  constructor(
    private loginService: LoginService
  ) {
    loginService.user.subscribe((user) => {
      this.user = user;
    });
  }

}
