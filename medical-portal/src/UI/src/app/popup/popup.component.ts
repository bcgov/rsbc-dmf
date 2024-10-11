import { Component, ElementRef, Inject, ViewChild, TemplateRef } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogActions } from '@angular/material/dialog';
import { PopupService } from './popup.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ChefsService } from '../shared/api/services';
import { v4 as uuidv4 } from 'uuid';
import { SubmissionStatus } from '@app/features/chefs/enums/chefs-status.enum';
import { MatIcon } from '@angular/material/icon';
import { ConfigurationService } from '@app/shared/services/configuration.service';

@Component({
  selector: 'app-popup',
  standalone: true,
  imports: [MatDialogActions, MatIcon],
  templateUrl: './popup.component.html',
  styleUrl: './popup.component.scss',
})
export class PopupComponent {
  @ViewChild('iframe') iframe!: ElementRef;
  @ViewChild('warningDialog') warningDialog!: TemplateRef<any>;
  public sanitizedSource!: SafeResourceUrl;
  iframeUrl: string | null = null;
  instanceId: string | null = null;
  caseId: string;
  documentId: string;

  constructor(
    private popupService: PopupService,
    private chefsService: ChefsService,
    private sanitizer: DomSanitizer,
    private configService: ConfigurationService,
    @Inject(MAT_DIALOG_DATA) public data: { caseId: string, documentId: string },
    private dialog: MatDialog
  ) {
      this.caseId = data.caseId;
      this.documentId = data.documentId;
  }

  getSourceURL(): SafeResourceUrl {
    return this.sanitizedSource;
  }

  ngOnInit() {
    this.instanceId = uuidv4();
    this.iframeUrl = `https://submit.digital.gov.bc.ca/app/form/submit?f=${this.configService.config.chefsFormId}&instanceId=${this.instanceId}`;
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
      console.log(`[HOST] TX (to iframe): Host fulfilled request of type: ${type} and retrieved payload:`, payload);
      this.iframe.nativeElement.contentWindow.postMessage(
        JSON.stringify({
          type,
          ...(payload ? payload : {}),
        }),
        // TODO this should be configurable
        'https://submit.digital.gov.bc.ca',
      );
    }
  }

  receiveMessage(event: {
    origin: string;
    data: {
      instanceId: string;
      type: string;
      status: SubmissionStatus;
      submission: any;
      flags: any | undefined;
      assign: string;
      priority: string;
    };
  }): void {
    // TODO this should be configurable
    if (event.origin !== 'https://submit.digital.gov.bc.ca' && event.origin !== 'https://common-logon-test.hlth.gov.bc.ca') {
      console.error(`Event message origin was not expected: ${event.origin}`);
      return; // Ensure message is from expected origin
    }

    const {
      data: { instanceId, type, status, submission, flags, assign, priority },
    } = event;

    if (instanceId !== this.instanceId) {
      console.warn( `[HOST] RX (from iframe): Ignoring message from old instanceId: ${instanceId}, current instanceId is: ${this.instanceId}`);
      return;
    }

    console.log(`[HOST] RX (from iframe): Host received request of type: ${type} begin processing...:`, event);

    if (type === 'GET_CHEFS_BUNDLE' && this.caseId) {
      let params: Parameters<ChefsService['apiChefsBundleGet$Json']>[0] = {
        caseId: this.caseId,
      };

      this.chefsService
        .apiChefsBundleGet$Json({ ...params })
        .subscribe((bundle) => {
          console.log(bundle);
          this.sendMessage(type, bundle);
          return bundle;
        });
    } else if (type === 'GET_CHEFS_SUBMISSION' && this.caseId) {
      let params: Parameters<ChefsService['apiChefsSubmissionGet$Json']>[0] = {
        caseId: this.caseId,
      };

      this.chefsService.apiChefsSubmissionGet$Json({ ...params }).subscribe(
        (submission) => {
          console.log(submission);
          this.sendMessage(type, submission);
          return submission;
        },
        (error) => {
          if (error.status === 404) {
            this.sendMessage(type, {});
          } else {
            console.error('[HOST] apiChefsSubmissionGet An error occurred:', error);
          }
        },
      );
    } else if (
      type === 'PUT_CHEFS_SUBMISSION' &&
      status &&
      submission &&
      this.caseId
    ) {
      let params: Parameters<ChefsService['apiChefsSubmissionPut$Json']>[0] = {
        caseId: this.caseId,
        documentId: this.documentId,
        body: {
          status,
          submission,
          flags,
          assign,
          priority,
        },
      };
      let previouSubmissionStatus = status;
      this.chefsService
        .apiChefsSubmissionPut$Json({ ...params })
        .subscribe((submission) => {
          console.log(submission);
          // if submission status was final but returned draft, it is because the user lacked permissions, so display a warning dialog
          if (previouSubmissionStatus == SubmissionStatus.Final && submission.status == SubmissionStatus.Draft) {
            this.dialog.open(this.warningDialog);
          }
          return submission;
        });
    }
    if (status === SubmissionStatus.Final) {
      this.closePopup();
    }
  }
}
