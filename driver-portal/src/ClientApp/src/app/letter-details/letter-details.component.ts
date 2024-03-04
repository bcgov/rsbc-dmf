import { Component, OnInit } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { CaseDocuments } from '../shared/api/models';
import { LoginService } from '../shared/services/login.service';

@Component({
  selector: 'app-letter-details',
  templateUrl: './letter-details.component.html',
  styleUrls: ['./letter-details.component.css'],
})
export class LetterDetailsComponent implements OnInit {
  caseDocuments?: CaseDocuments;

  constructor(
    private caseManagementService: CaseManagementService,
    private loginService: LoginService
  ) {}

  ngOnInit(): void {
    this.getCaseSubmissionDocuments(
      this.loginService.userProfile?.id as string
    );
  }

  getCaseSubmissionDocuments(driverId: string) {
    this.caseManagementService
      .getDriverDocuments({ driverId })
      .subscribe((caseDocuments: any) => {
        this.caseDocuments = caseDocuments;
        console.log(caseDocuments);
      });
  }
}
