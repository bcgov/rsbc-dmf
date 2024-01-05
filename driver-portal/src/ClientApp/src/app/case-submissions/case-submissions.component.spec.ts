import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaseSubmissionsComponent } from './case-submissions.component';

describe('CaseSubmissionsComponent', () => {
  let component: CaseSubmissionsComponent;
  let fixture: ComponentFixture<CaseSubmissionsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CaseSubmissionsComponent]
    });
    fixture = TestBed.createComponent(CaseSubmissionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
