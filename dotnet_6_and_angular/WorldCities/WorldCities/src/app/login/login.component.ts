import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { BaseFormComponent } from '../base-form.component';
import { LoginResult } from '../auth/login-result';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginRequest } from '../auth/login-request';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent extends BaseFormComponent implements OnInit {
  title?: string;
  loginResult?: LoginResult;

  override form = new FormGroup({
    email: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
  });

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private auth: AuthService
  ) {
    super();
  }

  onSubmit() {
    var loginRequest = <LoginRequest>{};
    loginRequest.email = this.form.controls['email'].value!;
    loginRequest.password = this.form.controls['password'].value!;

    this.auth.login(loginRequest).subscribe(
      (result) => {
        console.log(result);
        this.loginResult = result;

        if (result.success && result.token) {
          this.router.navigate(['/']);
        }
      },
      (error) => {
        console.log(error);
        if (error.status == 401) {
          this.loginResult = error.error;
        }
      }
    );
  }

  ngOnInit(): void {}
}
