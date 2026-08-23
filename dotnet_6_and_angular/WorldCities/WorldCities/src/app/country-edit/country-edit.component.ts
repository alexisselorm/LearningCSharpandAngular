import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  AsyncValidatorFn,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Country } from '../countries/country';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseFormComponent } from '../base-form.component';
import { CountryService } from '../countries/country.service';

@Component({
  standalone: false,
  selector: 'app-country-edit',
  templateUrl: './country-edit.component.html',
  styleUrls: ['./country-edit.component.scss'],
})
export class CountryEditComponent extends BaseFormComponent implements OnInit {
  title?: string;
  override form: FormGroup = new FormGroup({});

  country?: Country;
  countries?: Country[];
  id?: number;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private countryService: CountryService,
    private http: HttpClient
  ) {
    super();
  }
  ngOnInit(): void {
    let idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;

    this.form = new FormGroup(
      {
        name: new FormControl('', Validators.required),
        iso2: new FormControl('', [
          Validators.required,
          Validators.pattern(/^[a-zA-Z]{2}$/),
        ]),
        iso3: new FormControl('', [
          Validators.required,
          Validators.pattern(/^[a-zA-Z]{3}$/),
        ]),
      },
      null,
      this.isDupeField('name')
    );

    this.loadData();
  }

  isDupeField(fieldName: string): AsyncValidatorFn {
    return (
      control: AbstractControl
    ): Observable<{ [key: string]: any } | null> => {
      return this.countryService
        .isDupeField(this.id ?? 0, fieldName, control.value)
        .pipe(
          map((result) => {
            return result ? { isDupeField: true } : null;
          })
        );
    };
  }

  loadData(): void {
    let url = environment.baseUrl + 'api/Countries/' + this.id;
    if (this.id) {
      this.countryService.get(this.id).subscribe(
        (result) => {
          this.country = result;
          this.title = 'Edit - ' + this.country.name;
          this.form.patchValue(this.country);
        },
        (error) => console.error(error)
      );
    } else {
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
        url = url + country.id;
        this.countryService.put(country).subscribe(
          (result) => {
            console.log('Country' + country?.name + ' has been updated. ');
            this.router.navigate(['/countries']);
          },
          (error) => console.error(error)
        );
      } else {
        this.countryService.post(country).subscribe(
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
