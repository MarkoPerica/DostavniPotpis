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
