import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RouterModule, Router } from '@angular/router';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
  declarations: [HeaderComponent, FooterComponent, NavMenuComponent],
  imports: [
    CommonModule,
    RouterModule,
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
  ],
  exports: [HeaderComponent, FooterComponent, NavMenuComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class LayoutModule {}
