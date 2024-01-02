import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaseStatusComponent } from './case-status.component';

describe('CaseStatusComponent', () => {
  let component: CaseStatusComponent;
  let fixture: ComponentFixture<CaseStatusComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CaseStatusComponent]
    });
    fixture = TestBed.createComponent(CaseStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
