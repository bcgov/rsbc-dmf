import { ViewportScroller } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-quick-links',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './quick-links.component.html',
  styleUrl: './quick-links.component.scss',
})
export class QuickLinksComponent {
  constructor(private viewportScroller: ViewportScroller) {}

  public onClick(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }
}
