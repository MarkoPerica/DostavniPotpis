using DostavniPotpis.Services.Navigation;

namespace DostavniPotpis
{
    public partial class App : Application
    {
        public App(INavigationService navigationService)
        {
            InitializeComponent();

            MainPage = new AppShell(navigationService);
        }
    }
}
