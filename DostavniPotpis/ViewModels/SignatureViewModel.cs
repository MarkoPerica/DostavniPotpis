using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DostavniPotpis.Models;
using DostavniPotpis.Services;
using DostavniPotpis.Validations;
using Microsoft.Maui.Graphics.Skia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.ViewModels
{
    public partial class SignatureViewModel : ObservableObject, IQueryAttributable
    {
        public DocumentModel Document { get; set; } = new DocumentModel();

        public ObservableCollection<IDrawingLine> Lines { get; set; } = new ObservableCollection<IDrawingLine>();

        [ObservableProperty]
        private ValidatableObject<string> _potpisao = new();

        private readonly INavigationService _navigationService;
        private readonly IDatabaseService _databaseService;
        private readonly IApiService _apiService;
        private readonly IPreferencesService _preferencesService;

        public SignatureViewModel(INavigationService navigationService, IDatabaseService databaseService, IApiService apiService, IPreferencesService preferencesService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _apiService = apiService;
            _preferencesService = preferencesService;
        }

        [RelayCommand]
        public async Task ClearPotpis()
        {
            Lines.Clear();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Document = query["SelectedRacun"] as DocumentModel;
        }

        [RelayCommand]
        public async Task SavePotpis()
        {
            if (Potpisao.Value == null)
            {
                await Shell.Current.DisplayAlert("Ime i prezime nije upisano", "Upišite ime i prezime potpisivača!", "Ok");
                return;
            }

            const int width = 400;
            const int height = 400;

            var bitmapContext = new SkiaBitmapExportContext(width, height, 1.0f);
            var canvas = bitmapContext.Canvas;

            canvas.FillColor = Colors.White;
            canvas.FillRectangle(0, 0, width, height);

            var strokeColor = Colors.Black;
            var strokeWidth = 2f;

            var path = new PathF();

            foreach (var line in Lines)
            {
                if (line.Points.Count < 2)
                    continue;

                for (int i = 0; i < line.Points.Count - 1; i++)
                {
                    var start = line.Points[i];
                    var end = line.Points[i + 1];
                    path.MoveTo(start.X, start.Y);
                    path.LineTo(end.X, end.Y);
                }
            }

            canvas.Antialias = true;
            canvas.StrokeColor = strokeColor;
            canvas.StrokeSize = strokeWidth;

            canvas.DrawPath(path);

            using (var memoryStream = new MemoryStream())
            {
                bitmapContext.Image.Save(memoryStream, Microsoft.Maui.Graphics.ImageFormat.Png);
                Document.ImageData = memoryStream.ToArray();
            }

            Document.OznStDok = GlobalSettings.StatusIsporuceno;
            Document.Backgroundcolor = GlobalSettings.StatusIsporucenoColor;
            Document.Potpisao = Potpisao.Value;
            Document.Preneseno = false;
            await _databaseService.UpdateDocument(Document);

            await SendSignature();

            Lines.Clear();

            await _navigationService.PopAsync();

        }

        private async Task SendSignature()
        {
            try
            {
                var (poslano, responseMessage) = await _apiService.PosaljiDokumentAsync(Document, _preferencesService.GetPreferences("User", string.Empty), _preferencesService.GetPreferences("Password", string.Empty));

                if (poslano)
                {
                    Document.Preneseno = true;
                    await _databaseService.UpdateDocument(Document);
                    //await Shell.Current.DisplayAlert("Slanje podataka", "Dokument je poslan. Maknuti ovu poruku nakon testiranja.", "Ok");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Slanje podataka", $"Greška: {responseMessage}", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Slanje podataka", $"Greška: {ex.Message}", "Ok");
            }
        }

    }
}
