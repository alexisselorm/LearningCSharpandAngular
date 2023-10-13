import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { environment } from '../../environments/environment';
import { City } from './city';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { CityService } from './city.service';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss'],
})
export class CitiesComponent implements OnInit {
  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = 'name';
  public defaultSortOrder: 'asc' | 'desc' = 'asc';
  public defaultFilterColumn: string = 'name';
  filterQuery?: string;
  filterTextChanged: Subject<string> = new Subject<string>();
  protected cities!: MatTableDataSource<City>;
  public displayedColumns: string[] = [
    'id',
    'name',
    'lat',
    'lon',
    'countryName',
  ];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private cityService: CityService) {}

  getData(query?: string) {
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getCities(pageEvent);
  }

  getCities(event: PageEvent) {
    let url = environment.baseUrl + 'api/Cities';

    let sortColumn = this.sort ? this.sort.active : this.defaultSortColumn;
    let sortOrder = this.sort ? this.sort.direction : this.defaultSortOrder;
    let fillterColumn = this.filterQuery ? this.defaultFilterColumn : null;
    let filterQuery = this.filterQuery ? this.filterQuery : null;

    this.cityService
      .getData(
        event.pageIndex,
        event.pageSize,
        sortColumn,
        sortOrder,
        fillterColumn,
        filterQuery
      )
      .subscribe(
        (result) => {
          this.paginator.length = result.totalCount;
          this.paginator.pageIndex = result.pageIndex;
          this.paginator.pageSize = result.pageSize;
          this.cities = new MatTableDataSource<City>(result.data);
          /*this.cities.paginator = this.paginator;*/
        },
        (error) => console.log(error)
      );
  }

  //debounce filtertext changes
  onFilterTextChanged(filterText: string) {
    if (this.filterTextChanged.observers.length == 0) {
      this.filterTextChanged
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe((query) => {
          this.getData(query);
        });
    }
    this.filterTextChanged.next(filterText);
  }

  ngOnInit(): void {
    this.getData();
  }
}
