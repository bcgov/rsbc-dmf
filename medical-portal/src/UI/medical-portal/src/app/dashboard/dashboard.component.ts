import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CaseManagementService, DMERCase, DMERSearchCases } from '../shared/services/case-management/case-management.service';
import { Sort } from '@angular/material/sort';
import { faHourglassEnd } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  public dataSource: DMERCase[] = [];
  public sortedData: DMERCase[] = [];
  public searchBox: string = '';
  public searchCasesInput: string = '';
  public selectedStatus : string = 'All Status';
  public pageNumber = 1;
  public pageSize = 2;
  public totalRecords = 0;
  public isLoading = true;

  statuses = [
    { label: "All Status" },
    { label: "In Progress" },
    {label:"RSBC Received"},
    {label:"Under RSBC Review"},
    {label:"Decision Rendered"},
    { label: "Cancelled/Closed" },
    { label: "Trasferred" }
  ]


  constructor(
    private caseManagementService: CaseManagementService,
    private router: Router
  ) { }

  public ngOnInit(): void {
    this.searchCases({ byStatus: ['Pending'] })
  }

  public search(): void {
    console.debug('search', this.searchBox);

    let searchParams = {
      byTitle: this.searchBox
    };
    this.searchCases(searchParams)
  }

  searchCases(query?: any): void {
    let searchParams: DMERSearchCases = {
      ...query
    }
    if (this.searchCasesInput?.length > 0) {
      searchParams['byPatientName']  = this.searchCasesInput;
      searchParams['byTitle'] = this.searchCasesInput;
    } 

    if (this.selectedStatus?.length > 0) {
      searchParams['byStatus'] = [this.selectedStatus];
    }
    
    this.caseManagementService.getCases(searchParams).subscribe(cases => {
      this.totalRecords = cases.length;
      this.pageNumber = 1;
      this.dataSource = cases;
      this.sortedData = this.dataSource.slice(0, this.pageSize);

      this.isLoading =false;
    });
  }

  filterLocally() {
    const filteredData = this.dataSource.filter((item) => {
      if (this.selectedStatus !== 'All Status' && item.status !== this.selectedStatus) return false;
      if (this.searchCasesInput?.length > 0 && !(item.title?.includes(this.searchCasesInput))) return false;
      return true;  
    })

    this.totalRecords = filteredData.length;
    this.pageNumber = 1;

    this.sortedData = filteredData.slice(0, this.pageSize);
  }

  onStatusChanged() {
    this.filterLocally();
  }

  loadRecords(){
     this.sortedData = this.dataSource.slice(0, this.pageSize * ++this.pageNumber);
  }

  clear(){
    this.searchCasesInput='';
    this.selectedStatus='All Status';
    this.searchCases();
  }

  sortData(sort: Sort) {
    const data = this.dataSource.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedData = data;
      return;
    }

    this.sortedData = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'title':
          return compare(a.title, b.title, isAsc);
        case 'patientName':
          return compare(a.patientName, b.patientName, isAsc);
        case 'modifiedOn':
          return compare(a.modifiedOn, b.modifiedOn, isAsc);
        case 'clinicName':
          return compare(a.clinicName, b.clinicName, isAsc);
        case 'status':
          return compare(a.status, b.status, isAsc);
        default:
          return 0;
      }
    });
  }
}

function compare(a: string | undefined | null, b: number | string | undefined | null, isAsc: boolean) {
  // check for null or undefined
  if (a == null || b == null) {
    return 1;
  }
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
