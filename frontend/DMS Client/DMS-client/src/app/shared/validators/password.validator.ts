import { AbstractControl } from '@angular/forms';

export function ConfirmPassword(
  controls: AbstractControl
): { [key: string]: boolean } | null {
  var password = controls.get('password');
  var confirmPassword = controls.get('confirmPassword');
  if(password.pristine || confirmPassword.pristine)
    return null
  return password && confirmPassword && password.value !== confirmPassword.value
    ? { misMatch: true }
    : null;
}
