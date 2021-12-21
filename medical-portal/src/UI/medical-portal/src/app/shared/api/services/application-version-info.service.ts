import { Injectable } from "@angular/core";
import { HttpHeaders, HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { VersionInfo } from "../models/version-info";
import { BaseService } from "../base-service";
import { ApiConfiguration } from "../api-configuration";

@Injectable()
export class ApplicationVersionInfoService extends BaseService {

  apiPath = "api/ApplicationVersionInfo";
  headers = new HttpHeaders({
    'Content-Type': "application/json"
  });

  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  getVersionInfo(): Observable<VersionInfo> {
    return this.http.get<VersionInfo>(this.apiPath, { headers: this.headers });
  }
}

