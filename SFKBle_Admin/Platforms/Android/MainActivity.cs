using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.AppCompat.App;
using CommunityToolkit.Maui.ApplicationModel;
using Plugin.LocalNotification;
using SFKBle_Admin;
using static SFKBle_Admin.UsbSerialPortInfo;


namespace SFKBle_Admin
{
    [Activity(Label = "SFK Admin", Icon = "@mipmap/sunfunkitsappicon_xxhdpi", Theme = "@style/Theme.MaterialComponents.DayNight.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static MainActivity MainActivityInstance { get; private set; }

        const int RequestNotificationId = 0;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                RequestedOrientation = ScreenOrientation.Portrait; // Lock to portrait for phones
            }

            // Set the app theme
            SetTheme(Resource.Style.AppTheme);

            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            DeviceDisplay.KeepScreenOn = true;

            #region For screen Height & Width  

            var pixels = Resources.DisplayMetrics.WidthPixels;
            var scale = Resources.DisplayMetrics.Density;
            var dps = (double)((pixels - 0.5f) / scale);
            var ScreenWidth = (int)dps;
            App.ScreenWidth = ScreenWidth;
            pixels = Resources.DisplayMetrics.HeightPixels;
            dps = (double)((pixels - 0.5f) / scale);
            var ScreenHeight = (int)dps;
            App.ScreenHeight = ScreenHeight;

            #endregion For screen Height & Width  

            #region Notification

            await EnsureNotificationPermissionAsync();

            // Handle taps when app is cold
            LocalNotificationCenter.NotifyNotificationTapped(Intent);

            Preferences.Set("APP_BADGE_COUNT", 0);

            LocalNotificationCenter.Current.CancelAll();

            Badge.Default.SetCount(0);

            #endregion Notification

            MainActivityInstance = this;

            ApplyStatusBarStyle();

            if (!Android.OS.Build.Manufacturer.ToLower().Contains("amazon"))
            {
                var rootView = Window.DecorView.FindViewById(Android.Resource.Id.Content);

                if (rootView != null)
                {
                    rootView.SetOnApplyWindowInsetsListener(new WindowInsetsListener());
                    rootView.RequestApplyInsets();
                }
            }

            Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping("NoShadow", (handler, view) =>
            {
                handler.PlatformView.Elevation = 0;
            });
        }
        public static async Task<bool> EnsureNotificationPermissionAsync()
        {
            if (DeviceInfo.Platform != DevicePlatform.Android)
            {
                return true;
            }

            if (DeviceInfo.Version.Major < 13)
            {
                return true;
            }

            PermissionStatus status;

            status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }

            if (status != PermissionStatus.Granted)
            {
                return false;
            }

            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnNewIntent(Intent intent)
        {
            // Handle taps when app is already running
            base.OnNewIntent(intent);

            LocalNotificationCenter.NotifyNotificationTapped(intent);
        }
        public class BLEPermission : Permissions.BasePlatformPermission
        {
            public override (string androidPermission, bool isRuntime)[] RequiredPermissions
            {
                get
                {
                    bool isAmazon;

                    isAmazon = Build.Manufacturer != null && Build.Manufacturer.ToLower().Contains("amazon");

                    if (isAmazon == true)
                    {
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                        {
                            return new (string androidPermission, bool isRuntime)[]
                            {
                        (Manifest.Permission.AccessFineLocation, true),
                        (Manifest.Permission.AccessCoarseLocation, true),
                        (Manifest.Permission.PostNotifications, true)
                            };
                        }

                        return new (string androidPermission, bool isRuntime)[]
                        {
                    (Manifest.Permission.AccessFineLocation, true),
                    (Manifest.Permission.AccessCoarseLocation, true)
                        };
                    }

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                    {
                        return new (string androidPermission, bool isRuntime)[]
                        {
                    (Manifest.Permission.BluetoothScan, true),
                    (Manifest.Permission.BluetoothConnect, true),
                    (Manifest.Permission.AccessFineLocation, true),
                    (Manifest.Permission.PostNotifications, true)
                        };
                    }

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                    {
                        return new (string androidPermission, bool isRuntime)[]
                        {
                    (Manifest.Permission.BluetoothScan, true),
                    (Manifest.Permission.BluetoothConnect, true),
                    (Manifest.Permission.AccessFineLocation, true)
                        };
                    }

                    return new (string androidPermission, bool isRuntime)[]
                    {
                (Manifest.Permission.AccessFineLocation, true),
                (Manifest.Permission.AccessCoarseLocation, true)
                    };
                }
            }
        }
        public class WindowInsetsListener : Java.Lang.Object, Android.Views.View.IOnApplyWindowInsetsListener
        {
            public WindowInsets OnApplyWindowInsets(Android.Views.View v, WindowInsets insets)
            {
                int topInset = insets.SystemWindowInsetTop;
                int bottomInset = insets.SystemWindowInsetBottom;

                int topPadding = topInset > 0 ? topInset : 0;
                int bottomPadding = bottomInset > 0 ? bottomInset : 0;

                v.SetPadding(0, topPadding, 0, bottomPadding);

                return insets;
            }
        }
        public override Resources Resources
        {
            get
            {
                Resources res = base.Resources;
                Configuration config = new Configuration();
                config.SetToDefaults();
                res.UpdateConfiguration(config, res.DisplayMetrics);
                return res;
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            ApplyStatusBarStyle();
        }
        public void ApplyStatusBarStyle()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                return;
            }

