import { Component, OnInit, ViewChild } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatAccordion, MatExpansionModule } from '@angular/material/expansion';
import { MatToolbar } from '@angular/material/toolbar';
import { RecentCaseComponent } from '@app/recent-case/recent-case.component';
import { CaseStatusComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/case-status/case-status.component';
import { CaseTypeComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/case-type/case-type.component';
import { MedicalDmerTypesComponent } from '@app/definitions/medical-dmer-types/medical-dmer-types.component';
import { DecisionOutcomeComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/decision-outcome/decision-outcome.component';
import { SubmissionTypeComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/submission-type/submission-type.component';
import { SubmissionStatusComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/submission-status/submission-status.component';
import { SubmissionRequirementsComponent } from '../../app/submission-requirements/submission-requirements.component';
import { SubmissionHistoryComponent } from '@app/submission-history/submission-history.component';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { CaseSearch } from '@app/shared/api/models';

@Component({
  selector: 'app-case-search',
  standalone: true,
  imports: [
    MatCardModule,
    MatToolbar,
    MatExpansionModule,
    RecentCaseComponent,
    CaseStatusComponent,
    CaseTypeComponent,
    MedicalDmerTypesComponent,
    DecisionOutcomeComponent,
    SubmissionTypeComponent,
    SubmissionStatusComponent,
    SubmissionRequirementsComponent,
    SubmissionHistoryComponent,
    RouterLink,
  ],
  templateUrl: './case-search.component.html',
  styleUrl: './case-search.component.scss',
})
export class CaseSearchComponent implements OnInit{

  idCode = '';

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  
  caseDetails: CaseSearch = this.router.getCurrentNavigation()?.extras.state as CaseSearch;
  
  constructor(
    private caseManagementService: CaseManagementService,
    private activatedRoute: ActivatedRoute, 
    private router: Router  
  ) {}
    
  ngOnInit(): void {
  }
  
}
