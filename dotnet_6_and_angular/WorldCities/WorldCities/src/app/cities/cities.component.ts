import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';
import { City } from './city';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss']
})
export class CitiesComponent implements OnInit {
  protected cities!: City[]
  public displayedColumns: string[]=["id","name","lat","lon"]

  constructor(private http: HttpClient) { }

  getCities() {
    this.http.get<City[]>(environment.baseUrl + 'api/Cities').subscribe((result) => {
      this.cities = result
    }, error => console.log(error));
  }


  ngOnInit(): void {
    this.getCities();
    }
}