            var uiMode = Resources.Configuration.UiMode & UiMode.NightMask;

            bool isDarkMode = uiMode == UiMode.NightYes;

            if (isDarkMode)
            {
                Window.SetStatusBarColor(Android.Graphics.Color.Black);

                int flags = (int)Window.DecorView.SystemUiVisibility;

                flags &= ~(int)SystemUiFlags.LightStatusBar;

                Window.DecorView.SystemUiVisibility =
                    (StatusBarVisibility)flags;
            }
            else
            {
                Window.SetStatusBarColor(Android.Graphics.Color.White);

                int flags = (int)Window.DecorView.SystemUiVisibility;

                flags |= (int)SystemUiFlags.LightStatusBar;

                Window.DecorView.SystemUiVisibility =
                    (StatusBarVisibility)flags;
            }
        }

        #region USB Serial Communication

        public string ACTION_USB_PERMISSION = "com.companyname.sfkble_Admin";
        public UsbManager usbManager;
        public UsbSerialPortAdapter adapter;
        UsbReceiver usbReceiver;
        public UsbSerialPort selectedPort;
        public Task<IList<IUsbSerialDriver>> Drivers;
        public UsbSerialPort port;
        public SerialInputOutputManager serialIoManager;
        public string USBStatus = string.Empty;

        private async void OnUsbStatusChanged(object sender, string e)
        {
            try
            {
                RunOnUiThread(() => USBStatus = e);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ExecuteUSBDeviceProcess()
        {
            try
            {
                adapter = new UsbSerialPortAdapter(this);
                usbManager = GetSystemService(Context.UsbService) as UsbManager;

                adapter.Clear();
                Drivers = FindAllDriversAsync(usbManager);

                if (Drivers != null)
                {
                    foreach (var item in Drivers.Result)
                    {
                        var ports = item.Ports;
                        foreach (var port in ports)
                        {
                            adapter.Add(port);
                        }
                    }
                }

                adapter.NotifyDataSetChanged();

                //register the broadcast receivers
                usbReceiver = new UsbReceiver();
                RegisterReceiver(usbReceiver, new IntentFilter(UsbManager.ActionUsbDeviceAttached));
                RegisterReceiver(usbReceiver, new IntentFilter(UsbManager.ActionUsbDeviceDetached));

                UsbReceiver.UsbStatusChanged += OnUsbStatusChanged;
            }
            catch (Exception ex)
            {
            }
        }

        internal static Task<IList<IUsbSerialDriver>> FindAllDriversAsync(UsbManager usbManager)
        {
            var table = UsbSerialProber.DefaultProbeTable;
            var prober = new UsbSerialProber(table);
            return prober.FindAllDriversAsync(usbManager);
        }

        #endregion USB Serial Communication
    }

    #region USB Serial Communication

    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached, UsbManager.ActionUsbDeviceDetached })]
    public class UsbReceiver : BroadcastReceiver
    {
        public static event EventHandler<string> UsbStatusChanged;
        public static event EventHandler<UsbDevice> UsbDeviceChanged;
        public static List<string> UsbDeviceChangedList = new List<string>();

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                string action = intent.Action;
                bool usbattached = false;

                if (action == UsbManager.ActionUsbDeviceAttached)
                {
                    usbattached = true;
                    UsbStatusChanged?.Invoke(this, "USB device attached");
                }
                else if (action == UsbManager.ActionUsbDeviceDetached)
                {
                    usbattached = false;
                    UsbStatusChanged?.Invoke(this, "USB device detached");
                }

                UsbDevice device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                UsbDeviceChanged?.Invoke(this, device); // Notify specific device

                UsbReceiver.UsbDeviceChanged += (sender, device) =>
                {
                    if (device != null && device.ManufacturerName == "FTDI")
                    {
                        BroadcastService temp = new BroadcastService();

                        if (usbattached)
                        {
                            temp.RequestUsbPermissionAsync(device);
                            if (UsbDeviceChangedList != null && UsbDeviceChangedList.Count > 0)
                            {
                                if (!UsbDeviceChangedList.Contains(device.SerialNumber))
                                {
                                    UsbDeviceChangedList.Add(device.SerialNumber);
                                }
                            }
                            else
                            {
                                UsbDeviceChangedList.Add(device.SerialNumber);
                            }
                        }
                        else
                        {
                            if (UsbDeviceChangedList != null && UsbDeviceChangedList.Count > 0)
                            {
                                temp.RequestUsbPermissionAsync(device);
                                var deviceToRemove = UsbDeviceChangedList.FirstOrDefault(d => d == device.SerialNumber);

                                if (deviceToRemove != null)
                                {
                                    UsbDeviceChangedList.Remove(deviceToRemove);
                                }
                            }
                        }
                    }
                };
            }
            catch { }
        }
    }

    public class UsbPermissionReceiver : BroadcastReceiver
    {
        private readonly Action<bool> _onUsbPermissionResult;

        public UsbPermissionReceiver(Action<bool> onUsbPermissionResult)
        {
            _onUsbPermissionResult = onUsbPermissionResult;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == "com.companyname.sfkble_Admin")
            {
                var granted = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, true);
                _onUsbPermissionResult?.Invoke(granted);
            }
        }
    }

    public class BroadcastService : IBroadcastService
    {
        MainActivity mainactivity = MainActivity.MainActivityInstance;
        public TaskCompletionSource<bool> _tcs;

        public void ExecuteUSBDeviceProcess()
        {
            mainactivity.ExecuteUSBDeviceProcess();
        }

        public UsbSerialPortAdapter GetAdapter()
        {
            UsbSerialPortAdapter MainPageAdater;
            try
            {
                MainPageAdater = mainactivity.adapter;
            }
            catch (Exception ex)
            {
                throw;
            }
            return MainPageAdater;
        }

        public Task<IList<IUsbSerialDriver>> GetDriver()
        {
            Task<IList<IUsbSerialDriver>> Drivers;
            try
            {
                Drivers = mainactivity.Drivers;
            }
            catch (Exception ex)
            {
                throw;
            }
            return Drivers;
        }

        public UsbManager GetUsbManager()
        {
            UsbManager UsbManager;
            try
            {
                UsbManager = mainactivity.usbManager;
            }
            catch (Exception ex)
            {
                throw;
            }
            return UsbManager;
        }

        public Tuple<int, int, int> GetUsbSerialPortInfo(UsbSerialPort selectedPort)
        {
            Tuple<int, int, int> itemtuple;
            try
            {
                UsbSerialPortInfo device = new UsbSerialPortInfo(selectedPort);
                itemtuple = new Tuple<int, int, int>(device.VendorId, device.DeviceId, device.PortNumber);
            }
            catch (Exception ex)
            {
                throw;
            }
            return itemtuple;
        }

        public Task<bool> RequestUsbPermissionAsync(UsbDevice _UsbDevice)
        {
            _tcs = new TaskCompletionSource<bool>();

            if (_UsbDevice != null && !mainactivity.usbManager.HasPermission(_UsbDevice))
            {
                PendingIntent permissionIntent = PendingIntent.GetBroadcast(mainactivity.ApplicationContext, 0, new Intent(mainactivity.ACTION_USB_PERMISSION), PendingIntentFlags.Immutable);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                {
                    mainactivity.ApplicationContext.RegisterReceiver(new UsbPermissionReceiver(OnUsbPermissionResult), new IntentFilter(mainactivity.ACTION_USB_PERMISSION), ReceiverFlags.Exported);
                }
                else
                {
                    mainactivity.ApplicationContext.RegisterReceiver(new UsbPermissionReceiver(OnUsbPermissionResult), new IntentFilter(mainactivity.ACTION_USB_PERMISSION));
                }
                //mainactivity.ApplicationContext.RegisterReceiver(new UsbPermissionReceiver(OnUsbPermissionResult), new IntentFilter(mainactivity.ACTION_USB_PERMISSION));
                mainactivity.usbManager.RequestPermission(_UsbDevice, permissionIntent);
            }
            else
            {
                _tcs.SetResult(true);
            }

            return _tcs.Task;
        }

        public Task<bool> GetSingleUSBStatus()
        {
            bool UsbConnected = true;
            if (!string.IsNullOrWhiteSpace(mainactivity.USBStatus))
            {
                if (mainactivity.USBStatus == "USB device attached")
                {
                    UsbConnected = true;
                }
                else
                {
                    UsbConnected = false;
                }
            }

            return Task.Run(() => UsbConnected);
        }

        public Task<List<string>> GetUSBStatus()
        {
            return Task.Run(() => UsbReceiver.UsbDeviceChangedList);
        }

        private void OnUsbPermissionResult(bool granted)
        {
            try
            {
                _tcs.SetResult(granted);
            }
            catch (Exception ex)
            {
            }
        }
    }

    #endregion USB Serial Communication
}

#region KeyBoad Event

public class KeyboardService : IKeyboardService
{
    public void HideKeyboard()
    {
        var activity = MainActivity.MainActivityInstance;
        var inputMethodManager = activity.GetSystemService(Context.InputMethodService) as InputMethodManager;
        var view = activity.CurrentFocus;

        inputMethodManager?.HideSoftInputFromWindow(view?.WindowToken, HideSoftInputFlags.None);
    }
}
public interface IKeyboardService
{
    void HideKeyboard();
}

#endregion KeyBoad Event