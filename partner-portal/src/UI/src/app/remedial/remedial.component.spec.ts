import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RemedialComponent } from './remedial.component';

describe('RemedialComponent', () => {
  let component: RemedialComponent;
  let fixture: ComponentFixture<RemedialComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RemedialComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(RemedialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
