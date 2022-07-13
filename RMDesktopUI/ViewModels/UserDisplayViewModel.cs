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

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadUsers();
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

    }
}
