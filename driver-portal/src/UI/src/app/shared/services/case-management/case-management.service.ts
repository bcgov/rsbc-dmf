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
    private driverService: DriverService,
    private documentService: DocumentService,
    private profileService: ProfileService,
    private documentTypeService: DocumentTypeService,
    private callbacService: CallbackService
  ) {}

  // Case
  public getMostRecentCase() {
    return this.casesService.apiCasesMostRecentGet$Json();
  }

  public getClosedCases(
    params: Parameters<CasesService['apiCasesClosedGet$Json']>[0]
  ) {
    return this.casesService.apiCasesClosedGet$Json(params);
  }

  // Documents
  public getAllDriverDocuments() {
    return this.driverService.apiDriverAllDocumentsGet$Json();
  }

  public getDownloadDocument(
    params: Parameters<DocumentService['apiDocumentDocumentIdGet$Json']>[0]
  ) {
    return this.documentService.apiDocumentDocumentIdGet$Json(params);
  }

  public getDocumentSubTypes(
    params: Parameters<DocumentTypeService['apiDocumentTypeDriverGet$Json']>[0]
  ) {
    return this.documentTypeService.apiDocumentTypeDriverGet$Json(params);
  }

  // User Registration
  public userRegistration(
    params: Parameters<ProfileService['apiProfileRegisterPut$Json$Response']>[0]
  ) {
    return this.profileService.apiProfileRegisterPut$Json$Response(params);
  }

  public updateDriverProfile(
    params: Parameters<ProfileService['apiProfileDriverPut$Json$Response']>[0]
  ) {
    return this.profileService.apiProfileDriverPut$Json$Response(params);
  }

  public getDriverAddress(
    params: Parameters<DriverService['apiDriverInfoGet$Json']>[0]
  ) {
    return this.driverService.apiDriverInfoGet$Json(params);
  }
 
 // Call backs
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
