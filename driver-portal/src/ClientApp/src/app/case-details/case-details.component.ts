import { Component, OnInit } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { CaseDocuments } from '../shared/api/models';
import { LoginService } from '../shared/services/login.service';

@Component({
  selector: 'app-case-details',
  templateUrl: './case-details.component.html',
  styleUrls: ['./case-details.component.scss'],
})
export class CaseDetailsComponent implements OnInit {
  caseDocuments?: CaseDocuments;
  selectedIndex = 0;

  constructor(
    private caseManagementService: CaseManagementService,
    private loginService: LoginService
  ) {}

  ngOnInit(): void {
    if (this.loginService.userProfile?.id) {
      this.getCaseSubmissionDocuments(
        this.loginService.userProfile?.id as string
      );
    }
  }

  getCaseSubmissionDocuments(driverId: string) {
    this.caseManagementService
      .getDriverDocuments({ driverId })
      .subscribe((caseDocuments) => {
        this.caseDocuments = caseDocuments;
      });
  }

  onViewLetter() {
    this.selectedIndex = 2;
  }
}
