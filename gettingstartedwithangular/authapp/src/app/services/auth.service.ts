import { Injectable } from '@angular/core';
import * as auth0 from 'auth0-js';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private _idToken: string;
  private _accessToken: string;
  private _expiresAt: number;

  auth0 = new auth0.WebAuth({
    clientID: 'ibagtV8DSvEkQ9ClzcXYPcSqK2r0NtwD',
    domain: 'dev-uttho5cbbhvl2kh6.us.auth0.com',
    redirectUri: 'http://localhost:4200/',
    responseType: 'token id_token',
    scope: 'openid',
  });
  constructor(public router: Router) {
    this._idToken = '';
    this._accessToken = '';
    this._expiresAt = 0;
  }
  get accessToken(): string {
    return this._accessToken;
  }
  get idToken(): string {
    return this._idToken;
  }
  public login(): void {
    this.auth0.authorize();
  }

  public handleAuthentication() {
    this.auth0.parseHash((err: any, authResult: any) => {
      if (authResult && authResult.accessToken && authResult.idToken) {
        window.location.hash = '';
        this.localLogin(authResult);
        this.router.navigate(['/home']);
      } else if (err) {
        this.router.navigate(['/']);
        console.log(err);
      }
    });
  }

  private localLogin(authResult: any): void {
    // Set teh time the access token will expire
    const expiresAt = JSON.stringify(
      authResult.expiresIn * 1000 + Date.now()
    ) as string;
    this._accessToken = authResult.accessToken;
    this._idToken = authResult.idToken;
    // this._expiresAt = expiresAt;

    localStorage.setItem('access_token', this._accessToken);
    localStorage.setItem('id_token', this._idToken);
    localStorage.setItem('expires_at', expiresAt);
  }

  public renewTokens(): void {
    this.auth0.checkSession({}, (err, authResult: any): void => {
      if (authResult && authResult.accessToken && authResult.idToken) {
        this.localLogin(authResult);
      } else if (err) {
        alert(
          `Could not get a new token (${err.error}: ${err.error_description})`
        );

        console.log(err);
        this.logout();
      }
    });
  }

  public logout(): void {
    // Remove tokens and expiry time
    localStorage.removeItem('access_token');
    localStorage.removeItem('id_token');
    localStorage.removeItem('expires_at');
    this.router.navigate(['/']);
  }

  public isAuthenticated(): boolean {
    // Check whether the current time is past the access token expiration time
    const expiresAt = JSON.parse(localStorage.getItem('expires_at') || '{}');
    return new Date().getTime() < expiresAt;
  }
}
