import { Component, OnInit } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { CaseDocuments } from '../shared/api/models';
import { LoginService } from '../shared/services/login.service';
import { ViewportScroller } from '@angular/common';
import { SubmissionRequirementsComponent } from '../submission-requirements/submission-requirements.component';
import { RecentCaseComponent } from '../recent-case/recent-case.component';

@Component({
  selector: 'app-case-details',
  templateUrl: './case-details.component.html',
  styleUrls: ['./case-details.component.scss'],
  standalone: true,
  imports: [RecentCaseComponent, SubmissionRequirementsComponent],
})
export class CaseDetailsComponent implements OnInit {
  caseDocuments?: CaseDocuments;
  selectedIndex = 0;
  isLoading = true;

  constructor(
    private caseManagementService: CaseManagementService,
    private loginService: LoginService,
    private viewportScroller: ViewportScroller
  ) {}

  ngOnInit(): void {
    if (this.loginService.userProfile?.id) {
      this.getCaseSubmissionDocuments(
        this.loginService.userProfile?.id as string
      );
    }
  }

  // ngAfterViewInit ():void{
  //   setTimeout(() => {
  //     this.viewportScroller.scrollToAnchor('ScrollToSubmissionRequirement');
  //   }, 500)
  // }

  getCaseSubmissionDocuments(driverId: string) {
    this.caseManagementService
      .getDriverDocuments({ driverId })
      .subscribe((caseDocuments) => {
        this.caseDocuments = caseDocuments;
        this.isLoading = false;
      });
  }

  onViewLetter() {
    this.selectedIndex = 2;
  }
}
