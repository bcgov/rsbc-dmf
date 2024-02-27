import { Component, Input } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { ViewportScroller } from '@angular/common';

interface CallBackTopic {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-get-assistance',
  templateUrl: './get-assistance.component.html',
  styleUrls: ['./get-assistance.component.scss'],
})
export class GetAssistanceComponent {
  constructor(
    private caseManagementService: CaseManagementService,
    private viewportScroller: ViewportScroller
  ) {}
  isExpanded: Record<string, boolean> = {};
  pageSize = 10;
  display = 0;

  showCallBack = false;

  selectedValue?: string | undefined | null;

  callBackTopics: CallBackTopic[] = [
    { value: '1', viewValue: 'Upload an extra document' },
    { value: '2', viewValue: 'View DMER Submission' },
    { value: '3', viewValue: 'Received an Inccorect Letter' },
    { value: '4', viewValue: 'Request extension' },
  ];

  cancelCallback() {
    console.log('cancelCallback');
  }

  helpcard() {
    this.display = 5;
  }

  recommendedTopics() {
    this.display = 0;
  }
  onRequestCallBack() {
    console.log('onRequestCallBack');
    this.showCallBack = true;
  }

  back() {
    console.log('back');
    this.showCallBack = false;
  }

  viewRequest(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }
}
