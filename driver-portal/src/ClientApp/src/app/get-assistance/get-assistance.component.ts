import { Component, Input } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';

@Component({
  selector: 'app-get-assistance',
  templateUrl: './get-assistance.component.html',
  styleUrls: ['./get-assistance.component.scss'],
})
export class GetAssistanceComponent {
  constructor(private caseManagementService: CaseManagementService) {}
  isExpanded: Record<string, boolean> = {};
  pageSize = 10;
  display = 0;

  cancelCallback() {
    console.log('cancelCallback');
  }

  helpcard() {
    this.display = 5;
  }

  recommendedTopics() {
    this.display = 0;
  }
}
