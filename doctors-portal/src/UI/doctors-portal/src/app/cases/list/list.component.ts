import { Component, OnInit } from '@angular/core';
import { CaseManagementService, DMERForm } from 'src/app/shared/services/case-management/case-management.service';

@Component({
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  public dataSource: DMERForm[] = [];

  constructor(private caseManagementService: CaseManagementService) { }

  public ngOnInit(): void {
    this.dataSource = this.caseManagementService.getCases();
  }

}
