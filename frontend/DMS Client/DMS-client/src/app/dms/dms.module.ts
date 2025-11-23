import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DirectoryComponent } from './directory/directory.component';
import { DmsComponent } from './dms.component';
import { SharedModule } from '../shared/shared.module';
import { DirectoryContentsComponent } from './directory-contents/directory-contents.component';
import { DmsRoutingModule } from './dms-routing.module';

import { UsersComponent } from './admin/users/users.component';




@NgModule({
  declarations: [DmsComponent, DirectoryComponent, DirectoryContentsComponent, UsersComponent],
  imports: [
    CommonModule,
    SharedModule,
    DmsRoutingModule,
  ],
  exports: [
    DirectoryComponent,
  ]
})
export class DmsModule { }
