import { ViewportScroller } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-shared-quick-links',
    templateUrl: './quick-links.component.html',
    styleUrls: ['./quick-links.component.css'],
    standalone: true,
    imports: [RouterLink],
})
export class SharedQuickLinksComponent {
  constructor(private viewportScroller: ViewportScroller) {}

  public onClick(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }
}
