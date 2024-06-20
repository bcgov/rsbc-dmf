import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-denied',
  standalone: true,
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './denied.component.html',
  styleUrl: './denied.component.scss'
})
export class DeniedComponent {
  manageRole() {
    window.location.href = 'https://test.healthprovideridentityportal.gov.bc.ca/';
  }
}
