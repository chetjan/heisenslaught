import { Injectable } from '@angular/core';
import { LoginWindow } from './login-window';
import { Subject, Observable, Subscriber } from 'rxjs';

@Injectable()
export class LoginService {

  private _initialized: boolean = false;
  private _battlenetLoginWindow: LoginWindow = new LoginWindow('/auth?provider=BattleNet', 500, 620);
  private _authenticatedUser: any;
  private _authenticatedUserSubject: Subject<any> = new Subject();
  private _authenticatedUserObservable: Observable<any>;

  constructor() {
    window.addEventListener('loginEvent', (evt: CustomEvent) => {
      console.log(evt);
    });
  }


  public initialize(authenticatedUser: any): void {
    if (!this._initialized) {
      this.setAuthenticatedUser(authenticatedUser);
      this._initialized = true;
    }
  }

  private setAuthenticatedUser(authenticatedUser: any): void {
    this._authenticatedUser = authenticatedUser;
    this._authenticatedUserSubject.next(authenticatedUser);
  }

  public battleNetLogin(returnUrl = '/') {
    this._battlenetLoginWindow.open(returnUrl);
  }

  public get user(): Observable<any> {
    if (!this._authenticatedUserObservable) {
      this._authenticatedUserObservable = new Observable((subscriber: Subscriber<any>) => {
        let sub = this._authenticatedUserSubject.subscribe((next) => {
          subscriber.next(next);
        }, (err) => {
          subscriber.error(err);
        }, () => {
          subscriber.complete();
        });
        subscriber.add(() => {
          sub.unsubscribe();
        });
        subscriber.next(this._authenticatedUser);
      });
    }
    return this._authenticatedUserObservable;
  }

}
