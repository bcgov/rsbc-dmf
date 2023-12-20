/* tslint:disable */
/* eslint-disable */
import { Driver } from '../models/driver';
export interface Document {
  bcMailSent?: boolean | null;
  businessArea?: string | null;
  caseId?: string | null;
  documentId?: string | null;
  documentType?: string | null;
  documentTypeCode?: string | null;
  driver?: Driver;
  faxReceivedDate?: string | null;
  fileContents?: string | null;
  importDate?: string | null;
  sequenceNumber?: number | null;
  userId?: string | null;
}
