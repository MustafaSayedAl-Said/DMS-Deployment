import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { ActionLog } from '../../shared/Models/ActionLog';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  private hubConnection: signalR.HubConnection;

  startConnection() {
    var token = localStorage.getItem('token');
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://130.61.58.153:8080/notificationHub', {
        accessTokenFactory: () => {
          return token;
        },
      })
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))

      .catch((err) => console.log('Error while starting connection: ' + err));
  }

  addNotificationListener(callback: (message: string) => void) {
    this.hubConnection.on('ReceiveNotification', (message: string) => {
      try {
        // Parse the message once
        const parsedData = JSON.parse(message);

        // Log the structure to verify the fields
        console.log(parsedData);

        // Make sure fields exist before accessing them
        const actionLog: ActionLog = {
          Id: parsedData?.Id,
          ActionType: parsedData?.ActionType,
          UserId: parsedData?.UserId,
          DocumentId: parsedData?.DocumentId,
          UserName: parsedData?.UserName,
          DocumentName: parsedData?.DocumentName,
          CreationDate: new Date(parsedData?.CreationDate),
        };

        // Check if UserName is an object and has a 'Value' field
        console.log(parsedData.UserName?.Value || parsedData.UserName);

        console.log('New Action: ', actionLog);
        callback(message);
      } catch (error) {
        console.error('Error parsing notification message: ', error);
      }
    });
  }

  public broadcastChartData = () => {
    const data = 'Hello world';

    this.hubConnection
      .invoke('NotifyAdmin', data)
      .catch((err) => console.error(err));
  };

  public addBroadcastChartDataListener = () => {
    this.hubConnection.on('broadcastchartdata', (data) => {
      console.log('Testing');
    });
  };
  // public addBroadcastChartDataListener = () => {
  //   this.hubConnection.on('NotifyAdmin', (data) => {
  //     this.bradcastedData = data;
  //   })
  // }

  constructor() {}
}
