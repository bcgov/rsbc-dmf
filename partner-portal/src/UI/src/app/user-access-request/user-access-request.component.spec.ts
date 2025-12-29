import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAccessRequestComponent } from './user-access-request.component';

describe('UserAccessRequestComponent', () => {
  let component: UserAccessRequestComponent;
  let fixture: ComponentFixture<UserAccessRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserAccessRequestComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserAccessRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
