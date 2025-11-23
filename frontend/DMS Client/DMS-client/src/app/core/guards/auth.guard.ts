import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { map, Observable, of } from 'rxjs';
import { AccountService } from '../../account/account.service';
import { jwtDecode } from 'jwt-decode';
import { IUser } from '../../shared/Models/user';
@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private accountServices: AccountService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.accountServices.currentUser$.pipe(
      map((auth) => {
        const token = localStorage.getItem('token');

        if (auth) {
          console.log('User is authenticated');
          return true;
        }

        if (token) {
          try {
            const decodedToken: any = jwtDecode(token);

            // Check if the token is expired
            const currentTime = Date.now() / 1000;
            if (decodedToken.exp < currentTime) {
              console.log('Token is expired');
              this.router.navigate(['account/login'], {
                queryParams: { returnUrl: state.url },
              });
              return false;
            }

            // console.log('Token is valid');
            return true;
          } catch (error) {
            console.error('Invalid token format', error);
            this.router.navigate(['account/login'], {
              queryParams: { returnUrl: state.url },
            });
            return false;
          }
        }

        console.log('No user authenticated and no valid token');
        this.router.navigate(['account/login'], {
          queryParams: { returnUrl: state.url },
        });
        return false;
      })
    );
  }
}
