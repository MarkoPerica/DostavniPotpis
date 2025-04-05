using DostavniPotpis.ViewModels;

namespace DostavniPotpis.Views;

public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}