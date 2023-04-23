import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditUserProfileDialogComponent } from './edit-user-profile-dialog.component';

describe('EditUserProfileDialogComponent', () => {
  let component: EditUserProfileDialogComponent;
  let fixture: ComponentFixture<EditUserProfileDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditUserProfileDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditUserProfileDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
