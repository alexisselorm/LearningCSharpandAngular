import { Component, Input } from '@angular/core';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css'],
})
export class PostComponent {
  @Input() post: any = {};
  @Input() read = false;
  @Input() admin = false;

  constructor(private commonService: CommonService) {}

  setPostToEdit(post: any) {
    this.commonService.setPostToEdit(post);
  }

  setPostToDelete(post: any) {
    this.commonService.setPostToDelete(post);
  }
}
