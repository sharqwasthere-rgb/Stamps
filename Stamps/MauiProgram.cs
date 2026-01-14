using Microsoft.Extensions.Logging;
using Stamps.Services;
using Stamps.Shared.Services;
using ZXing.Net.Maui.Controls;

namespace Stamps
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the Stamps.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<IPreferencesService, PreferencesService>();
            
            // Add Auth State Service (singleton for session persistence)
            builder.Services.AddSingleton<AuthStateService>();
            
            // Add Permission Service
            builder.Services.AddSingleton<IPermissionService, PermissionService>();
            
            // Add Scanner Service
            builder.Services.AddSingleton<IScannerService, ScannerService>();
            
            // Add API service
            builder.Services.AddHttpClient<Stamps.Shared.Services.IApiService, ApiService>();
            
            // Add HttpClient for direct API calls
            builder.Services.AddScoped(sp => {
                var client = new HttpClient { BaseAddress = new Uri("https://byssal-janene-lyingly.ngrok-free.dev") };
                client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
                return client;
            });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
