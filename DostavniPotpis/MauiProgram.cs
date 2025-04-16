using CommunityToolkit.Maui;
using DostavniPotpis.Services;
using DostavniPotpis.ViewModels;
using DostavniPotpis.Views;
using Microsoft.Extensions.Logging;

namespace DostavniPotpis;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
		builder.Services.AddSingleton<INavigationService, NavigationService>();
		builder.Services.AddSingleton<IPreferencesService, PreferencesService>();
		builder.Services.AddSingleton<IApiService, ApiService>();

		builder.Services.AddTransient<LoginView>();
		builder.Services.AddTransient<MainView>();
		builder.Services.AddTransient<SignatureView>();
		builder.Services.AddTransient<TransferView>();

		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<TransferViewModel>();
		builder.Services.AddSingleton<LoginViewModel>();
		builder.Services.AddSingleton<SignatureViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
