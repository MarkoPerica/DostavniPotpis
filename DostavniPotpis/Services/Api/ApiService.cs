using DostavniPotpis.Services.Preferences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Services.Api
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

        private HttpClient GetOrCreateHttpClient()
        {
            throw new NotImplementedException();
        }

        private async Task<string> GetServerUri()
        {
            throw new NotImplementedException();
        }
    }
}
