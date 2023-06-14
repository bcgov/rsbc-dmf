import { Component, Inject, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';
import Chart, { ChartConfiguration } from 'chart.js/auto';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
 
  public chart: any;

  chartConfig: any;
  baseUrl: string;

  constructor(private http: HttpClient, private elementRef: ElementRef, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

  }

  ngOnInit(): void {
    this.http.get<ChartConfiguration>(this.baseUrl + 'api/chart').subscribe(result => {

      this.chartConfig = result;

      let htmlRef = this.elementRef.nativeElement.querySelector("#theChart");

      //var ctx = document.getElementById("theChart");
      this.chart = new Chart(htmlRef, this.chartConfig);


    }); 
    
  }
   
  

}

interface MonthlyCountStat {
  recordedTime: string;

  start: string;

  end: string;

  category: string;
    
  sourceCount: number;
  destinationCount: number;

}


interface SeriesData {
  name: string;

  type: string;

  data: [];

}
