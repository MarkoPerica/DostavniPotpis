using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DostavniPotpis.Models;
using DostavniPotpis.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.ViewModels
{
    public partial class MainViewModel : ObservableObject, INotifyPropertyChanged
    {
        public static List<DocumentModel> DocumentsListForSearch { get; private set; } = new List<DocumentModel>();

        public ObservableCollection<DocumentModel> Documents { get; set; } = new ObservableCollection<DocumentModel>();

        public ObservableCollection<object> SelectedDocuments { get; set; } = new ObservableCollection<object>();

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isSearchBar;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private bool isPotpisiButtonVisible;

        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        private readonly IPreferencesService _preferencesService;
        private readonly IApiService _apiService;

        public MainViewModel(IApiService apiService, INavigationService navigationService, IDatabaseService databaseService, IPreferencesService preferencesService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _preferencesService = preferencesService;
            _apiService = apiService;

            
        }

        public async Task GetDocumentsList()
        {
            Documents.Clear();

            var documentList = await _databaseService.GetDocumentList();

            if(documentList?.Count > 0)
            {
                documentList = documentList.OrderBy(d => d.OznStDok).ThenBy(d => d.KupacDio).ToList();
                foreach(var document in documentList)
                {
                    Documents.Add(document);
                }
            }
        }

        [RelayCommand]
        public async Task EditDocumentImage(DocumentModel documentModel)
        {
            var navigationParameter = new Dictionary<string, object> { { "SelectedRacun", documentModel } };

            if (documentModel.ImageData != null)
            {
                var result = await Shell.Current.DisplayAlert("Dokument je već potpisan", "Jeste li sigurni da želite potpisati ponovno dokument?", "Da", "Ne");
                if (result)
                {
                    documentModel.ImageData = null;
                    documentModel.OznStDok = GlobalSettings.StatusUTijeku;
                    documentModel.Backgroundcolor = GlobalSettings.StatusUTijekuColor;
                    documentModel.Preneseno = false;
                    await _databaseService.UpdateDocument(documentModel);
                    await _navigationService.NavigateToAsync($"SignatureView", navigationParameter);
                }
            }
            else
                await _navigationService.NavigateToAsync($"SignatureView", navigationParameter);
        }

        [RelayCommand]
        public async Task DeleteDocument(DocumentModel documentModel)
        {
            if (documentModel.OznStDok != GlobalSettings.StatusIsporuceno)
            {
                var result = await Shell.Current.DisplayAlert("Izbrisati odabrani dokument", "Jeste li sigurni da želite izbrisati dokument?", "Da", "Ne");
                if (result)
                {
                    var delResponse = await _databaseService.DeleteDocument(documentModel);
                    if (delResponse > 0)
                    {
                        Documents.Remove(documentModel);
                    }
                }
            }
        }

        [RelayCommand]
        public async Task DeleteDocumentAll()
        {
            var result = await Shell.Current.DisplayAlert("Izbrisati potpisnu listu", "Jeste li sigurni da želite izbrisati sve dokumente?", "Da", "Ne");
            if (result)
            {
                await _databaseService.DeleteDocumentAll();
                await GetDocumentsList();
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await GetDocumentsList();
            IsRefreshing = false;
        }
    }
}
