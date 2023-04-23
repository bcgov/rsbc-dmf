import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMedicalStaffAssociationDialogComponent } from './edit-medical-staff-association-dialog.component';

describe('EditMedicalStaffAssociationDialogComponent', () => {
  let component: EditMedicalStaffAssociationDialogComponent;
  let fixture: ComponentFixture<EditMedicalStaffAssociationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditMedicalStaffAssociationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditMedicalStaffAssociationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
