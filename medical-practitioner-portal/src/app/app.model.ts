// export enum DmerStatusEnum {
//   'Adjudicate' = 100000000,
//   'Reject' = 100000001,
//   'Clean Pass' = 100000002,
//   'Manual Pass' = 100000003,
// }

export enum DmerTypeEnum {
  '1 - NSC' = 100000000,
  '2 - Age' = 100000001,
  '3 - Industrial Road' = 100000002,
  '4 - Known Condition' = 100000003,
  '5 - Possible Condition' = 100000004,
}

export const TranslatDmerStatus: Record<string, string> = {
  10000001: 'Uploaded',
  100000008: 'Sent',
  100000007: 'Actioned Non-comply',
  100000001: 'Received',
  100000005: 'Non-Comply',
  100000004: 'Rejected',
  100000003 : 'Reviewed'
  // 'Clean Pass' = 100000009,
  // 'Issued' = 100000011,
  // 'Manual Pass' = 100000012,
  // 'Non-comply' = 100000005,
  // 'Open-Required' = 100000000,
  // 'Received' = 100000001,
  // 'Rejected' = 100000004,
  // 'Reviewed' = 100000003,
};
