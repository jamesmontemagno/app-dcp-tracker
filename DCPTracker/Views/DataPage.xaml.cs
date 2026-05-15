using DCPTracker.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DCPTracker.Views;

public partial class DataPage : ContentPage
{
	public DataPage()
		: this(ResolveViewModel())
	{
	}

	public DataPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	private static MainPageViewModel ResolveViewModel() =>
		Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<MainPageViewModel>()
		?? throw new InvalidOperationException("MainPageViewModel is not available from the app service provider.");
}
