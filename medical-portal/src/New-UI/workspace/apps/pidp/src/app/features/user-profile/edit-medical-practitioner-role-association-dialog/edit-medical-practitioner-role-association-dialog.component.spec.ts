import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMedicalPractitionerRoleAssociationDialogComponent } from './edit-medical-practitioner-role-association-dialog.component';

describe('EditMedicalPractitionerRoleAssociationDialogComponent', () => {
  let component: EditMedicalPractitionerRoleAssociationDialogComponent;
  let fixture: ComponentFixture<EditMedicalPractitionerRoleAssociationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditMedicalPractitionerRoleAssociationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditMedicalPractitionerRoleAssociationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
