using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace DostavniPotpis.Services
{
    public class PreferencesService : IPreferencesService
    {
        public string GetPreferences(string key, string defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public bool GetPreferences(string key, bool defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public void SavePreferences(string key, string value)
        {
            Preferences.Set(key, value);
        }

        public void SavePreferences(string key, bool value)
        {
            Preferences.Set(key, value);
        }
    }
}
