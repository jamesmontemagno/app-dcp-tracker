namespace DCPTracker.Views;

public partial class ProjectionsPage : ContentPage
{
    public ProjectionsPage(ViewModels.ProjectionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ViewModels.ProjectionsViewModel vm)
        {
            _ = vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
