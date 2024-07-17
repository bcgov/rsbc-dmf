/* tslint:disable */
/* eslint-disable */
import { Driver } from '../models/driver';
export interface Comment {
  caseId?: string | null;
  commentDate?: string | null;
  commentId?: string | null;
  commentText: string;
  commentTypeCode: string;
  driver: Driver;
  sequenceNumber?: number | null;
  userId: string;
}
