import { CUSTOM_ELEMENTS_SCHEMA, Component, ViewChild } from '@angular/core';

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
import { FormsModule } from '@angular/forms';
// import { DmerTypeComponent } from '@shared/case-definitions';
// import { CoreUiModule } from '@shared/core-ui';
// import { DmerTypeComponent } from '@shared/core-ui';
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
    // DmerTypeComponent,
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
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  constructor(
    //private router: Router,
    private viewportScroller: ViewportScroller
  ) {}

  public onClick(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }
  searchCases() {
    console.log('search cases');
  }

  clear() {
    console.log('clear');
  }

  searchDmerCase() {
    console.log('search DMER Case');
    this.showSearchResults = true;
  }

  clearResults() {
    this.showSearchResults = false;
  }
  
  
}
