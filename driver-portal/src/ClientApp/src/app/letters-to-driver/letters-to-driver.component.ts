import { Component } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { PortalsEnum, SharedLettersToDriverComponent} from '@shared/core-ui';


@Component({
    selector: 'app-letters-to-driver',
    templateUrl: './letters-to-driver.component.html',
    styleUrls: ['./letters-to-driver.component.scss'],
    standalone: true,
    imports: [
        SharedLettersToDriverComponent
    ],
})
export class LettersToDriverComponent 
{

  PortalsEnum = PortalsEnum;
    
  constructor(public caseManagementService: CaseManagementService) 
    {}
}
