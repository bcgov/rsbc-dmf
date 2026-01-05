import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatTableModule } from '@angular/material/table';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';
import { User, UserRole } from '@app/shared/api/models';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-admin-details',
  standalone: true,
  imports: [
    CommonModule, // Required for ngIf, ngFor, etc.
    MatCardModule, // Card UI component
    MatButtonModule, // Button component
    MatCheckboxModule, // Checkbox component
    MatFormFieldModule, // Form field module
    MatInputModule, // Input field module
    MatRadioModule, // Radio button module
    MatTableModule, // Table module
    MatDatepickerModule,
    FormsModule,
    ReactiveFormsModule,
    NgxSpinnerModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './admin-details.component.html',
  styleUrls: ['./admin-details.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminDetailsComponent {
  @Input() user: User = {} as User;
  @Output() backToDashboard = new EventEmitter<void>();
  xsStatusColumns: string[] = ['roleId', 'description', 'action'];
  auditDetailColumns: string[] = ['entryId', 'entryDate', 'description'];
  adminRoles: UserRole[] = [];
  noResults: boolean = false;
  buttonDisabled: boolean = false;

  get expiryDateAsDate(): Date {
    return this.user.expiryDate ? new Date(this.user.expiryDate) : new Date();
  }

  set expiryDateAsDate(value: Date | null) {
    this.user.expiryDate = value ? value.toISOString() : undefined;
  }

  get unassignedAdminRoles(): UserRole[] {
    return this.adminRoles.filter(
      (role) => !this.user.roles?.some((userRole) => userRole.id === role.id),
    );
  }

  constructor(
    private caseManagementService: CaseManagementService,
    private spinner: NgxSpinnerService,
    private _snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit() {
    this.getRoles();
  }

  updateUser() {
    this.caseManagementService.updatePortalUser({ body: this.user }).subscribe({
      next: () => {
        this._snackBar.open('Successfully Update User Info', 'Close', {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 5000,
          panelClass: ['success-snackbar'],
        });
      },
      error: () => {
        this.noResults = true;
        this._snackBar.open('Error Updating User Info', 'Close', {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 5000,
          panelClass: ['error-snackbar'],
        });
      },
    });
  }

  getRoles() {
    this.spinner.show();
    this.caseManagementService.getContactRoles({ body: undefined }).subscribe({
      next: (roles) => {
        this.adminRoles = roles;
        this.cdr.detectChanges();
        this.spinner.hide();

        if (this.user.authorized == false) {
          this.authorizeUser();
        }
      },
    });
  }

  authorizeUser() {
    console.log('authorizing user');
    this.user.effectiveDate = new Date().toISOString();
          this.user.authorized = true;
          this.updateUser();
          this.user.roles = [this.adminRoles.find(r => r.name === 'USER')!];
          this.cdr.detectChanges();
  }

  updateRoles(role: UserRole, addRole: boolean) {
    this.spinner.show();
    this.buttonDisabled = true;
    this.caseManagementService
      .updateContactRoles({
        body: { contactId: this.user.id, roleId: role.id, addRole: addRole },
      })
      .subscribe({
        next: (roles) => {
          if (addRole) {
            this.user.roles = [...(this.user.roles ?? []), role];
          } else {
            this.user.roles = this.user.roles?.filter((r) => r.id !== role.id);
          }
          this.buttonDisabled = false;
          this.cdr.detectChanges();
          this.spinner.hide();
          this._snackBar.open('Successfully Update User Role', 'Close', {
            horizontalPosition: 'center',
            verticalPosition: 'top',
            duration: 5000,
            panelClass: ['success-snackbar'],
          });
        },
        error: (error) => {
          this.noResults = true;
          console.error('error', error);
          this.buttonDisabled = false;
          this.spinner.hide();
        },
      });
  }

  onBackToDashboardButtonClicked() {
    this.backToDashboard.emit();
  }

  formatDate(dateString: string | null | undefined): string {
    return dateString?.split('T')[0] ?? '';
  }
}
