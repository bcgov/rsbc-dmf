import { Component, Inject, ElementRef, Input, InjectionToken } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';

export const BASE_URL = new InjectionToken<string>('BASE_URL');

@Component({
  selector: 'app-progress-table',
  templateUrl: './progress-table.component.html',
  styleUrls: ['./progress-table.component.css']
})
export class ProgressTableComponent {

  @Input() category!: string;

  @Input() title!: string;


  chartConfig: any;
  baseUrl: string;

  velocityData: any;
  constructor(private http: HttpClient, private elementRef: ElementRef, @Inject(BASE_URL) baseUrl: string) {
    this.baseUrl = baseUrl;
    this.velocityData = {};

  }

  ngOnInit(): void {

    this.http.get<VelocityData[]>(this.baseUrl + 'api/Velocity/' + this.category).subscribe(result => {



      //let htmlRef = this.elementRef.nativeElement.querySelector("#theChart");

      //var ctx = document.getElementById("theChart");
      this.velocityData = result;


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


