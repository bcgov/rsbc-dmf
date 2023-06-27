import { Component, Input, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';


@Component({
  selector: 'app-show-version',
  templateUrl: './show-version.component.html',
  styleUrls: ['./show-version.component.css']
})
export class ShowVersionComponent {
  @Input()
    token!: string;
  @Input()
    name!: string;
  versionData!: VersionData;

  baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

  }


  ngOnInit(): void {
    this.http.get<VersionData>(this.baseUrl + 'api/versions/' + this.token).subscribe(result => {

      var splitted = result.productVersion.split("!", 4);

      result.gitReference = splitted[0] + "/" + splitted[1] + "/commits/" + splitted[3]
      result.gitBranch = splitted[2];

      this.versionData = result;

      


    });

  }
}


interface VersionData {
  environment: string;
  fileCreationTime: string;
  fileVersion: string;
  productVersion: string;
  gitReference: string;
  gitBranch: string;

}
