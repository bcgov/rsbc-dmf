import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageMedicalStaffAssociationDialogComponent } from './manage-medical-staff-association-dialog.component';

describe('ManageMedicalStaffAssociationDialogComponent', () => {
  let component: ManageMedicalStaffAssociationDialogComponent;
  let fixture: ComponentFixture<ManageMedicalStaffAssociationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageMedicalStaffAssociationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageMedicalStaffAssociationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
