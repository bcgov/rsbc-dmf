import { Component } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-user-registration',
    templateUrl: './user-registration.component.html',
    styleUrls: ['./user-registration.component.scss'],
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink],
})
export class UserRegistrationComponent {
  acceptControl = new FormControl(false);
 
  
  
}
