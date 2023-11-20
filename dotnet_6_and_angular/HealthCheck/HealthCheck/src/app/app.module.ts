import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {ServiceWorkerModule} from '@angular/service-worker';
import {environment} from "../environments/environment";
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { AppRoutingModule } from './app-routing.module';
import { HealthCheckComponent } from './health-check/health-check.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatToolbarModule } from "@angular/material/toolbar"

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    FetchDataComponent,
    NavMenuComponent,
    HealthCheckComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    ServiceWorkerModule.register('ngsw-worker.js',{
      enabled:environment.production,
      //Register the service worker as soon as the app is stable
      //or after 30 seconds (whichever comes first)
      registrationStrategy:'registerWhenStable:30000'
    })
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
