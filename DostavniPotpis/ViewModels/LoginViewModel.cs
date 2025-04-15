using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DostavniPotpis.Models;
using DostavniPotpis.Services;
using DostavniPotpis.Services.Navigation;
using DostavniPotpis.Services.Preferences;
using DostavniPotpis.Validations;
using Newtonsoft.Json;
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
        
        private long _isBusy;

        public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;

        [ObservableProperty]
        private ValidatableObject<string> _userName = new();

        [ObservableProperty]
        private ValidatableObject<string> _password = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private bool _isValid;

        public LoginViewModel(IApiService apiService, INavigationService navigationService, IPreferencesService preferencesService)
        {
            _apiService = apiService;
            _navigationService = navigationService;
            _preferencesService = preferencesService;
        }

        public async Task IsBusyFor(Func<Task> unitOfWork)
        {
            Interlocked.Increment(ref _isBusy);
            OnPropertyChanged(nameof(IsBusy));

            try
            {
                await unitOfWork();
            }
            finally
            {
                Interlocked.Decrement(ref _isBusy);
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        [RelayCommand]
        private async Task SignIn()
        {
            if (UserName.Value == GlobalSettings.AdminUser && Password.Value == GlobalSettings.AdminPassword)
            {
                UserName.Value = "";
                Password.Value = "";
                await _navigationService.NavigateToAsync("//MainView");
            }
            else if (await _apiService.Ping() == "Connection failure")
            {
                await Shell.Current.DisplayAlert("Povezivanje", "Greška: Povezivanje s poslužiteljem nije moguće", "OK");
            }
            else if (await _apiService.Ping() == "An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set.")
            {
                await Shell.Current.DisplayAlert("Povezivanje", "Greška: nije upisan ispravan URL poslužitelja", "OK");
            }
            else
            {
                await IsBusyFor(async () =>
                {
                    var (poslano, responseMessage) = await _apiService.Login(UserName.Value, Password.Value).ConfigureAwait(false);

                    if (poslano)
                    {
                        _preferencesService.SavePreferences("User", UserName.Value);
                        _preferencesService.SavePreferences("Password", Password.Value);

                        UserName.Value = "";
                        Password.Value = "";

                        await _navigationService.NavigateToAsync("//MainView");
                    }
                    else
                    {
                        string errorMessage = responseMessage;

                        try
                        {
                            var errorResponse = JsonConvert.DeserializeObject<ErrorResponseModel>(responseMessage);
                            if (errorResponse != null && errorResponse.NumErrors > 0)
                            {
                                errorMessage = errorResponse.ErrorMessage;
                            }
                        }
                        catch (JsonException)
                        {
                            //TODO:Ako parsiranje ne uspije, ostavi originalni `responseMessage`
                        }

                        await ShowAlertAsync("Prijava neuspješna", errorMessage);
                    }
                });
            }
        }

        private async Task ShowAlertAsync(string title, string message)
        {
            if (Shell.Current == null)
            {
                Console.WriteLine("Shell.Current is null! Alert can't show.");
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    message = message.Replace("\n", " ").Replace("\r", " ");
                    await Shell.Current.DisplayAlert(title, message, "OK");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with displaying alert: {ex.Message}");
                }
            });
        }

        [RelayCommand]
        private void Validate()
        {
            IsValid = UserName.Validate() && Password.Validate();
        }

        private void AddValidations()
        {
            UserName.Validations.Add(new IsNotEmptyOrNull<string> { ValidationMessage = "Potrebno je upisati korisničko ime." });
            Password.Validations.Add(new IsNotEmptyOrNull<string> { ValidationMessage = "Potrebno je upisati lozinku." });
        }
    }
}
