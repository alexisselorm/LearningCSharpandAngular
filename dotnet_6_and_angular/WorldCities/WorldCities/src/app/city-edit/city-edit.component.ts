import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { City } from '../cities/city';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss'],
})
export class CityEditComponent {
  constructor(
    private http: HttpClient,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {
    this.loadData();
  }

  title?: string;
  city?: City;

  form: FormGroup = new FormGroup({
    name: new FormControl(''),
    lat: new FormControl(''),
    lon: new FormControl(''),
  });

  loadData() {
    //retrieve id from the id parameter
    let idParam = this.activatedRoute.snapshot.paramMap.get('id');
    let id = idParam ? +idParam : 0;

    //fetch the city fro the server
    let url = environment.baseUrl + '/api/Cities/' + id;
    this.http.get<City>(url).subscribe(
      (response) => {
        this.city = response;
        this.title = 'Edit - ' + this.city.name;

        //update the form with the city value
        this.form.patchValue(this.city);
      },
      (error) => console.error(error)
    );
  }

  onSubmit() {
    let city = this.city;
    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;

      let url = environment.baseUrl + 'api/cities/' + city.id;
      this.http.put<City>(url, city).subscribe(
        (result) => {
          console.log('City ' + city!.id + ' has been updated');

          //go back to the cities view
          this.router.navigate(['/cities']);
        },
        (error) => console.error(error)
      );
    }
  }
}
