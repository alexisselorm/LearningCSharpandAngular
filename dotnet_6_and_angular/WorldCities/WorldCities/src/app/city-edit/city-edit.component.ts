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
import { BaseFormComponent } from '../base-form.component';
import { CityService } from '../cities/city.service';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss'],
})
export class CityEditComponent extends BaseFormComponent implements OnInit {
  constructor(
    private http: HttpClient,
    private router: Router,
    private cityService: CityService,
    private activatedRoute: ActivatedRoute
  ) {
    super();
  }
  ngOnInit(): void {
    this.loadCountries();
    this.loadData();
  }

  title?: string;
  city?: City;
  countries?: Country[];
  id!: number;

  override form: FormGroup = new FormGroup(
    {
      name: new FormControl('', Validators.required),
      lat: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/),
      ]),
      lon: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/),
      ]),
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

      return this.cityService.isDupeCity(city).pipe(
        map((result) => {
          return result ? { isDupeCity: true } : null;
        })
      );
    };
  }

  loadCountries() {
    this.cityService.getCountries(0, 9999, 'name', 'asc', null, null).subscribe(
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
      this.cityService.get(this.id).subscribe(
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
        this.cityService.put(city).subscribe(
          (result) => {
            console.log('City ' + city!.id + ' has been updated');

            //go back to the cities view
            this.router.navigate(['/cities']);
          },
          (error) => console.error(error)
        );
      } else {
        //CREATE A NEW City
        this.cityService.post(city).subscribe(
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
