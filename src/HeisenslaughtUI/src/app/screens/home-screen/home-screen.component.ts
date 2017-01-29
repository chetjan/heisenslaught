import { Component, OnDestroy } from '@angular/core';
import { LoginService, AuthenticatedUser } from '../../modules/users/shared/services/login.service';
import { Subscription } from 'rxjs';
@Component({
  selector: 'home-screen',
  templateUrl: './home-screen.component.html',
  styleUrls: ['./home-screen.component.css']
})
export class HomeScreenComponent implements OnDestroy {
  public user: AuthenticatedUser;
  public userSub: Subscription;
  constructor(
    private loginService: LoginService
  ) {

    this.userSub = loginService.user.subscribe((user) => {
      this.user = user;
    });
  }

  public ngOnDestroy() {
    if (this.userSub) {
      this.userSub.unsubscribe();

    }
  }

}
