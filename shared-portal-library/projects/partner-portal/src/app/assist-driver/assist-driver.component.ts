import { Component } from '@angular/core';
import { GetAssistanceComponent } from '../get-assistance/get-assistance.component';

@Component({
  selector: 'app-assist-driver',
  standalone: true,
  imports: [GetAssistanceComponent],
  templateUrl: './assist-driver.component.html',
  styleUrl: './assist-driver.component.scss',
})
export class AssistDriverComponent {}
