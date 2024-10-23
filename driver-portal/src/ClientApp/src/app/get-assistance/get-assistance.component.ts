import { Component } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { UserService } from '../shared/services/user.service';
import { SharedGetAssistanceComponent } from '@shared/core-ui';

interface CallBackTopic {
  value: string;
  viewValue: string;
}

enum HelpTopics {
  ALL_TOPICS = 0,
  UPLOAD_EXTRA_DOCUMENT = 1,
  VIEW_DMER_SUBMISSION = 2,
  RECEIVED_AN_INCORRECT_LETTER = 3,
  DOCUMENT_HISTORY = 4,
  REPLACEMENT_DOCUMENT = 5,
  REQUEST_EXTENSION = 6,
}

@Component({
    selector: 'app-get-assistance',
    templateUrl: './get-assistance.component.html',
    styleUrls: ['./get-assistance.component.scss'],
    standalone: true,
    imports: [
      SharedGetAssistanceComponent,
  
    ],
})

export class GetAssistanceComponent  {
  constructor(
    public caseManagementService: CaseManagementService,
    public userService: UserService,
  ) {}

}
