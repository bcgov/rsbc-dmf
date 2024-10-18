import { ViewportScroller } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import{ PortalsEnum } from '../app.model';

@Component({
    selector: 'app-shared-quick-links',
    templateUrl: './quick-links.component.html',
    styleUrls: ['./quick-links.component.scss'],
    standalone: true,
    imports: [RouterLink],
})
export class SharedQuickLinksComponent {

  @Input() portal!: PortalsEnum;

  PortalsEnum = PortalsEnum;
  
  constructor(private viewportScroller: ViewportScroller) {}

  public onClick(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }
}
