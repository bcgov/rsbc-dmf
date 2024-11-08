import { CUSTOM_ELEMENTS_SCHEMA, Component, ViewChild } from '@angular/core';
import { MatExpansionModule, MatAccordion, MatExpansionPanel } from '@angular/material/expansion';
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
import { DmerDocument, PatientCase, UserProfile } from '../shared/api/models';
import { MatCommonModule } from '@angular/material/core';
import { CommonModule, ViewportScroller } from '@angular/common';
import { MedicalDmerTypesComponent } from '@app/definitions/medical-dmer-types/medical-dmer-types.component';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { DMERStatusEnum } from '@app/app.model';
import { ProfileManagementService } from '@app/shared/services/profile.service';
import { DmerButtonsComponent } from '@app/dmer-buttons/dmer-buttons.component';
import { ClaimDmerPopupComponent } from '@app/claim-dmer-popup/claim-dmer-popup.component';

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
    MatExpansionPanel,
    MedicalDmerTypesComponent,
    MatDialogModule,
    DmerButtonsComponent
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
  public searchedCase?: PatientCase | null;
  public practitionerDMERList: DmerDocument[] = [];
  public filteredData?: DmerDocument[] = [];
  public _allDocuments?: DmerDocument[] | null = [];
  public profile?: UserProfile;

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
    private profileManagementService: ProfileManagementService,
    private dialog: MatDialog,
  ) { }

  public onClick(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }

  ngOnInit(): void {
    this.profile = this.profileManagementService.getCachedProfile();
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
    let searchParams: Parameters<
      CasesService['apiCasesSearchIdCodeGet$Json']
    >[0] = {
      idCode: this.searchBox.value as string,
    };

    this.isSearching = true;
    this.noResults = false;
    this.searchedCase = null;

    this.casesService.apiCasesSearchIdCodeGet$Json(searchParams).subscribe({
      next: (dmerCase) => {
        if (dmerCase) {
          this.searchedCase = dmerCase;
        }
      },
      error: (err) => {
        // TODO display user friendly message
        this.noResults = true;
        console.error("Failed to get case", err);
      },
      complete: () => {
        this.isSearching = false;
      },
    });

    this.prevSearchBox = this.searchBox.value as string;
    this.showSearchResults = true;
  }

  clear() {
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

      const matchCaseNumber =
        this.searchCasesInput?.length === 0 ||
        item.idCode?.toLowerCase().includes(this.searchCasesInput?.toLowerCase()) ||
        item.fullName?.toLowerCase().includes(this.searchCasesInput?.toLowerCase());

      if (matchStatus && matchCaseNumber) return true;
      else return false;
    });
  }

  viewMore() {
    const pageSize = (this.filteredData?.length ?? 0) + this.pageSize;

    this.filteredData = this._allDocuments?.slice(0, pageSize);
  }

  popupClosed() {
    //TODO # optimize this not to re-query the database on refresh
    this.getClaimedDmerCases();
    this.searchDmerCase();
  }

  openClaimPopup(documentId?: string | null) {
    const dialogRef = this.dialog.open(ClaimDmerPopupComponent, {
      height: '600px',
      width: '820px',
      data: documentId,
    });
  }
}
