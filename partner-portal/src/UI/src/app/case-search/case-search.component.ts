import { Component, OnInit, ViewChild, inject, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatAccordion, MatExpansionModule } from '@angular/material/expansion';
import { MatToolbar } from '@angular/material/toolbar';
import { RecentCaseComponent } from '@app/recent-case/recent-case.component';
import { SubmissionRequirementsComponent } from '../../app/submission-requirements/submission-requirements.component';
import { SubmissionHistoryComponent } from '@app/submission-history/submission-history.component';
import { RouterLink } from '@angular/router';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { CaseSearch } from '@app/shared/api/models';
import { MatTabsModule } from '@angular/material/tabs';
import { LettersToDriverComponent } from '@app/letters-to-driver/letters-to-driver.component';
import { GetAssistanceComponent } from '@app/get-assistance/get-assistance.component';
import { CommentsComponent } from '@app/comments/comments.component';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-case-search',
  standalone: true,
  imports: [
    MatCardModule,
    MatToolbar,
    MatExpansionModule,
    RecentCaseComponent,
    SubmissionRequirementsComponent,
    SubmissionHistoryComponent,
    RouterLink,
    MatTabsModule,
    LettersToDriverComponent,
    GetAssistanceComponent,
    CommentsComponent,
    MatDialogModule,
    MatIconModule
  ],
  templateUrl: './case-search.component.html',
  styleUrl: './case-search.component.scss',
})
export class CaseSearchComponent implements OnInit{

  //idCode = '';
  caseId = input<string>();

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  
  readonly dialog = inject(MatDialog);
  
  caseDetails: CaseSearch | null = null;
  
  constructor(
    private caseManagementService: CaseManagementService,
  ) {}
    
  ngOnInit(): void {
    this.searchByCaseId();
    console.log(this.caseDetails, this.caseId())
  }
  

  searchByCaseId(){
    this.caseManagementService.searchByCaseId({idCode: this.caseId() as string})
    .subscribe({
      next: (caseDetails) => {
        this.caseDetails = caseDetails
      },
      error: (error) => {
        console.error('error', error);
      }
  
    });
  }

  dialogRef? : MatDialogRef<CommentsComponent, any>

  openCommentsDialog(width?:string) {
   this.dialogRef = this.dialog.open(CommentsComponent, {
      height: '730px',
      width: '400px',
      position: {
        bottom: '8px',
        right: '8px',
      },
    });

    this.dialogRef.afterClosed().subscribe((result) => {
      console.log(`Dialog result: ${result}`);
    });
  }


}
