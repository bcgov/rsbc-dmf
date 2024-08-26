import { Injectable } from '@angular/core';
import {
  CallbackService,
  CasesService,
  CommentsService,
  DocumentService,
  DocumentTypeService,
  DriverService,
  //ProfileService,
} from '../../api/services';

@Injectable({
  providedIn: 'root',
})
export class CaseManagementService {
  constructor(
    private casesService: CasesService,
    private driverService: DriverService,
    private documentService: DocumentService,
    //private profileService: ProfileService,
    private documentTypeService: DocumentTypeService,
    private callbackService: CallbackService,
    private commentsService: CommentsService
  ) { }

  public getMostRecentCase() {
    return this.casesService.apiCasesMostRecentGet$Json();
  }

  public getClosedCases(
    params: Parameters<CasesService['apiCasesClosedGet$Json']>[0]
  ) {
    return this.casesService.apiCasesClosedGet$Json(params);
  }

  // public getDriverDocuments(
  //   params: Parameters<DriverService['apiDriverDocumentsGet$Json']>[0]
  // ) {
  //   return this.driversService.apiDriverDocumentsGet$Json(params);
  // }

  public getAllDriverDocuments() {
    return this.driverService.apiDriverAllDocumentsGet$Json()
  }

  public getDownloadDocument(
    params: Parameters<DocumentService['apiDocumentDocumentIdGet$Json']>[0]
  ) {
    return this.documentService.apiDocumentDocumentIdGet$Json(params);
  }

  // public userRegistration(
  //   params: Parameters<ProfileService['apiProfileRegisterPut$Json$Response']>[0]
  // ) {
  //   return this.profileService.apiProfileRegisterPut$Json$Response(params);
  // }

  // public updateDriverProfile(
  //   params: Parameters<ProfileService['apiProfileDriverPut$Json$Response']>[0]
  // ) {
  //   return this.profileService.apiProfileDriverPut$Json$Response(params);
  // }

  // public getDocumentSubTypes(
  //   params: Parameters<DocumentTypeService['apiDocumentTypeDriverGet$Json']>[0]
  // ) {
  //   return this.documentTypeService.apiDocumentTypeDriverGet$Json(params);
  // }

  public getDocumentSubTypes() {
    return this.documentTypeService.apiDocumentTypeDocumentSubTypeGet$Json();
  }

  public getCallBackRequest(
    params: Parameters<CallbackService['apiCallbackDriverGet$Json']>[0]
  ) {
    return this.callbackService.apiCallbackDriverGet$Json(params);
  }

  public createCallBackRequest(
    params: Parameters<CallbackService['apiCallbackCreatePost$Json']>[0]
  ) {
    return this.callbackService.apiCallbackCreatePost$Json(params);
  }

  public cancelCallBackRequest(
    params: Parameters<CallbackService['apiCallbackCancelPut$Json']>[0]
  ) {
    return this.callbackService.apiCallbackCancelPut$Json(params);
  }

  public getComments(
    params: Parameters<CommentsService['apiCommentsGet$Json']>[0]
  ) {
    return this.commentsService.apiCommentsGet$Json(params);
  }


  public searchByDriver(
    params: Parameters<DriverService['apiDriverInfoDriverLicenceNumberGet$Json']>[0]
  ) {
    return this.driverService.apiDriverInfoDriverLicenceNumberGet$Json(params);
  }
  // public getDriverAddress(
  //   params: Parameters<DriverService['apiDriverInfoGet$Json']>[0]
  // ) {
  //   return this.driversService.apiDriverInfoGet$Json(params);
  // }
}
