import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment.prod';
import { HealthCheckService, Result } from './health-check.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-health-check',
  templateUrl: './health-check.component.html',
  styleUrls: ['./health-check.component.css'],
})
export class HealthCheckComponent implements OnInit {
  public result: Observable<Result | null>;

  constructor(public service: HealthCheckService) {
    this.result = this.service.result;
  }

  onRefresh() {
    this.service.sendClientUpdate();
  }

  ngOnInit() {
    this.service.startConnection();
    this.service.addDataListeners();
  }
}
