﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models; 

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private readonly IEventAggregator _events;
        private readonly SalesViewModel _salesVM;
        private readonly ILoggedInUserModel _loggedInUser;
        private readonly IAPIHelper _apiHelper;

        public bool IsLoggedIn { get => String.IsNullOrWhiteSpace(_loggedInUser.Token) == false; }

        public ShellViewModel(
            IEventAggregator events,
            SalesViewModel salesVM,
            ILoggedInUserModel loggedInUser,
            IAPIHelper apiHelper)
        {
            _events = events;
            _salesVM = salesVM;
            _loggedInUser = loggedInUser;
            _apiHelper = apiHelper;
            _events.SubscribeOnUIThread(this);

            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }
        public async Task ExitApplication()
        {
            await this.TryCloseAsync();
        }

        public async Task LogOut()
        {
            _loggedInUser.ResetUserModel();
            _apiHelper.LogOffUser();
            await _salesVM.ResetSalesViewModel();
            await ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
