using Caliburn.Micro;
using RMDesktopUI.Helpers;
using RMDesktopUI.Library.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName;
        private string _userPassword;
        private IAPIHelper _apiHelper;
        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set 
            { 
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => IsErrorVisible);
            }
        }
        public bool IsErrorVisible
        {
            get => ErrorMessage?.Length > 0;
        }
        public string UserName 
        {
            get { return _userName; }
            set 
            { 
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }
        public string UserPassword 
        {
            get { return _userPassword; }
            set 
            {
                _userPassword = value;
                NotifyOfPropertyChange(() => UserPassword);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public LoginViewModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public bool CanLogIn
        {
            get 
            {
                bool output = false;

                if (String.IsNullOrEmpty(UserName) == false && String.IsNullOrEmpty(UserPassword) == false)
                {
                    output = true;
                }

                return output;
            }
            
        }

        public async Task LogIn() 
        {
            try
            {
                ErrorMessage = "";
                var result = await _apiHelper.Authenticate(UserName, UserPassword);

                // Capture more information about the user
                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);
                
                MessageBox.Show("You logged in!");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

    }
}
