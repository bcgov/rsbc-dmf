import {
  AfterViewInit,
  Component,
  ElementRef,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { PopupService } from './popup.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ChefsService } from '../shared/api/services';

@Component({
  selector: 'app-popup',
  standalone: true,
  imports: [],
  templateUrl: './popup.component.html',
  styleUrl: './popup.component.scss',
})
export class PopupComponent {
  @ViewChild('iframe') iframe!: ElementRef;
  public sanitizedSource!: SafeResourceUrl;
  iframeUrl: string =
    'https://submit.digital.gov.bc.ca/app/form/submit?f=5383fc89-b219-49a2-924c-251cd1557eb8';

  constructor(
    private popupService: PopupService,
    private chefsService: ChefsService,
    private sanitizer: DomSanitizer,
  ) {}

  getSourceURL(): SafeResourceUrl {
    return this.sanitizedSource;
  }

  ngOnInit() {
    this.sanitizedSource = this.sanitizer.bypassSecurityTrustResourceUrl(
      this.iframeUrl
    );
    // Listen for messages from iframe
    window.addEventListener('message', this.receiveMessage.bind(this), false);
  }

  closePopup() {
    this.popupService.closePopup();
  }

  iframeLoaded() {
    // call sendMessage() and pass in data needed
  }

  sendMessage() {
    if (this.iframe) {
      console.log('acknowledging message from iframe...');
      this.iframe.nativeElement.contentWindow.postMessage(
        'ackMessage',
        'https://submit.digital.gov.bc.ca'
      );
    }
  }

  receiveMessage(event: {
    origin: string;
    data: { type: string; status: string; submission: any };
  }): void {
    if (event.origin !== 'https://submit.digital.gov.bc.ca') return; // Ensure message is from expected origin

    const {
      data: { type, status, submission },
    } = event;

    if (type === 'PUT_CHEFS_SUBMISSION') {
      let params: Parameters<ChefsService['apiChefsSubmissionPut']>[0] = {
        status,
        submission,
      };
      this.chefsService
        .apiChefsSubmissionPut({ ...params })
        .subscribe((submission) => {
          console.log(submission);
          return submission;
        });
    }
    if (status === 'FINAL') {
      this.closePopup();
    }
  }
}
