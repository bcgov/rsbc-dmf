import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  standalone: true,
  imports: [MatToolbarModule, RouterLink],
  templateUrl: './medical-nav-menu.component.html',
  styleUrl: './medical-nav-menu.component.scss',
})
export class MedicalNavMenuComponent {}
