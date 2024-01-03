import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LetterTopicComponent } from './letter-topic.component';

describe('LetterTopicComponent', () => {
  let component: LetterTopicComponent;
  let fixture: ComponentFixture<LetterTopicComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LetterTopicComponent]
    });
    fixture = TestBed.createComponent(LetterTopicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
