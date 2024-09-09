export enum CaseStageEnum {
  'Opened' = 'Opened',
  'OpenPendingSubmission' = 'Open Pending Submission',
  'UnderReview' = 'Under Review',
  'FileEndTasks' = 'File End Tasks',
  'IntakeValidation' = 'Intake Validation',
  'Closed' = 'Closed',
}

export enum SubmittalStatusEnum {
  'OpenRequired' = 'Open-Required',
  'Issued' = 'Issued',
  'Sent' = 'Sent',
}

export enum SESSION_STORAGE_KEYS {
  DRIVER = 'driver'
}

export const CallStatusDescription = new Map<number | undefined, string>([
  [0, 'Open'],
  [1, 'Closed']
])
 

export enum CommentOrigin{
  'System' = 'System',
  'User' = 'User'
}