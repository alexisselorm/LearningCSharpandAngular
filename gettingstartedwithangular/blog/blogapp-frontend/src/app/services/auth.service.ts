import { Injectable } from '@angular/core';
import { User } from '../models/user.model';
import { HttpClient } from '@angular/common/http';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) {}

  login(user: User) {
    return this.http.post('/api/user/login', {
      username: user.getUsername(),
      password: user.getPassword(),
    });
  }

  setCurrentUser(user: User) {
    localStorage.setItem('currentUser', user.getUsername());
  }

  isAuthenticated(): boolean {
    const currentUser = localStorage.getItem('currentUser');
    return currentUser ? true : false;
  }

  logout() {
    localStorage.removeItem('currentUser');
  }
}
