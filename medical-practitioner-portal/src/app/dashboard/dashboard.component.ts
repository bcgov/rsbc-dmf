import {
  CUSTOM_ELEMENTS_SCHEMA,
  Component,
  OnInit,
  ViewChild,
} from '@angular/core';

// import { CommonModule, ViewportScroller } from '@angular/common';

import {
  MatExpansionModule,
  MatAccordion,
  MatExpansionPanel,
} from '@angular/material/expansion';
import { MatCardModule } from '@angular/material/card';
// import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
// import { MatSelectModule } from '@angular/material/select';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { DmerStatusComponent } from '@shared/core-ui';
import { CasesService, DocumentService } from '../shared/api/services';
import { CaseDocument, PatientCase } from '../shared/api/models';
import { MatCommonModule } from '@angular/material/core';

interface Status {
  value: number;
  viewValue: string;
}
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    MatCommonModule,
    MatExpansionModule,
    MatCardModule,
    // MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    // MatSelectModule,
    FormsModule,
    // CommonModule,
    ReactiveFormsModule,
    RouterLink,
    RouterLinkActive,
    DmerStatusComponent,
    // DmerTypeComponent,
    MatExpansionPanel,
  ],

  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DashboardComponent {
  // status: Status[] = [
  //   { value: 1, viewValue: 'All Status' },
  //   { value: 2, viewValue: 'Not Requested' },
  //   { value: 3, viewValue: 'Required - Unclaimed' },
  //   { value: 4, viewValue: 'Required - Claimed' },
  //   { value: 5, viewValue: 'Submitted' },
  //   { value: 100000003, viewValue: 'Reviewed' },
  //   { value: 100000005, viewValue: 'Non-Comply' },
  //   { value: 100000001, viewValue: 'Received' },
  // ];

  // selectedStatus: number = 1;
  showSearchResults = false;
  public searchBox = new FormControl('');
  public prevSearchBox: string = '';
  // public searchCasesInput: string = '';
  public searchedCase?: PatientCase;
  // public practitionerDMERList: CaseDocument[] = [];
  // public filteredData?: CaseDocument[] = [];
  // public _allDocuments?: CaseDocument[] | null = [];

  isSearching: boolean = false;
  noResults: boolean = false;

  // //isExpanded: Record<string, boolean> = {};

  // pageSize = 10;

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  constructor(
    // private viewportScroller: ViewportScroller,
    private casesService: CasesService,
    private documentService: DocumentService,
  ) {
    console.info('At Dashboard Constructor');
  }

  // public onClick(event: any, elementId: string): void {
  //   event.preventDefault();
  //   this.viewportScroller.scrollToAnchor(elementId);
  // }

  // ngOnInit(): void {
  //   // this.practitionerDMERList = PractitionerDMERList_SEED_DATA;
  //   console.info('At Dashboard OnInit');

  //   this.documentService.apiDocumentMyDmersGet$Json({}).subscribe((data) => {
  //     this.practitionerDMERList = data;
  //     //this.filteredData = [...this.practitionerDMERList];
  //     console.info('Got data');
  //     this.filterCasesData();
  //   });

  // }

  searchDmerCase(): void {
    console.log('search DMER Case');
    if (
      this.prevSearchBox === '' ||
      this.prevSearchBox !== this.searchBox.value
    ) {
      let searchParams: Parameters<
        CasesService['apiCasesSearchIdCodeGet$Json']
      >[0] = {
        idCode: this.searchBox.value as string,
      };

      this.isSearching = true;
      this.noResults = false;

      this.casesService.apiCasesSearchIdCodeGet$Json(searchParams).subscribe({
        next: (dmerCase) => {
          if (dmerCase) this.searchedCase = dmerCase;
          console.log(searchParams, this.searchedCase);
        },
        error: (err) => {
          this.noResults = true;
        },
        complete: () => {
          this.isSearching = false;
        },
      });
    }
    this.prevSearchBox = this.searchBox.value as string;
    this.showSearchResults = true;
  }

  // clear() {
  //   console.log('clear');
  //   this.searchCasesInput = '';
  //   this.selectedStatus = 1;
  //   this.filterCasesData();
  // }

  // clearResults() {
  //   this.showSearchResults = false;
  // }

  // filterCasesData() {
  //   this.filteredData = this.practitionerDMERList.filter((item) => {
  //     const matchStatus =
  //       this.selectedStatus === 1 || item.dmerStatus === this.selectedStatus;

  //     const matchCaseNumber =
  //       this.searchCasesInput?.length === 0 ||
  //       item.caseNumber?.includes(this.searchCasesInput) ||
  //       item.fullName?.includes(this.searchCasesInput);

  //     if (matchStatus && matchCaseNumber) return true;
  //     else return false;
  //   });
  // }

  // viewMore() {
  //   const pageSize = (this.filteredData?.length ?? 0) + this.pageSize;

  //   this.filteredData = this._allDocuments?.slice(0, pageSize);
  // }
}
