using DostavniPotpis.Models;
using DostavniPotpis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Services
{
    public class ApiService : IApiService
    {
        private readonly IPreferencesService _preferencesService;

        public ApiService(IPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
        }

        public async Task<string> Ping()
        {
            string serverUri = await GetServerUri();

            if (string.IsNullOrEmpty(serverUri))
            {
                return serverUri;
            }

            serverUri = serverUri + GlobalSettings.PingUri;

            try
            {
                using (var client = GetOrCreateHttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(serverUri);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    return response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<(bool Poslano, string ResponseContent)> Login(string username, string password, string domain = "")
        {
            string serverUri = await GetServerUri();

            if (string.IsNullOrEmpty(serverUri))
            {
                return (false, "URI nije upisan u postavke.");
            }

            serverUri = serverUri + GlobalSettings.LoginUri;

            try
            {
                using (var client = GetOrCreateHttpClient(username, password, domain))
                {
                    HttpResponseMessage response = await client.GetAsync(serverUri);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var parsedResponse = JsonConvert.DeserializeObject<LoginModel>(responseContent);

                            if (parsedResponse != null && !parsedResponse.HasErrors)
                            {
                                return (true, responseContent);
                            }
                            else
                            {
                                return (false, responseContent);
                            }
                        }
                        catch (JsonException ex)
                        {
                            return (false, $"Greška pri parsiranju odgovora: {ex.Message}\nOdgovor servera: {responseContent}");
                        }
                    }
                    else
                    {
                        // Ako nije uspješan status, pokušaj parsirati grešku
                        try
                        {
                            var errorResponse = JsonConvert.DeserializeObject<ErrorResponseModel>(responseContent);
                            if (errorResponse != null && errorResponse.NumErrors > 0)
                            {
                                return (false, $"Greška: {errorResponse.ErrorMessage}");
                            }
                        }
                        catch (JsonException)
                        {
                            return (false, $"Greška: {response.StatusCode} - {responseContent}");
                        }

                        return (false, $"Greška: {response.StatusCode} - {responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Greška: {ex.Message}");
            }
        }

        public async Task<(bool Poslano, string ResponseContent)> PosaljiDokumentAsync(DocumentModel document, string username, string password, string domain = "")
        {
            string serverUri = await GetServerUri();

            if (string.IsNullOrEmpty(serverUri))
            {
                return (false, "Pogrešan URI");
            }

            serverUri = serverUri + GlobalSettings.DocumentSendUri;

            try
            {
                using (var client = GetOrCreateHttpClient(username, password, domain))
                {
                    var jsonContent = JsonConvert.SerializeObject(document);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(serverUri, content);
                        string responseContent = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            try
                            {
                                var parsedResponse = JsonConvert.DeserializeObject<DocumentResponseModel>(responseContent);

                                if (parsedResponse != null && !parsedResponse.HasErrors)
                                {
                                    return (true, responseContent);
                                }
                                else
                                {
                                    string errorMessage = parsedResponse?.Message ?? "Nepoznata greška";
                                    if (parsedResponse?.PasoeResponses != null && parsedResponse.PasoeResponses.Count > 0)
                                    {
                                        errorMessage = string.Join("\n", parsedResponse.PasoeResponses
                                            .Where(e => e.IsError)
                                            .Select(e => e.ErrorMessage));
                                    }
                                    return (false, $"Greška prilikom slanja dokumenta: {errorMessage}");
                                }
                            }
                            catch (JsonException ex)
                            {
                                return (false, $"Greška pri parsiranju odgovora: {ex.Message}");
                            }
                        }
                        else
                        {
                            return (false, $"Greška: {response.StatusCode} - {responseContent}");
                        }
                    }
                    catch (Exception ex)
                    {
                        return (false, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Poslano, List<int> uspjesnoPoslani, List<PasoeResponse> neuspjesniDokumenti, string ResponseContent)>
        PosaljiDokumenteAsync(List<DocumentModel> dokumenti, string username, string password, string domain = "")
        {
            string serverUri = await GetServerUri();
            if (string.IsNullOrEmpty(serverUri))
                return (false, new List<int>(), new List<PasoeResponse>(), "Pogrešan URI");

            serverUri = serverUri + GlobalSettings.DocumentSendUri;

            try
            {
                using (var client = GetOrCreateHttpClient(username, password, domain))
                {
                    var jsonContent = JsonConvert.SerializeObject(dokumenti);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(serverUri, content);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var parsedResponse = JsonConvert.DeserializeObject<DocumentResponseModel>(responseContent);

                        if (parsedResponse != null)
                        {
                            //spremim ID-eve prenesenih dokumenata u listu
                            var uspjesniDokumenti = parsedResponse.ReceivedDocuments ?? new List<int>();
                            //spremim greške koje je vratio pasoe
                            var neuspjesniDokumenti = parsedResponse.PasoeResponses ?? new List<PasoeResponse>();

                            return (uspjesniDokumenti.Count > 0, uspjesniDokumenti, neuspjesniDokumenti, responseContent);
                        }
                    }

                    return (false, new List<int>(), new List<PasoeResponse>(), $"Greška: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, new List<int>(), new List<PasoeResponse>(), $"Greška: {ex.Message}");
            }
        }

        private HttpClient GetOrCreateHttpClient(string username = "", string password = "", string domain = "")
        {
            HttpClient httpClient = new HttpClient();

            var user = username;
            var pass = password;
            var basicAuthValue = "";

            if (domain == "")
                basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            else
                basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{domain}:{username}:{password}"));

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("X-Authorization", basicAuthValue);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "exTra Dostava");

            return httpClient;

        }

        private async Task<string> GetServerUri()
        {
            return await Task.FromResult(_preferencesService.GetPreferences("Appserver", string.Empty));
        }

        
    }
}
