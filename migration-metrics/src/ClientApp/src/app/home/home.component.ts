
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

  ngOnInit(): void {
    this.http.get<string[]>(this.baseUrl + 'api/MonthlyCountStats/Categories').subscribe(result => {



      //var ctx = document.getElementById("theChart");
      this.categories = result;
      this.categories.forEach((category) => {

        this.http.get<VelocityData[]>(this.baseUrl + 'api/Velocity/' + category).subscribe(result => {



          //let htmlRef = this.elementRef.nativeElement.querySelector("#theChart");

          //var ctx = document.getElementById("theChart");
          this.velocityData[category] = result;


        });

        
      });
      

    });


   

    

  }

}


interface VelocityData {
  label: string;

  total: number;

  improvement: number;

  time: number;

  velocity: number;
  projectedEnd: string;

}

