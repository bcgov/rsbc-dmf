import { Component, OnInit } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent {
  constructor(
    private caseManagementService: CaseManagementService,
    private router: Router
  ) {}

  // public  ngOnInit(): void {
  //     this.getRecentCase('F4B8B3');
  // }

  // getRecentCase(caseId: string){
  //   this.caseManagementService.getCaseById({caseId }).subscribe(
  //     (caseResponse) => {
  //       console.log(caseResponse);
  //     }
  //   );
  // }
}
