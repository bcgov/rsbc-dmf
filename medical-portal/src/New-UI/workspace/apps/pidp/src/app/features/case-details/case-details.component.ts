import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CaseManagementService, DMERCase } from '../shared/services/case-management/case-management.service';
import {MatIconRegistry} from "@angular/material/icon";
import {DomSanitizer} from "@angular/platform-browser";

@Component({
  selector: 'app-case-details',
  templateUrl: './case-details.component.html',
  styleUrls: ['./case-details.component.scss']
})
export class CaseDetailsComponent implements OnInit {
  selectedCase!: DMERCase;

  constructor(private router: Router, private caseManagementService: CaseManagementService,
              private matIconRegistry: MatIconRegistry,
              private domSanitizer: DomSanitizer
              ) {

    this.matIconRegistry.addSvgIcon(
      "medical-professionals-icon",
      this.domSanitizer.bypassSecurityTrustResourceUrl("../../assets/images/medical-professionals-icon.svg")
    );

  }

  public ngOnInit(): void {
    if (!this.caseManagementService.selectedCase) {
      this.router.navigate(['dashboard']);
      return;
    }

    this.selectedCase = this.caseManagementService.selectedCase;
  }
}
