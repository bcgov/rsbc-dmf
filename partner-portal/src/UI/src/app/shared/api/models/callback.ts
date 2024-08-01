/* tslint:disable */
/* eslint-disable */
import { CallStatus } from '../models/call-status';
import { PreferredTime } from '../models/preferred-time';
export interface Callback {
  callStatus?: CallStatus;
  closed?: string;
  id?: string;
  phone?: string | null;
  preferredTime?: PreferredTime;
  requestCallback?: string;
  topic?: string | null;
}
