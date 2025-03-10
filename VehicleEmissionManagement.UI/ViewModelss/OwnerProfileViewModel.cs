using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;
using VehicleEmissionManagement.UI.Commands;

namespace VehicleEmissionManagement.UI.ViewModelss
{
    public partial class OwnerProfileViewModel : INotifyPropertyChanged
    {
        private readonly IOwnerService _ownerService;
        private User _currentUser;

        public event PropertyChangedEventHandler PropertyChanged;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public ICommand UpdateProfileCommand { get; }

        public OwnerProfileViewModel(IOwnerService ownerService)
        {
            _ownerService = ownerService;
            UpdateProfileCommand = new RelayCommand(async () => await UpdateProfile(), CanUpdateProfile);
            LoadUserProfile();
        }

        private async void LoadUserProfile()
        {
            // Lấy thông tin từ CurrentUser của AuthService
            CurrentUser = AuthService.CurrentUser;
            if (CurrentUser != null)
            {
                FullName = CurrentUser.FullName;
                Email = CurrentUser.Email;
                Phone = CurrentUser.Phone;
                Address = CurrentUser.Address;
            }
        }

        private async Task UpdateProfile()
        {
            if (CurrentUser != null)
            {
                CurrentUser.FullName = FullName;
                CurrentUser.Email = Email;
                CurrentUser.Phone = Phone;
                CurrentUser.Address = Address;
                CurrentUser.UpdatedAt = DateTime.Now;

                var result = await _ownerService.UpdateProfileAsync(CurrentUser);
                if (result)
                {
                    AuthService.CurrentUser = CurrentUser; // Cập nhật lại CurrentUser
                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update profile.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanUpdateProfile()
        {
            return !string.IsNullOrEmpty(FullName) &&
                   !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(Phone) &&
                   !string.IsNullOrEmpty(Address);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}