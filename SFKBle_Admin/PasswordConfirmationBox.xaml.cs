using Android.App.Job;
using Android.Content;
using Android.Hardware.Usb;
using Android.Locations;
using Newtonsoft.Json;
using RGPopup.Maui.Pages;
using RGPopup.Maui.Services;
using SFKBle.Models;
using SFKBle_Admin.SFK_Protocol;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text.RegularExpressions;
using static SFKBle_Admin.UsbSerialPortInfo;

namespace SFKBle_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordConfirmationBox : PopupPage
    {
        IProtocol[] objProtocols = new IProtocol[8];
        DeviceInformation[] objDeviceInformations = new DeviceInformation[8];

        string BarcodeValue = string.Empty;
        DevicePlatform platform = DeviceInfo.Platform;
        Version version = DeviceInfo.Version;
        Version targetVersion = new Version(12, 0);

        private List<DisplayDevice> _ConnectedDevices;

        private int nWriteCounter = 0;

        public bool PasswordCheck06CalledFrom_Please_Enter_Password = false;
        public bool PasswordCheck06CalledFrom_Reset_Password = false;
        public bool PasswordCheck06CalledFrom_IntialLoad = false;
        private int EstablishConnectionDatAttemps = 10;

        public bool[] IsValidDevicePassword = new bool[8];
        public bool[] CheckDefaultPasswordForDevice = new bool[8];
        public bool[] bProceefForDevice = new bool[8];
        public bool[] _IsSFKH2Device = new bool[8];

        public double DeviceHeight = 0;
        public double DeviceWidth = 0;

        public string PopUpStatus = string.Empty;

        int passwordCount = 0;
        int DesignCount = 0;

        public int BmsDeviceCount = 0;

        public BusyIndicatorViewModel objBusyIndicator;
        int ResponceCount = 0;
        bool ProceedAhead = false;

        public bool RemoteSessionDisplayMessageOK = false;
        public string[] BLEDeviceName = new string[8];

        public PasswordConfirmationBox(List<DisplayDevice> ConnectedDevices, List<DisplayDevice> USBDeviceList = null)
        {
            try
            {
                InitializeComponent();

                ShowBusyindicatior(true);

                stackBusyIndicator.IsVisible = PasswordConfirmBoxSL.IsVisible = false;

                _ConnectedDevices = ConnectedDevices;

                for (int i = 0; i < IsValidDevicePassword.Length; i++)
                {
                    IsValidDevicePassword[i] = false;
                    CheckDefaultPasswordForDevice[i] = false;
                    bProceefForDevice[i] = true;
                }

                BmsDeviceCount = _ConnectedDevices.Count();
                Main();
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task Main()
        {
            try
            {
                await ShowBusyindicatior(true);

                if (_ConnectedDevices != null && BmsDeviceCount > 0)
                {
                    if (BmsDeviceCount > 1)
                    {
                        string NickName = string.Empty;
                        string MacAddress = string.Empty;
                        string ModelName = string.Empty;

                        await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.GetProtocolType, null);

                        ResponceCount = 0;
                        while (ResponceCount < 30)
                        {
                            await Task.Delay(1000);
                            if (objDeviceInformations != null)
                            {
                                break;
                            }
                            ResponceCount++;
                        }

                        for (int i = 0; i < BmsDeviceCount; i++)
                        {
                            string BLEDeviceName = Regex.Replace(Convert.ToString(_ConnectedDevices[i].DeviceName).ToLower().Trim(), "[^0-9a-zA-Z:,]+", "");

                            MacAddress = _ConnectedDevices[i].MacAddress;
                            ModelName = _ConnectedDevices[i].DeviceName;

                            NickName = Preferences.Get(MacAddress, string.Empty);

                            await InitializeDevice(i);

                            await UpdateBatteryLabel(i, NickName, ModelName);

                            await Task.Delay(1000);
                        }

                        bool NavigateSuccess = true;
                        ResponceCount = 0;
                        while (ResponceCount < 15)
                        {
                            await Task.Delay(1000);
                            ResponceCount++;
                        }

                        if (!Enumerable.Range(0, 8).All(i => bProceefForDevice[i]))
                        {
                            NavigateSuccess = false;
                        }

                        if (NavigateSuccess)
                        {
                            var frames = new[]
                            {
                                frameFirstBattery,frameSecondBattery,frameThirdBattery,frameFourthBattery,
                                frameFifthBattery,frameSixthBattery,frameSeventhBattery,frameEighthBattery
                            };

                            // hide all frames beyond available device count
                            for (int i = BmsDeviceCount; i < frames.Length; i++)
                            {
                                frames[i].IsVisible = false;
                            }

                            NavigationToSetupPage();
                        }
                        else
                        {
                            if (DesignCount > 0 && !RemoteSessionDetails._remote.IsConnected)
                            {
                                stackBusyIndicator.IsVisible = PasswordConfirmBoxSL.IsVisible = true;
                                if (DesignCount <= 2)
                                {
                                    grdPasswordConfirmBoxSL.HeightRequest = ((DesignCount * 150) + 300);
                                    grdMainBatteryPasswordstack.HeightRequest = (DesignCount * 150);
                                }
                                else
                                {
                                    grdPasswordConfirmBoxSL.HeightRequest = ((2 * 150) + 300);
                                    grdMainBatteryPasswordstack.HeightRequest = (DesignCount * 150);
                                }
                                await ShowBusyindicatior(false);
                            }
                        }
                    }
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
        protected override void OnAppearing()
        {
            base.OnAppearing();

            RemoteSessionDetails._remote = App.Services.GetService<RemoteSessionService>();
            RemoteSessionDetails._remote.OnReceive += PasswordPageReceive;
            Task.Run(() => CheckRemoteSessionPingStatus());
        }
        public async void PasswordPageReceive(string fromDevice, string Datatype, string jsondata)
        {
            try
            {
                if (Datatype == RemoteSessionReceiveDataTypes.byteCommand.ToString())
                {
                    var payload = Serializer.Deserialize<string>(jsondata).Split('^');
                    if (payload.Length < 2)
                        return;

                    var byteArr = Serializer.Deserialize<byte[]>(payload[0]);
                    var modelIndex = Serializer.Deserialize<int>(payload[1]);

                    Func<byte[], Task>[] dispatch =
                    {
                        FirstDeviceReceiveBytes,
                        SecoundDeviceReceiveBytes,
                        ThirdDeviceReceiveBytes,
                        FourthDeviceReceiveBytes,
                        FifthDeviceReceiveBytes,
                        SixthDeviceReceiveBytes,
                        SeventhDeviceReceiveBytes,
                        EighthDeviceReceiveBytes
                    };

                    if ((uint)modelIndex >= dispatch.Length) return;

                    await MainThread.InvokeOnMainThreadAsync(() => dispatch[modelIndex](byteArr));
                }
                else if (Datatype == RemoteSessionReceiveDataTypes.SuccessMultiPage.ToString())
                {
                    ProceedAhead = Serializer.Deserialize<bool>(jsondata);
                }
                else if (Datatype == RemoteSessionReceiveDataTypes.ProtocolType.ToString())
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
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async void NavigationToSetupPage()
        {
            try
            {
                if (frameFirstBattery.IsVisible == false && frameSecondBattery.IsVisible == false &&
                    frameThirdBattery.IsVisible == false && frameFourthBattery.IsVisible == false &&
                    frameFifthBattery.IsVisible == false && frameSixthBattery.IsVisible == false &&
                    frameSeventhBattery.IsVisible == false && frameEighthBattery.IsVisible == false)
                {
                    //await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ProceedForMultiView, null);

                    RemoteSessionDetails._remote.OnReceive -= PasswordPageReceive;

                    MultiDeviceSetupPage Page = new MultiDeviceSetupPage(_ConnectedDevices);
                    await Navigation.PushAsync(Page);

                    await ShowBusyindicatior(false);
                    await PopupNavigation.Instance.PopAllAsync(true);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task DefaultPasswordDesign(double width, double height)
        {
            try
            {
                if ((frameFirstBattery.IsVisible == true && frameSecondBattery.IsVisible == false) || (frameFirstBattery.IsVisible == false && frameSecondBattery.IsVisible == true))
                {
                    if (frameFirstBattery.IsVisible == true && frameSecondBattery.IsVisible == false)
                    {
                        Grid.SetColumn(frameFirstBattery, 0);
                        Grid.SetColumn(frameSecondBattery, 0);
                        Grid.SetColumnSpan(frameFirstBattery, 2);
                    }
                    else if (frameFirstBattery.IsVisible == false && frameSecondBattery.IsVisible == true)
                    {
                        Grid.SetColumn(frameFirstBattery, 0);
                        Grid.SetColumn(frameSecondBattery, 0);
                        Grid.SetColumnSpan(frameSecondBattery, 2);
                    }
                }

                if ((frameThirdBattery.IsVisible == true && frameFourthBattery.IsVisible == false) || (frameThirdBattery.IsVisible == false && frameFourthBattery.IsVisible == true))
                {
                    if (frameThirdBattery.IsVisible == true && frameFourthBattery.IsVisible == false)
                    {
                        Grid.SetColumn(frameThirdBattery, 0);
                        Grid.SetColumn(frameFourthBattery, 0);
                        Grid.SetColumnSpan(frameThirdBattery, 2);

                        if (Device.Idiom == TargetIdiom.Phone)
                        {
                            if ((frameFirstBattery.IsVisible == true && frameSecondBattery.IsVisible == false) || (frameFirstBattery.IsVisible == false && frameSecondBattery.IsVisible == true))
                            {
                                if (DesignCount == 2)
                                {
                                    Grid.SetRow(frameFirstBattery, 0);
                                    Grid.SetRow(frameSecondBattery, 0);
                                    Grid.SetRow(frameThirdBattery, 1);
                                    Grid.SetRow(frameFourthBattery, 0);
                                }
                            }
                            else if (frameFirstBattery.IsVisible == true && frameSecondBattery.IsVisible == true)
                            {
                                if (DesignCount == 3)
                                {
                                    if (width > height)
                                    {
                                        Grid.SetRow(frameFirstBattery, 0);
                                        Grid.SetRow(frameSecondBattery, 0);
                                        Grid.SetRow(frameThirdBattery, 1);
                                        Grid.SetRow(frameFourthBattery, 0);
                                    }
                                    else
                                    {
                                        Grid.SetRow(frameFirstBattery, 0);
                                        Grid.SetRow(frameSecondBattery, 1);
                                        Grid.SetRow(frameThirdBattery, 2);
                                        Grid.SetRow(frameFourthBattery, 0);
                                    }
                                }
                            }
                        }
                    }
                    else if (frameThirdBattery.IsVisible == false && frameFourthBattery.IsVisible == true)
                    {
                        Grid.SetColumn(frameThirdBattery, 0);
                        Grid.SetColumn(frameFourthBattery, 0);
                        Grid.SetColumnSpan(frameFourthBattery, 2);

                        if (Device.Idiom == TargetIdiom.Phone)
                        {
                            if ((frameFirstBattery.IsVisible == true && frameSecondBattery.IsVisible == false) || (frameFirstBattery.IsVisible == false && frameSecondBattery.IsVisible == true))
                            {
                                if (DesignCount == 2)
                                {
                                    Grid.SetRow(frameFirstBattery, 0);
                                    Grid.SetRow(frameSecondBattery, 0);
                                    Grid.SetRow(frameThirdBattery, 0);
                                    Grid.SetRow(frameFourthBattery, 1);
                                }
                            }
                            else if (frameFirstBattery.IsVisible == true && frameSecondBattery.IsVisible == true)
                            {
                                if (DesignCount == 3)
                                {
                                    if (width > height)
                                    {
                                        Grid.SetColumn(frameThirdBattery, 1);
                                        Grid.SetRow(frameFirstBattery, 0);
                                        Grid.SetRow(frameSecondBattery, 0);
                                        Grid.SetRow(frameThirdBattery, 0);
                                        Grid.SetRow(frameFourthBattery, 1);
                                    }
                                    else
                                    {
                                        Grid.SetRow(frameFirstBattery, 0);
                                        Grid.SetRow(frameSecondBattery, 1);
                                        Grid.SetRow(frameThirdBattery, 0);
                                        Grid.SetRow(frameFourthBattery, 2);
                                    }
                                }
                            }
                        }
                    }
                }

                if (frameFirstBattery.IsVisible == true && frameSecondBattery.IsVisible == true)
                {
                    if (height > width)
                    {
                        if (height > 1000)
                        {
                            grdMainBatteryPassword.ColumnDefinitions.Clear();
                            grdMainBatteryPassword.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                            grdMainBatteryPassword.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                            Grid.SetRow(frameFirstBattery, 0);
                            Grid.SetRow(frameSecondBattery, 0);

                            Grid.SetColumn(frameFirstBattery, 0);
                            Grid.SetColumn(frameSecondBattery, 1);
                            Grid.SetColumn(frameThirdBattery, 0);
                            Grid.SetColumn(frameFourthBattery, 1);
                        }
                        else
                        {
                            grdMainBatteryPassword.RowDefinitions.Clear();
                            grdMainBatteryPassword.RowDefinitions.Add(new RowDefinition { Height = 120 });
                            grdMainBatteryPassword.RowDefinitions.Add(new RowDefinition { Height = 120 });

                            grdMainBatteryPassword.ColumnDefinitions.Clear();
                            grdMainBatteryPassword.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                            Grid.SetRow(frameFirstBattery, 0);
                            Grid.SetRow(frameSecondBattery, 1);

                            Grid.SetColumn(frameFirstBattery, 0);
                            Grid.SetColumn(frameSecondBattery, 0);

                        }
                    }
                }

                if (frameThirdBattery.IsVisible == true && frameFourthBattery.IsVisible == true)
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        if ((frameFirstBattery.IsVisible == true && frameSecondBattery.IsVisible == false) || (frameFirstBattery.IsVisible == false && frameSecondBattery.IsVisible == true))
                        {
                            if (DesignCount == 3)
                            {
                                if (width > height)
                                {
                                    Grid.SetRow(frameFirstBattery, 0);
                                    Grid.SetRow(frameSecondBattery, 0);
                                    Grid.SetRow(frameThirdBattery, 0);
                                    Grid.SetRow(frameFourthBattery, 1);
                                }
                                else
                                {
                                    Grid.SetRow(frameFirstBattery, 0);
                                    Grid.SetRow(frameSecondBattery, 0);
                                    Grid.SetRow(frameThirdBattery, 1);
                                    Grid.SetRow(frameFourthBattery, 2);
                                }
                            }
                        }
                        if (frameFirstBattery.IsVisible == true && frameSecondBattery.IsVisible == true)
                        {
                            if (DesignCount == 3)
                            {
                                if (width > height)
                                {
                                    Grid.SetRow(frameFirstBattery, 0);
                                    Grid.SetRow(frameSecondBattery, 0);
                                    Grid.SetRow(frameThirdBattery, 1);
                                    Grid.SetRow(frameFourthBattery, 1);
                                }
                                else
                                {
                                    Grid.SetRow(frameFirstBattery, 0);
                                    Grid.SetRow(frameSecondBattery, 1);
                                    Grid.SetRow(frameThirdBattery, 2);
                                    Grid.SetRow(frameFourthBattery, 3);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        protected async override void OnSizeAllocated(double width, double height)
        {
            try
            {
                base.OnSizeAllocated(width, height);
                DeviceWidth = width;
                DeviceHeight = height;

                if (width > height)
                {
                    MainStack.Orientation = StackOrientation.Horizontal;

                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        clnPopupWidth.Width = (width / 100) * 75;

                        imgLock.IsVisible = false;
                        grdButtons.Margin = new Thickness(0, -10, 0, 0);

                        if (BmsDeviceCount > 2)
                        {
                            rowPopupHeight.Height = (height / 100) * 85;
                            scrlBox.IsEnabled = true;
                            scrlBox.VerticalScrollBarVisibility = ScrollBarVisibility.Always;
                        }
                        else
                        {
                            rowPopupHeight.Height = (height / 100) * 85;
                        }

                    }
                    else
                    {
                        clnPopupWidth.Width = (width / 100) * 80;

                        if (BmsDeviceCount > 2)
                        {
                            rowPopupHeight.Height = (height / 100) * ((3 * 20) + 18);
                        }
                        else
                        {
                            rowPopupHeight.Height = (height / 100) * ((2 * 20) + 18);
                        }

                        grdMainBorder.MinimumWidthRequest = grdMainBorder.WidthRequest = (width / 100) * 30;
                        brdMainBorder.MinimumWidthRequest = brdMainBorder.WidthRequest = (width / 100) * 30;
                        grdPasswordConfirmBoxSL.WidthRequest = ((width / 100) * 95);

                        PasswordConfirmBoxSL.WidthRequest = (width / 100) * 35;
                    }

                    Grid.SetRow(frameFirstBattery, 0);
                    Grid.SetRow(frameSecondBattery, 0);

                    Grid.SetRow(frameThirdBattery, 1);
                    Grid.SetRow(frameFourthBattery, 1);

                    Grid.SetColumn(frameFirstBattery, 0);
                    Grid.SetColumn(frameSecondBattery, 1);

                    Grid.SetColumn(frameThirdBattery, 0);
                    Grid.SetColumn(frameFourthBattery, 1);

                    Grid.SetColumnSpan(frameFirstBattery, 1);
                    Grid.SetColumnSpan(frameSecondBattery, 1);
                    Grid.SetColumnSpan(frameThirdBattery, 1);
                    Grid.SetColumnSpan(frameFourthBattery, 1);

                    if (BmsDeviceCount == 3)
                    {
                        Grid.SetColumnSpan(frameThirdBattery, 2);
                    }
                }
                else if (height > width)
                {
                    MainStack.Orientation = StackOrientation.Vertical;

                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        grdPasswordConfirmBoxSL.WidthRequest = ((width / 100) * 95);
                    }
                    else
                    {
                        if (width > 1000)
                        {
                            Grid.SetRow(frameFirstBattery, 0);
                            Grid.SetRow(frameSecondBattery, 0);

                            Grid.SetRow(frameThirdBattery, 1);
                            Grid.SetRow(frameFourthBattery, 1);

                            Grid.SetColumn(frameFirstBattery, 0);
                            Grid.SetColumn(frameSecondBattery, 1);

                            Grid.SetColumn(frameThirdBattery, 0);
                            Grid.SetColumn(frameFourthBattery, 1);

                            lblDevicePin.Margin = new Thickness(0, -20, 0, 0);
                        }
                        else if (width < 1000)
                        {
                            grdMainBatteryPassword.ColumnDefinitions.Clear();
                            grdMainBatteryPassword.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                            grdPasswordConfirmBoxSL.WidthRequest = ((width / 100) * 70);

                            Grid.SetRow(frameFirstBattery, 0);
                            Grid.SetColumn(frameFirstBattery, 0);

                            Grid.SetRow(frameSecondBattery, 1);
                            Grid.SetColumn(frameSecondBattery, 0);

                            Grid.SetRow(frameThirdBattery, 2);
                            Grid.SetColumn(frameThirdBattery, 0);

                            Grid.SetRow(frameFourthBattery, 3);
                            Grid.SetColumn(frameFourthBattery, 0);

                            lblDevicePin.Margin = new Thickness(0, -10, 0, 0);

                        }

                        clnPopupWidth.Width = (width / 100) * 90;

                        if (BmsDeviceCount > 2)
                        {
                            rowPopupHeight.Height = (height / 100) * ((3 * 15) + 18);
                        }
                        else
                        {
                            rowPopupHeight.Height = (height / 100) * ((2 * 20) + 18);
                        }

                        if (BmsDeviceCount > 2)
                        {
                            rowThiredHeight.Height = 0;
                            rowFourthHeight.Height = 0;
                        }
                        else
                        {
                            rowSecoundHeight.Height = 0;
                            rowThiredHeight.Height = 0;
                            rowFourthHeight.Height = 0;
                        }

                        grdMainBorder.MinimumWidthRequest = grdMainBorder.WidthRequest = (width / 100) * 45;
                        brdMainBorder.MinimumWidthRequest = brdMainBorder.WidthRequest = (width / 100) * 45;
                        grdPasswordConfirmBoxSL.WidthRequest = ((width / 100) * 95);

                        if (BmsDeviceCount == 3)
                        {
                            Grid.SetColumnSpan(frameThirdBattery, 2);
                        }

                        PasswordConfirmBoxSL.WidthRequest = (width / 100) * 50;
                    }

                    if (DesignCount <= 2)
                    {
                        grdPasswordConfirmBoxSL.HeightRequest = ((DesignCount * 150) + 250);
                        grdMainBatteryPasswordstack.HeightRequest = (DesignCount * 140);
                    }
                    else
                    {
                        grdPasswordConfirmBoxSL.HeightRequest = ((2 * 150) + 250);
                        grdMainBatteryPasswordstack.HeightRequest = (DesignCount * 140);
                    }
                }

                if (BmsDeviceCount > 1)
                {
                    await DefaultPasswordDesign(width, height);
                }

                if (stkDisplayMessagePopUp.IsVisible)
                {
                    await ShowDisplayPopupDesign(width, height);
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
        private async void btnQuit_Clicked(object sender, EventArgs e)
        {
            try
            {
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DiscounnectDevice, null);
                await PopupNavigation.Instance.PopAllAsync(true);
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async void btnSubmit_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                PasswordConfirmBoxSL.IsVisible = false;

                if (BmsDeviceCount > 1)
                {
                    for (int i = 0; i < BmsDeviceCount; i++)
                    {
                        if (i == 0 && frameFirstBattery.IsVisible == true)
                        {
                            await ConfirmUserPassword(txtFirstBattery.PINValue, i);
                            await Task.Delay(1000);
                        }
                        else if (i == 1 && frameSecondBattery.IsVisible == true)
                        {
                            await ConfirmUserPassword(txtSecondBattery.PINValue, i);
                            await Task.Delay(1000);
                        }
                        else if (i == 2 && frameThirdBattery.IsVisible == true)
                        {
                            await ConfirmUserPassword(txtThirdBattery.PINValue, i);
                            await Task.Delay(1000);
                        }
                        else if (i == 3 && frameFourthBattery.IsVisible == true)
                        {
                            await ConfirmUserPassword(txtFourthBattery.PINValue, i);
                            await Task.Delay(1000);
                        }
                        else if (i == 4 && frameFifthBattery.IsVisible == true)
                        {
                            await ConfirmUserPassword(txtFifthBattery.PINValue, i);
                            await Task.Delay(1000);
                        }
                        else if (i == 5 && frameSixthBattery.IsVisible == true)
                        {
                            await ConfirmUserPassword(txtSixthBattery.PINValue, i);
                            await Task.Delay(1000);
                        }
                        else if (i == 6 && frameSeventhBattery.IsVisible == true)
                        {
                            await ConfirmUserPassword(txtSeventhBattery.PINValue, i);
                            await Task.Delay(1000);
                        }
                        else if (i == 7 && frameEighthBattery.IsVisible == true)
                        {
                            await ConfirmUserPassword(txtEighthBattery.PINValue, i);
                            await Task.Delay(1000);
                        }
                    }

                    int validCount = 0;

                    if (frameFirstBattery.IsVisible == true && !IsValidDevicePassword[0])
                    {
                        await ShowBusyindicatior(false);
                        lblErrorMassage.IsVisible = true;
                        PasswordConfirmBoxSL.IsVisible = true;
                        txtFirstBattery.Color = Colors.Red;
                    }
                    else if (IsValidDevicePassword[0])
                    {
                        validCount++;
                        txtFirstBattery.Color = Colors.Black;
                    }

                    if (frameSecondBattery.IsVisible == true && !IsValidDevicePassword[1])
                    {
                        await ShowBusyindicatior(false);
                        lblErrorMassage.IsVisible = true;
                        PasswordConfirmBoxSL.IsVisible = true;
                        txtSecondBattery.Color = Colors.Red;
                    }
                    else if (IsValidDevicePassword[1] && BmsDeviceCount >= 2)
                    {
                        validCount++;
                        txtSecondBattery.Color = Colors.Black;
                    }

                    if (BmsDeviceCount >= 3)
                    {
                        if (frameThirdBattery.IsVisible == true && !IsValidDevicePassword[2])
                        {
                            await ShowBusyindicatior(false);
                            lblErrorMassage.IsVisible = true;
                            PasswordConfirmBoxSL.IsVisible = true;
                            txtThirdBattery.Color = Colors.Red;
                        }
                        else if (IsValidDevicePassword[2])
                        {
                            validCount++;
                            txtThirdBattery.Color = Colors.Black;
                        }

                        if (BmsDeviceCount >= 4)
                        {
                            if (frameFourthBattery.IsVisible == true && !IsValidDevicePassword[3])
                            {
                                await ShowBusyindicatior(false);
                                lblErrorMassage.IsVisible = true;
                                PasswordConfirmBoxSL.IsVisible = true;
                                txtFourthBattery.Color = Colors.Red;
                            }
                            else if (IsValidDevicePassword[3])
                            {
                                validCount++;
                                txtFourthBattery.Color = Colors.Black;
                            }

                            if (BmsDeviceCount >= 5)
                            {
                                if (frameFifthBattery.IsVisible == true && !IsValidDevicePassword[4])
                                {
                                    await ShowBusyindicatior(false);
                                    lblErrorMassage.IsVisible = true;
                                    PasswordConfirmBoxSL.IsVisible = true;
                                    txtFifthBattery.Color = Colors.Red;
                                }
                                else if (IsValidDevicePassword[4])
                                {
                                    validCount++;
                                    txtFifthBattery.Color = Colors.Black;
                                }

                                if (BmsDeviceCount >= 6)
                                {
                                    if (frameSixthBattery.IsVisible == true && !IsValidDevicePassword[5])
                                    {
                                        await ShowBusyindicatior(false);
                                        lblErrorMassage.IsVisible = true;
                                        PasswordConfirmBoxSL.IsVisible = true;
                                        txtSixthBattery.Color = Colors.Red;
                                    }
                                    else if (IsValidDevicePassword[5])
                                    {
                                        validCount++;
                                        txtSixthBattery.Color = Colors.Black;
                                    }

                                    if (BmsDeviceCount >= 7)
                                    {
                                        if (frameSeventhBattery.IsVisible == true && !IsValidDevicePassword[6])
                                        {
                                            await ShowBusyindicatior(false);
                                            lblErrorMassage.IsVisible = true;
                                            PasswordConfirmBoxSL.IsVisible = true;
                                            txtSeventhBattery.Color = Colors.Red;
                                        }
                                        else if (IsValidDevicePassword[6])
                                        {
                                            validCount++;
                                            txtSeventhBattery.Color = Colors.Black;
                                        }

                                        if (BmsDeviceCount == 8)
                                        {
                                            if (frameEighthBattery.IsVisible == true && !IsValidDevicePassword[7])
                                            {
                                                await ShowBusyindicatior(false);
                                                lblErrorMassage.IsVisible = true;
                                                PasswordConfirmBoxSL.IsVisible = true;
                                                txtEighthBattery.Color = Colors.Red;
                                            }
                                            else if (IsValidDevicePassword[7])
                                            {
                                                validCount++;
                                                txtEighthBattery.Color = Colors.Black;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (validCount >= DesignCount)
                    {
                        if (chkRememberSettings.IsChecked == true)
                        {
                            for (int i = 0; i < BmsDeviceCount; i++)
                            {
                                string MacAddress = _ConnectedDevices[i].MacAddress;
                                MacAddress = MacAddress + "_Password";

                                string Password = string.Empty;
                                var pins = new[]
                                {
                                    txtFirstBattery.PINValue,txtSecondBattery.PINValue,txtThirdBattery.PINValue,txtFourthBattery.PINValue,
                                    txtFifthBattery.PINValue,txtSixthBattery.PINValue,txtSeventhBattery.PINValue,txtEighthBattery.PINValue
                                };

                                Password = Convert.ToString(pins[i]);
                                if (string.IsNullOrWhiteSpace(Password))
                                {
                                    Password = "012345";
                                }

                                Preferences.Set(MacAddress, Password);
                            }
                        }

                        await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ProceedForMultiView, null);

                        ProceedAhead = false;
                        while (true)
                        {
                            if (ProceedAhead)
                            {
                                RemoteSessionDetails._remote.OnReceive -= PasswordPageReceive;

                                MultiDeviceSetupPage Page = new MultiDeviceSetupPage(_ConnectedDevices);
                                await Navigation.PushAsync(Page);

                                await ShowBusyindicatior(false);
                                await PopupNavigation.Instance.PopAllAsync(true);
                                break;
                            }
                            await Task.Delay(1000);
                        }
                    }
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
        public async Task SendCommandToBMS(byte[] SendCommand, int ModelIndex = 0)
        {
            try
            {
                string data = Serializer.Serialize(SendCommand);
                data += "^" + ModelIndex;
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.byteCommand, data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task SendWriteOrReadStartCommandToBMS(int ModelIndex)
        {
            try
            {
                // var Start = new byte[] { 0xdd, 0x5a, 0x00, 0x02, 0x56, 0x78, 0xff, 0x30, 0x77 };
                var Start = objProtocols[ModelIndex].StartandEndCommand("start");
                string data = Serializer.Serialize(Start);
                data += "^" + ModelIndex;
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.byteCommand, data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task SendWriteOrReadEndCommandToBMS(int ModelIndex)
        {
            try
            {
                // var End = new byte[] { 0xdd, 0x5a, 0x01, 0x02, 0x00, 0x00, 0xff, 0xfd, 0x77 };                
                var End = objProtocols[ModelIndex].StartandEndCommand("end");
                string data = Serializer.Serialize(End);
                data += "^" + ModelIndex;
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.byteCommand, data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task UpdateBatteryLabel(int i, string nickName, string modelName)
        {
            Label label = i switch
            {
                0 => lblFirstBattery,
                1 => lblSecondBattery,
                2 => lblThirdBattery,
                3 => lblFourthBattery,
                4 => lblFifthBattery,
                5 => lblSixthBattery,
                6 => lblSeventhBattery,
                7 => lblEighthBattery,
                _ => null
            };

            if (label == null) return;

            label.Text = !string.IsNullOrWhiteSpace(nickName) ? $"{nickName} ({modelName})" : modelName;
        }
        private async void scrlBox_Scrolled(object sender, ScrolledEventArgs e)
        {
            try
            {
                string data = "Box" + '^' + Convert.ToString(e.ScrollX) + '^' + Convert.ToString(e.ScrollY);
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ScrollEvent, data);
            }
            catch (Exception ex) { }
        }

        #region Device Connection

        private async Task InitializeDevice(int i)
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS(i);

                await Task.Delay(500);

                await btnCellCount_Clicked(i);

                await Task.Delay(500);

                await btnThreeCommand_Clicked(i);

                await Task.Delay(500);

                await ClearPINCommand(i);

                await Task.Delay(500);

                await CheckForDefaultPassword(i);

                await Task.Delay(500);

                await SendWriteOrReadEndCommandToBMS(i);

                if (nWriteCounter >= 10)
                {
                    await ShowDisplayPopup("Error", "Unable to connect.");
                    MainPage Page = new MainPage(false);
                    await Navigation.PushAsync(Page);
                }
            }
            catch (Exception)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task FirstDeviceReceiveBytes(byte[] receivedBytes)
        {
            try
            {
                if (receivedBytes != null && ((byte[])receivedBytes).Length > 1)
                {
                    if (((byte[])receivedBytes)[0].ToString() == "221" && ((((byte[])receivedBytes)[1].ToString() == "06") || (((byte[])receivedBytes)[1].ToString() == "6")))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();
                        if (sResult.StartsWith("dd060000000077"))
                        {
                            frameFirstBattery.IsVisible = false;
                            IsValidDevicePassword[0] = true;
                        }
                        else
                        {
                            if (CheckDefaultPasswordForDevice[0])
                            {
                                frameFirstBattery.IsVisible = true;
                                IsValidDevicePassword[0] = false;
                                if (bProceefForDevice[0])
                                {
                                    DesignCount++;
                                    bProceefForDevice[0] = false;
                                }
                            }
                            else
                            {
                                frameFirstBattery.IsVisible = true;
                                IsValidDevicePassword[0] = false;
                                if (bProceefForDevice[0])
                                {
                                    DesignCount++;
                                    bProceefForDevice[0] = false;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "162"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            BarcodeValue = string.Empty;
                            for (int i = 5; i < ((byte[])receivedBytes).Length - 3; i++)
                            {
                                BarcodeValue += Convert.ToChar(((byte[])receivedBytes)[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task SecoundDeviceReceiveBytes(byte[] receivedBytes)
        {
            try
            {
                if (receivedBytes != null && ((byte[])receivedBytes).Length > 1)
                {
                    if (((byte[])receivedBytes)[0].ToString() == "221" && ((((byte[])receivedBytes)[1].ToString() == "06") || (((byte[])receivedBytes)[1].ToString() == "6")))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();
                        if (sResult.StartsWith("dd060000000077"))
                        {
                            frameSecondBattery.IsVisible = false;
                            IsValidDevicePassword[1] = true;
                        }
                        else
                        {
                            if (CheckDefaultPasswordForDevice[1])
                            {
                                frameSecondBattery.IsVisible = true;
                                IsValidDevicePassword[1] = false;
                                if (bProceefForDevice[1])
                                {
                                    DesignCount++;
                                    bProceefForDevice[1] = false;
                                }
                            }
                            else
                            {
                                frameSecondBattery.IsVisible = true;
                                IsValidDevicePassword[1] = false;
                                if (bProceefForDevice[1])
                                {
                                    DesignCount++;
                                    bProceefForDevice[1] = false;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "162"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            BarcodeValue = string.Empty;
                            for (int i = 5; i < ((byte[])receivedBytes).Length - 3; i++)
                            {
                                BarcodeValue += Convert.ToChar(((byte[])receivedBytes)[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task ThirdDeviceReceiveBytes(byte[] receivedBytes)
        {
            try
            {
                if (receivedBytes != null && ((byte[])receivedBytes).Length > 1)
                {
                    if (((byte[])receivedBytes)[0].ToString() == "221" && ((((byte[])receivedBytes)[1].ToString() == "06") || (((byte[])receivedBytes)[1].ToString() == "6")))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();
                        if (sResult.StartsWith("dd060000000077"))
                        {
                            frameThirdBattery.IsVisible = false;
                            IsValidDevicePassword[2] = true;
                        }
                        else
                        {
                            if (CheckDefaultPasswordForDevice[2])
                            {
                                frameThirdBattery.IsVisible = true;
                                IsValidDevicePassword[2] = false;
                                if (bProceefForDevice[2])
                                {
                                    DesignCount++;
                                    bProceefForDevice[2] = false;
                                }
                            }
                            else
                            {
                                frameThirdBattery.IsVisible = true;
                                IsValidDevicePassword[2] = false;
                                if (bProceefForDevice[2])
                                {
                                    DesignCount++;
                                    bProceefForDevice[2] = false;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "162"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            BarcodeValue = string.Empty;
                            for (int i = 5; i < ((byte[])receivedBytes).Length - 3; i++)
                            {
                                BarcodeValue += Convert.ToChar(((byte[])receivedBytes)[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task FourthDeviceReceiveBytes(byte[] receivedBytes)
        {
            try
            {
                if (receivedBytes != null && ((byte[])receivedBytes).Length > 1)
                {
                    if (((byte[])receivedBytes)[0].ToString() == "221" && ((((byte[])receivedBytes)[1].ToString() == "06") || (((byte[])receivedBytes)[1].ToString() == "6")))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();
                        if (sResult.StartsWith("dd060000000077"))
                        {
                            frameFourthBattery.IsVisible = false;
                            IsValidDevicePassword[3] = true;
                        }
                        else
                        {
                            if (CheckDefaultPasswordForDevice[3])
                            {
                                frameFourthBattery.IsVisible = true;
                                IsValidDevicePassword[3] = false;
                                if (bProceefForDevice[3])
                                {
                                    DesignCount++;
                                    bProceefForDevice[3] = false;
                                }
                            }
                            else
                            {
                                frameFourthBattery.IsVisible = true;
                                IsValidDevicePassword[3] = false;
                                if (bProceefForDevice[3])
                                {
                                    DesignCount++;
                                    bProceefForDevice[3] = false;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "162"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            BarcodeValue = string.Empty;
                            for (int i = 5; i < ((byte[])receivedBytes).Length - 3; i++)
                            {
                                BarcodeValue += Convert.ToChar(((byte[])receivedBytes)[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task FifthDeviceReceiveBytes(byte[] receivedBytes)
        {
            try
            {
                if (receivedBytes != null && ((byte[])receivedBytes).Length > 1)
                {
                    if (((byte[])receivedBytes)[0].ToString() == "221" && ((((byte[])receivedBytes)[1].ToString() == "06") || (((byte[])receivedBytes)[1].ToString() == "6")))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();
                        if (sResult.StartsWith("dd060000000077"))
                        {
                            frameFifthBattery.IsVisible = false;
                            IsValidDevicePassword[4] = true;
                        }
                        else
                        {
                            if (CheckDefaultPasswordForDevice[4])
                            {
                                frameFifthBattery.IsVisible = true;
                                IsValidDevicePassword[4] = false;
                                if (bProceefForDevice[4])
                                {
                                    DesignCount++;
                                    bProceefForDevice[4] = false;
                                }
                            }
                            else
                            {
                                frameFifthBattery.IsVisible = true;
                                IsValidDevicePassword[4] = false;
                                if (bProceefForDevice[4])
                                {
                                    DesignCount++;
                                    bProceefForDevice[4] = false;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "162"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            BarcodeValue = string.Empty;
                            for (int i = 5; i < ((byte[])receivedBytes).Length - 3; i++)
                            {
                                BarcodeValue += Convert.ToChar(((byte[])receivedBytes)[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task SixthDeviceReceiveBytes(byte[] receivedBytes)
        {
            try
            {
                if (receivedBytes != null && ((byte[])receivedBytes).Length > 1)
                {
                    if (((byte[])receivedBytes)[0].ToString() == "221" && ((((byte[])receivedBytes)[1].ToString() == "06") || (((byte[])receivedBytes)[1].ToString() == "6")))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();
                        if (sResult.StartsWith("dd060000000077"))
                        {
                            frameSixthBattery.IsVisible = false;
                            IsValidDevicePassword[5] = true;
                        }
                        else
                        {
                            if (CheckDefaultPasswordForDevice[6])
                            {
                                frameSixthBattery.IsVisible = true;
                                IsValidDevicePassword[6] = false;
                                if (bProceefForDevice[6])
                                {
                                    DesignCount++;
                                    bProceefForDevice[6] = false;
                                }
                            }
                            else
                            {
                                frameSixthBattery.IsVisible = true;
                                IsValidDevicePassword[6] = false;
                                if (bProceefForDevice[6])
                                {
                                    DesignCount++;
                                    bProceefForDevice[6] = false;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "162"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            BarcodeValue = string.Empty;
                            for (int i = 5; i < ((byte[])receivedBytes).Length - 3; i++)
                            {
                                BarcodeValue += Convert.ToChar(((byte[])receivedBytes)[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task SeventhDeviceReceiveBytes(byte[] receivedBytes)
        {
            try
            {
                if (receivedBytes != null && ((byte[])receivedBytes).Length > 1)
                {
                    if (((byte[])receivedBytes)[0].ToString() == "221" && ((((byte[])receivedBytes)[1].ToString() == "06") || (((byte[])receivedBytes)[1].ToString() == "6")))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();
                        if (sResult.StartsWith("dd060000000077"))
                        {
                            frameSeventhBattery.IsVisible = false;
                            IsValidDevicePassword[6] = true;
                        }
                        else
                        {
                            if (CheckDefaultPasswordForDevice[6])
                            {
                                frameSeventhBattery.IsVisible = true;
                                IsValidDevicePassword[6] = false;
                                if (bProceefForDevice[6])
                                {
                                    DesignCount++;
                                    bProceefForDevice[6] = false;
                                }
                            }
                            else
                            {
                                frameSeventhBattery.IsVisible = true;
                                IsValidDevicePassword[6] = false;
                                if (bProceefForDevice[6])
                                {
                                    DesignCount++;
                                    bProceefForDevice[6] = false;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "162"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            BarcodeValue = string.Empty;
                            for (int i = 5; i < ((byte[])receivedBytes).Length - 3; i++)
                            {
                                BarcodeValue += Convert.ToChar(((byte[])receivedBytes)[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task EighthDeviceReceiveBytes(byte[] receivedBytes)
        {
            try
            {
                if (receivedBytes != null && ((byte[])receivedBytes).Length > 1)
                {
                    if (((byte[])receivedBytes)[0].ToString() == "221" && ((((byte[])receivedBytes)[1].ToString() == "06") || (((byte[])receivedBytes)[1].ToString() == "6")))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();
                        if (sResult.StartsWith("dd060000000077"))
                        {
                            frameEighthBattery.IsVisible = false;
                            IsValidDevicePassword[7] = true;
                        }
                        else
                        {
                            if (CheckDefaultPasswordForDevice[7])
                            {
                                frameEighthBattery.IsVisible = true;
                                IsValidDevicePassword[7] = false;
                                if (bProceefForDevice[7])
                                {
                                    DesignCount++;
                                    bProceefForDevice[7] = false;
                                }
                            }
                            else
                            {
                                frameEighthBattery.IsVisible = true;
                                IsValidDevicePassword[7] = false;
                                if (bProceefForDevice[7])
                                {
                                    DesignCount++;
                                    bProceefForDevice[7] = false;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "162"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            BarcodeValue = string.Empty;
                            for (int i = 5; i < ((byte[])receivedBytes).Length - 3; i++)
                            {
                                BarcodeValue += Convert.ToChar(((byte[])receivedBytes)[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task btnCellCount_Clicked(int ModelIndex)
        {
            try
            {
                // await SendCommandToBMS(new byte[] { 0xdd, 0xa5, 0x04, 0x00, 0xff, 0xfc, 0x77 }, ModelIndex);
                var bytes2 = objProtocols[ModelIndex].CreateReadCommand(RegisterEnum.REG_CELL);
                await SendCommandToBMS(bytes2, ModelIndex);
            }
            catch (Exception ex)
            {

                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task btnThreeCommand_Clicked(int ModelIndex)
        {
            try
            {
                // await SendCommandToBMS(new byte[] { 0xdd, 0xa5, 0x03, 0x00, 0xff, 0xfd, 0x77 }, ModelIndex);
                var bytes2 = objProtocols[ModelIndex].CreateReadCommand(RegisterEnum.REG_GENERAL);
                await SendCommandToBMS(bytes2, ModelIndex);
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task ConfirmUserPassword(string Password, int ModelIndex)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    if (CheckValidPassword(Password))
                    {
                        var SubmitPassword = objProtocols[ModelIndex].PrepareCustomerPassword(Password.Trim());

                        await SendCommandToBMS(SubmitPassword, ModelIndex);

                        CheckDefaultPasswordForDevice[ModelIndex] = false;

                        PasswordCheck06CalledFrom_Please_Enter_Password = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        static bool CheckValidPassword(string number)
        {
            bool a = false;
            if (number.Length > 0 && number.Length == 6 && number.All(Char.IsDigit))
            {
                a = true;
            }
            return a;
        }
        public async Task CheckForDefaultPassword(int ModelIndex)
        {
            try
            {
                // var DefaultPassword = new byte[] { 0xdd, 0x5a, 0x06, 0x07, 0x06, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0xff, 0xde, 0x77 };
                var DefaultPassword = objProtocols[ModelIndex].PrepareCustomerPassword("012345");
                await SendCommandToBMS(DefaultPassword, ModelIndex);

                string strPassword = string.Empty;
                if (!IsValidDevicePassword[ModelIndex])
                {
                    string ConnectedDeviceMacAddress = _ConnectedDevices[ModelIndex].MacAddress;

                    strPassword = Preferences.Get(ConnectedDeviceMacAddress + "_Password", string.Empty);
                }

                CheckDefaultPasswordForDevice[ModelIndex] = true;
                passwordCount++;

                if (!string.IsNullOrWhiteSpace(strPassword))
                {
                    await ConfirmUserPassword(strPassword, ModelIndex);
                    await Task.Delay(1000);
                    if (IsValidDevicePassword[ModelIndex])
                    {
                        DesignCount--;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public async Task ClearPINCommand(int ModelIndex)
        {
            try
            {
                var SetPassword = objProtocols[ModelIndex].PrepareDefaultPin09();
                await SendCommandToBMS(SetPassword, ModelIndex);
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }

        #endregion Device Connection        

        #region BusyIndecator And Reconnect Functionality
        private async Task ShowBusyindicatior(bool bShowIndicator, bool IsChekingpin = false)
        {
            try
            {
                if (!IsChekingpin)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        IsBusyIndicator.IsVisible = bShowIndicator;
                        IsBusyIndicator.IsEnabled = bShowIndicator;
                        sfbIsBusyIndicator.IsVisible = bShowIndicator;
                        lblBusyIndicator.Text = "Connecting..";

                        stackBusy.IsVisible = bShowIndicator;
                    });
                    await Task.Delay(100);
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        IsBusyIndicator.IsVisible = bShowIndicator;
                        IsBusyIndicator.IsEnabled = bShowIndicator;
                        sfbIsBusyIndicator.IsVisible = bShowIndicator;
                        lblBusyIndicator.Text = "Checking PIN...";

                        stackBusy.IsVisible = bShowIndicator;
                    });
                    await Task.Delay(100);
                }
            }
            catch { }
        }
        private async Task ShowDisplayPopup(string Status, string Message)
        {
            try
            {
                stkDisplayMessagePopUp.IsVisible = true;
                lblText.TextColor = Color.FromHex("#2D2D2D");
                stackCancelAndYesButton.IsVisible = false;
                btnOK.IsVisible = true;
                btnOK.BackgroundColor = Color.FromHex("#39B54A");
                btnOK.StrokeThickness = 0;
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
                else if (Status == "Location Permission")
                {
                    imgDisplay.Source = "location.png";
                    lblText.Text = Message;
                }

                await ShowDisplayPopupDesign(DeviceWidth, DeviceHeight);

            }
            catch (Exception ex)
            {

            }
        }
        private void DisplayPopup_Clicked(object sender, EventArgs e)
        {
            stkDisplayMessagePopUp.IsVisible = false;
        }
        private async void DisplayPopupYes_Clicked(object sender, EventArgs e)
        {
            stkDisplayMessagePopUp.IsVisible = false;
        }
        private async Task ShowDisplayPopupDesign(double width, double height)
        {
            try
            {
                scrPopupText.IsEnabled = false;
                if (width > height) //landscape
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        sbbtn1.Padding = new Thickness(5, 0, 5, 0);
                        sbbtn2.Padding = new Thickness(5, 0, 5, 0);
                        stkDisplayMessagePopUp.HeightRequest = (height / 100) * 80;
                        stkDisplayMessagePopUp.WidthRequest = (width / 100) * 50;

                        if (PopUpStatus == "Location Permission")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 95;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 85;
                        }
                    }
                    else
                    {
                        stkDisplayMessagePopUp.HeightRequest = (height / 100) * 50;
                        stkDisplayMessagePopUp.WidthRequest = (width / 100) * 50;

                        if (PopUpStatus == "Location Permission")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 60;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 65;
                        }
                    }
                }
                else //potrait
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        sbbtn1.Padding = new Thickness(5, 0, 5, 0);
                        sbbtn2.Padding = new Thickness(5, 0, 5, 0);

                        stkDisplayMessagePopUp.HeightRequest = (height / 100) * 30;
                        stkDisplayMessagePopUp.WidthRequest = (width / 100) * 65;

                        if (PopUpStatus == "Location Permission")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 60;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 85;
                        }
                    }
                    else
                    {
                        stkDisplayMessagePopUp.HeightRequest = (height / 100) * 35;
                        stkDisplayMessagePopUp.WidthRequest = (width / 100) * 70;

                        if (PopUpStatus == "Location Permission")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 60;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 80;
                        }
                    }
                }

            }
            catch (Exception ex) { }
        }
        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (PopUpStatus == "Location Permission")
                {
                    await ShowBusyindicatior(false);
                    stackBusyIndicator.IsVisible = false;
                    await PopupNavigation.Instance.PopAllAsync(true);
                }
                else
                {
                    stkDisplayMessagePopUp.IsVisible = false;
                }

                if (RemoteSessionDisplayMessageOK)
                {
                    await ShowBusyindicatior(true);
                    MainPage Page = new MainPage(false);
                    await Navigation.PushAsync(Page);
                }
            }
            catch (Exception ex) { }
        }

        #endregion BusyIndecator And Reconnect Functionality        

        #region Remote Session       
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

                        if (currentPage is PasswordConfirmationBox)
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
