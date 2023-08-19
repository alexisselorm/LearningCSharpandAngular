import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  AsyncValidatorFn,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { City } from '../cities/city';
import { Country } from '../countries/country';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss'],
})
export class CityEditComponent implements OnInit {
  constructor(
    private http: HttpClient,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}
  ngOnInit(): void {
    this.loadCountries();
    this.loadData();
  }

  title?: string;
  city?: City;
  countries?: Country[];
  id?: number;

  form: FormGroup = new FormGroup(
    {
      name: new FormControl('', Validators.required),
      lat: new FormControl('', Validators.required),
      lon: new FormControl('', Validators.required),
      countryId: new FormControl('', Validators.required),
    },
    null,
    this.isDupeCity()
  );

  //Custome async validator to check if a city has the same name, lat, and lon properties
  isDupeCity(): AsyncValidatorFn {
    return (
      control: AbstractControl
    ): Observable<{ [key: string]: any } | null> => {
      let city = <City>{};
      let url = environment.baseUrl + 'api/Cities/IsDupeCity';
      city.id = this.id ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lat = this.form.controls['lat'].value;
      city.lon = this.form.controls['lon'].value;
      city.countryId = this.form.controls['countryId'].value;

      return this.http.post<boolean>(url, city).pipe(
        map((result) => {
          return result ? { isDupeCity: true } : null;
        })
      );
    };
  }

  loadCountries() {
    let url = environment.baseUrl + 'api/countries';
    var params = new HttpParams()
      .set('pageIndex', 0)
      .set('pageSize', 9999)
      .set('sortColumn', 'name');

    this.http.get<any>(url, { params }).subscribe(
      (result) => {
        this.countries = result.data;
      },
      (error) => console.error(error)
    );
  }

  loadData() {
    //retrieve id from the id parameter
    let idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    let url = environment.baseUrl + 'api/Cities/';

    if (this.id) {
      // If Id exists Edit the city
      //fetch the city fro the server
      url = url + this.id;
      this.http.get<City>(url).subscribe(
        (response) => {
          this.city = response;
          this.title = 'Edit - ' + this.city.name;

          //update the form with the city value
          this.form.patchValue(this.city);
        },
        (error) => console.error(error)
      );
    } else {
      // Add a new city
      this.title = 'Create a new city';
    }
  }

  onSubmit() {
    let city = this.id ? this.city : <City>{};
    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = this.form.controls['countryId'].value;

      let url = environment.baseUrl + 'api/cities/';
      if (this.id) {
        //EDIT MODE
        url = url + city.id;
        this.http.put<City>(url, city).subscribe(
          (result) => {
            console.log('City ' + city!.id + ' has been updated');

            //go back to the cities view
            this.router.navigate(['/cities']);
          },
          (error) => console.error(error)
        );
      } else {
        //CREATE A NEW City
        this.http.post<City>(url, city).subscribe(
          (result) => {
            console.log('City' + result.id + 'has been created');

            //go back to the cities view
            this.router.navigate(['/cities']);
          },
          (error) => console.error(error)
        );
      }
    }
  }
}
