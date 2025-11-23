import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../shared/shared.module';
import { AdminDirectoryComponent } from './admin-directory/admin-directory.component';
import { WorkspaceContentRoutingModule } from './workspace-content-routing.module';



@NgModule({
  declarations: [
    AdminDirectoryComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    WorkspaceContentRoutingModule
  ]
})
export class WorkspaceContentModule { }
