import { TranslatDmerStatus } from '../app/app.model';
import { CaseDocument } from '../app/shared/api/models';

export const PractitionerDMERList_SEED_DATA: CaseDocument[] = [
  {
    birthday: 'May 13, 1952',
    caseNumber: 'DMR1234',
    complianceDate: 'May 2 2013',
    dmerStatus: 100000001, // Received
    dmerType: 2,
    fullName: 'David Minto',
  },
  {
    birthday: 'April 20, 1999',
    caseNumber: 'DMR3578',
    complianceDate: 'May 2 2013',
    dmerStatus: 100000005,
    dmerType: 2,
    fullName: 'Sheppard, Sonia Tamara',
  },
  {
    birthday: 'April 23, 1988',
    caseNumber: 'DMR8299',
    complianceDate: 'May 2 2013',
    dmerStatus: 100000004,
    dmerType: 2,
    fullName: 'Irving, Misha Anja',
  },
  {
    birthday: 'June 1, 1999',
    caseNumber: 'DMR8909',
    complianceDate: 'May 2 2013',
    dmerStatus: 100000008,
    dmerType: 2,
    fullName: 'Rahul Minto',
  },
];
