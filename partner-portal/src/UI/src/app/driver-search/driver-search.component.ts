import { CUSTOM_ELEMENTS_SCHEMA, Component, ViewChild } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatToolbar } from '@angular/material/toolbar';
import { RouterLink } from '@angular/router';
import { MatAccordion, MatExpansionModule } from '@angular/material/expansion';
import { CaseStatusComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/case-status/case-status.component';
import { CaseTypeComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/case-type/case-type.component';
import { MedicalDmerTypesComponent } from '@app/definitions/medical-dmer-types/medical-dmer-types.component';
import { DecisionOutcomeComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/decision-outcome/decision-outcome.component';
import { EligibleLicenseClassComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/eligible-license-class/eligible-license-class.component';
import { RecentCaseComponent } from '@app/recent-case/recent-case.component';

@Component({
  selector: 'app-driver-search',
  standalone: true,
  imports: [
    MatCardModule,
    MatToolbar,
    RouterLink,
    MatExpansionModule,
    CaseStatusComponent,
    CaseTypeComponent,
    MedicalDmerTypesComponent,
    DecisionOutcomeComponent,
    EligibleLicenseClassComponent,
    RecentCaseComponent
  ],
  templateUrl: './driver-search.component.html',
  styleUrl: './driver-search.component.scss',

  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DriverSearchComponent {
  @ViewChild(MatAccordion) accordion!: MatAccordion;
}
