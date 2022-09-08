import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DmerSubmissionConfirmationComponent } from './dmer-submission-confirmation.component';

describe('DmerSubmissionConfirmationComponent', () => {
  let component: DmerSubmissionConfirmationComponent;
  let fixture: ComponentFixture<DmerSubmissionConfirmationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DmerSubmissionConfirmationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DmerSubmissionConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
