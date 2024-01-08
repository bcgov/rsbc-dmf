import { ViewportScroller } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-quick-links',
  templateUrl: './quick-links.component.html',
  styleUrls: ['./quick-links.component.css'],
})
export class QuickLinksComponent {
  constructor(private viewportScroller: ViewportScroller) {}

  public onClick(elementId: string): void {
    this.viewportScroller.scrollToAnchor(elementId);
  }
}
