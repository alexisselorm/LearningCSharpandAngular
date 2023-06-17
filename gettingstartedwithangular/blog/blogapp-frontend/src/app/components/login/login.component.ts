import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { User } from 'src/app/models/user.model';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  date = new Date();
  loginForm: FormGroup;
  submitted = false;
  loading = false;
  user: User = new User('', '');
  error: string = '';
  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });

    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/home']);
    }
  }

  get controls() {
    return this.loginForm.controls;
  }

  onSubmit() {
    this.submitted = true;
    if (this.loginForm.invalid) {
      return;
    }
    this.loading = true;
    this.user = new User(
      this.controls['username'].value,
      this.controls['password'].value
    );

    this.authService.login(this.user).subscribe({
      next: (result: any) => {
        console.log(result);

        if (result['status'] == 'success') {
          this.authService.setCurrentUser(this.user);
          this.router.navigate(['/home']);
        } else {
          this.error = 'Wrong username or password';
        }
      },
      error: (e) => console.log(e),
      complete: () => {
        this.loading = false;
        this.submitted = false;
        console.info('complete');
      },
    });
  }
  ngOnInit() {}
}
