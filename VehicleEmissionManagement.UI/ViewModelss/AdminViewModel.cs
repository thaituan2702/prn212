using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.UI.Viewss;
namespace VehicleEmissionManagement.UI.ViewModelss
{
    public class AdminViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private ObservableCollection<User> _users;
        private User _selectedUser;

        public AdminViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            LoadUsers();
            InitializeCommands();
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                EditUserCommand.NotifyCanExecuteChanged();
                DeleteUserCommand.NotifyCanExecuteChanged();
            }
        }

        public IRelayCommand AddUserCommand { get; private set; }
        public IRelayCommand EditUserCommand { get; private set; }
        public IRelayCommand DeleteUserCommand { get; private set; }

        private void InitializeCommands()
        {
            AddUserCommand = new RelayCommand(AddUser);
            EditUserCommand = new RelayCommand(EditUser, () => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(DeleteUser, () => SelectedUser != null);
        }

        private async void LoadUsers()
        {
            var users = await _userRepository.GetAllAsync();
            Users = new ObservableCollection<User>(users);
        }

        private async void AddUser()
        {
            var newUser = new User();
            var dialog = new UserDialog(newUser);

            if (dialog.ShowDialog() == true)
            {
                await _userRepository.CreateAsync(newUser);
                Users.Add(newUser);
            }
        }

        private async void EditUser()
        {
            if (SelectedUser == null) return;
            var userToEdit = (User)SelectedUser.Clone();
            var dialog = new UserDialog(userToEdit);

            if (dialog.ShowDialog() == true)
            {
                var result = await _userRepository.UpdateAsync(userToEdit);
                if (result)
                {
                    var index = Users.IndexOf(SelectedUser);
                    Users[index] = userToEdit;
                    MessageBox.Show("User updated successfully!");
                }
            }
        }

        private async void DeleteUser()
        {
            if (SelectedUser == null) return;

            if (MessageBox.Show("Are you sure you want to delete this user?",
                "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var result = await _userRepository.DeleteAsync(SelectedUser.UserID);
                if (result)
                {
                    Users.Remove(SelectedUser);
                    MessageBox.Show("User deleted successfully!");
                }
            }
        }


    }
}
