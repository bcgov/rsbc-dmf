import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMedicalPractitionerUserProfileDialogComponent } from './edit-medical-practitioner-user-profile-dialog.component';

describe('EditMedicalPractitionerUserProfileDialogComponent', () => {
  let component: EditMedicalPractitionerUserProfileDialogComponent;
  let fixture: ComponentFixture<EditMedicalPractitionerUserProfileDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditMedicalPractitionerUserProfileDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditMedicalPractitionerUserProfileDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
