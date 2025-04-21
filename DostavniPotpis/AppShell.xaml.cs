using CommunityToolkit.Mvvm.Messaging;
using DostavniPotpis.Messages;
using DostavniPotpis.Services;
using DostavniPotpis.Views;

namespace DostavniPotpis
{
    public partial class AppShell : Shell
    {
        private readonly INavigationService _navigationService;
        public AppShell(INavigationService navigationService)
        {
            _navigationService = navigationService;

            AppShell.InitializeRouting();

            InitializeComponent();
        }

        protected override async void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);

            if (args.Current.Location.OriginalString.Contains("TransferView"))
            {
                WeakReferenceMessenger.Default.Send(new LoadDataForTransfer());
            }
            else if (args.Current.Location.OriginalString.Contains("MainView"))
            {
                WeakReferenceMessenger.Default.Send(new RefreshCollectionMessage());
            }
        }

        private static void InitializeRouting()
        {
            Routing.RegisterRoute("SignatureView", typeof(SignatureView));
            Routing.RegisterRoute("MainView", typeof(MainView));
            Routing.RegisterRoute("TransferView", typeof(TransferView));            
        }
    }
}
