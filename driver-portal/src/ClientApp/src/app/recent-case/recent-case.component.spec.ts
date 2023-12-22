import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecentCaseComponent } from './recent-case.component';

describe('RecentCaseComponent', () => {
  let component: RecentCaseComponent;
  let fixture: ComponentFixture<RecentCaseComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RecentCaseComponent],
    });
    fixture = TestBed.createComponent(RecentCaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
