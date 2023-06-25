import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PostService {
  constructor(private http: HttpClient) {}

  getAllPosts() {
    return this.http.get('/api/post/getAllPosts', {});
  }
  getPostsByAuthor() {
    const currentUser = JSON.parse(
      localStorage.getItem('currentUser') as string
    );
    return this.http.post('/api/post/getPostsByAuthor', {
      author_id: currentUser.id,
    });
  }
}
