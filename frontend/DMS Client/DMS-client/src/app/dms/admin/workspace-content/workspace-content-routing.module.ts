import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { WorkspaceContentComponent } from './workspace-content.component';
import { AdminDirectoryComponent } from './admin-directory/admin-directory.component';
import { adminAuthGuard } from '../../../core/guards/admin-auth.guard';


const routes:Routes = [
  {path: '', component: WorkspaceContentComponent},
  {
    path: 'dms/:id/:name',
    component: AdminDirectoryComponent,
  }
]

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes),
    CommonModule
  ],
  exports:[
    RouterModule
  ]
})
export class WorkspaceContentRoutingModule { }
