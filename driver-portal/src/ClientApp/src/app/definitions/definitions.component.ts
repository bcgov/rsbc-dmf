import { Component, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { CaseStatusComponent, DecisionOutcomeComponent, DmerTypeComponent, EligibleLicenseClassComponent, LetterTopicComponent, SubmissionStatusComponent, SubmissionTypeComponent } from '@shared/core-ui';

@Component({
  selector: 'app-definitions',
  standalone: true,
  imports: [
    MatAccordion,
    MatExpansionPanel,
    MatExpansionPanelHeader,
    MatExpansionPanelTitle,
    CaseStatusComponent,
    DmerTypeComponent,
    DecisionOutcomeComponent,
    EligibleLicenseClassComponent,
    LetterTopicComponent,
    SubmissionTypeComponent,
    SubmissionStatusComponent,
  ],
  templateUrl: './definitions.component.html',
  styleUrl: './definitions.component.scss'
})
export class DefinitionsComponent {
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  isExpanded: Record<string, boolean> = {};

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }
}
