import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DmerSubmissionConfirmationDialogComponent } from './dmer-submission-confirmation-dialog.component';

describe('DmerSubmissionConfirmationDialogComponent', () => {
  let component: DmerSubmissionConfirmationDialogComponent;
  let fixture: ComponentFixture<DmerSubmissionConfirmationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DmerSubmissionConfirmationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DmerSubmissionConfirmationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
