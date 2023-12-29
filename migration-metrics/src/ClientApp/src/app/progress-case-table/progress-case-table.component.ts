
import {DecimalPipe, NgForOf} from "@angular/common";
import { Component, Inject, ElementRef, Input, InjectionToken } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';

//export const BASE_URL = new InjectionToken<string>('BASE_URL');

@Component({
  selector: 'app-progress-case-table',
  templateUrl: './progress-case-table.component.html',
  styleUrl: './progress-case-table.component.css'
})
export class ProgressCaseTableComponent implements OnInit {
  @Input() title!: string;
  chartConfig: any;
  baseUrl: string;
  tableData: any;

  constructor(private http: HttpClient, private elementRef: ElementRef, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.tableData = {};
  }

  ngOnInit(): void {

      this.http.get<ProgressData[]>(this.baseUrl + 'api/MonthlyCountStats/CaseProgress').subscribe(result => {

      //let htmlRef = this.elementRef.nativeElement.querySelector("#theChart");

      //var ctx = document.getElementById("theChart");
      this.tableData = result;

    });
  }

}


interface ProgressData {
  label: string;

  oracleCount: number;

  dynamicsCount: number;

  difference: number;

  percentage: number;
}



