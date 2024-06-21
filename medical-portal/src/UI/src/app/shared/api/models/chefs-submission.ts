/* tslint:disable */
/* eslint-disable */
import { SubmissionStatus } from '../models/submission-status';
export interface ChefsSubmission {
  status: SubmissionStatus;
  submission?: {
    [key: string]: any | null;
  };
}
