import { Component, OnInit, ChangeDetectionStrategy, Optional } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {ChangeDetectorRef} from '@angular/core';
import { ApplicationVersionInfoService } from 'src/app/shared/api/services/application-version-info.service';
import { VersionInfoComponent } from 'src/app/shared/components/version-info/version-info.component';
import { ApplicationVersionInfo } from 'src/app/shared/api/models';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FooterComponent implements OnInit {

  versionInfo: ApplicationVersionInfo = {
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
    @Optional() private versionInfoDataService: ApplicationVersionInfoService) {

  }

  ngOnInit(): void {
    if (this.versionInfoDataService !== null)
    {
      this.versionInfoDataService.apiApplicationVersionInfoGet$Json()
        .subscribe((versionInfo: ApplicationVersionInfo) => {
          this.versionInfo = versionInfo;
          this.cd.detectChanges();
        });
    }
  }

  openVersionInfoDialog() {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "500px",
      data: this.versionInfo
    };

    // open dialog, get reference and process returned data from dialog
    this.dialog.open(VersionInfoComponent, dialogConfig);
  }

}
