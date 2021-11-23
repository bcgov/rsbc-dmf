import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {ChangeDetectorRef} from '@angular/core';
import { VersionInfo } from 'src/app/shared/api/models/version-info';
import { ApplicationVersionInfoService } from 'src/app/shared/api/services/application-version-info.service';
import { VersionInfoComponent } from 'src/app/shared/components/version-info/version-info.component';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FooterComponent implements OnInit {

  versionInfo: VersionInfo | undefined;

  constructor(
    private cd: ChangeDetectorRef,
    public dialog: MatDialog,
    private versionInfoDataService: ApplicationVersionInfoService) { 
            
  }

  ngOnInit(): void {
    this.versionInfoDataService.getVersionInfo()      
    .subscribe((versionInfo: VersionInfo) => {
      this.versionInfo = versionInfo;
      this.cd.detectChanges();
    }); 
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
