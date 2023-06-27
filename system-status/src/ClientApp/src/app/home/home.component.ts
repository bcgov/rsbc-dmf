
import { Component, Inject, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  baseUrl: string;

  categories!: string[];

  velocityData: any;
  constructor(private http: HttpClient, private elementRef: ElementRef, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.velocityData = {};


  }




}

