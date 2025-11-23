import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WorkspaceContentComponent } from './workspace-content/workspace-content.component';
import { AdminRoutingModule } from './admin-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { DmsModule } from '../dms.module';
import { ActionLogsComponent } from './action-logs/action-logs.component';

@NgModule({
  declarations: [WorkspaceContentComponent, ActionLogsComponent],
  imports: [CommonModule, AdminRoutingModule, SharedModule, DmsModule],
})
export class AdminModule {}
