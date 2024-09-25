import { CUSTOM_ELEMENTS_SCHEMA, Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
//import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.scss'],
    imports : [
      //RouterLink, RouterLinkActive,
      MatToolbarModule],
    standalone: true,
    schemas : [CUSTOM_ELEMENTS_SCHEMA]
})
export class NavMenuComponent {}
