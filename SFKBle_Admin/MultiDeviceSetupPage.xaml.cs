using Android.Telephony.Data;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using SFKBle.Models;
using SFKBle_Admin.SFK_Protocol;
using System.Text.RegularExpressions;
using System.Xml;

namespace SFKBle_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MultiDeviceSetupPage : ContentPage
    {
        private List<DisplayDevice> _ConnectedDevices = new List<DisplayDevice>();
        private int EstablishConnectionDatAttemps = 10;
        private int nWriteCounter = 0;
        public string SettingName = string.Empty;
        public bool is24vBattery = false;
        public BusyIndicatorViewModel objBusyIndicator;
        public string[] BLEDeviceName = new string[8];
        IProtocol[] objProtocols = new IProtocol[8];
        public DeviceInformation[] objDeviceInformations = new DeviceInformation[8];
        public int BmsDeviceCount = 0;
        public bool[] _IsSFKH2Device = new bool[8];

        int ResponceCount = 0;
        bool IsDeviceConnectionStatus = false;
        bool ProceedAhead = false;
        public double DeviceHeight = 0;
        public double DeviceWidth = 0;

        public string PopUpStatus = string.Empty;
        string CurrentPopup = string.Empty;
        public bool RemoteSessionDisplayMessageOK = false;
        public RadioButtonViewModel objRadioButtonViewModel;
        public MultiDeviceSetupPage(List<DisplayDevice> ConnectedDevice)
        {
            try
            {
                InitializeComponent();

                _ConnectedDevices = ConnectedDevice;
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (MainActivity.MainActivityInstance != null)
            {
                MainActivity.MainActivityInstance.ApplyStatusBarStyle();
            }

            RemoteSessionDetails._remote = App.Services.GetService<RemoteSessionService>();
            RemoteSessionDetails._remote.OnReceive += MultiDeviceSetupReceive;

            Task.Run(() => CheckRemoteSessionPingStatus());

            Dispatcher.Dispatch(async () =>
            {
                await Initialization();
            });
        }
        public async Task Initialization()
        {

            await ShowBusyindicatior(true);

            BmsDeviceCount = _ConnectedDevices.Count;

            await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.GetProtocolType, null);

            ResponceCount = 0;
            while (ResponceCount < 30)
            {
                await Task.Delay(1000);
                if (objDeviceInformations[BmsDeviceCount - 1] != null)
                {
                    break;
                }
                ResponceCount++;
            }

            for (int i = 0; i < BmsDeviceCount; i++)
            {
                string rawName = _ConnectedDevices[i].DeviceName;

                BLEDeviceName[i] = Regex.Replace(rawName?.ToLower().Trim() ?? "", "[^0-9a-zA-Z:,]+", "");
            }

            if (objDeviceInformations[0] != null)
            {
                is24vBattery = objDeviceInformations[0].Is24VBattery;
            }

            if (BmsDeviceCount == 2)
            {
                SettingName = "BatteryConnectionSetupforTwo";

                stackVerticalParallelandSerial.IsVisible = false;

                if (is24vBattery)
                {
                    rdSerial.Text = "2 in Series (48v)";
                    imgSerial.Source = "series2.png";

                    rdParallel.Text = "2 in Parallel (24v)";
                    imgParallel.Source = "parallel2.png";
                }
                else
                {
                    rdSerial.Text = "2 in Series (24v)";
                    imgSerial.Source = "series2.png";

                    rdParallel.Text = "2 in Parallel (12v)";
                    imgParallel.Source = "parallel2.png";
                }

                Grid.SetColumnSpan(stackSerial, 2);

                Grid.SetColumn(stackSerial, 0);
            }
            else if (BmsDeviceCount == 3)
            {
                SettingName = "BatteryConnectionSetupforThree";
                stackVerticalParallelandSerial.IsVisible = false;
                if (is24vBattery)
                {
                    sfSerial.IsVisible = false;

                    rdParallel.Text = "3 in Parallel (24v)";
                    imgParallel.Source = "parallel3.png";

                    Grid.SetColumnSpan(stackSerial, 2);
                    Grid.SetColumn(stackSerial, 0);
                }
                else
                {
                    rdSerial.Text = "3 in Series (36v)";
                    imgSerial.Source = "series3.png";

                    rdParallel.Text = "3 in Parallel (12v)";
                    imgParallel.Source = "parallel3.png";

                    Grid.SetColumnSpan(stackSerial, 2);

                    Grid.SetColumn(stackSerial, 0);
                }
            }
            else if (BmsDeviceCount == 4)
            {
                SettingName = "BatteryConnectionSetupforFour";
                stackVerticalParallelandSerial.IsVisible = true;

                if (is24vBattery)
                {
                    sfSerial.IsVisible = false;

                    rdParallelandSerial.Text = "2 Series \n2 parallel (48v)";
                    rdParallelandSerial.HeightRequest = 70;

                    rdParallel.Text = "4 in Parallel (24v)";
                    imgParallel.Source = "parallel4.png";
                    rdParallelandSerial.HeightRequest = 70;

                    Grid.SetColumn(stackSerial, 0);
                }
                else
                {
                    rdSerial.Text = "4 in Series (48v)";
                    imgSerial.Source = "series4.png";

                    rdParallel.Text = "4 in Parallel (12v)";
                    imgParallel.Source = "parallel4.png";
                }
            }
            else if (BmsDeviceCount == 5)
            {
                SettingName = "BatteryConnectionSetupforFive";
                stackVerticalParallelandSerial.IsVisible = false;

                sfSerial.IsVisible = false;

                if (is24vBattery)
                {
                    rdParallel.Text = "5 in Parallel (24v)";
                    imgParallel.Source = "parallel5.png";
                }
                else
                {
                    rdParallel.Text = "5 in Parallel (12v)";
                    imgParallel.Source = "parallel5.png";
                }

                Grid.SetColumnSpan(stackSerial, 2);
                Grid.SetColumn(stackSerial, 0);
            }
            else if (BmsDeviceCount == 6)
            {
                SettingName = "BatteryConnectionSetupforSix";
                if (is24vBattery)
                {
                    stackVerticalParallelandSerial.IsVisible = false;
                    rdParallel.Text = "6 in Parallel (24v)";
                    imgParallel.Source = "parallel6.png";
                    rdParallel.HeightRequest = 70;

                    rdSerial.Text = "2 in series, \n3 in parallel (48v)";
                    rdSerial.HeightRequest = 70;
                    imgSerial.Source = "series2parallel3.png";
                }
                else
                {
                    stackVerticalParallelandSerial.IsVisible = true;
                    rdParallel.Text = "6 in Parallel (12v)";
                    imgParallel.Source = "parallel6.png";
                    rdParallel.HeightRequest = 70;

                    rdSerial.Text = "2 in series, \n3 in parallel (24v)";
                    rdSerial.HeightRequest = 70;
                    imgSerial.Source = "series2parallel3.png";

                    rdParallelandSerial.Text = "3 in series, \n2 in parallel (36v)";
                    rdParallelandSerial.HeightRequest = 70;
                    imgVerticalParallelandSerial.Source = "series3parallel2.png";
                }
            }
            else if (BmsDeviceCount == 7)
            {
                SettingName = "BatteryConnectionSetupforSeven";

                stackVerticalParallelandSerial.IsVisible = false;

                sfSerial.IsVisible = false;

                if (is24vBattery)
                {
                    rdParallel.Text = "7 in Parallel (24v)";
                    imgParallel.Source = "parallel7.png";
                }
                else
                {
                    rdParallel.Text = "7 in Parallel (12v)";
                    imgParallel.Source = "parallel7.png";
                }

                Grid.SetColumnSpan(stackSerial, 2);
                Grid.SetColumn(stackSerial, 0);
            }
            else if (BmsDeviceCount == 8)
            {
                SettingName = "BatteryConnectionSetupforEight";
                if (is24vBattery)
                {
                    stackVerticalParallelandSerial.IsVisible = false;
                    rdParallel.Text = "8 in Parallel (24v)";
                    imgParallel.Source = "parallel8.png";

                    rdSerial.Text = "2 in series, \n4 in parallel (48v)";
                    rdSerial.HeightRequest = 70;
                    imgSerial.Source = "series2parallel4.png";
                }
                else
                {
                    stackVerticalParallelandSerial.IsVisible = true;
                    rdParallel.Text = "8 in Parallel (12v)";
                    imgParallel.Source = "parallel8.png";

                    rdSerial.Text = "4 in series, \n2 in parallel (48v)";
                    rdSerial.HeightRequest = 70;
                    imgSerial.Source = "series4parallel2.png";

                    rdParallelandSerial.Text = "2 in series, \n4 in parallel (24v)";
                    rdParallelandSerial.HeightRequest = 70;
                    imgVerticalParallelandSerial.Source = "series2parallel4.png";
                }
            }

            await ReadBatterySetup();

            await ShowBusyindicatior(false);
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            try
            {
                base.OnSizeAllocated(width, height);
                DeviceWidth = width;
                DeviceHeight = height;

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    imgSerial.HeightRequest = 100;
                    imgParallel.HeightRequest = 100;
                    imgVerticalParallelandSerial.HeightRequest = 100;
                }
                else
                {
                    imgSerial.HeightRequest = 200;
                    imgParallel.HeightRequest = 200;
                    imgVerticalParallelandSerial.HeightRequest = 200;
                }

                if (Width > Height)
                {
                    sfParallelandSerial.HorizontalOptions = LayoutOptions.StartAndExpand;
                    stackSerial.Padding = new Thickness(25, 0, 0, 0);
                    stackSerial.Margin = new Thickness(20, 0, 0, 0);
                    if (is24vBattery && BmsDeviceCount == 4)
                    {
                        stackSerial.WidthRequest = width / 2;
                    }
                    else
                    {
                        stackSerial.WidthRequest = (width / 100) * 58;
                        stackVerticalParallelandSerial.WidthRequest = (width / 100) * 45;
                    }
                    stackBorders.Orientation = StackOrientation.Horizontal;

                    imgSerial.WidthRequest = imgParallel.WidthRequest = imgVerticalParallelandSerial.WidthRequest = ((width / 100) * 26);

                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        RowTwoHeight.Height = 25;
                    }
                    else
                    {
                        RowTwoHeight.Height = 70;
                    }

                    if (BmsDeviceCount == 5 || BmsDeviceCount == 7)
                    {
                        imgParallel.WidthRequest = (width / 100) * 40;
                        imgParallel.HeightRequest = (height / 100) * 50;
                    }
                }
                else if (Height > Width)
                {
                    sfParallelandSerial.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    rdSerial.Margin = new Thickness(0, 0, 0, 0);
                    stackSerial.Margin = new Thickness(0, 0, 0, 0);
                    stackSerial.Padding = new Thickness(0, 0, 0, 0);
                    if (is24vBattery && BmsDeviceCount == 4)
                    {
                        stackSerial.WidthRequest = width / 2;
                    }
                    else
                    {
                        stackSerial.WidthRequest = width;
                    }

                    stackBorders.Orientation = StackOrientation.Vertical;

                    imgSerial.WidthRequest = (width / 2);
                    imgParallel.WidthRequest = (width / 2);
                    imgVerticalParallelandSerial.WidthRequest = (width / 100) * 60;

                    imgSerial.WidthRequest = ((Width / 100) * 30);
                    imgSerial.HeightRequest = ((Width / 100) * 30);

                    imgParallel.WidthRequest = ((Width / 100) * 30);
                    imgParallel.HeightRequest = ((Width / 100) * 30);

                    RowTwoHeight.Height = 80;

                    if (BmsDeviceCount == 5 || BmsDeviceCount == 7)
                    {
                        imgParallel.WidthRequest = (width / 100) * 60;
                        imgParallel.HeightRequest = (height / 100) * 30;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnSubmitBatterySetup_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                string SettingValue = Preferences.Get(SettingName, string.Empty);
                if (string.IsNullOrWhiteSpace(SettingValue))
                {
                    if (BmsDeviceCount > 4)
                    {
                        if (BmsDeviceCount == 6)
                        {
                            Preferences.Set(SettingName, "2Serial3Parallel");
                        }
                        else if (BmsDeviceCount == 8)
                        {
                            if (is24vBattery)
                            {
                                Preferences.Set(SettingName, "2Serial4Parallel");
                            }
                            else
                            {
                                Preferences.Set(SettingName, "4Serial2Parallel");
                            }
                        }
                    }
                    else
                    {
                        Preferences.Set(SettingName, "Serial");
                    }
                }

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ProceedForMultiView, null);

                ProceedAhead = false;
                while (true)
                {
                    if (ProceedAhead)
                    {
                        RemoteSessionDetails._remote.OnReceive -= MultiDeviceSetupReceive;

                        MultiDeviceDetails Page = new MultiDeviceDetails(_ConnectedDevices, false);
                        await Navigation.PushAsync(Page);
                        break;
                    }
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                await ShowBusyindicatior(false);
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task ReadBatterySetup()
        {
            try
            {
                var _temp = Preferences.Get(SettingName, string.Empty);

                sfSerial.Stroke = rdSerial.BackgroundColor = Color.FromHex("#F0C000");
                sfParallel.Stroke = rdParallel.BackgroundColor = Color.FromHex("#F0C000");
                sfParallelandSerial.Stroke = rdParallelandSerial.BackgroundColor = Color.FromHex("#F0C000");

                imgSerialSelected.Source = imgParallelSelected.Source = imgParallelandSerialSelected.Source = "deselect.png";

                rdSerial.TextColor = rdParallel.TextColor = rdParallelandSerial.TextColor = Color.FromHex("#000000");

                if (_temp == "Serial")
                {
                    imgSerialSelected.Source = "enable.png";

                    sfSerial.Stroke = rdSerial.BackgroundColor = Color.FromHex("#39B54A");

                    rdSerial.TextColor = Color.FromHex("#FFFFFF");
                }
                else if (_temp == "Parallel")
                {
                    imgParallelSelected.Source = "enable.png";

                    sfParallel.Stroke = rdParallel.BackgroundColor = Color.FromHex("#39B54A");

                    rdParallel.TextColor = Color.FromHex("#FFFFFF");
                }
                else if (_temp == "ParallelandSerial")
                {
                    if (BmsDeviceCount == 4)
                    {
                        imgParallelandSerialSelected.Source = "enable.png";

                        sfParallelandSerial.Stroke = rdParallelandSerial.BackgroundColor = Color.FromHex("#39B54A");

                        rdParallelandSerial.TextColor = Color.FromHex("#FFFFFF");
                    }
                    else
                    {
                        Preferences.Set(SettingName, "Serial");

                        await ReadBatterySetup();
                    }
                }
                else
                {
                    imgSerialSelected.Source = "enable.png";

                    sfSerial.Stroke = rdSerial.BackgroundColor = Color.FromHex("#39B54A");
                    rdSerial.TextColor = Color.FromHex("#FFFFFF");
                }
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                StackLayout objstack = (StackLayout)sender;

                sfSerial.Stroke = rdSerial.BackgroundColor = Color.FromHex("#F0C000");
                sfParallel.Stroke = rdParallel.BackgroundColor = Color.FromHex("#F0C000");
                sfParallelandSerial.Stroke = rdParallelandSerial.BackgroundColor = Color.FromHex("#F0C000");

                imgSerialSelected.Source = imgParallelSelected.Source = imgParallelandSerialSelected.Source = "deselect.png";

                rdSerial.TextColor = rdParallel.TextColor = rdParallelandSerial.TextColor = Color.FromHex("#000000");

                if (objstack.ClassId == "SerialClass")
                {
                    imgSerialSelected.Source = "enable.png";

                    sfSerial.Stroke = rdSerial.BackgroundColor = Color.FromHex("#39B54A");

                    rdSerial.TextColor = Color.FromHex("#FFFFFF");
                    if (BmsDeviceCount > 4)
                    {
                        if (BmsDeviceCount == 6)
                        {
                            Preferences.Set(SettingName, "2Serial3Parallel");
                        }
                        else if (BmsDeviceCount == 8)
                        {
                            if (is24vBattery)
                            {
                                Preferences.Set(SettingName, "2Serial4Parallel");
                            }
                            else
                            {
                                Preferences.Set(SettingName, "4Serial2Parallel");
                            }
                        }
                    }
                    else
                    {
                        Preferences.Set(SettingName, "Serial");
                    }
                }
                else if (objstack.ClassId == "ParallelClass")
                {
                    imgParallelSelected.Source = "enable.png";

                    sfParallel.Stroke = rdParallel.BackgroundColor = Color.FromHex("#39B54A");

                    rdParallel.TextColor = Color.FromHex("#FFFFFF");

                    Preferences.Set(SettingName, "Parallel");
                }
                else if (objstack.ClassId == "ParallelandSerialClass")
                {
                    imgParallelandSerialSelected.Source = "enable.png";

                    sfParallelandSerial.Stroke = rdParallelandSerial.BackgroundColor = Color.FromHex("#39B54A");

                    rdParallelandSerial.TextColor = Color.FromHex("#FFFFFF");

                    if (BmsDeviceCount == 6)
                    {
                        Preferences.Set(SettingName, "3Serial2Parallel");
                    }
                    else if (BmsDeviceCount == 8)
                    {
                        Preferences.Set(SettingName, "2Serial4Parallel");
                    }
                    else
                    {
                        Preferences.Set(SettingName, "2Parallel2Serial");
                    }
                }

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.BatteryConnectionSetup, objstack.ClassId);

            }
            catch (Exception ex)
            {

            }
        }
        private async Task ShowBusyindicatior(bool bShowIndicator)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    objBusyIndicator = new BusyIndicatorViewModel();

                    IsBusyIndicator.HorizontalOptions = IsBusyIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;
                    sfbIsBusyIndicator.HorizontalOptions = sfbIsBusyIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;

                    IsBusyIndicator.Margin = new Thickness(0, -25, 0, 0);

                    objBusyIndicator.IsBusy = bShowIndicator;
                    objBusyIndicator.IsVisible = bShowIndicator;

                    IsBusyIndicator.BindingContext = objBusyIndicator;
                    IsBusyIndicator.IsEnabled = objBusyIndicator.IsBusy;
                    lblBusyIndicator.Text = "Updating...";
                    lblBusyIndicator.TextColor = Color.FromHex("#f0c000");
                    sfbIsBusyIndicator.IsVisible = objBusyIndicator.IsVisible;
                    sfbIsBusyIndicator.BackgroundColor = Colors.White;
                    sfbIsBusyIndicator.HeightRequest = 260;
                    sfbIsBusyIndicator.WidthRequest = 280;

                    stackBusy.IsVisible = objBusyIndicator.IsVisible;

                });
                await Task.Delay(100);
            }
            catch { }
        }
        private async Task ShowBusyindicatior(bool bShowIndicator, string Command = "")
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (string.IsNullOrWhiteSpace(Command))
                    {
                        objBusyIndicator = new BusyIndicatorViewModel();

                        IsBusyIndicator.HorizontalOptions = IsBusyIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;
                        sfbIsBusyIndicator.HorizontalOptions = sfbIsBusyIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;

                        IsBusyIndicator.Margin = new Thickness(0, 25, 0, 0);

                        objBusyIndicator.IsBusy = bShowIndicator;
                        objBusyIndicator.IsVisible = bShowIndicator;

                        IsBusyIndicator.BindingContext = objBusyIndicator;
                        IsBusyIndicator.IsEnabled = objBusyIndicator.IsBusy;
                        lblBusyIndicator.Text = "Updating...";
                        lblBusyIndicator.TextColor = Color.FromHex("#f0c000");
                        sfbIsBusyIndicator.IsVisible = objBusyIndicator.IsVisible;
                        sfbIsBusyIndicator.BackgroundColor = Colors.White;
                        sfbIsBusyIndicator.HeightRequest = 260;
                        sfbIsBusyIndicator.WidthRequest = 280;

                        stackBusy.IsVisible = objBusyIndicator.IsVisible;
                    }
                    else
                    {
                        objBusyIndicator = new BusyIndicatorViewModel();

                        IsBusyIndicator.HorizontalOptions = IsBusyIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;
                        sfbIsBusyIndicator.HorizontalOptions = sfbIsBusyIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;

                        IsBusyIndicator.Margin = new Thickness(0, 25, 0, 0);

                        objBusyIndicator.IsBusy = bShowIndicator;
                        objBusyIndicator.IsVisible = bShowIndicator;

                        IsBusyIndicator.BindingContext = objBusyIndicator;
                        IsBusyIndicator.IsEnabled = objBusyIndicator.IsBusy;
                        lblBusyIndicator.Text = "Updating...";
                        lblBusyIndicator.TextColor = Color.FromHex("#f0c000");
                        sfbIsBusyIndicator.IsVisible = objBusyIndicator.IsVisible;
                        sfbIsBusyIndicator.BackgroundColor = Colors.White;
                        sfbIsBusyIndicator.HeightRequest = 260;
                        sfbIsBusyIndicator.WidthRequest = 280;

                        stackBusy.IsVisible = objBusyIndicator.IsVisible;
                    }
                });
                await Task.Delay(100);
            }
            catch { }
        }
        private async Task ShowDisplayPopup(string Status, string Message)
        {
            try
            {
                stkDisplayMessagePopUp.IsVisible = true;
                lblText.TextColor = Color.FromHex("#2D2D2D");
                btnOK.IsVisible = true;
                btnOK.BackgroundColor = Color.FromHex("#39B54A");
                btnOK.StrokeThickness = 0;
                lblText.HorizontalTextAlignment = TextAlignment.Center;
                lblOK.TextColor = Color.FromHex("#FFF");
                imgOK.Source = "yes.png";
                PopUpStatus = Status;

                if (Status == "Error")
                {
                    imgDisplay.Source = "error.png";
                    lblText.Text = Message;
                    btnOK.BackgroundColor = Color.FromHex("#F0C000");
                    lblOK.TextColor = Color.FromHex("#000");
                    imgOK.Source = "yesblack.png";
                }
                else if (Status == "ComfirmationPopUp")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    lblText.HorizontalTextAlignment = TextAlignment.Start;
                    btnOK.IsVisible = false;
                }
                else if (Status == "ComfirmationPopUpDisableHeating")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    lblText.HorizontalTextAlignment = TextAlignment.Start;
                    btnOK.IsVisible = false;
                    CurrentPopup = Status;
                }
                else if (Status == "ComfirmationPopUpResetCapacity")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    btnOK.IsVisible = false;
                }
                else if (Status == "ComfirmationPopUpRTCSync")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    btnOK.IsVisible = false;
                    CurrentPopup = Status;
                }
                else if (Status == "Alert")
                {
                    imgDisplay.Source = "disconnectbattery.png";
                    btnOK.IsVisible = false;
                    lblText.Text = Message;
                }
                else if (Status == "Warning")
                {
                    imgDisplay.Source = "information.png";
                    lblText.Text = Message;
                    btnOK.BackgroundColor = Color.FromHex("#F0C000");
                    lblOK.TextColor = Color.FromHex("#000");
                    imgOK.Source = "yesblack.png";
                }
                else if (Status == "WarningConfirmation")
                {
                    imgDisplay.Source = "information.png";
                    lblText.Text = Message;
                    btnOK.IsVisible = false;
                }
                else if (Status == "Firwware Warning")
                {
                    imgDisplay.Source = "information.png";
                    lblText.Text = Message;
                    btnOK.IsVisible = false;
                }
                else if (Status == "Bluetooth")
                {
                    imgDisplay.Source = "blutooth.png";
                    lblText.Text = Message;
                }
                else if (Status == "Information")
                {
                    imgDisplay.Source = "information.png";
                    lblText.Text = Message;
                }
                else if (Status == "Password")
                {
                    imgDisplay.Source = "settingsupdated.png";
                    lblText.Text = Message;
                    lblText.TextColor = Color.FromHex("#39B54A");
                }
                else if (Status == "Success")
                {
                    imgDisplay.Source = "settingsupdated.png";
                    if (!string.IsNullOrWhiteSpace(Message))
                    {
                        lblText.Text = Message;
                    }
                    else
                    {
                        lblText.Text = "Setting Updated";
                    }

                    lblText.TextColor = Color.FromHex("#39B54A");
                }
                else if (Status == "NotSuccess")
                {
                    imgDisplay.Source = "settingsnotupdated.png";
                    if (!string.IsNullOrWhiteSpace(Message))
                    {
                        lblText.Text = Message;
                    }
                    else
                    {
                        lblText.Text = "Setting Not Updated";
                    }
                    lblText.TextColor = Color.FromHex("#DC2624");
                    btnOK.BackgroundColor = Color.FromHex("#F0C000");
                    lblOK.TextColor = Color.FromHex("#000");
                    imgOK.Source = "yesblack.png";
                }
                else if (Status == "BmsMismatch")
                {
                    imgDisplay.Source = "information.png";
                    lblText.Text = Message;
                    btnOK.IsVisible = false;
                    CurrentPopup = Status;
                }
                else if (Status == "ComfirmationPopUpChargePulse")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    lblText.HorizontalTextAlignment = TextAlignment.Start;
                    btnOK.IsVisible = false;
                    CurrentPopup = Status;
                }

                await ShowDisplayPopupDesign(DeviceWidth, DeviceHeight);
            }
            catch (Exception ex)
            {

            }
        }
        private async Task ShowDisplayPopupDesign(double width, double height)
        {
            try
            {
                scrPopupText.IsEnabled = true;
                if (width > height) //landscape
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        if (PopUpStatus == "ComfirmationPopUp")
                        {
                            scrPopupText.IsEnabled = true;

                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 90;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 90;
                        }
                        else if (PopUpStatus == "ComfirmationPopUpResetCapacity")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 90;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 90;
                        }
                        else if (PopUpStatus == "Alert")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 85;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 90;
                        }
                        else if (PopUpStatus == "Success" || PopUpStatus == "NotSuccess")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 65;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 40;
                        }
                        else
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 80;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 50;
                        }
                    }
                    else
                    {
                        if (PopUpStatus == "ComfirmationPopUp")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 60;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 80;
                        }
                        else if (PopUpStatus == "ComfirmationPopUpResetCapacity")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 35;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 80;
                        }
                        else if (PopUpStatus == "Alert")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 50;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 80;
                        }
                        else if (PopUpStatus == "Success" || PopUpStatus == "NotSuccess")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 50;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 45;
                        }
                        else
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 50;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 50;
                        }
                    }
                }
                else //potrait
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        if (PopUpStatus == "ComfirmationPopUp")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 60;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 85;
                        }
                        else if (PopUpStatus == "ComfirmationPopUpResetCapacity")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 50;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 85;
                        }
                        else if (PopUpStatus == "Alert")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 50;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 85;
                        }
                        else if (PopUpStatus == "Success" || PopUpStatus == "NotSuccess")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 35;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 65;
                        }
                        else
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 50;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 90;
                        }
                    }
                    else
                    {
                        if (PopUpStatus == "ComfirmationPopUp")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 60;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 70;
                        }
                        else if (PopUpStatus == "ComfirmationPopUpResetCapacity")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 35;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 70;
                        }
                        else if (PopUpStatus == "Alert")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 40;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 70;
                        }
                        else if (PopUpStatus == "Success" || PopUpStatus == "NotSuccess")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 35;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 60;
                        }
                        else
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 35;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 85;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        private async void DisplayPopup_Clicked(object sender, EventArgs e)
        {
            if (RemoteSessionDisplayMessageOK)
            {
                await ShowBusyindicatior(true, "Please wait..");
                MainPage Page = new MainPage(false);
                await Navigation.PushAsync(Page);
            }
        }

        #region Remote Session       
        public void MultiDeviceSetupReceive(string fromDevice, string Datatype, string jsondata)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    if (Datatype == RemoteSessionReceiveDataTypes.ProtocolType.ToString())
                    {
                        bool[] _IsSFKH2Devices = Serializer.Deserialize<bool[]>(jsondata);

                        for (int i = 0; i < BmsDeviceCount; i++)
                        {
                            string rawName = _ConnectedDevices[i].DeviceName;

                            BLEDeviceName[i] = Regex.Replace(rawName?.ToLower().Trim() ?? "", "[^0-9a-zA-Z:,]+", "");

                            if (!string.IsNullOrEmpty(BLEDeviceName[i]))
                            {
                                objDeviceInformations[i] = await DeviceDefaultValues.GetDeviceData(BLEDeviceName[i]);

                                _IsSFKH2Device[i] = _IsSFKH2Devices[i];

                                if (objDeviceInformations[i] != null)
                                {
                                    objProtocols[i] = await DeviceDefaultValues.GetProtocolDetails(_IsSFKH2Device[i] ? "SFKV4" : "SFKV1");
                                }
                            }
                        }
                    }
                    else if (Datatype == RemoteSessionReceiveDataTypes.SuccessMultiPage.ToString())
                    {
                        ProceedAhead = Serializer.Deserialize<bool>(jsondata);
                    }
                    else if (Datatype == RemoteSessionReceiveDataTypes.RemoteSessionEnd.ToString())
                    {
                        string data = Serializer.Deserialize<string>(jsondata);
                        if (data == "CONNECTION_LOST")
                        {
                            await RemoteConnectionLost(false);
                        }
                        else
                        {
                            RemoteSessionDetails.CustomerName = data;
                            await StopRemoteSession();
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            });
        }
        public async Task StopRemoteSession()
        {
            try
            {
                await RemoteSessionDetails.StopRemoteSessionService();

                RemoteSessionDisplayMessageOK = true;

                MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await ShowDisplayPopup("Information", "The remote session has ended by " + RemoteSessionDetails.CustomerName);
                });
            }
            catch (Exception ex) { }
        }
        public async Task CheckRemoteSessionPingStatus()
        {
            try
            {
                while (true)
                {
                    if (Application.Current.MainPage is NavigationPage navigationPage)
                    {
                        var currentPage = navigationPage.CurrentPage;

                        if (currentPage is MultiDeviceSetupPage)
                        {
                            bool Status = RemoteSessionDetails.CheckRemoteSessionPingStatus();
                            if (!Status)
                            {
                                await RemoteConnectionLost(true);
                                break;
                            }
                            await Task.Delay(5000);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        public async Task RemoteConnectionLost(bool SendSignal)
        {
            if (SendSignal)
            {
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.RemoteSessionEnd, "CONNECTION_LOST");
            }

            await RemoteSessionDetails.StopRemoteSessionService();

            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                RemoteSessionDisplayMessageOK = true;
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.RemoteSessionEnd, "CONNECTION_LOST");
                await ShowDisplayPopup("Information", "Remote Session Connection Lost.");
            });
        }

        #endregion Remote Session
    }
}