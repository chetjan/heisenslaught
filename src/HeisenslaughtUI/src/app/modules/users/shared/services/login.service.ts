import { Injectable, Optional } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { LoginWindow } from './login-window';
import { Subject, Observable, Subscriber } from 'rxjs';
import { Http } from '@angular/http';
import { AuthenticatedUser } from './types/user';
import { SignalRConnectionService } from '../../../../services/signalr/signalr-connection';

export * from './types/user';

@Injectable()
export class LoginService {

  private _initialized: boolean = false;
  private _battlenetLoginWindow: LoginWindow = new LoginWindow('/auth?provider=BattleNet', 500, 620);
  private _battlenetLoginSwitchWindow: LoginWindow = new LoginWindow('/auth/switch?provider=BattleNet', 500, 620);
  private _authenticatedUser: AuthenticatedUser;
  private _authenticatedUserSubject: Subject<AuthenticatedUser> = new Subject();
  private _authenticatedUserObservable: Observable<AuthenticatedUser>;
  private _returnUrl: string;
  private _currentUrl: string;
  private _isLogoutCheck: boolean;


  constructor(
    private http: Http,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    @Optional() private signalRService: SignalRConnectionService
  ) {
    if (window.localStorage) {
      this._returnUrl = window.localStorage.getItem('login.returnUrl');
      window.addEventListener('storage', (event: StorageEvent) => {
        if (event.key === 'login.userChanged') {
          this.handleExternalUserChange();
        }
      });
    }
    window.addEventListener('loginEvent', (evt: CustomEvent) => {
      if (evt.detail['success']) {
        this.setAuthenticatedUser(evt.detail['data']);
        this.reconnectSignalR();
        this._battlenetLoginWindow.close();
        this._battlenetLoginSwitchWindow.close();
      }
    });
    router.events.subscribe((navEvent) => {
      if (navEvent instanceof NavigationEnd) {
        if (!navEvent.urlAfterRedirects.startsWith('/login')) {
          this._currentUrl = navEvent.urlAfterRedirects;
        }
      }
    });

  }

  public set returnUrl(value: string) {
    this._returnUrl = value;
    if (window.localStorage) {
      if (value) {
        window.localStorage.setItem('login.returnUrl', value);
        console.log('returnUrl changed')
      } else {
        window.localStorage.removeItem('login.returnUrl');
        console.log('returnUrl removed')
      }
    }
  }

  public get returnUrl(): string {
    return this._returnUrl;
  }

  public get isLogoutCheck(): boolean {
    return this._isLogoutCheck;
  }

  public initialize(authenticatedUser: AuthenticatedUser): void {
    if (!this._initialized) {
      this.setAuthenticatedUser(authenticatedUser);
      this._initialized = true;
    }
  }

  public get isLoggedIn(): boolean {
    return !!this._authenticatedUser;
  }

  private handleExternalUserChange(): void {
    this.http.get('/auth/user').map(res => res.json()).toPromise().then((user: AuthenticatedUser) => {
      if (
        (!this._authenticatedUser && user) ||
        (this._authenticatedUser && !user) ||
        (this._authenticatedUser.id !== user.id)
      ) {
        this.reconnectSignalR();
        this.doLogoutCheck();
      }
      this.setAuthenticatedUser(user, true);
    });
  }

  private reconnectSignalR(): void {
    if (this.signalRService) {
      this.signalRService.reconnectAll(false);
    }
  }

  private connectSignalR(): void {
    if (this.signalRService) {
      this.signalRService.connectAll();
    }
  }

  private disconnectSignalR(notify = true): void {
    if (this.signalRService) {
      this.signalRService.disconnectAll(notify);
    }
  }

  private setAuthenticatedUser(authenticatedUser: AuthenticatedUser, noEvent?: boolean): void {
    this._authenticatedUser = authenticatedUser;
    if (!noEvent && window.localStorage) {
      window.localStorage.setItem('login.userChanged', new Date().getTime().toString());
    }
    this._authenticatedUserSubject.next(authenticatedUser);
  }

  public battleNetLogin(returnUrl = '/'): Promise<AuthenticatedUser> {
    return new Promise<AuthenticatedUser>((resolve, reject) => {
      this._battlenetLoginWindow.open(() => {
        resolve(this._authenticatedUser);
      }, returnUrl);
    });
  }

  public battleNetLoginSwitchUser(returnUrl = '/'): Promise<AuthenticatedUser> {
    return new Promise<AuthenticatedUser>((resolve, reject) => {
      this._battlenetLoginSwitchWindow.open(() => {
        resolve(this._authenticatedUser);
      }, returnUrl);
    });
  }

  public loginRedirect() {
    this._isLogoutCheck = false;
    this.router.navigate([this.returnUrl || '/'], {
      preserveFragment: true,
      preserveQueryParams: true,
      replaceUrl: true
    });
    this.returnUrl = undefined;
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

  public logOut(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.disconnectSignalR();
      let p = this.http.get('/auth/logout').toPromise();
      p.then(() => {
        this.setAuthenticatedUser(null);
        this.connectSignalR();
        this.doLogoutCheck();
        resolve();
      }, (err) => {
        reject(err);
      });
    });
  }

  private doLogoutCheck(): void {
    this.returnUrl = this._currentUrl;
    this._isLogoutCheck = true;
    this.router.navigate(['/login'], {
      preserveFragment: true,
      preserveQueryParams: true,
      replaceUrl: true
    });
  }
}
