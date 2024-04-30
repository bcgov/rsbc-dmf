import { Component, OnInit, ChangeDetectionStrategy, Optional, CUSTOM_ELEMENTS_SCHEMA, Input } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import {ChangeDetectorRef} from '@angular/core';
import { VersionInfoComponent } from '../version-info/version-info.component'
import { RouterLink } from '@angular/router';
import {MatToolbarModule} from '@angular/material/toolbar';


@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss'],
  imports: [RouterLink, MatToolbarModule, MatDialogModule, VersionInfoComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone : true,
  schemas : [CUSTOM_ELEMENTS_SCHEMA]
})
export class FooterComponent {

  @Input() versionInfo: any = {
    baseUri: "",
    basePath: "",
    environment: "",
    sourceCommit: "",
    sourceRepository: "",
    sourceReference: "",
    fileCreationTime: "",
    fileVersion : "Loading..." };

  constructor(
    private cd: ChangeDetectorRef,
    @Optional() private dialog: MatDialog,
    ) {

  }

  openVersionInfoDialog() {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "500px",
      data: this.versionInfo,
      height:"500px"
    };

    // open dialog, get reference and process returned data from dialog
    this.dialog.open(VersionInfoComponent, dialogConfig);
  }

}

