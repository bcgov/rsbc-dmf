import { Component, Inject, ElementRef, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';
import Chart, { ChartConfiguration } from 'chart.js/auto';

@Component({
  selector: 'app-progress-chart',
  templateUrl: './progress-chart.component.html',
  styleUrls: ['./progress-chart.component.css']
})
export class ProgressChartComponent {

  @Input() chartId!: string;

  @Input() title!: string;

  @Input() showRed!: string;

  public chart: any;

  chartConfig: any;
  baseUrl: string;

  constructor(private http: HttpClient, private elementRef: ElementRef, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

  }

  ngOnInit(): void {
    this.http.get<ChartConfiguration>(this.baseUrl + 'api/chart/' + this.chartId + '?showRed=' + this.showRed ).subscribe(result => {

      this.chartConfig = result;

      let htmlRef = this.elementRef.nativeElement.querySelector("#theChart" + this.chartId);

      //var ctx = document.getElementById("theChart");
      this.chart = new Chart(htmlRef, this.chartConfig);


    });
  }
}
