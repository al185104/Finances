namespace Finances
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute($"{nameof(Views.HomePage)}/{nameof(Views.AddTransactionPage)}", typeof(Views.AddTransactionPage));
        }
    }
}
