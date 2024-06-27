import { Component, Inject } from '@angular/core';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { DocumentService } from '@app/shared/api/services';
import { PatientCase } from '@app/shared/api/models';

@Component({
  selector: 'app-claim-dmer-popup',
  standalone: true,
  imports: [
    MatIconModule,
    MatDialogModule,
    MatRadioModule,
    MatButtonModule,
    MatSelectModule,
  ],
  templateUrl: './claim-dmer-popup.component.html',
  styleUrl: './claim-dmer-popup.component.scss',
})
export class ClaimDmerPopupComponent {
  constructor(
    private documentService: DocumentService,
    @Inject(MAT_DIALOG_DATA) public data: PatientCase,
  ) {}


  onClaimDmer() {
    this.documentService
      .apiDocumentClaimDmerPost$Json({
        documentId: this.data.documentId as string,
      })
      .subscribe();
  }


  onUnclaimDmer() {
    this.documentService.apiDocumentUnclaimDmerPost$Json({
      documentId: this.data.documentId as string,
    }).subscribe();
  }
}
