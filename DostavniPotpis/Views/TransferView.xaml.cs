using DostavniPotpis.ViewModels;

namespace DostavniPotpis.Views;

public partial class TransferView : ContentPage
{
	public TransferView(TransferViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();		
	}
}