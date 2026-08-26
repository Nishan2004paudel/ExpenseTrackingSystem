import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';
import { Notification } from '../models/notification.model';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private hubConnection?: signalR.HubConnection;

  // --- REST calls ---

  getAll() {
    return this.http.get<Notification[]>(`${environment.apiUrl}/notification`);
  }

  getUnread() {
    return this.http.get<Notification[]>(`${environment.apiUrl}/notification/unread`);
  }

  markAsRead(notificationId: number) {
    return this.http.patch<{ message: string }>(`${environment.apiUrl}/notification/${notificationId}/read`, {});
  }

  markAllAsRead() {
    return this.http.patch<{ message: string }>(`${environment.apiUrl}/notification/read-all`, {});
  }

  // --- SignalR connection ---

  startConnection() {
    if (this.hubConnection) {
      return; // already connected/connecting
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl, {
        accessTokenFactory: () => this.auth.getToken() ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveNotification', (notification: Notification) => {
      console.log('Received notification:', notification);
    });

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.error('SignalR connection error:', err));
  }

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = undefined;
    }
  }
}