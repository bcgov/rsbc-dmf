/* tslint:disable */
/* eslint-disable */
import { CallbackCallStatus } from '../models/callback-call-status';
import { PreferredTime } from '../models/preferred-time';
export interface Callback2 {
  callStatus?: CallbackCallStatus;
  closed?: string;
  id?: string;
  phone?: string | null;
  preferredTime?: PreferredTime;
  requestCallback?: string;
  topic?: string | null;
}
