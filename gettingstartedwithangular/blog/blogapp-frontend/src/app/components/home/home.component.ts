import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { PostService } from 'src/app/services/post.service';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  posts: any[] = [];
  constructor(
    public authService: AuthService,
    private router: Router,
    private postService: PostService
  ) {}

  getPosts() {
    this.postService.getAllPosts().subscribe({
      next: (result: any) => {
        this.posts = result['data'];
        console.log(this.posts);
      },
    });
  }
  login() {
    this.router.navigate(['']);
  }
  logout() {
    this.authService.logout();
    this.router.navigate(['']);
  }

  ngOnInit(): void {
    this.getPosts();
  }
}
