using DostavniPotpis.ViewModels;

namespace DostavniPotpis.Views;

public partial class SignatureView : ContentPage
{
	public SignatureView(SignatureViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}