/* tslint:disable */
/* eslint-disable */
import { CallbackCallStatus } from '../models/callback-call-status';
export interface Callback2 {
  callStatus?: CallbackCallStatus;
  closed?: string;
  id?: string;
  requestCallback?: string;
  topic?: string | null;
}
