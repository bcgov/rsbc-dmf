import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './medical-footer.component.html',
  styleUrl: './medical-footer.component.scss',
})
export class MedicalFooterComponent {}
