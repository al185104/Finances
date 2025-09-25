using Finances.Interfaces;
using Finances.Services;
using Finances.ViewModels;
using Finances.Views;
using Microsoft.Extensions.Logging;

namespace Finances
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Feather.ttf", "Icons");
                    fonts.AddFont("Nunito-Light.ttf", "NunitoLight");
                    fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
                    fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemiBold");
                    fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
                    fonts.AddFont("Pacifico-Regular.ttf", "Pacifico");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var baseUri = new Uri(DeviceInfo.Platform == DevicePlatform.Android
                              ? "http://10.0.2.2:8000/"
                              : "http://127.0.0.1:8000/");

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton(new HttpClient { BaseAddress = baseUri });

            builder.Services.AddSingleton<ICategoryService, CategoryService>();
            builder.Services.AddSingleton<ITransactionService, TransactionService>();
            builder.Services.AddSingleton<ISummaryService, SummaryService>();

            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<HomeViewModel>();

            builder.Services.AddTransient<AddTransactionPage>();
            builder.Services.AddTransient<AddTransactionViewModel>();

            return builder.Build();
        }
    }
}
