/* tslint:disable */
/* eslint-disable */
import { Driver } from '../models/driver';
export interface Document {
  bcMailSent?: boolean | null;
  businessArea?: string | null;
  caseId?: string | null;
  createDate?: string;
  description?: string | null;
  documentId?: string | null;
  documentType?: string | null;
  documentTypeCode?: string | null;
  documentUrl?: string | null;
  driver?: Driver;
  dueDate?: string | null;
  faxReceivedDate?: string | null;
  fileContents?: string | null;
  importDate?: string | null;
  sequenceNumber?: number | null;
  submittalStatus?: string | null;
  userId?: string | null;
}
