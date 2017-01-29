import { Component, OnInit } from '@angular/core';
import { LoginService } from '../../modules/users/shared/services/login.service';

@Component({
  selector: 'permission-error-screen',
  templateUrl: './permission-error-screen.component.html',
  styleUrls: ['./permission-error-screen.component.scss']
})
export class PermissionErrorScreenComponent implements OnInit {

  constructor(
    private loginService: LoginService
  ) { }

  ngOnInit() {
  }

  public switchUser() {
    this.loginService.battleNetLoginSwitchUser().then(() => {
      this.loginService.loginRedirect();
    });
  }

}
