import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LettersToDriverComponent } from './letters-to-driver.component';

describe('LettersToDriverComponent', () => {
  let component: LettersToDriverComponent;
  let fixture: ComponentFixture<LettersToDriverComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LettersToDriverComponent]
    });
    fixture = TestBed.createComponent(LettersToDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
