import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-user-registration',
  templateUrl: './user-registration.component.html',
  styleUrls: ['./user-registration.component.scss'],
})
export class UserRegistrationComponent {
  acceptControl = new FormControl(false);
 
  
  
}
