import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { RecentCaseComponent, CaseStatusComponent, CaseTypeComponent, DecisionOutcomeComponent, EligibleLicenseClassComponent, PortalsEnum } from '@shared/core-ui';
@Component({
  selector: 'app-remedial-case-details',
  standalone: true,
  imports: [RecentCaseComponent],
  templateUrl: './remedial-case-details.component.html',
  styleUrl: './remedial-case-details.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class RemedialCaseDetailsComponent {

    PortalsEnum = PortalsEnum;

     constructor(
         public caseManagementService: CaseManagementService,
         //private userService: UserService,
         //private route: ActivatedRoute
         ) {}
}
