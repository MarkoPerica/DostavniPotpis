﻿using DostavniPotpis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IPreferencesService _preferencesService;
        public NavigationService(IPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
        }

        public Task InitializeAsync() =>
            NavigateToAsync(
                string.IsNullOrEmpty(_preferencesService.GetPreferences("User", string.Empty)) &&
                string.IsNullOrEmpty(_preferencesService.GetPreferences("Password", string.Empty))
                ? "//LoginView"
                : "//MainView"
                );

        public Task NavigateToAsync(string route, IDictionary<string, object> routeParam = null)
        {
            var shellNavigation = new ShellNavigationState(route);

            return routeParam != null
                ? Shell.Current.GoToAsync(shellNavigation, routeParam)
                : Shell.Current.GoToAsync(shellNavigation);
        }

        public Task PopAsync() =>
            Shell.Current.GoToAsync("..");
    }
}
