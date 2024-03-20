/* tslint:disable */
/* eslint-disable */
import { BringForwardPriority } from '../models/bring-forward-priority';
export interface BringForwardRequest {
  assignee?: string | null;
  caseId?: string | null;
  description?: string | null;
  priority?: BringForwardPriority;
  subject?: string | null;
}
