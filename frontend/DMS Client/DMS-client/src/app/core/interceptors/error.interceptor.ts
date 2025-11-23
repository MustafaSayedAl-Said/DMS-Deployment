import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
} from '@angular/common/http';
import { NavigationExtras, Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private toast: ToastrService) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      //delay(1000),
      catchError((err: HttpErrorResponse) => {
        if (err.status === 400) {
          if (err.error.errors) {
            this.toast.error(err.error.errors[0]);
          } else {
            this.toast.error(err.error);
          }
        }
        if (err.status === 401) {
          this.toast.error(err.error.errors[0]);
        }
        if (err.status === 404) {
          console.error('Error 404: Resource not found.');
          this.router.navigate(['/not-found']);
        }
        if (err.status === 500) {
          const navigationExtra: NavigationExtras = {state : {error:err.error}}
          console.error('Error 500: Internal Server Error.');
          this.router.navigate(['/server-error', navigationExtra]);
        }
        return throwError(() => err.message || 'Server Not Found');
      })
    );
  }
}
