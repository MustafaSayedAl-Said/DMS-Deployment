import { Injectable } from '@angular/core';
import { AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { map, of, switchMap, timer } from 'rxjs';
import { AccountService } from '../../account/account.service';

@Injectable({ providedIn: 'root' })
export class EmailValidator {
  constructor(private accountService: AccountService) {}

  ValidateEmailNotTaken(): AsyncValidatorFn {
    return (controls) => {
      return timer(1000).pipe(
        switchMap(() => {
          if (!controls.value) {
            return of(null);
          }
          return this.accountService.checkEmailExist(controls.value).pipe(
            map((res) => {
              return res ? { emailExists: true } : null;
            })
          );
        })
      );
    };
  }
}
