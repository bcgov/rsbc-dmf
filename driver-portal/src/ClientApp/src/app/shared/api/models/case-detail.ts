/* tslint:disable */
/* eslint-disable */
import { Comment } from '../models/comment';
import { Document } from '../models/document';
export interface CaseDetail {
  assigneeTitle?: string | null;
  caseId?: string | null;
  caseSequence?: number | null;
  caseType?: string | null;
  comments?: Array<Comment> | null;
  decisionDate?: string | null;
  decisionForClass?: string | null;
  dmerType?: string | null;
  documents?: Array<Document> | null;
  dpsProcessingDate?: string;
  idCode?: string | null;
  lastActivityDate?: string;
  latestDecision?: string | null;
  openedDate?: string;
  status?: string | null;
  title?: string | null;
}
