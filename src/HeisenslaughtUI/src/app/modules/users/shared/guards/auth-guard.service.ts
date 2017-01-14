import { Injectable } from '@angular/core';
import {
  CanActivate, CanLoad, CanActivateChild, Router, NavigationCancel,
  ActivatedRouteSnapshot, RouterStateSnapshot, Route, NavigationExtras
} from '@angular/router';
import { LoginService, AuthenticatedUser } from '../services/login.service';

const SUPER_USER_ROLE = 'SU';
const AUTHGUARD_CANCEL_REASON_REGEX = /Cannot load children because the guard of the route "path: '(.*?)'" returned false/;
const AUTHGUARD_RETURN_URL_PLACEHOLDER_REGEX = /^!!CANCELED_PATH!!:(.*)$/;

interface LoginParams {
  url: string;
  extras?: NavigationExtras;
}

@Injectable()
export class AuthGuard implements CanActivate, CanLoad, CanActivateChild {

  public static anyRole(...roles: string[]) {
    return (user: AuthenticatedUser) => {
      for (let i = 0; i < roles.length; i++) {
        if (user.roles && user.roles.indexOf(roles[i]) !== -1) {
          return true;
        }
      }
      return false;
    };
  }

  public static allRoles(...roles: string[]) {
    return (user: AuthenticatedUser) => {
      for (let i = 0; i < roles.length; i++) {
        if (!user.roles || user.roles.indexOf(roles[i]) === -1) {
          return false;
        }
      }
      return true;
    };
  }

  public static noRoles(...roles: string[]) {
    return (user: AuthenticatedUser) => {
      for (let i = 0; i < roles.length; i++) {
        if (user.roles && user.roles.indexOf(roles[i]) !== -1) {
          return false;
        }
      }
      return true;
    };
  }

  constructor(private loginService: LoginService, private router: Router) {
    // workaround to get canLoad full requested path
    router.events.subscribe((event) => {
      if (event instanceof NavigationCancel) {
        let resonResult = AUTHGUARD_CANCEL_REASON_REGEX.exec(event.reason);
        let placeholderResult = AUTHGUARD_RETURN_URL_PLACEHOLDER_REGEX.exec(this.loginService.returnUrl);
        if (resonResult && placeholderResult && resonResult[1] === placeholderResult[1]) {
          this.loginService.returnUrl = event.url;
        }
      }
    });
  }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
    return this.checkAccess(route, {
      url: state.url,
      extras: {
        queryParams: route.queryParams,
        fragment: route.fragment
      }
    });
  }

  public canLoad(route: Route): Promise<boolean> {
    return this.checkAccess(route, {
      url: '!!CANCELED_PATH!!:' + route.path
    });
  }

  public canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
    return this.checkAccess(childRoute, {
      url: state.url,
      extras: {
        queryParams: childRoute.queryParams,
        fragment: childRoute.fragment
      }
    });
  }

  protected checkAccess(route: ActivatedRouteSnapshot | Route, loginParams: LoginParams): Promise<boolean> {
    return new Promise<boolean>((resolve, reject) => {
      let sub = this.loginService.user.subscribe((user) => {
        if (!user) {
          this.gotoLogin(loginParams);
          resolve(false);
        } else {
          let roleCheck = route.data ? route.data['checkRoles'] : null;
          if (!roleCheck) {
            resolve(true);
          } else {
            if (user.roles.indexOf(SUPER_USER_ROLE) !== -1) {
              resolve(true);
            } else {
              let roleFn = roleCheck;
              if (Array.isArray(roleCheck)) {
                roleFn = AuthGuard.anyRole.apply(null, roleCheck);
              }
              if (roleFn && typeof (roleFn) === 'function') {
                if (roleFn(user)) {
                  resolve(true);
                } else {
                  this.gotoLogin(loginParams);
                  resolve(false);
                }
              } else {
                reject(new Error('checkRoles must be a function or array of roles.'));
              }
            }
          }
        }
        sub.unsubscribe();
      });
    });
  }

  protected gotoLogin(params: LoginParams) {
    this.loginService.returnUrl = params.url;
    this.router.navigate(['/login'], params.extras);
  }
}
