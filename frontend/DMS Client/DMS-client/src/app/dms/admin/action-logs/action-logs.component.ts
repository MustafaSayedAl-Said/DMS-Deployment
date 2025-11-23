import { Component, OnInit } from '@angular/core';
import { SignalrService } from '../signalr.service';
import { ActionLog } from '../../../shared/Models/ActionLog';
import { AdminService } from '../admin.service';

import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-action-logs',
  templateUrl: './action-logs.component.html',
  styleUrl: './action-logs.component.scss',
})
export class ActionLogsComponent implements OnInit {
  actionLogs: ActionLog[] = [];

  constructor(
    private signalRService: SignalrService,
    private adminService: AdminService,
    private toast: ToastrService
  ) {}
  ngOnInit(): void {
    this.signalRService.startConnection();
    this.signalRService.addNotificationListener((message: string) => {
      try {
        // Parse the message
        const parsedData = JSON.parse(message);

        // Log the parsed data for verification
        console.log('Parsed data:', parsedData);

        // Ensure the correct case-sensitive field names are used
        const actionLog: ActionLog = {
          Id: parsedData?.Id, // Should match 'Id' from parsedData
          ActionType: parsedData?.ActionType, // Should match 'ActionType' from parsedData
          UserId: parsedData?.UserId, // Should match 'UserId' from parsedData
          DocumentId: parsedData?.DocumentId, // Should match 'DocumentId' from parsedData
          UserName: parsedData?.UserName, // Should match 'UserName' from parsedData
          DocumentName: parsedData?.DocumentName, // Should match 'DocumentName' from parsedData
          CreationDate: new Date(parsedData?.CreationDate || Date.now()), // Parse CreationDate correctly
        };

        // Add the action log to the array
        this.actionLogs.push(actionLog);

        // Log the action log for confirmation
        console.log('New Action Log added: ', actionLog);
      } catch (error) {
        console.error('Error processing notification: ', error);
      }
    });
    this.loadLogs();
  }

  loadLogs() {
    this.adminService.getActionLogs().subscribe(
      (res) => {
        console.log('Server Response:', res); // Log the raw response from the server

        // Check if response exists and if it contains the expected data
        if (res && Array.isArray(res)) {
          this.actionLogs = res.map((log) => ({
            Id: log.id, // Ensure that the response field matches 'Id'
            ActionType: log.actionType, // Provide fallback values
            UserId: log.userId,
            DocumentId: log.documentId,
            UserName: log.userName || 'Unknown',
            DocumentName: log.documentName || 'Unknown',
            CreationDate: log.creationDate
              ? new Date(log.creationDate)
              : new Date(), // Handle date parsing
          }));

          console.log('Mapped Action Logs:', this.actionLogs); // Log the mapped action logs
        } else {
          console.error('Unexpected response format:', res);
          this.toast.error('Failed to load action logs, unexpected format.');
        }
      },
      (error) => {
        console.error('HTTP request error:', error);
        this.toast.error('Failed to fetch action logs.');
      }
    );
  }
}
