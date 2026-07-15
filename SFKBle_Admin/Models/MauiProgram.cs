using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using RGPopup.Maui.Extensions;
using SFKBle.Models;
using Syncfusion.Maui.Core.Hosting;

namespace SFKBle_Admin
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCompatibility()
                .ConfigureSyncfusionCore()
                .ConfigureEssentials()
                .UseMauiCommunityToolkit()
                .UseMauiRGPopup()                
                .UseLocalNotification(config =>
                {
                    config.AddAndroid(android =>
                    {
                        android.AddChannel(new NotificationChannelRequest
                        {
                            Id = "default",
                            Name = "General",
                            Description = "General notifications",
                            Importance = AndroidImportance.Max,
                            Sound = "default",
                        });
                    });
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemiBold");
                });

            builder.Logging.AddDebug();

            //Register your services here
            builder.Services.AddSingleton<IBroadcastService, BroadcastService>();

            builder.Services.AddTransient<IKeyboardService, KeyboardService>();

            builder.Services.AddSingleton<RemoteSessionService, RemoteSessionService>();

            return builder.Build();
        }
    }
}
