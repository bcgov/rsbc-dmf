
import { Component } from '@angular/core';
import { SharedSubmissionHistoryComponent } from '@shared/core-ui';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';

@Component({
  selector: 'app-submission-history',
  templateUrl: './submission-history.component.html',
  styleUrls: ['./submission-history.component.scss'],
  standalone: true,
  imports: [
    SharedSubmissionHistoryComponent
  ],
})
export class SubmissionHistoryComponent  {
  
  constructor(public caseManagementService: CaseManagementService) {}
}
