import { Component, ViewChild } from '@angular/core';

import { ViewportScroller } from '@angular/common';
import { Router } from '@angular/router';
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
// import { CoreUiModule } from '../../../../shared-portal-ui/projects/core-ui/src/public-api';
// import { DmerTypeComponent } from '../../../../shared-portal-ui/projects/core-ui/src/lib/case-definations/dmer-type/dmer-type.component';
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
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  viewProviders: [MatExpansionPanel],
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
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  constructor(
    private router: Router,
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
}
