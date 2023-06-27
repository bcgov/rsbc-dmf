import { Component, Input, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OnInit, OnDestroy } from '@angular/core';


@Component({
  selector: 'app-show-version',
  templateUrl: './show-version.component.html',
  styleUrls: ['./show-version.component.css']
})
export class ShowVersionComponent implements OnInit, OnDestroy {
  @Input()
    token!: string;
  @Input()
    name!: string;
  versionData!: VersionData;

  baseUrl: string;
  id: any;
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

  }

  refresh() {
    this.http.get<VersionData>(this.baseUrl + 'api/versions/' + this.token).subscribe(result => {

      var splitted = result.productVersion.split("!", 4);

      result.gitReference = splitted[0] + "/" + splitted[1] + "/commits/" + splitted[3]
      result.gitBranch = splitted[2];

      this.versionData = result;




    });
  }

  ngOnDestroy() {
    if (this.id) {
      clearInterval(this.id);
    }
  }

  ngOnInit(): void {
    this.refresh();
    this.id = setInterval(() => {
      this.refresh();
    }, 5000);


    

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
