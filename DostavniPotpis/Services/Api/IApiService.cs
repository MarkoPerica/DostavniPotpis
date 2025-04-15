using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Services
{
    public interface IApiService
    {
        Task<string> Ping();
        Task<(bool Poslano, string ResponseContent)> Login(string username, string password, string domain = "");
    }
}
