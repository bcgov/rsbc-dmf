import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CaseManagementService, DMERCase } from 'src/app/shared/services/case-management/case-management.service';

@Component({
  selector: 'app-cases-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  @Input() clinicId?: string | null = null;
  @Input() clinicName?: string | null = null;

  public dataSource: DMERCase[] = [];

  statusFilter  = new FormControl('' );
  lastModifiedByFilter  = new FormControl('' );
  
  constructor(
    private caseManagementService: CaseManagementService,
    private route: ActivatedRoute
  ) { }

  public ngOnInit(): void { 
    if (this.clinicId)   
    {
      let searchParams = {
        byClinicId: this.clinicId
      };
      console.debug('list', searchParams);
      this.caseManagementService.getCases(searchParams).subscribe(cases => this.dataSource = cases);
    }
  }

}
