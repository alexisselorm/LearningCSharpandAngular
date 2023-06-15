import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ReqresService } from 'src/app/services/reqres.service';
import { User } from 'src/app/user';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  constructor(private reqresServer: ReqresService, private router: Router) {
    this.getUsers();
  }
  users: User[] = [];
  loading: boolean = false;

  getUsers() {
    this.loading = true;
    this.reqresServer.getUsers().subscribe(
      (res: User[]) => {
        this.users = res;
        this.loading = false;
        // console.log(res);
      },
      (err) => {
        console.error(err);
      }
    );
  }
  userDetails(id: number) {
    this.router.navigate(['user', id]);
  }

  addUser(): void {
    this.router.navigate(['add']);
  }
  deleteUser(user: User) {
    this.users = this.users.filter((user) => user !== user);
    this.reqresServer.deleteUser(user).subscribe();
  }
}
