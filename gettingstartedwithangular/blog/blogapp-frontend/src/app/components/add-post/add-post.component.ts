import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Post } from 'src/app/models/post.model';
import { AddPostService } from 'src/app/services/add-post.service';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'app-add-post',
  templateUrl: './add-post.component.html',
  styleUrls: ['./add-post.component.css'],
})
export class AddPostComponent {
  postForm: FormGroup;
  submitted: boolean = false;
  post: Post = new Post('', '');
  @ViewChild('closeBtn') closeBtn!: ElementRef;

  constructor(
    private formBuilder: FormBuilder,
    private addPostService: AddPostService,
    private commonService: CommonService
  ) {
    this.postForm = this.formBuilder.group({
      title: ['', Validators.required],
      text: ['', Validators.required],
    });
  }

  get controls() {
    return this.postForm.controls;
  }
  onSubmit() {
    this.submitted = true;

    if (this.postForm.invalid) {
      return;
    }
    this.post = new Post(this.postForm.value.title, this.postForm.value.text);
    this.addPostService.addPost(this.post).subscribe({
      next: (result: any) => {
        if (result['status'] == 'success') {
          this.closeBtn.nativeElement.click();
          this.commonService.notifyPostAddition('');
        } else {
          console.log('Error adding post');
        }
      },
      error: (error: any) => {
        console.error('Error adding post. Error subscriber');
      },
      complete: () => {
        this.submitted = false;
        console.info('Completed successfully');
      },
    });
  }
}
