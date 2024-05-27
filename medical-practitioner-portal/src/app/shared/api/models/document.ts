/* tslint:disable */
/* eslint-disable */
import { Case } from '../models/case';
import { DocumentType } from '../models/document-type';
import { Timestamp } from '../models/timestamp';
export interface Document {
  case?: Case;
  complianceDate?: Timestamp;
  createdOn?: Timestamp;
  description?: string | null;
  dmerStatus?: number;
  dmerType?: number;
  documentType?: DocumentType;
  documentUrl?: string | null;
  submittalStatus?: string | null;
}
