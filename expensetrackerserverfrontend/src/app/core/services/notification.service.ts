import { Injectable, inject, signal, computed } from '@angular/core';
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

  notifications = signal<Notification[]>([]);
  unreadCount = computed(() => this.notifications().filter(n => !n.isRead).length);
  unreadNotifications = computed(() => this.notifications().filter(n => !n.isRead));
  readNotifications = computed(() => this.notifications().filter(n => n.isRead));
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

  // --- Load + sync into the reactive list ---

  loadNotifications() {
    this.getAll().subscribe({
      next: (list) => this.notifications.set(list)
    });
  }

  markOneAsReadLocal(notificationId: number) {
    this.markAsRead(notificationId).subscribe({
      next: () => {
        this.notifications.update(list =>
          list.map(n => n.notificationId === notificationId ? { ...n, isRead: true } : n)
        );
      }
    });
  }

  markAllAsReadLocal() {
    this.markAllAsRead().subscribe({
      next: () => {
        this.notifications.update(list => list.map(n => ({ ...n, isRead: true })));
      }
    });
  }

  // --- SignalR connection ---

  startConnection() {
    if (this.hubConnection) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl, {
        accessTokenFactory: () => this.auth.getToken() ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveNotification', (notification: Notification) => {
      this.notifications.update(list => [notification, ...list]);
    });

    this.hubConnection
      .start()
      .then(() => this.loadNotifications())
      .catch(err => console.error('SignalR connection error:', err));
  }

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = undefined;
    }
    this.notifications.set([]);
  }
}