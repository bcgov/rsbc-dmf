import { Component, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { CaseDetail } from '../shared/api/models';

@Component({
  selector: 'app-case',
  templateUrl: './case.component.html',
  styleUrls: ['./case.component.scss'],
})
export class CaseComponent implements OnInit {
  isExpanded: Record<string, boolean> = {
    '1': false,
  };
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  public closedCaseDetails: CaseDetail[] = [];

  constructor(private caseManagementService: CaseManagementService) {}

  ngOnInit(): void {
    this.getClosedCases('e27d7c69-3913-4116-a360-f5e990600056');
  }

  getClosedCases(driverId: string) {
    this.caseManagementService
      .getClosedCases({ driverId })
      .subscribe((closedCases: any) => {
        this.closedCaseDetails = closedCases;
      });
  }

  toggleisExpandable(id: string) {
    this.isExpanded[id] = !this.isExpanded[id];
  }
}
