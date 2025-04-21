using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DostavniPotpis.Messages;
using DostavniPotpis.Models;
using DostavniPotpis.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DostavniPotpis.ViewModels
{
    public partial class TransferViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly IApiService _apiService;
        private readonly IDatabaseService _databaseService;
        private readonly IPreferencesService _preferencesService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<DocumentModel> _documents;
        private double _progress;
        private bool _isMessengerRegistered = false;

        [ObservableProperty]
        public bool _isOffline;

        [ObservableProperty]
        private string appserver;        

        private bool _isProgressBarVisible;
        private bool _isButtonEnabled;

        public TransferViewModel(IApiService apiService, IDatabaseService databaseService, IPreferencesService preferencesService, INavigationService navigationService)
        {
            _apiService = apiService;
            _databaseService = databaseService;
            _preferencesService = preferencesService;
            _navigationService = navigationService;

            _documents = new ObservableCollection<DocumentModel>();

            Appserver = _preferencesService.GetPreferences("Appserver", string.Empty);
            IsOffline = _preferencesService.GetPreferences("OfflineMode", false);            

            _progress = 0;

            _isButtonEnabled = true;
            _isProgressBarVisible = false;

            
            UpdateButtonState();

            if (!_isMessengerRegistered)
            {
                WeakReferenceMessenger.Default.Register<TransferViewModel, LoadDataForTransfer>(this, (recipient, message) =>
                {
                    recipient.LoadDocuments();
                });

                _isMessengerRegistered = true;
            }
        }

        public double Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsButtonEnabled
        {
            get => _isButtonEnabled;
            set
            {
                if (_isButtonEnabled != value)
                {
                    _isButtonEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsProgressBarVisible
        {
            get => _isProgressBarVisible;
            set
            {
                if (_isProgressBarVisible != value)
                {
                    _isProgressBarVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private async void LoadDocuments()
        {
            _documents.Clear();
            var dokumenti = await _databaseService.GetDocumentList();

            foreach (var dokument in dokumenti)
            {
                _documents.Add(dokument);
            }
        }

        private void UpdateButtonState()
        {
            IsButtonEnabled = !IsOffline;
            OnPropertyChanged(nameof(IsButtonEnabled));
        }

        [RelayCommand]
        private async void Logout()
        {
            _preferencesService.SavePreferences("User", "");
            _preferencesService.SavePreferences("Password", "");

            await _navigationService.NavigateToAsync("//LoginView");
        }

        [RelayCommand]
        async Task OfflineMode(bool state)
        {
            _preferencesService.SavePreferences("OfflineMode", state);
            IsOffline = state;
            UpdateButtonState();
        }

        public ICommand PrijenosCommand => new Command(async () => await SendData(), () => IsButtonEnabled);

        private async Task SendData()
        {
            Progress = 0;
            var dokumentiZaPrijenos = _documents.Where(d => !d.Preneseno).ToList();
            var totalDocs = dokumentiZaPrijenos.Count;

            if (totalDocs == 0)
            {
                await Shell.Current.DisplayAlert("Obavijest", "Nema dokumenata za prijenos.", "OK");
                return;
            }

            OnPropertyChanged(nameof(Progress));
            IsProgressBarVisible = true;
            IsButtonEnabled = false;

            try
            {
                var (poslano, uspjesnoPoslani, neuspjesniDokumenti, responseMessage) = await _apiService.PosaljiDokumenteAsync(
                    dokumentiZaPrijenos,
                    _preferencesService.GetPreferences("extraUser", string.Empty),
                    _preferencesService.GetPreferences("extraPassword", string.Empty)
                );

                if (uspjesnoPoslani.Count > 0)
                {
                    int processedDocs = 0;

                    // Označavamo samo uspješno poslane dokumente
                    foreach (var dokument in dokumentiZaPrijenos)
                    {
                        if (uspjesnoPoslani.Contains(dokument.Id))
                        {
                            dokument.Preneseno = true;
                            await _databaseService.UpdateDocument(dokument);
                        }
                        processedDocs++;
                        Progress = (processedDocs / (double)totalDocs);
                        await Task.Delay(100);
                    }
                }

                // Prikazujemo korisniku listu neuspješnih dokumenata
                if (neuspjesniDokumenti.Any())
                {
                    string errorMessages = string.Join("\n", neuspjesniDokumenti
                        .Select(e => $"Dokument {e.Id}: {e.ErrorMessage}"));

                    await Shell.Current.DisplayAlert("Greška u prijenosu",
                        $"Sljedeći dokumenti nisu poslani:\n{errorMessages}", "OK");
                }
                else if (uspjesnoPoslani.Count > 0)
                {
                    await Shell.Current.DisplayAlert("Slanje podataka", "Svi dokumenti su uspješno poslani.", "Ok");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Greška", $"Slanje dokumenata nije uspjelo: {responseMessage}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Slanje podataka", $"Greška: {ex.Message}", "Ok");
            }

            IsProgressBarVisible = false;
            UpdateButtonState();
        }
    }
}
