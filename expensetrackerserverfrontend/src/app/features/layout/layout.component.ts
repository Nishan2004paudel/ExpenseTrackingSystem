import { Component, inject, signal, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './layout.component.html'
})
export class LayoutComponent {
  auth = inject(AuthService);
  private router = inject(Router);
  notifications = inject(NotificationService);
  private elementRef = inject(ElementRef);

  sidebarCollapsed = signal(false);

  toggleSidebar() {
    this.sidebarCollapsed.update(v => !v);
  }

  logout() {
    this.notifications.stopConnection();
    this.auth.logout().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login'])
    });
  }
  showNotificationPanel = signal(false);
  showPreviousNotifications = signal(false);

  toggleNotificationPanel() {
    this.showNotificationPanel.update(v => !v);
  }

  markAsRead(notificationId: number) {
    this.notifications.markOneAsReadLocal(notificationId);
  }

  markAllAsRead() {
    this.notifications.markAllAsReadLocal();
    this.showNotificationPanel.set(false);
  }
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.showNotificationPanel()) return;

    const target = event.target as HTMLElement;
    const bellSection = this.elementRef.nativeElement.querySelector('[data-notification-section]');

    if (bellSection && !bellSection.contains(target)) {
      this.showNotificationPanel.set(false);
    }
  }
}