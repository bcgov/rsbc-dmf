import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RsbcCaseAssignmentComponent } from './rsbc-case-assignment.component';

describe('RsbcCaseAssignmentComponent', () => {
  let component: RsbcCaseAssignmentComponent;
  let fixture: ComponentFixture<RsbcCaseAssignmentComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RsbcCaseAssignmentComponent]
    });
    fixture = TestBed.createComponent(RsbcCaseAssignmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
