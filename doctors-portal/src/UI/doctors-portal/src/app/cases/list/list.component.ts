import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CaseManagementService, DMERCase } from 'src/app/shared/services/case-management/case-management.service';

@Component({
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  public dataSource: DMERCase[] = [];

  constructor(
    private caseManagementService: CaseManagementService,
    private route: ActivatedRoute
  ) { }

  public ngOnInit(): void {
    this.route.params.subscribe(params => {
      let searchParams = {
        byCaseId: params['id'],
        byDriverLicense: params['dl'],
        byPatientName: params['name'],
        byStatus: params['status']
      };
      console.debug('list', searchParams);
      this.caseManagementService.getCases(searchParams).subscribe(cases => this.dataSource = cases);
    });
  }

}
