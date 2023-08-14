import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { environment } from '../../environments/environment';
import { City } from './city';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss']
})
export class CitiesComponent implements OnInit {
  protected cities!: MatTableDataSource<City>;
  public displayedColumns: string[]=["id","name","lat","lon"]
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(private http: HttpClient) { }

  getCities() {
    this.http.get<City[]>(environment.baseUrl + 'api/Cities').subscribe((result) => {
      this.cities = new MatTableDataSource<City>(result)
      this.cities.paginator = this.paginator;
    }, error => console.log(error));
  }


  ngOnInit(): void {
    this.getCities();
    }
}
