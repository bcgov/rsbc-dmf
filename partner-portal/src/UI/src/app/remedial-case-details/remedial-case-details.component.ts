import { Component, CUSTOM_ELEMENTS_SCHEMA, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionModule } from '@angular/material/expansion';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { RecentCaseComponent, CaseStatusComponent, CaseTypeComponent, DecisionOutcomeComponent, EligibleLicenseClassComponent, PortalsEnum } from '@shared/core-ui';
import { MedicalDmerTypesComponent } from '../../app/definitions/medical-dmer-types/medical-dmer-types.component';
@Component({
  selector: 'app-remedial-case-details',
  standalone: true,
  imports: [
    RecentCaseComponent,
    MatExpansionModule,
    CaseStatusComponent,
    CaseTypeComponent,
    DecisionOutcomeComponent,
    EligibleLicenseClassComponent,
    MedicalDmerTypesComponent],
  templateUrl: './remedial-case-details.component.html',
  styleUrl: './remedial-case-details.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class RemedialCaseDetailsComponent {

  PortalsEnum = PortalsEnum;
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  constructor(
    public caseManagementService: CaseManagementService,
  ) { }
}
