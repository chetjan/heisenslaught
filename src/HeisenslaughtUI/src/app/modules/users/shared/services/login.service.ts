import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { LoginWindow } from './login-window';
import { Subject, Observable, Subscriber } from 'rxjs';
import { Http } from '@angular/http';
import { AuthenticatedUser } from './types/user';

export * from './types/user';

@Injectable()
export class LoginService {

  private _initialized: boolean = false;
  private _battlenetLoginWindow: LoginWindow = new LoginWindow('/auth?provider=BattleNet', 500, 620);
  private _authenticatedUser: AuthenticatedUser;
  private _authenticatedUserSubject: Subject<AuthenticatedUser> = new Subject();
  private _authenticatedUserObservable: Observable<AuthenticatedUser>;

  public returnUrl: string;

  constructor(
    private http: Http,
    private router: Router
  ) {
    window.addEventListener('loginEvent', (evt: CustomEvent) => {
      if (evt.detail['success']) {
        this.setAuthenticatedUser(evt.detail['data']);
        this._battlenetLoginWindow.close();
      }
    });
  }


  public initialize(authenticatedUser: AuthenticatedUser): void {
    if (!this._initialized) {
      this.setAuthenticatedUser(authenticatedUser);
      this._initialized = true;
    }
  }

  private setAuthenticatedUser(authenticatedUser: AuthenticatedUser): void {
    this._authenticatedUser = authenticatedUser;
    this._authenticatedUserSubject.next(authenticatedUser);
  }

  public battleNetLogin(returnUrl = '/'): Promise<AuthenticatedUser> {
    return new Promise<AuthenticatedUser>((resolve, reject) => {
      this._battlenetLoginWindow.open(() => {
        resolve(this._authenticatedUser);
      }, returnUrl);
    });
  }

  public loginRedirect() {
    this.router.navigate([this.returnUrl || '/'], {
      preserveFragment: true,
      preserveQueryParams: true,
      replaceUrl: true
    });
  }

  public get user(): Observable<AuthenticatedUser> {
    if (!this._authenticatedUserObservable) {
      this._authenticatedUserObservable = new Observable((subscriber: Subscriber<AuthenticatedUser>) => {
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

  public logOut(): void {
    this.http.get('/auth/logout').toPromise().then(() => {
      this.setAuthenticatedUser(null);
    });
  }
}
