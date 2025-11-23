import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DmsComponent } from './dms.component';
import { DirectoryContentsComponent } from './directory-contents/directory-contents.component';
import { ActivatedRouteSnapshot, RouterModule, Routes } from '@angular/router';


const routes: Routes = [
  { path: '', component: DmsComponent },
  {
    path: ':id/:name',
    component: DirectoryContentsComponent,
  },
];

@NgModule({
  declarations: [],
  imports: [CommonModule, RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DmsRoutingModule {}
