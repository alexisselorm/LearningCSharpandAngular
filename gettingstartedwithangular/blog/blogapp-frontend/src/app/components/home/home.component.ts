import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  constructor(public authService: AuthService, private router: Router) {}

  login() {
    this.router.navigate(['']);
  }
  logout() {
    this.authService.logout();
    this.router.navigate(['']);
  }
}
