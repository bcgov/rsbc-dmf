import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EndorsementComponentComponent } from './endorsement-component.component';

describe('EndorsementComponentComponent', () => {
  let component: EndorsementComponentComponent;
  let fixture: ComponentFixture<EndorsementComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EndorsementComponentComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EndorsementComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
