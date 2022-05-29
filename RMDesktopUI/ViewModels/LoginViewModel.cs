using Caliburn.Micro;
using RMDesktopUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName;
        private string _userPassword;
        private IAPIHelper _apiHelper;

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
                var result = await _apiHelper.Authenticate(UserName, UserPassword);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
