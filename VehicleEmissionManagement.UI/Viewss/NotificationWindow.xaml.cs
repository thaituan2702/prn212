using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class NotificationWindow : Window
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationWindow()
        {
            InitializeComponent();
            _notificationRepository = ((App)Application.Current)._serviceProvider.GetService<INotificationRepository>();
            LoadNotifications();
        }

        private async void LoadNotifications()
        {
            try
            {
                var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(AuthService.CurrentUser.UserID);
                NotificationsListView.ItemsSource = notifications;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading notifications: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private async void MarkAsRead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var notification = (Notification)((Button)sender).DataContext;
                notification.IsRead = true;
                await _notificationRepository.UpdateNotificationAsync(notification);
                LoadNotifications();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking notification as read: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private async void DeleteNotification_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var notification = (Notification)((Button)sender).DataContext;
                var result = MessageBox.Show("Are you sure you want to delete this notification?",
                                          "Confirm Delete",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _notificationRepository.DeleteNotificationAsync(notification.NotificationID);
                    LoadNotifications();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting notification: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private async void MarkAllAsRead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _notificationRepository.MarkAllAsReadAsync(AuthService.CurrentUser.UserID);
                LoadNotifications();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking all notifications as read: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private async void DeleteAllRead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to delete all read notifications?",
                                          "Confirm Delete",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _notificationRepository.DeleteAllReadAsync(AuthService.CurrentUser.UserID);
                    LoadNotifications();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting read notifications: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}