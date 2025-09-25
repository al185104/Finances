using Finances.ViewModels;

namespace Finances.Views;

public partial class AddTransactionPage : ContentPage
{
	public AddTransactionPage(AddTransactionViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AddTransactionViewModel vm)
        {
            vm.InitializeCommand.Execute(null);
        }
    }
}