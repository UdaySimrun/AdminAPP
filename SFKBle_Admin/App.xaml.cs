using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Syncfusion.Licensing;

namespace SFKBle_Admin;

public partial class App : Application
{
    public static double ScreenHeight, ScreenWidth;
    public static bool IsAppActive { get; set; } = false;
    public static IServiceProvider Services { get; private set; }
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXdfeHRQRGFeVEByWUBWYEk=");

        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
        ScreenHeight = mainDisplayInfo.Height;
        ScreenWidth = mainDisplayInfo.Width;
        Services = serviceProvider;

        LoadStyles();

        //MainPage = new MainPage(true);
        MainPage = new NavigationPage(new MainPage(true))
        {
            BarBackgroundColor = Colors.Black,   // optional
            BarTextColor = Colors.White          // optional
        };
    }
    public void LoadStyles()
    {
        if (ScreenWidth > ScreenHeight) // landscapepr
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                if (ScreenWidth > 1024 && ScreenHeight > 800)
                {
                    dictionary.MergedDictionaries.Add(TabletDeviceStyle.SharedInstance);
                }
                else
                {
                    dictionary.MergedDictionaries.Add(SmallTabletDeviceStyle.SharedInstance);
                }
            }
            else if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                dictionary.MergedDictionaries.Add(MobileDevicesStyle.SharedInstance);
            }
        }
        else //portrait
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                if (ScreenHeight > 1024 && ScreenWidth > 800)
                {
                    dictionary.MergedDictionaries.Add(TabletDeviceStyle.SharedInstance);
                }
                else
                {
                    dictionary.MergedDictionaries.Add(SmallTabletDeviceStyle.SharedInstance);
                }
            }
            else if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                dictionary.MergedDictionaries.Add(MobileDevicesStyle.SharedInstance);
            }
        }
    }
    protected override void OnResume()
    {
        base.OnResume();
        IsAppActive = true;
        // App is opened again
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        IsAppActive = false;
        // App is minimized (background)
    }

}
