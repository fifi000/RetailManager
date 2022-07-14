using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _msgBox;
        private readonly IWindowManager _window;
        private readonly IUserEndpoint _userEndpoint;
        private BindingList<UserModel> _users;
        private UserModel _selectedUser;
        private BindingList<string> _selectedUserRoles = new BindingList<string>();
        private List<string> _roles;
        private BindingList<string> _availableRoles;

        public UserDisplayViewModel(
            StatusInfoViewModel msgBox,
            IWindowManager window,
            IUserEndpoint userEndpoint)
        {
            _msgBox = msgBox;
            _window = window;
            _userEndpoint = userEndpoint;
        }

        public BindingList<UserModel> Users 
        {
            get { return _users; }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }
        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set 
            { 
                _selectedUser = value;
                SelectedUserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
                AvailableRoles = new BindingList<string>(AllRoles.Except(SelectedUserRoles).ToList());
                NotifyOfPropertyChange(() => SelectedUser);
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }
        public string SelectedUserName 
        { 
            get => SelectedUser?.EmailAddress; 
        }
        public BindingList<string> SelectedUserRoles
        {
            get { return _selectedUserRoles; }
            set 
            {
                _selectedUserRoles = value;
                NotifyOfPropertyChange(() => SelectedUserRoles);
            }
        }
        public BindingList<string> AvailableRoles 
        {
            get 
            {
                return _availableRoles;
            }
            set
            {
                _availableRoles = value;
                NotifyOfPropertyChange(() => AvailableRoles);
            }
        }
        public List<string> AllRoles
        {
            get { return _roles; }
            set { _roles = value; }
        }
        public string SelectedUserRole { get; set; }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadUsers();
                await LoadRoles();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;

                if (ex.Message == "Unauthorized")
                {
                    _msgBox.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form.");
                    await _window.ShowDialogAsync(_msgBox, settings: settings);

                }
                else
                {
                    _msgBox.UpdateMessage("Fatal Exception", ex.Message);
                    await _window.ShowDialogAsync(_msgBox, settings: settings);

                }

                await TryCloseAsync();
            }
        }

        private async Task LoadUsers()
        {
            var users = await _userEndpoint.GetAll();
            Users = new BindingList<UserModel>(users);
        }
        private async Task LoadRoles()
        {
            var rolesDict = await _userEndpoint.GetAllRoles();
            AllRoles = rolesDict.Select(x => x.Value).ToList();
        }

        public async void AddSelectedRole()
        {
            if (String.IsNullOrWhiteSpace(SelectedUserRole) == false)
            {
                var role = SelectedUserRole;

                await _userEndpoint.AddUserToRole(SelectedUser.Id, role); 

                SelectedUserRoles.Add(role);
                AvailableRoles.Remove(role);

                NotifyOfPropertyChange(() => SelectedUserRoles);
                NotifyOfPropertyChange(() => AvailableRoles);
            }
        }
        public async void RemoveSelectedRole()
        {
            if (String.IsNullOrWhiteSpace(SelectedUserRole) == false)
            {
                var role = SelectedUserRole;

                await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, role);

                SelectedUserRoles.Remove(role);
                AvailableRoles.Add(role);

                NotifyOfPropertyChange(() => SelectedUserRoles);
                NotifyOfPropertyChange(() => AvailableRoles);
            }
        }

    }
}
