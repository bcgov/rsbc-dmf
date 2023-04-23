import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMedicalPractitionerRoleAssociationDialogComponent } from './create-medical-practitioner-role-association-dialog.component';

describe('CreatetMedicalPractitionerRoleAssociationDialogComponent', () => {
  let component: CreateMedicalPractitionerRoleAssociationDialogComponent;
  let fixture: ComponentFixture<CreateMedicalPractitionerRoleAssociationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateMedicalPractitionerRoleAssociationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateMedicalPractitionerRoleAssociationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
