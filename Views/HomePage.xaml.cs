using Finances.ViewModels;

namespace Finances.Views;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is HomeViewModel vm)
		{
			vm.InitializeCommand.Execute(null);
        }
    }
}