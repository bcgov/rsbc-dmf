import { Component } from '@angular/core';
import { PidpService } from '@app/shared/api/services';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent
{
  public constructor(private pidpService: PidpService)
  {
    this.pidpService.apiPidpEndorsementsGet().subscribe((data) =>
    {
      console.log("endorsement response", data);
    });
  }
}
