import { CdkMenu, CdkMenuItem } from '@angular/cdk/menu';
import { Component } from '@angular/core';
import { MatCard, MatCardContent } from '@angular/material/card';

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
  selector: 'app-get-help',
  standalone: true,
  imports: [
    CdkMenu,
    CdkMenuItem,
    MatCard, 
    MatCardContent
  ],
  templateUrl: './get-help.component.html',
  styleUrl: './get-help.component.scss'
})
export class GetHelpComponent {
  HelpTopics = HelpTopics;
  display: HelpTopics = HelpTopics.ALL_TOPICS;

  helpcard(helpTopic: HelpTopics) {
    this.display = helpTopic;
  }

  recommendedTopics() {
    this.display = HelpTopics.ALL_TOPICS;
  }
}
