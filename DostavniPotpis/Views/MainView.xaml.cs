using CommunityToolkit.Mvvm.Messaging;
using DostavniPotpis.Messages;
using DostavniPotpis.ViewModels;

namespace DostavniPotpis.Views;

public partial class MainView : ContentPage
{
	private MainViewModel _viewModel;
	public MainView(MainViewModel viewModel)
	{
		_viewModel = viewModel;
		BindingContext = viewModel;
		InitializeComponent();

		WeakReferenceMessenger.Default.Register<SendBarcodeDecode>(this, (r, m) =>
		{
			collView.Focus();
		});
	}

	private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
	{
		_viewModel.SearchText = e.NewTextValue;
		await _viewModel.SearchBuyer();		
	}
}