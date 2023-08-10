import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { environment } from '../../environments/environment.prod';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styleUrls: ['./fetch-data.component.css'],
})
export class FetchDataComponent {
  public forecasts?: WeatherForecast[];

  constructor(http: HttpClient) {
    http
      .get<WeatherForecast[]>(environment.baseUrl + 'api/weatherforecast')
      .subscribe(
        (result) => {
          this.forecasts = result;
          console.log(this.forecasts);
        },
        (error) => console.log(error)
      );
  }
}
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
