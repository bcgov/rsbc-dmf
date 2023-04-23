import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageMedicalPractitionerAssociationDialogComponent } from './manage-medical-practitioner-association-dialog.component';

describe('ManageMedicalPractitionerAssociationDialogComponent', () => {
  let component: ManageMedicalPractitionerAssociationDialogComponent;
  let fixture: ComponentFixture<ManageMedicalPractitionerAssociationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageMedicalPractitionerAssociationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageMedicalPractitionerAssociationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
