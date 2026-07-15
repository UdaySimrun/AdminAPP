using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using static Android.Resource;
using Application = Android.App.Application;

namespace SFKBle_Admin.Platforms.Android
{
    [Service(Enabled = true, Exported = false, ForegroundServiceType = ForegroundService.TypeDataSync)]
    [Register("com.sfkble_Admin.mauiservice.BackGroundServices")]
    public sealed class BackGroundServices : Service
    {
        CancellationTokenSource? _cts;

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            StartForeground(1001, BuildNotification());

            // Run your background work loop
            _ = Task.Run(async () =>
            {
                try
                {
                    if (MainPage.MainPageInstance != null)
                    {
                        await MainPage.MainPageInstance.GetDeviceDetailRemoteSessionInfoInServer(); ;
                    }
                }
                catch (TaskCanceledException) { }
                catch (Exception ex) { }
            }, _cts.Token);

            return StartCommandResult.NotSticky;
        }

        public override IBinder? OnBind(Intent? intent) => null;

        public override void OnDestroy()
        {
            _cts?.Cancel();
            StopForeground(true);
            base.OnDestroy();
        }
        Notification BuildNotification()
        {
            var channelId = "default";

            var manager = (NotificationManager)GetSystemService(NotificationService);

            var channel = new NotificationChannel(
                channelId,
                "General",
                NotificationImportance.High
            );

            channel.SetShowBadge(true);

            manager.CreateNotificationChannel(channel);

            return new Notification.Builder(this, channelId)
                .SetSmallIcon(Drawable.StatNotifyMore)
                .SetOngoing(true)
                .SetPriority((int)NotificationPriority.High)
                .SetVisibility(NotificationVisibility.Public)
                .SetContentTitle("SFK BMS Admin")
                .SetContentText("Service running")
                .Build();
        }
    }

    public static class ServiceHelper
    {
        public static bool IsServiceRunning<T>() where T : Service
        {
            var ctx = Application.Context;
            var manager = (ActivityManager)ctx.GetSystemService(Context.ActivityService);
            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service?.ClassName?.Equals(Java.Lang.Class.FromType(typeof(T)).Name) == true)
                    return true;
            }
            return false;
        }

        public static void StartService()
        {
            if (IsServiceRunning<BackGroundServices>())
                return; // already running, do nothing

            var ctx = Application.Context;
            var intent = new Intent(ctx, typeof(BackGroundServices));

            if (OperatingSystem.IsAndroidVersionAtLeast(26))
                ctx.StartForegroundService(intent);
            else
                ctx.StartService(intent);
        }

        public static void StopService()
        {
            if (!IsServiceRunning<BackGroundServices>())
                return; // not running, nothing to stop

            var ctx = Application.Context;
            var intent = new Intent(ctx, typeof(BackGroundServices));
            ctx.StopService(intent);
        }
    }
}