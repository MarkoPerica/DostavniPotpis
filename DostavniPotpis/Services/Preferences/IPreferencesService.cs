using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Services
{
    public interface IPreferencesService
    {
        string GetPreferences(string key, string defaultValue);
        void SavePreferences(string key, string value);
    }
}
