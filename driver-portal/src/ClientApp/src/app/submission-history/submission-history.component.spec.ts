import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmissionHistoryComponent } from './submission-history.component';

describe('SubmissionHistoryComponent', () => {
  let component: SubmissionHistoryComponent;
  let fixture: ComponentFixture<SubmissionHistoryComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SubmissionHistoryComponent]
    });
    fixture = TestBed.createComponent(SubmissionHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
