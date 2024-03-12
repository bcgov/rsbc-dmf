import { Injectable } from '@angular/core';
import {
  CallbackService,
  CasesService,
  DocumentService,
  DocumentTypeService,
  DriverService,
  ProfileService,
} from '../../api/services';

@Injectable({
  providedIn: 'root',
})
export class CaseManagementService {
  constructor(
    private casesService: CasesService,
    private driversService: DriverService,
    private documentService: DocumentService,
    private profileService: ProfileService,
    private documentTypeService: DocumentTypeService,
    private callbacService: CallbackService
  ) {}

  public getMostRecentCase(
    params: Parameters<CasesService['apiCasesMostRecentGet$Json']>[0]
  ) {
    return this.casesService.apiCasesMostRecentGet$Json(params);
  }

  public getClosedCases(
    params: Parameters<CasesService['apiCasesClosedGet$Json']>[0]
  ) {
    return this.casesService.apiCasesClosedGet$Json(params);
  }

  public getDriverDocuments(
    params: Parameters<DriverService['apiDriverDocumentsGet$Json']>[0]
  ) {
    return this.driversService.apiDriverDocumentsGet$Json(params);
  }

  public getAllDocuments(
    params: Parameters<DriverService['apiDriverAllDocumentsGet$Json']>[0]
  ) {
    return this.driversService.apiDriverAllDocumentsGet$Json(params);
  }

  public getDownloadDocument(
    params: Parameters<DocumentService['apiDocumentDocumentIdGet$Json']>[0]
  ) {
    return this.documentService.apiDocumentDocumentIdGet$Json(params);
  }

  public userRegistration(
    params: Parameters<ProfileService['apiProfileRegisterPut$Json']>[0]
  ) {
    return this.profileService.apiProfileRegisterPut$Json(params);
  }

  public updateDriverProfile(
    params: Parameters<ProfileService['apiProfileDriverPut$Response']>[0]
  ) {
    return this.profileService.apiProfileDriverPut$Response(params);
  }

  public getDocumentSubTypes(
    params: Parameters<DocumentTypeService['apiDocumentTypeDriverGet$Json']>[0]
  ) {
    return this.documentTypeService.apiDocumentTypeDriverGet$Json(params);
  }

  public getCallBackRequest(
    params: Parameters<CallbackService['apiCallbackDriverGet$Json']>[0]
  ) {
    return this.callbacService.apiCallbackDriverGet$Json(params);
  }

  public createCallBackRequest(
    params: Parameters<CallbackService['apiCallbackCreatePost$Json']>[0]
  ) {
    return this.callbacService.apiCallbackCreatePost$Json(params);
  }

  public cancelCallBackRequest(
    params: Parameters<CallbackService['apiCallbackCancelPut$Json']>[0]
  ) {
    return this.callbacService.apiCallbackCancelPut$Json(params);
  }
}
