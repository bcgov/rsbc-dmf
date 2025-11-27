import { Injectable } from '@angular/core';
import {
  CallbackService,
  CasesService,
  CommentsService,
  DocumentService,
  DocumentTypeService,
  DriverService,
  RemedialService,
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
    private commentsService: CommentsService,
    private remedialService: RemedialService
  ) { }

  // Case
  public getMostRecentCase(params: Parameters<CasesService['apiCasesMostRecentGet$Json']>[0]) {
    return this.casesService.apiCasesMostRecentGet$Json(params);
  }

  public getClosedCases(
    params: Parameters<CasesService['apiCasesClosedGet$Json']>[0]
  ) {
    return this.casesService.apiCasesClosedGet$Json(params);
  }

  // Documents
  public getAllDriverDocuments() {
    return this.driverService.apiDriverAllDocumentsGet$Json()
  }

  public getDownloadDocument(
    params: Parameters<DocumentService['apiDocumentDocumentIdGet$Json']>[0]
  ) {
    return this.documentService.apiDocumentDocumentIdGet$Json(params);
  }

  public getDocumentSubTypes() {
    return this.documentTypeService.apiDocumentTypeDocumentSubTypeGet$Json();
  }


  // Call Back
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

  // Search 
  public searchByDriver(
    params: Parameters<DriverService['apiDriverInfoDriverLicenceNumberSurCodeGet$Json']>[0]
  ) {
    return this.driverService.apiDriverInfoDriverLicenceNumberSurCodeGet$Json(params);
  }


  public searchByCaseId(
    params: Parameters<CasesService['apiCasesSearchIdCodeGet$Json']>[0]
  ) {
    return this.casesService.apiCasesSearchIdCodeGet$Json(params);
  }

  // Comments

  public getComments(
    params: Parameters<CommentsService['apiCommentsGetCommentsGet$Json']>[0]
  ) {
    return this.commentsService.apiCommentsGetCommentsGet$Json(params);
  }

  public addComments(
    params: Parameters<CommentsService['apiCommentsCreatePost$Json']>[0]
  ) {
    return this.commentsService.apiCommentsCreatePost$Json(params);
  }

  // Remedial Cased

  public getIgnitionInterlockDetails(
    params: Parameters<RemedialService['apiRemedialGetIgnitionInterlockGet$Json']>[0]
  ) {
    return this.remedialService.apiRemedialGetIgnitionInterlockGet$Json(params);
  }

  public getRehabTriggerDetails(
    params: Parameters<RemedialService['apiRemedialGetRehabTriggersGet$Json']>[0]
  ) {
    return this.remedialService.apiRemedialGetRehabTriggersGet$Json(params);
  }
  
}
