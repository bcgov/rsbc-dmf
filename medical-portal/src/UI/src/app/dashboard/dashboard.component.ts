import {
  CUSTOM_ELEMENTS_SCHEMA,
  Component,
  OnInit,
  ViewChild,
} from '@angular/core';

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
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { DmerStatusComponent } from '@shared/core-ui';

import { CasesService, DocumentService } from '../shared/api/services';
import { CaseDocument, DmerDocument, PatientCase } from '../shared/api/models';
import { MatCommonModule } from '@angular/material/core';
import { CommonModule, ViewportScroller } from '@angular/common';
import { MedicalDmerTypesComponent } from '@app/definitions/medical-dmer-types/medical-dmer-types.component';
import { PopupService } from '@app/popup/popup.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ClaimDmerPopupComponent } from '@src/claim-dmer-popup/claim-dmer-popup.component';
import { DMERStatusEnum } from '@app/app.model';

interface Status {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    MatCommonModule,
    MatExpansionModule,
    MatCardModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    RouterLinkActive,
    DmerStatusComponent,
    //DmerTypeComponent,
    MatExpansionPanel,
    MedicalDmerTypesComponent,
    MatDialogModule,
    ClaimDmerPopupComponent,
  ],

  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DashboardComponent {
  status: Status[] = [
    { value: 'All Status', viewValue: 'All Status' },
    { value: 'Required - Claimed', viewValue: 'Required - Claimed' },
    { value: 'Submitted', viewValue: 'Submitted' },
    { value: 'Non-Comply - Claimed', viewValue: 'Non-Comply - Claimed' },
  ];

  selectedStatus: string = 'All Status';
  showSearchResults = false;
  public searchBox = new FormControl('');
  public prevSearchBox: string = '';
  public searchCasesInput: string = '';
  public searchedCase?: PatientCase;
  public practitionerDMERList: DmerDocument[] = [];
  public filteredData?: DmerDocument[] = [];
  public _allDocuments?: DmerDocument[] | null = [];

  isSearching: boolean = false;
  noResults: boolean = false;
  DMERStatusEnum = DMERStatusEnum;
  isExpanded: Record<string, boolean> = {};

  pageSize = 10;

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  constructor(
    private viewportScroller: ViewportScroller,
    private casesService: CasesService,
    private documentService: DocumentService,
    private popupService: PopupService,
    private dialog: MatDialog,
  ) {
    console.info('At Dashboard Constructor');
  }

  public onClick(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }

  ngOnInit(): void {
    this.getClaimedDmerCases();
  }

  getClaimedDmerCases() {
    this.documentService.apiDocumentMyDmersGet$Json({}).subscribe((data) => {
      this.practitionerDMERList = data;
      console.info('Got data');
      this.filterCasesData();
    });
  }

  searchDmerCase(): void {
    console.log('search DMER Case');

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
      },
      error: (err) => {
        this.noResults = true;
      },
      complete: () => {
        this.isSearching = false;
      },
    });

    this.prevSearchBox = this.searchBox.value as string;
    this.showSearchResults = true;
  }

  clear() {
    console.log('clear');
    this.searchCasesInput = '';
    this.selectedStatus = 'All Status';
    this.filterCasesData();
  }

  clearResults() {
    this.showSearchResults = false;
  }

  filterCasesData() {
    this.filteredData = this.practitionerDMERList.filter((item) => {
      const matchStatus =
        this.selectedStatus === 'All Status' ||
        item.dmerStatus === this.selectedStatus;

      console.log(this.selectedStatus, item.dmerStatus);

      const matchCaseNumber =
        this.searchCasesInput?.length === 0 ||
        item.idCode?.includes(this.searchCasesInput) ||
        item.fullName?.includes(this.searchCasesInput);

      if (matchStatus && matchCaseNumber) return true;
      else return false;
    });
  }

  viewMore() {
    const pageSize = (this.filteredData?.length ?? 0) + this.pageSize;

    this.filteredData = this._allDocuments?.slice(0, pageSize);
  }

  openPopup() {
    this.popupService.openPopup();
  }

  openClaimPopup(searchedCase: PatientCase) {
    const dialogRef = this.dialog.open(ClaimDmerPopupComponent, {
      height: '600px',
      width: '820px',
      data: searchedCase,
    });
    dialogRef.afterClosed().subscribe((result) => {
      //TODO # optimize this not to re-query the database on refresh
      this.getClaimedDmerCases();
      this.searchDmerCase();
      console.log('The dialog was closed', result);
    });
  }
}
