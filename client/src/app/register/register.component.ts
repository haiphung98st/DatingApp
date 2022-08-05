import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormControl, FormGroup, RequiredValidator, ValidatorFn, Validators } from '@angular/forms';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;
  constructor(private accountService: AccountService, private toast: ToastrService) { }

  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(10)]),
      confirmPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(10),
        this.matchValues("password")
      ]),
    })
    this.registerForm.controls.password.valueChanges.subscribe(() =>{
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })
  }
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : { isMatching: true };
    }
  }
  register() {
    console.log(this.registerForm.value);

    // this.accountService.register(this.model).subscribe(
    //   response => {
    //     console.log(response);
    //     this.cancel();
    //   },
    //   error => {
    //     console.log(error);
    //     this.toast.error(error.error);
    //   });
  }
  cancel() {
    this.cancelRegister.emit(false);
  }

}
