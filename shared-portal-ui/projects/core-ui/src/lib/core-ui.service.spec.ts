import { TestBed } from '@angular/core/testing';

import { CoreUiService } from './core-ui.service';

describe('CoreUiService', () => {
  let service: CoreUiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CoreUiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
