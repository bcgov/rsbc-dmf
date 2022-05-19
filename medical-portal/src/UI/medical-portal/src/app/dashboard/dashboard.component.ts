import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CaseManagementService, DMERCase, DMERSearchCases } from '../shared/services/case-management/case-management.service';
import { Sort } from '@angular/material/sort';
import { faHourglassEnd } from '@fortawesome/free-solid-svg-icons';
import {MatAccordion} from '@angular/material/expansion';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  public dataSource: DMERCase[] = [];
  public filteredData: DMERCase[] = [];
  public showingDataInView: DMERCase[] = [];
  public searchBox: string = '';
  public prevSearchBox: string = '';
  public searchCasesInput: string = '';
  public selectedStatus : string = 'All Statuses';
  public pageNumber = 1;
  public pageSize = 2;
  public totalRecords = 0;
  public isLoading = true;
  public isShowResults = false;
  public searchedCase: DMERCase | null = {};

  @ViewChild(MatAccordion) accordion!: MatAccordion;

  statuses = [
    { label: "All Statuses" },
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
    this.searchCases({ byStatus: ['All Statuses'] })
  }

  public search(): void {
    if (this.searchBox === '' || this.prevSearchBox === this.searchBox) return;
    // console.debug('search', this.searchBox);

    let searchParams = {
      byTitle: this.searchBox
    };
    this.caseManagementService.getCases(searchParams).subscribe(cases => {
      if(cases && Array.isArray(cases) && cases?.[0]) {
        this.searchedCase = cases[0];
        // console.log(this.searchedCase)
      } else {
        this.searchedCase  = null;
      }

      this.prevSearchBox = this.searchBox;
      this.isShowResults = true;
    })
  }

  closeResults(){
    this.searchBox = '';
    this.prevSearchBox = '';
    this.searchedCase = null;
    this.isShowResults = false;
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
      this.filteredData = cases;
      this.showingDataInView =  this.dataSource.slice(0, this.pageSize);

      this.isLoading =false;
    });
  }

  filterLocally() {
    this.filteredData = this.dataSource.filter((item) => {
      if (this.selectedStatus !== 'All Statuses' && item.status !== this.selectedStatus) return false;
      if (this.searchCasesInput?.length > 0 && !(item.title?.includes(this.searchCasesInput))) return false;
      return true;  
    })

    this.totalRecords = this.filteredData.length;
    this.pageNumber = 1;

    this.showingDataInView = this.filteredData.slice(0, this.pageSize);
  }

  onStatusChanged() {
    this.filterLocally();
  }

  loadRecords(){
    if (this.pageNumber * this.pageSize > this.totalRecords) return; 
    this.showingDataInView = this.filteredData.slice(0, this.pageSize * ++this.pageNumber);
  }

  clear(){
    this.searchCasesInput='';
    this.selectedStatus='All Statuses';
    this.searchCases();
  }

  sortData(sort: Sort) {
    const data = this.filteredData.slice(0, this.pageSize * ++this.pageNumber);
    if (!sort.active || sort.direction === '') {
      this.showingDataInView = data;
      return;
    }

    this.showingDataInView = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'title':
          return compare(a.title, b.title, isAsc);
        case 'patientName':
          return compare(a.patientName, b.patientName, isAsc);
        case 'driverBirthDate':
            return compare(a.driverBirthDate, b.driverBirthDate, isAsc);
        case 'dmerType':
            return compare(a.dmerType, b.dmerType, isAsc);
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
