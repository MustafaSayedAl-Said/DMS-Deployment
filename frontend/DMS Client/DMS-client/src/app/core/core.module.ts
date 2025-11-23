import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { RouterModule } from '@angular/router';
import { TestErrorComponent } from './test-error/test-error.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { ServerErrorComponent } from './server-error/server-error.component';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SectionHeaderComponent } from './section-header/section-header.component';
import { BreadcrumbComponent } from 'xng-breadcrumb';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';
import { MatDialogModule } from '@angular/material/dialog';

@NgModule({
  declarations: [
    NavBarComponent,
    TestErrorComponent,
    NotFoundComponent,
    ServerErrorComponent,
    SectionHeaderComponent,
    ConfirmationDialogComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      countDuplicates: true,
      preventDuplicates: true,
    }),
    BrowserAnimationsModule,
    BreadcrumbComponent,
    BsDropdownModule.forRoot(),
    MatDialogModule
  ],
  exports: [NavBarComponent, SectionHeaderComponent],
})
export class CoreModule {}
