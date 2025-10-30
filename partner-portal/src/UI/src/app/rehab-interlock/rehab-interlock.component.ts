import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-rehab-interlock',
  standalone: true,
  imports: [MatCardModule, MatIconModule, MatExpansionModule,],
  templateUrl: './rehab-interlock.component.html',
  styleUrl: './rehab-interlock.component.scss'
})
export class RehabInterlockComponent {

  toggleIsExpandable(): void {
    // this.isExpandable = !this.isExpandable;
  }
}
