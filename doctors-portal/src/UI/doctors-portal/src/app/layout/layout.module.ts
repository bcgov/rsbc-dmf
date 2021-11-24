import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '../shared/material.module';

@NgModule({
  declarations: [
    FooterComponent,
    HeaderComponent,
    NavMenuComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    MaterialModule
  ],
  exports: [
    FooterComponent,
    HeaderComponent,
    NavMenuComponent,
    MaterialModule
  ],
  providers: [],
})
export class LayoutModule { }
