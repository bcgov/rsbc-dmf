import { Component } from '@angular/core';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-claim-dmer-popup',
  standalone: true,
  imports: [
    MatIconModule,
    MatDialogModule,
    MatRadioModule,
    MatButtonModule,
    MatSelectModule,
  ],
  templateUrl: './claim-dmer-popup.component.html',
  styleUrl: './claim-dmer-popup.component.scss',
})
export class ClaimDmerPopupComponent {
  onClaimDmer() {}
  onUncliamDmer() {}
}
