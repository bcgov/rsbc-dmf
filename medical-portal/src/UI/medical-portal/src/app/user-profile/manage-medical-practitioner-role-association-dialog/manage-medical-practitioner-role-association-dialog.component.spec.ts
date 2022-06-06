import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageMedicalPractitionerRoleAssociationDialogComponent } from './manage-medical-practitioner-role-association-dialog.component';

describe('ManageMedicalPractitionerRoleAssociationDialogComponent', () => {
  let component: ManageMedicalPractitionerRoleAssociationDialogComponent;
  let fixture: ComponentFixture<ManageMedicalPractitionerRoleAssociationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageMedicalPractitionerRoleAssociationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageMedicalPractitionerRoleAssociationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
