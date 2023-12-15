import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
})
export class AccountComponent implements OnInit {
  isEditView = false;
  setEmailAddress = true;
  
  accountForm = this.fb.group({
    mail: [false]
  })

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.accountForm.valueChanges.subscribe((val) => {
      console.log(val);
    })
  }

  onEdit() {
    this.setEmailAddress = false;
    this.isEditView = true;
  }
}
