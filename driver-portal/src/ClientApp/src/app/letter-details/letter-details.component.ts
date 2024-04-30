import { Component, OnInit } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { CaseDocuments } from '../shared/api/models';
import { LoginService } from '../shared/services/login.service';
import { LettersToDriverComponent } from '../letters-to-driver/letters-to-driver.component';

@Component({
    selector: 'app-letter-details',
    templateUrl: './letter-details.component.html',
    styleUrls: ['./letter-details.component.css'],
    standalone: true,
    imports: [LettersToDriverComponent],
})
export class LetterDetailsComponent implements OnInit {
  caseDocuments?: CaseDocuments;
  isLoading = true;

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
      .subscribe((caseDocuments) => {
        this.caseDocuments = caseDocuments;
        this.isLoading = false;
      });
  }
}
