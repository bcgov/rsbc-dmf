import {
  CUSTOM_ELEMENTS_SCHEMA,
  Component,
} from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { PortalsEnum, SharedSubmissionRequirementsComponent } from '@shared/core-ui';

@Component({
    selector: 'app-submission-requirements',
    templateUrl: './submission-requirements.component.html',
    styleUrls: ['./submission-requirements.component.scss'],
    standalone: true,
    imports: [
      SharedSubmissionRequirementsComponent
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class SubmissionRequirementsComponent{
 
  PortalsEnum = PortalsEnum;
    
  constructor(public caseManagementService: CaseManagementService) 
    {}

}
