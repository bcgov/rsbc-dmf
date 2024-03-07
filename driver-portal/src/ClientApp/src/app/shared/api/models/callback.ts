/* tslint:disable */
/* eslint-disable */
import { CallbackCallStatus } from '../models/callback-call-status';
import { CallbackTopic } from '../models/callback-topic';
export interface Callback {
  callStatus?: CallbackCallStatus;
  closed?: string;
  id?: string;
  requestCallback?: string;
  topic?: CallbackTopic;
}
