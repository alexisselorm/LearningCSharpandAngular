import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  AsyncValidatorFn,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Country } from '../countries/country';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseFormComponent } from '../base-form.component';

@Component({
  selector: 'app-country-edit',
  templateUrl: './country-edit.component.html',
  styleUrls: ['./country-edit.component.scss'],
})
export class CountryEditComponent extends BaseFormComponent implements OnInit {
  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {
    super();
  }
  ngOnInit(): void {
    this.loadData();
  }
  title?: string;
  override form: FormGroup = this.fb.group({
    name: ['', Validators.required, this.isDupeField('name')],
    iso2: [
      '',
      [Validators.required, Validators.pattern(/^[a-zA-Z]{2}$/)],
      this.isDupeField('iso2'),
    ],
    iso3: [
      '',
      [Validators.required, Validators.pattern(/^[a-zA-Z]{3}$/)],
      this.isDupeField('iso3'),
    ],
  });

  country?: Country;
  countries?: Country[];
  id?: number;

  isDupeField(fieldName: string): AsyncValidatorFn {
    return (
      control: AbstractControl
    ): Observable<{ [key: string]: any } | null> => {
      var params = new HttpParams()
        .set('countryId', this.id ? this.id.toString() : '0')
        .set('fieldName', fieldName)
        .set('fieldValue', control.value);

      let url = environment.baseUrl + 'api/Countries/IsDupeField';
      return this.http.post<boolean>(url, null, { params }).pipe(
        map((result) => {
          return result ? { isDupeField: true } : null;
        })
      );
    };
  }

  loadData(): void {
    // /retrieve Id from 'id' parameter
    let idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    let url = environment.baseUrl + 'api/Countries/' + this.id;
    if (this.id) {
      //EDIT MODE
      // FETCH COUNTRY DATA FROM SERVER
      this.http.get<Country>(url).subscribe(
        (result) => {
          this.country = result;
          this.title = 'Edit - ' + this.country.name;

          //update the form values with the details of the country
          this.form.patchValue(this.country);
        },
        (error) => console.error(error)
      );
    } else {
      // CREATE c
      this.title = 'Create a new country';
    }
  }

  onSubmit() {
    let country = this.id ? this.country : <Country>{};
    let url = environment.baseUrl + 'api/Countries/';
    if (country) {
      country.name = this.form.controls['name'].value;
      country.iso2 = this.form.controls['iso2'].value;
      country.iso3 = this.form.controls['iso3'].value;

      if (this.id) {
        //EDIT MODE
        url = url + country.id;
        this.http.put<Country>(url, country).subscribe(
          (result) => {
            console.log('Country' + country?.name + ' has been updated. ');

            this.router.navigate(['/countries']);
          },
          (error) => console.error(error)
        );
      } else {
        this.http.post<Country>(url, country).subscribe(
          (result) => {
            console.log('Country' + country?.name + ' has been created');

            this.router.navigate(['/countries']);
          },
          (error) => console.error(error)
        );
      }
    }
  }
}
