import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptors } from '@angular/common/http';
import { NgModule, isDevMode } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { CitiesComponent } from './cities/cities.component';
import { AngularMaterialModule } from './angular-material.module';
import { CountriesComponent } from './countries/countries.component';
import { CityEditComponent } from './city-edit/city-edit.component';
import { CountryEditComponent } from './country-edit/country-edit.component';
import { LoginComponent } from './login/login.component';
import { AuthInterceptor } from './auth/auth.interceptor';
import { provideServiceWorker } from '@angular/service-worker';
import { environment } from '../environments/environment';
import {
  ConnectionServiceModule,
  ConnectionServiceOptions,
  ConnectionServiceOptionsToken,
} from 'angular-connection-service';

//APOLLO - GRAPHQL
import { APOLLO_OPTIONS } from 'apollo-angular';
import { HttpLink } from 'apollo-angular/http';
import { InMemoryCache } from '@apollo/client/core';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NavMenuComponent,
    CitiesComponent,
    CountriesComponent,
    CityEditComponent,
    CountryEditComponent,
    LoginComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    AngularMaterialModule,
    ReactiveFormsModule,
    ConnectionServiceModule,
  ],
  providers: [
    provideHttpClient(withInterceptors([AuthInterceptor])),
    provideAnimationsAsync(),
    provideServiceWorker('ngsw-worker.js', {
      enabled: !isDevMode(),
      registrationStrategy: 'registerWhenStable:30000',
    }),
    {
      provide: ConnectionServiceOptionsToken,
      useValue: <ConnectionServiceOptions>{
        heartbeatUrl: environment.baseUrl + 'api/heartbeat',
      },
    },
    {
      provide: APOLLO_OPTIONS,
      useFactory: (httpLink: HttpLink) => {
        return {
          cache: new InMemoryCache({
            addTypename: false,
          }),
          link: httpLink.create({
            uri: environment.baseUrl + 'api/graphql/',
          }),
          defaultOptions: {
            watchQuery: { fetchPolicy: 'no-cache' },
            query: { fetchPolicy: 'no-cache' },
          },
        };
      },
      deps: [HttpLink],
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
