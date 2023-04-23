// import { Component, Input } from '@angular/core';
// import { FormControl, FormGroup } from '@angular/forms';
// import { IdentityProvider } from '@app/features/auth/enums/identity-provider.enum';
// import { EMPTY, Observable, catchError, of, tap } from 'rxjs';
// import { AuthorizedUserService } from '@app/features/auth/services/authorized-user.service';

// @Component({
//   selector: 'ui-contact-info-form',
//   templateUrl: './contact-info-form.component.html',
//   styleUrls: ['./contact-info-form.component.scss'],
// })
// export class ContactFormComponent {

//   /**
//    * @description
//    * Contact information form instance.
//    */
//   @Input() public form!: FormGroup;
//   public constructor(
//     private authorizedUserService: AuthorizedUserService
//   ) {
//     this.identityProvider$ = this.authorizedUserService.identityProvider$;
//   }

//   public identityProvider$: Observable<IdentityProvider>;
//   public IdentityProvider = IdentityProvider;

//   public get phone(): FormControl {
//     return this.form.get('phone') as FormControl;
//   }

//   public get email(): FormControl {
//     return this.form.get('email') as FormControl;
//   }
// }
