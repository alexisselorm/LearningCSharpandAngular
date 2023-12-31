import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { City } from './city';
import { ApiResult, BaseService } from '../base.service';
import { Observable } from 'rxjs';
import { Country } from '../countries/country';
@Injectable({ providedIn: 'root' })
export class CityService extends BaseService<City> {
  constructor(private httpClient: HttpClient) {
    super(httpClient);
  }
  url = this.getUrl('api/Cities/');

  getData(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null
  ): Observable<ApiResult<City>> {
    var params = new HttpParams()
      .set('pageIndex', pageIndex)
      .set('pageSize', pageSize)
      .set('sortColumn', sortColumn)
      .set('sortOrder', sortOrder);

    if (filterColumn && filterQuery) {
      params = params
        .set('filterColumn', filterColumn)
        .set('filterQuery', filterQuery);
    }
    return this.http.get<ApiResult<City>>(this.url, { params });
  }

  get(id: number): Observable<City> {
    return this.http.get<City>(this.url + id);
  }
  put(item: City): Observable<City> {
    return this.http.put<City>(this.url + item.id, item);
  }
  post(item: City): Observable<City> {
    return this.http.post<City>(this.url, item);
  }

  getCountries(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null
  ): Observable<ApiResult<Country>> {
    var url = this.getUrl('api/Countries');
    var params = new HttpParams()
      .set('pageIndex', pageIndex.toString())
      .set('pageSize', pageSize.toString())
      .set('sortColumn', sortColumn)
      .set('sortOrder', sortOrder);
    if (filterColumn && filterQuery) {
      params = params
        .set('filterColumn', filterColumn)
        .set('filterQuery', filterQuery);
    }
    return this.http.get<ApiResult<Country>>(url, { params });
  }
  isDupeCity(item: City): Observable<boolean> {
    var url = this.getUrl('api/Cities/isDupeCity');
    return this.http.post<boolean>(url, item);
  }
}
