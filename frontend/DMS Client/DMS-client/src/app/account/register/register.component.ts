import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ConfirmPassword } from '../../shared/validators/password.validator';
import { EmailValidator } from '../../shared/validators/validateEmailNotTaken.validate';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  errors: string[];
  registerForm: FormGroup;
  constructor(
    private accountService: AccountService,
    private router: Router,
    private fb: FormBuilder,
    private emailValidator: EmailValidator
  ) {}
  ngOnInit(): void {
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        displayName: ['', [Validators.required, Validators.minLength(3)]],
        email: [
          '',
          [
            Validators.required,
            Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$'),
          ],
          [this.emailValidator.ValidateEmailNotTaken()],
        ],
        workspaceName: ['', [Validators.required, Validators.minLength(3)]],
        password: ['', Validators.required],
        confirmPassword: ['', [Validators.required]],
      },
      { validators: [ConfirmPassword] }
    );
  }
  get _displayName() {
    return this.registerForm.get('displayName');
  }
  get _workspaceName() {
    return this.registerForm.get('workspaceName');
  }
  get _email() {
    return this.registerForm.get('email');
  }
  get _password() {
    return this.registerForm.get('password');
  }
  get _confirmPassword() {
    return this.registerForm.get('confirmPassword');
  }
  onSubmit() {
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.errors = err.errors;
      },
    });
  }
}
