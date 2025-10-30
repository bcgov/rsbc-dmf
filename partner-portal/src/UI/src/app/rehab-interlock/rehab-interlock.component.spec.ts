import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RehabInterlockComponent } from './rehab-interlock.component';

describe('RehabInterlockComponent', () => {
  let component: RehabInterlockComponent;
  let fixture: ComponentFixture<RehabInterlockComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RehabInterlockComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RehabInterlockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
