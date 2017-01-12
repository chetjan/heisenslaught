import { Injectable } from '@angular/core';
import {
  CanActivate, CanLoad, CanActivateChild, Router,
  ActivatedRouteSnapshot, RouterStateSnapshot, Route, NavigationExtras
} from '@angular/router';
import { LoginService, AuthenticatedUser } from '../services/login.service';

const SUPER_USER_ROLE = 'SU';

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

  constructor(private loginService: LoginService,  private router: Router) { }

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
      url: window.location.pathname
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
