using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMDesktopUI.EventModels;

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private SimpleContainer _container;

        public ShellViewModel(
            IEventAggregator events,
            SalesViewModel salesVM,
            SimpleContainer container)
        {
            _events = events;
            _salesVM = salesVM;
            _container = container;

            _events.SubscribeOnUIThread(this);

            ActivateItemAsync(_container.GetInstance<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesVM);
        }
    }
}
