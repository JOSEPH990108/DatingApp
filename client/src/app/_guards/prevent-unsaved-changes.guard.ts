import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_-services/confirm.service';
import { inject } from '@angular/core';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {

  const confirmService = inject(ConfirmService);

  if(component.editProfileForm?.dirty){
    return confirmService.confirm();
  }

  return true;
};
