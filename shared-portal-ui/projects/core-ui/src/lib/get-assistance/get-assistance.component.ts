import { Component, Input, OnInit } from '@angular/core';
import {
  ViewportScroller,
  NgIf,
  NgFor,
  NgClass,
  DatePipe,
} from '@angular/common';
import { Callback, CallStatus, PreferredTime } from '../api';
import { SharedCancelCallbackDialogComponent } from './cancel-callback-dialog/cancel-callback-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
  FormBuilder,
  Validators,
  ReactiveFormsModule,
  FormsModule,
} from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatInput } from '@angular/material/input';
import { MatOption } from '@angular/material/core';
import { MatSelect } from '@angular/material/select';
import { MatFormField, MatLabel, MatError } from '@angular/material/form-field';
import { MatCard, MatCardContent } from '@angular/material/card';
import { CdkMenu, CdkMenuItem } from '@angular/cdk/menu';
import { CallStatusDescription } from '../app.model';


interface CallBackTopic {
  value: string;
  viewValue: string;
}

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
  selector: 'app-shared-get-assistance',
  templateUrl: './get-assistance.component.html',
  styleUrls: ['./get-assistance.component.scss'],
  standalone: true,
  imports: [
    CdkMenu,
    CdkMenuItem,
    MatCard,
    MatCardContent,
    NgIf,
    ReactiveFormsModule,
    FormsModule,
    MatFormField,
    MatLabel,
    MatSelect,
    MatOption,
    MatError,
    MatInput,
    MatButton,
    NgFor,
    NgClass,
    MatIcon,
    DatePipe,
  ],
})
export class SharedGetAssistanceComponent implements OnInit {
  HelpTopics = HelpTopics;
  CallStatusDescription = CallStatusDescription;
  showCallBackCreate = false;

  @Input() caseManagementService: any;
  @Input() userService: any;
  
  constructor(
    private viewportScroller: ViewportScroller,
    public dialog: MatDialog,
    private _snackBar: MatSnackBar,
    private fb: FormBuilder,
  ) {}

  callbackRequestForm = this.fb.group({
    caseId: [''],
    description: [''],
    subject: ['', Validators.required],
    phone: ['', Validators.compose([Validators.required, Validators.maxLength(10)])],
    preferredTime: [0],
  });

  isExpanded: Record<string, boolean> = {};
  pageSize = 10;
  display: HelpTopics = HelpTopics.ALL_TOPICS;

  filteredCallbacks?: Callback[] | null = [];

  _allCallBackRequests?: Callback[] | null = [];

  disableCallBack = true;

  @Input() set allCallBacks(callbacks: Callback[] | null | undefined) {
    this._allCallBackRequests = callbacks;

    this._allCallBackRequests?.forEach((req) => {
      if (req.id) this.isExpanded[req.id] = false;
    });

    this.filteredCallbacks = this._allCallBackRequests?.slice(0, this.pageSize);
  }

  get allCallBacks() {
    return this._allCallBackRequests;
  }

  showCallBack = false;

  showOpenCallbackMessagePredicate = (r: Callback) =>
    r.callStatus === CallStatus.Open;

  selectedValue?: string | undefined | null;

  callBackTopics: CallBackTopic[] = [
    { value: '1', viewValue: 'Upload an extra document' },
    { value: '2', viewValue: 'View DMER Submission' },
    { value: '3', viewValue: 'Received an Incorrect Letter' },
    { value: '4', viewValue: 'Request extension' },
  ];

  ngOnInit(): void {
    var userId = this.userService.getUserId();
    if (userId) {
      this.getCallbackRequests(userId);
    }
  }

  getCallbackRequests(driverId: string) {
    this.caseManagementService
      .getCallBackRequest(driverId)
      .subscribe((callBacks: any) => {
        this._allCallBackRequests = callBacks;
        this.filteredCallbacks = this._allCallBackRequests?.slice(
          0,
          this.pageSize
        );
        this.disableCallBack = !!callBacks.find(
          (y: any) => y.callStatus == 'Open'
        );
      });
  }

  isCreatingCallBack = false;

  createCallBack() {
    console.log("phone",  this.callbackRequestForm.controls.phone.errors);
    if (this.callbackRequestForm.invalid) {
      this.callbackRequestForm.markAllAsTouched();
      return;
    }
    if (this.isCreatingCallBack) {
      return;
    }
    const callback: any = {

      phone: String(this.callbackRequestForm.value.phone),
      preferredTime: Number(this.callbackRequestForm.value.preferredTime),
      subject: this.callBackTopics.find(
        (x) => x.value == this.callbackRequestForm.value.subject,
      )?.viewValue,
    };
    this.isCreatingCallBack = true;
    this.caseManagementService
      .createCallBackRequest({ body: callback })
      .subscribe(() => {
        this.callbackRequestForm.reset();
        let userId = this.userService.getUserId();
        this.getCallbackRequests(userId);
        this.showCallBack = false;
        this._snackBar.open('Successfully created call back request', 'Close', {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 5000,
        });
        this.isCreatingCallBack = false;
      });
  }

  openCancelCallbackDialog(callback: any) {
    this.dialog
      .open(SharedCancelCallbackDialogComponent, {
        height: '650px',
        width: '820px',
        data: {
          callbackId: callback.id,
          caseManagementService: this.caseManagementService
        },
      })
      .afterClosed()
      .subscribe({
        next: () => {
          let userId = this.userService.getUserId();
          this.getCallbackRequests(userId);
        },
      });
  }

  helpcard(helpTopic: HelpTopics) {
    this.display = helpTopic;
    this.showCallBackCreate = true;
  }

  recommendedTopics() {
    this.display = HelpTopics.ALL_TOPICS;
    this.showCallBackCreate = false;
  }
  onRequestCallBack() {
    this.showCallBack = true;
  }

  back() {
    this.showCallBack = false;
  }

  viewRequest(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

  viewMore() {
    const pageSize = (this.filteredCallbacks?.length ?? 0) + this.pageSize;

    this.filteredCallbacks = this._allCallBackRequests?.slice(0, pageSize);
  }
}
