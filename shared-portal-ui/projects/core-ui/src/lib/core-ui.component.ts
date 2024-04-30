import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'lib-core-ui',
    template: `
    <p>
      core-ui works!
    </p>
  `,
    styles: [],
    standalone: true
})
export class CoreUiComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
