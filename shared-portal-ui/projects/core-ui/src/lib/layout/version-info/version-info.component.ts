import { CUSTOM_ELEMENTS_SCHEMA, Component, Inject, OnInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from "@angular/material/dialog";
import {MatButtonModule} from '@angular/material/button';
//import { ApplicationVersionInfo } from "../../api/models";
@Component({
    selector: 'app-version-info',
    templateUrl: './version-info.component.html',
    styleUrls: ['./version-info.component.scss'],
    standalone: true,
    imports:[MatDialogModule, MatButtonModule],
    schemas : [CUSTOM_ELEMENTS_SCHEMA]
})
export class VersionInfoComponent implements OnInit {
  versionInfo:any;

  constructor(
    public dialogRef: MatDialogRef<VersionInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    if (data) {
      this.versionInfo = data;
    }
    else
    {
      this.versionInfo = {baseUri: "",
        basePath: "",
        environment: "Unknown",
        sourceCommit: "Unknown",
        sourceRepository: "Unknown",
        sourceReference: "Unknown",
        fileCreationTime: "Unknown",
        fileVersion: "Unknown"};
    }
  }

  public ngOnInit(): void {
    return;
  }

  close() {
    this.dialogRef.close();
  }
}


