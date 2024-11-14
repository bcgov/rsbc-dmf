import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { CaseManagementService } from './case-management.service';

describe('CaseManagementService', () => {
  let service: CaseManagementService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ]
    });
    service = TestBed.inject(CaseManagementService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
