using CommunityToolkit.Mvvm.ComponentModel;
using DostavniPotpis.Services.Api;
using DostavniPotpis.Services.Navigation;
using DostavniPotpis.Services.Preferences;
using DostavniPotpis.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;
        private readonly IPreferencesService _preferencesService;

        [ObservableProperty]
        private ValidatableObject<string> _userName = new();

        [ObservableProperty]
        private ValidatableObject<string> _password = new();

        public LoginViewModel(IApiService apiService, INavigationService navigationService, IPreferencesService preferencesService)
        {
            _apiService = apiService;
            _navigationService = navigationService;
            _preferencesService = preferencesService;
        }

        private async Task SignIn()
        {
            if (UserName.Value == GlobalSettings.AdminUser && Password.Value == GlobalSettings.AdminPassword)
            {
                UserName.Value = "";
                Password.Value = "";
                await _navigationService.NavigateToAsync("//MainView");
            }            
        }
    }
}
