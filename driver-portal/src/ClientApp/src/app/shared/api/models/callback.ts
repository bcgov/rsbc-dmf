/* tslint:disable */
/* eslint-disable */
import { CallbackCallStatus } from '../models/callback-call-status';
import { CallbackPriority } from '../models/callback-priority';
import { PreferredTime } from '../models/preferred-time';
import { Timestamp } from '../models/timestamp';
export interface Callback {
  assignee?: string | null;
  callStatus?: CallbackCallStatus;
  caseId?: string | null;
  closedDate?: Timestamp;
  description?: string | null;
  id?: string | null;
  origin?: number;
  phone?: string | null;
  preferredTime?: PreferredTime;
  priority?: CallbackPriority;
  requestCallback?: Timestamp;
  subject?: string | null;
}
