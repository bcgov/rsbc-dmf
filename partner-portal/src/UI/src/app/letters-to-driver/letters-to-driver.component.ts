import { Component } from '@angular/core';
import { QuickLinksComponent } from '../quick-links/quick-links.component';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { SharedLettersToDriverComponent} from '@shared/core-ui';

@Component({
  selector: 'app-letters-to-driver',
  templateUrl: './letters-to-driver.component.html',
  styleUrls: ['./letters-to-driver.component.scss'],
  standalone: true,
  imports: [
      QuickLinksComponent,
      SharedLettersToDriverComponent
    
  ],
})
export class LettersToDriverComponent 
{
  constructor(public caseManagementService: CaseManagementService) 
    {}
}
