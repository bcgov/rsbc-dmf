import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMedicalStaffAssociationDialogComponent } from './create-medical-staff-association-dialog.component';

describe('CreateMedicalStaffAssociationDialogComponent', () => {
  let component: CreateMedicalStaffAssociationDialogComponent;
  let fixture: ComponentFixture<CreateMedicalStaffAssociationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateMedicalStaffAssociationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateMedicalStaffAssociationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
