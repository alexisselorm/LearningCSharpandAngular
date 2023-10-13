import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

import { Country } from './country';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { CountryService } from './country.service';

@Component({
  selector: 'app-countries',
  templateUrl: './countries.component.html',
  styleUrls: ['./countries.component.scss'],
})
export class CountriesComponent implements OnInit {
  public displayedColumns: string[] = [
    'id',
    'name',
    'iso2',
    'iso3',
    'totCities',
  ];
  public countries!: MatTableDataSource<Country>;
  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = 'name';
  public defaultSortOrder: 'asc' | 'desc' = 'asc';

  defaultFilterColumn: string = 'name';
  filterQuery?: string;
  filterTextChanged: Subject<string> = new Subject<string>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private countryService: CountryService) {}

  getData(query?: string) {
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getCountries(pageEvent);
  }

  getCountries(event: PageEvent) {
    let sortColumn = this.sort ? this.sort.active : this.defaultSortColumn;
    let sortOrder = this.sort ? this.sort.direction : this.defaultSortOrder;
    let fillterColumn = this.filterQuery ? this.defaultFilterColumn : null;
    let filterQuery = this.filterQuery ? this.filterQuery : null;

    this.countryService
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
          this.countries = new MatTableDataSource<Country>(result.data);
        },
        (error) => console.error(error)
      );
  }

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
