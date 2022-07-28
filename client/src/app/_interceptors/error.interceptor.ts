import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private route: Router, private toarts: ToastrService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(
        error => {
          if (error) {
            switch (error.status) {
              case 404:
                this.route.navigateByUrl("/not-found");
                break;
              case 400:
                if (error.error.errors) {
                  const modalStateError = [];
                  for (const key in error.error.errors) {
                    if (error.error.errors[key]) {
                      modalStateError.push(error.error.errors[key])
                    }
                  }
                  throw modalStateError.flat();
                }
                else {
                  this.toarts.error(error.statusText, error.status);
                }
                break;
              case 500:
                const navigationExtras: NavigationExtras = { state: { error: error.error } }
                this.route.navigateByUrl("/server-error", navigationExtras);
                break;
              case 401:
                this.toarts.error(error.statusText, error.status);
                break;
              default:
                this.toarts.error("Unexpected error !!!");
                console.log(error);
                break;
            }
          }
          return throwError(error);
        }
      )
    );
  }
}
