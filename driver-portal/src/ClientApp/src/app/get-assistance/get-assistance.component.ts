import { Component, Input, OnInit } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { ViewportScroller } from '@angular/common';
import { LoginService } from '../shared/services/login.service';
import { BringForwardRequest, Callback } from '../shared/api/models';
import { CancelCallbackDialogComponent } from './cancel-callback-dialog/cancel-callback-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

interface CallBackTopic {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-get-assistance',
  templateUrl: './get-assistance.component.html',
  styleUrls: ['./get-assistance.component.scss'],
})
export class GetAssistanceComponent implements OnInit {
  constructor(
    private caseManagementService: CaseManagementService,
    private viewportScroller: ViewportScroller,
    private loginService: LoginService,
    public dialog: MatDialog,
    private _snackBar: MatSnackBar
  ) {}
  isExpanded: Record<string, boolean> = {};
  pageSize = 10;
  display = 0;

  filteredCallbacks?: Callback[] | null = [];

  _allCallBackRequests?: Callback[] | null = [];

  @Input() set allCallBacks(callbacks: Callback[] | null | undefined) {
    this._allCallBackRequests = callbacks;

    this._allCallBackRequests?.forEach((req) => {
      if (req.id) this.isExpanded[req.id] = false;
    });

    this.filteredCallbacks = this._allCallBackRequests?.slice(0, this.pageSize);
    console.log(this.filteredCallbacks);
  }

  get allCallBacks() {
    return this._allCallBackRequests;
  }

  showCallBack = false;

  selectedValue?: string | undefined | null;

  callBackTopics: CallBackTopic[] = [
    { value: '1', viewValue: 'Upload an extra document' },
    { value: '2', viewValue: 'View DMER Submission' },
    { value: '3', viewValue: 'Received an Inccorect Letter' },
    { value: '4', viewValue: 'Request extension' },
  ];

  ngOnInit(): void {
    if (this.loginService.userProfile) {
      this.getCallbackRequests(this.loginService.userProfile.id as string);
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
      });
  }

  createCallBack() {
    const callback: BringForwardRequest = {
      caseId: '',
      description: 'Test',
      subject: 'Testing callback',
    };
    return this.caseManagementService
      .createCallBackRequest({ body: callback })
      .subscribe((response) => {
        this.getCallbackRequests(this.loginService.userProfile?.id as string);
        this.showCallBack = false;
        this._snackBar.open('Successfully created call back request', 'Close', {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 2000,
        });
      });
  }

  openCancelCallbackDialog(callback: Callback) {
    const dialogRef = this.dialog
      .open(CancelCallbackDialogComponent, {
        height: '650px',
        width: '820px',
        data: {
          callbackId: callback.id,
        },
      })
      .afterClosed()
      .subscribe({
        next: () => {
          this.getCallbackRequests(this.loginService.userProfile?.id as string);
        },
      });
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

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

  viewMore() {
    const pageSize = (this.filteredCallbacks?.length ?? 0) + this.pageSize;

    this.filteredCallbacks = this._allCallBackRequests?.slice(0, pageSize);
    console.log(pageSize);
  }
}
