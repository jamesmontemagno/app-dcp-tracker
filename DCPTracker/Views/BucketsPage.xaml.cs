namespace DCPTracker.Views;

public partial class BucketsPage : ContentPage
{
    public BucketsPage(ViewModels.BucketsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ViewModels.BucketsViewModel vm)
        {
            _ = vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
