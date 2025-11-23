import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { delay, finalize, Observable } from 'rxjs';
import { LoaderService } from '../services/loader.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(private loaderService: LoaderService) {}
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (!req.url.includes('check')) {
      this.loaderService.loader();
    }

    return next.handle(req).pipe(
      //delay(1000),
      finalize(() => {
        this.loaderService.hidingLoader();
        console.log('Finished handling request:', req.url);
      })
    );
  }
}
