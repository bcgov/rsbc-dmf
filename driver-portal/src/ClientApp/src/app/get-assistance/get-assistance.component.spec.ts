import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetAssistanceComponent } from './get-assistance.component';

describe('GetAssistanceComponent', () => {
  let component: GetAssistanceComponent;
  let fixture: ComponentFixture<GetAssistanceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [GetAssistanceComponent]
    });
    fixture = TestBed.createComponent(GetAssistanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
