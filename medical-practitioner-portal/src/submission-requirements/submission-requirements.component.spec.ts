import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmissionRequirementsComponent } from './submission-requirements.component';

describe('SubmissionRequirementsComponent', () => {
  let component: SubmissionRequirementsComponent;
  let fixture: ComponentFixture<SubmissionRequirementsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubmissionRequirementsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SubmissionRequirementsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
