import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService, private router: Router) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    //GET THE AUTH TOKEN
    var token = this.authService.getToken();

    //if the token is present, clone the request
    //replacing the the original headers with the auth token
    if (token) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` },
      });
    }

    //send the request to the next handler
    return next.handle(req).pipe(
      catchError((error) => {
        //Perform Logount on 401 - Unauthorized Http response errors
        if (error instanceof HttpErrorResponse && error.status === 401) {
          this.authService.logout();
          this.router.navigate(['login']);
        }
        return throwError(error);
      })
    );
  }
}
