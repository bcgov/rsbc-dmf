import { Component, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCommonModule } from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { DMERStatusEnum } from '@app/app.model';
import { ClaimDmerPopupComponent } from '@app/claim-dmer-popup/claim-dmer-popup.component';
import { Role } from '@app/features/auth/enums/identity-provider.enum';
import { PatientCase } from '@app/shared/api/models';
import { ProfileManagementService } from '@app/shared/services/profile.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-dmer-buttons',
  standalone: true,
  imports: [
    MatCommonModule,
    //MatExpansionModule,
    //MatCardModule,
    //MatIconModule,
    //MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    //MatSelectModule,
    ClaimDmerPopupComponent
  ],
  templateUrl: './dmer-buttons.component.html',
  styleUrl: './dmer-buttons.component.scss'
})
export class DmerButtonsComponent {

  public accessLevel: Role = Role.Moa;
  @Input() public searchedCase?: PatientCase | null;
  DMERStatusEnum = DMERStatusEnum;

  constructor(
    private profileManagementService: ProfileManagementService,
    private dialog: MatDialog,
  ) {

  }

  ngOnInit(): void {
    console.log('DMER Buttons Component Initialized', this.searchedCase);
    let profile = this.profileManagementService.getCachedProfile();
    // TODO will be using "accessLevel" in other areas, move to profile service
    this.accessLevel = profile.roles?.find((role) => role === Role.Practitioner) ? Role.Practitioner : Role.Moa;
  }

  openPopup() {
    if (!this.searchedCase) {
      console.error('Case data was missing', this.searchedCase);
      return;
    }
    // this.popupService
    //   .openPopup(this.searchedCase.caseId as string, this.searchedCase.documentId as string)
    //   .subscribe((result) => {
    //     this.searchDmerCase();
    //   });
  }

  openClaimPopup(documentId?: string | null): Observable<any> {
    const dialogRef = this.dialog.open(ClaimDmerPopupComponent, {
      height: '600px',
      width: '820px',
      data: documentId,
    });
    return dialogRef.afterClosed();
    // dialogRef.afterClosed().subscribe((result) => {
    //   //TODO # optimize this not to re-query the database on refresh
    //   this.getClaimedDmerCases();
    //   this.searchDmerCase();
    //   console.log('The dialog was closed', result);
    // });
  }

  public get Role() {
    return Role;
  }
}
