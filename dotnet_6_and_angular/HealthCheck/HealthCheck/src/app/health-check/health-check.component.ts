import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-health-check',
  templateUrl: './health-check.component.html',
  styleUrls: ['./health-check.component.css'],
})
export class HealthCheckComponent implements OnInit {
  public result?: Result;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<Result>(environment.baseUrl + 'api/health').subscribe(
      (result) => {
        this.result = result;
        console.log(this.result.checks);
      },
      (error) => console.log(error)
    );
  }
}

interface Result {
  checks: Check[];
  totalStatus: string;
  totalResponse: number;
}
interface Check {
  name: string;
  responseTime: number;
  status: string;
  description: string;
}
