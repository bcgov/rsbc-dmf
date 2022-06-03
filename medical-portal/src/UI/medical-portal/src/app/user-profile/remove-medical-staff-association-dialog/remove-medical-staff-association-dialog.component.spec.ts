import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RemoveMedicalStaffAssociationDialogComponent } from './remove-medical-staff-association-dialog.component';

describe('RemoveMedicalStaffAssociationDialogComponent', () => {
  let component: RemoveMedicalStaffAssociationDialogComponent;
  let fixture: ComponentFixture<RemoveMedicalStaffAssociationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RemoveMedicalStaffAssociationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RemoveMedicalStaffAssociationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
