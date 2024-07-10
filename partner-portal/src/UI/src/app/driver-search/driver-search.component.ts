import { Component } from '@angular/core';
import { MatCardContent, MatCardModule } from '@angular/material/card';
import { MatToolbar } from '@angular/material/toolbar';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-driver-search',
  standalone: true,
  imports: [MatCardModule, MatToolbar, RouterLink],
  templateUrl: './driver-search.component.html',
  styleUrl: './driver-search.component.scss',
})
export class DriverSearchComponent {}
