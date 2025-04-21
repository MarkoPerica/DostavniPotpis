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
        bool GetPreferences(string key, bool defalutValue);
        void SavePreferences(string key, string value);
        void SavePreferences(string key, bool value);
    }
}
