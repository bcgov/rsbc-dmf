import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssistDriverComponent } from './assist-driver.component';

describe('AssistDriverComponent', () => {
  let component: AssistDriverComponent;
  let fixture: ComponentFixture<AssistDriverComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssistDriverComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AssistDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
