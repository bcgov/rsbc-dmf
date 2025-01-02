/* tslint:disable */
/* eslint-disable */
import { CallStatus } from '../models/call-status';
import { PreferredTime } from '../models/preferred-time';
export interface Callback {
  callStatus?: CallStatus;
  closed?: string;
  id?: string;
  preferredTime?: PreferredTime;
  requestCallback?: string;
  subject?: string | null;
  description?: string | null;
}
