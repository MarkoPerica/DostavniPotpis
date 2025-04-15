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

        private static void InitializeRouting()
        {
            Routing.RegisterRoute("SignatureView", typeof(SignatureView));
            Routing.RegisterRoute("MainView", typeof(MainView));
            Routing.RegisterRoute("TransferView", typeof(TransferView));            
        }
    }
}
