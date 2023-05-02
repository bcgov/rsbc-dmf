/* eslint-disable @typescript-eslint/explicit-member-accessibility */
import { Component, OnInit } from '@angular/core';

import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-endorsement-license',
  templateUrl: './endorsement-license.component.html',
  styleUrls: ['./endorsement-license.component.scss'],
})
export class EndorsementLicenseComponent implements OnInit {
  public constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {}

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void {
    //this.config.data
  }
}
