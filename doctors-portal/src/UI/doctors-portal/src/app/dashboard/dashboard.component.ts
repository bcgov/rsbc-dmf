import { Component, OnInit } from '@angular/core';
import { CaseManagementService, DMERForm } from '../shared/services/case-management/case-management.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  public dataSource: DMERForm[] = [];

  constructor(private caseManagementService: CaseManagementService) { }

  public ngOnInit(): void {
    this.dataSource = this.caseManagementService.getCases();
  }

}
