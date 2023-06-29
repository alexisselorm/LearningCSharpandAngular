import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { CommonService } from 'src/app/services/common.service';
import { PostService } from 'src/app/services/post.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  posts: any[] = [];
  @ViewChild('addPost') addBtn!: ElementRef;
  @ViewChild('editPost') editBtn!: ElementRef;

  constructor(
    private postService: PostService,
    private auth: AuthService,
    private router: Router,
    private commonService: CommonService
  ) {
    this.commonService.postToEdit_Observable.subscribe((res) => {
      this.editBtn.nativeElement.click();
    });
  }
  resetPost() {
    this.commonService.setPostToAdd();
  }

  getPosts() {
    this.postService.getPostsByAuthor().subscribe({
      next: (result: any) => {
        this.posts = result['data'];
        console.log(this.posts);
      },
    });
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['']);
  }
  ngOnInit(): void {
    this.getPosts();

    this.commonService.postAdded_Observable.subscribe((res) => {
      this.getPosts();
    });
  }
}
