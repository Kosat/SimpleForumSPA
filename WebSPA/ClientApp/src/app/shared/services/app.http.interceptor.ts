import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpSentEvent,
  HttpHeaderResponse,
  HttpProgressEvent,
  HttpResponse,
  HttpUserEvent,
  HttpErrorResponse,
  HttpEvent
} from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Router, ActivatedRouteSnapshot, NavigationExtras } from '@angular/router';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/finally';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/take';
import { Logger } from 'angular2-logger/core';
import { AuthService } from './auth.service';

@Injectable()
export class AppHttpInterceptor implements HttpInterceptor {

  isRefreshingToken = false;
  tokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>(null);

  constructor(
    private logger: Logger,
    private authService: AuthService,
    private router: Router) { }

  addToken(req: HttpRequest<any>, token: string): HttpRequest<any> {
    if (token) {
      return req.clone({ setHeaders: { Authorization: 'Bearer ' + token } });
    }
    // Workaround for IdentityServer4.
    return req.clone({ setHeaders: { Authorization: '' } });
  }

  intercept(req: HttpRequest<any>, next: HttpHandler)
    : Observable<any> {
    this.logger.debug(`Http request ${req.url} intercepted...`);

    return next.handle(this.addToken(req, this.authService.GetToken()))
      .catch(error => {
        if (error instanceof HttpErrorResponse) {
          switch ((<HttpErrorResponse>error).status) {
            case 400:
              return this.handle400Error(error);
            case 401:
              return this.handle401Error(req, next);
          }
          const navExtras: NavigationExtras = {
            queryParams: { code: error.status, message: error.message }
          };
          this.router.navigate(['/error'], navExtras);
          return Observable.empty();
        } else {
          return Observable.throw(error);
        }
      });
  }

  handle400Error(error) {

    if (error && error.status === 400 && error.error && error.error.error === 'invalid_grant') {
      // The token is no longer valid.
      // TODO
    }

    return Observable.throw(error);
  }

  handle401Error(req: HttpRequest<any>, next: HttpHandler) {

    const navExtras: NavigationExtras = {
      queryParams: { code: 401, message: 'You are not authorized to access this page.' }
    };
    this.router.navigate(['/error'], navExtras);
    return  Observable.empty();
  }

}
