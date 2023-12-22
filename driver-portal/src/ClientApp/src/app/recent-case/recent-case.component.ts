import { Component, OnInit } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Router } from '@angular/router';
import { CaseDetail } from '../shared/api/models';

@Component({
  selector: 'app-recent-case',
  templateUrl: './recent-case.component.html',
  styleUrls: ['./recent-case.component.scss'],
})
export class RecentCaseComponent implements OnInit {
  public caseDetails: CaseDetail | undefined;

  constructor(
    private caseManagementService: CaseManagementService,
    private router: Router
  ) {}

  public ngOnInit(): void {
    this.caseManagementService.getMostRecentCase({}).subscribe((recentCase) => {
      this.caseDetails = recentCase;
    });
  }
}
