import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CaseManagementService, DMERCase } from '../shared/services/case-management/case-management.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  public dataSource: DMERCase[] = [];
  public searchBox: string = '';

  constructor(
    private caseManagementService: CaseManagementService,
    private router: Router
  ) { }

  public ngOnInit(): void {
    this.caseManagementService.getCases({ byStatus: ['Pending'] }).subscribe(cases => this.dataSource = cases);
  }

  public search(): void {
    console.debug('search', this.searchBox);
    this.router.navigate(['/cases/list', { 'id': this.searchBox }])
  }

}
