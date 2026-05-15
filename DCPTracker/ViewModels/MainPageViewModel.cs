using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DCPTracker.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
	[ObservableProperty]
	private int count;

	public string CounterText => Count == 0
		? "Click me"
		: Count == 1
			? $"Clicked {Count} time"
			: $"Clicked {Count} times";

	[RelayCommand]
	private void IncrementCount()
	{
		Count++;
		OnPropertyChanged(nameof(CounterText));
		SemanticScreenReader.Announce(CounterText);
	}
}