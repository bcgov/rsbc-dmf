import { Component } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { UserServices } from '../shared/services/user.service';
import { SharedGetAssistanceComponent } from '@shared/core-ui';


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
    public userService: UserServices,
  ) {}

}
