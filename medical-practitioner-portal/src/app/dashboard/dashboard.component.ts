import {
  CUSTOM_ELEMENTS_SCHEMA,
  Component,
  ViewChild,
  signal,
} from '@angular/core';

import { CommonModule, ViewportScroller } from '@angular/common';

import {
  MatExpansionModule,
  MatAccordion,
  MatExpansionPanel,
} from '@angular/material/expansion';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { DmerStatusComponent } from '../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/dmer-status/dmer-status.component';
import { DmerTypeComponent } from '../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/dmer-type/dmer-type.component';
import { CasesService } from '../shared/api/services';

interface Status {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    MatExpansionModule,
    MatCardModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    RouterLinkActive,
    DmerStatusComponent,
    DmerTypeComponent,
  ],

  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  viewProviders: [MatExpansionPanel],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DashboardComponent {
  status: Status[] = [
    { value: 'allStatus', viewValue: 'All Status' },
    { value: 'notRequested', viewValue: 'Not Requested' },
    { value: 'requiredUnclaimed', viewValue: 'Required - Unclaimed' },
    { value: 'requiredClaimed', viewValue: 'Required - Claimed' },
    { value: 'submitted', viewValue: 'Submitted' },
    { value: 'reviewed', viewValue: 'Reviewed' },
    { value: 'noncomply', viewValue: 'Non-Comply' },
  ];

  selectedStatus: string = 'allStatus';
  showSearchResults = false;
  public searchBox = new FormControl('');
  public prevSearchBox: string = '';
  public searchCasesInput: string = '';
  public searchedCase: any | null = {};

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  constructor(
    private viewportScroller: ViewportScroller,
    private casesService: CasesService
  ) {}

  public onClick(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }

  searchDmerCase(): void {
    console.log('search DMER Case');
    if (
      this.prevSearchBox === '' ||
      this.prevSearchBox !== this.searchBox.value
    ) {
      let searchParams: Parameters<CasesService['apiCasesCaseIdGet$Json']>[0] =
        {
          caseId: this.searchBox.value as string,
        };
      this.casesService
        .apiCasesCaseIdGet$Json(searchParams)
        .subscribe((dmerCase) => {
          if (dmerCase) this.searchedCase = dmerCase;
          console.log(searchParams, this.searchedCase);
        });
    }
    this.prevSearchBox = this.searchBox.value as string;
    this.showSearchResults = true;
  }

  searchCases() {
    console.log('search cases');
  }

  clear() {
    console.log('clear');
  }

  clearResults() {
    this.showSearchResults = false;
  }
}
