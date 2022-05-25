import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CaseManagementService, DMERCase } from '../shared/services/case-management/case-management.service';

@Component({
  selector: 'app-case-details',
  templateUrl: './case-details.component.html',
  styleUrls: ['./case-details.component.scss']
})
export class CaseDetailsComponent implements OnInit {
  selectedCase!: DMERCase;

  constructor(private router: Router, private caseManagementService: CaseManagementService) { }

  public ngOnInit(): void {
    if (!this.caseManagementService.selectedCase) {
      this.router.navigate(['dashboard']);
      return;
    }

    this.selectedCase = this.caseManagementService.selectedCase;
  }
}
