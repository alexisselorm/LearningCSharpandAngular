import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
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

  getPageEvent() {
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = 0;
    pageEvent.pageSize = 10;
    this.getCities(pageEvent);
  }

  getCities(event: PageEvent) {
    let url = environment.baseUrl + 'api/Cities';
    let params = new HttpParams().set("pageIndex", event.pageIndex.toString()).set("pageSize", event.pageSize.toString());
    this.http.get<any>(url, {params}).subscribe((result) => {
      this.paginator.length = result.totalCount;
      this.paginator.pageIndex = result.pageIndex;
      this.paginator.pageSize = result.pageSize;
      this.cities = new MatTableDataSource<City>(result.data)
      /*this.cities.paginator = this.paginator;*/
    }, error => console.log(error));
  }


  ngOnInit(): void {
    this.getPageEvent();
    }
}
