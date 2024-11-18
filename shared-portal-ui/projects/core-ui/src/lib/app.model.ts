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


export enum PortalsEnum{
  'DriverPortal' = 'DriverPortal',
  'MedicalPortal' = 'MedicalPortal',
  'PartnerPortal' = 'PartnerPortal'
}

export const CallStatusDescription = new Map<number | undefined, string>([
  [0, 'Open'],
  [1, 'Closed']
])

export enum PortalUrlEnum {
  'DriverPortal' = '/driver-portal',
  'MedicalPortal' = '/medical-portal',
  'PartnerPortal' = '/partner-portal'
}