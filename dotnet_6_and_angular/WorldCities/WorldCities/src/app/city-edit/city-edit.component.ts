import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { City } from '../cities/city';

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
    this.loadData();
  }

  title?: string;
  city?: City;
  id?: number;

  form: FormGroup = new FormGroup({
    name: new FormControl(''),
    lat: new FormControl(''),
    lon: new FormControl(''),
  });

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
