import { Component, Inject, ElementRef, Input, InjectionToken } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';

@Component({
  selector: 'app-progress-case-decision-table',
  templateUrl: './progress-case-decision-table.component.html',
  styleUrl: './progress-case-decision-table.component.css'
})
export class ProgressCaseDecisionTableComponent implements OnInit {
  @Input() title!: string;
  chartConfig: any;
  baseUrl: string;
  tableData: any;

  constructor(private http: HttpClient, private elementRef: ElementRef, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.tableData = {};
  }

  ngOnInit(): void {

    this.http.get<ProgressData[]>(this.baseUrl + 'api/MonthlyCountStats/CaseDecisionProgress').subscribe(result => {

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

