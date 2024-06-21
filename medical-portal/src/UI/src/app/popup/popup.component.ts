import {
  AfterViewInit,
  Component,
  ElementRef,
  Inject,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PopupService } from './popup.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ChefsService } from '../shared/api/services';
import { v4 as uuidv4 } from 'uuid';

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
  iframeUrl: string | null = null;
  instanceId: string | null = null;
  caseId: string | null = null;

  constructor(
    private popupService: PopupService,
    private chefsService: ChefsService,
    private sanitizer: DomSanitizer,
    @Inject(MAT_DIALOG_DATA) public data: { caseId: string | null },
  ) {
    this.caseId = data.caseId;
  }

  getSourceURL(): SafeResourceUrl {
    return this.sanitizedSource;
  }

  ngOnInit() {
    this.instanceId = uuidv4();
    this.iframeUrl = `https://submit.digital.gov.bc.ca/app/form/submit?f=5383fc89-b219-49a2-924c-251cd1557eb8&instanceId=${this.instanceId}`;
    this.sanitizedSource = this.sanitizer.bypassSecurityTrustResourceUrl(
      this.iframeUrl,
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

  sendMessage(type: string, payload: any) {
    if (this.iframe?.nativeElement?.contentWindow) {
      console.log(
        `[HOST] TX (to iframe): Host fulfilled request of type: ${type} and retrieved payload:`,
      );
      console.log(payload);
      this.iframe.nativeElement.contentWindow.postMessage(
        JSON.stringify({
          type,
          ...(payload ? payload : {}),
        }),
        'https://submit.digital.gov.bc.ca',
      );
    }
  }

  receiveMessage(event: {
    origin: string;
    data: { instanceId: string; type: string; status: string; submission: any };
  }): void {
    if (event.origin !== 'https://submit.digital.gov.bc.ca') return; // Ensure message is from expected origin

    const {
      data: { instanceId, type, status, submission },
    } = event;

    if (instanceId !== this.instanceId) {
      console.warn(
        `[HOST] RX (from iframe): Ignoring message from old instanceId: ${instanceId}, current instanceId is: ${this.instanceId}`,
      );
      return;
    }

    console.log(
      `[HOST] RX (from iframe): Host received request of type: ${type} begin processing...:`,
    );
    console.log(event);

    if (type === 'GET_CHEFS_BUNDLE') {
      let params: Parameters<ChefsService['apiChefsBundleGet']>[0] = {
        caseId: this.caseId,
      };

      this.chefsService.apiChefsBundleGet({ ...params }).subscribe((bundle) => {
        console.log(bundle);
        this.sendMessage(type, bundle);
        return bundle;
      });
    } else if (type === 'GET_CHEFS_SUBMISSION') {
      let params: Parameters<ChefsService['apiChefsSubmissionGet']>[0] = {
        caseId: this.caseId,
      };

      this.chefsService.apiChefsSubmissionGet({ ...params }).subscribe(
        (submission) => {
          console.log(submission);
          this.sendMessage(type, submission);
          return submission;
        },
        (error) => {
          if (error.status === 404) {
            this.sendMessage(type, {});
          } else {
            console.error(
              '[HOST] apiChefsSubmissionGet An error occurred:',
              error,
            );
            // Handle other types of errors here
          }
        },
      );
    } else if (type === 'PUT_CHEFS_SUBMISSION' && status && submission) {
      let params: Parameters<ChefsService['apiChefsSubmissionPut']>[0] = {
        caseId: this.caseId,
        body: {
          status,
          submission,
        },
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
