import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CaseManagementService, DMERCase } from '../shared/services/case-management/case-management.service';
import {Sort} from '@angular/material/sort';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  public dataSource: DMERCase[] = [];
  public sortedData: DMERCase[] = [];
  public searchBox: string = '';

  constructor(
    private caseManagementService: CaseManagementService,
    private router: Router
  ) { }

  public ngOnInit(): void {
    this.caseManagementService.getCases({ byStatus: ['Pending'] }).subscribe(cases => {
      this.dataSource = cases;
      this.sortedData = this.dataSource.slice();
    } );
  }

  public search(): void {
    console.debug('search', this.searchBox);

    let searchParams = {
      byTitle: this.searchBox
    };
    this.caseManagementService.getCases(searchParams).subscribe(cases => {
      this.dataSource = cases;
      this.sortedData = this.dataSource.slice();
    });


  }

  sortData(sort: Sort) {
    const data = this.dataSource.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedData = data;
      return;
    }

    this.sortedData = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'title':
          return compare(a.title, b.title, isAsc);
        case 'patientName':
          return compare(a.patientName, b.patientName, isAsc);
        case 'modifiedOn':
          return compare(a.modifiedOn, b.modifiedOn, isAsc);
        case 'clinicName':
          return compare(a.clinicName, b.clinicName, isAsc);
        case 'status':
          return compare(a.status, b.status, isAsc);
        default:
          return 0;
      }
    });
  }
}

function compare(a: string | undefined | null, b: number | string | undefined | null, isAsc: boolean) {
  // check for null or undefined
  if (a == null || b == null) {
    return 1;
  }
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
