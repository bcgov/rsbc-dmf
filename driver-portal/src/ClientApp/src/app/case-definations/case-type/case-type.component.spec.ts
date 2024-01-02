import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaseTypeComponent } from './case-type.component';

describe('CaseTypeComponent', () => {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  let component: CaseTypeComponent;
  let fixture: ComponentFixture<CaseTypeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CaseTypeComponent],
    });
    fixture = TestBed.createComponent(CaseTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    //   expect(component).toBeTruthy();
  });
});
