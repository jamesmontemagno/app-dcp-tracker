using DCPTracker.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DCPTracker.Views;

public partial class OnboardingPage : ContentPage
{
	public OnboardingPage()
		: this(ResolveViewModel())
	{
	}

	public OnboardingPage(OnboardingViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	private static OnboardingViewModel ResolveViewModel() =>
		Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<OnboardingViewModel>()
		?? throw new InvalidOperationException("OnboardingViewModel is not available from the app service provider.");
}