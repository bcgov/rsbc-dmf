import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetHelpComponent } from './get-help.component';

describe('GetHelpComponent', () => {
  let component: GetHelpComponent;
  let fixture: ComponentFixture<GetHelpComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetHelpComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GetHelpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
