using Android.Gms.Common.Apis;
using CommunityToolkit.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.Maui.Audio;
using RGPopup.Maui.Services;
using SFKBle.Models;
using SFKBle_Admin.Platforms.Android;
using SFKBle_Admin.SFK_Protocol;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SFKBle_Admin
{
    public partial class MainPage : ContentPage
    {
        #region Variable Declaration

        string sPassword = string.Empty;
        private DisplayDevice CurrentSelectedDevice = new DisplayDevice();
        private List<DisplayDevice> SelectedDevice = new List<DisplayDevice>();
        private List<DisplayDevice> VSelectedDevice = new List<DisplayDevice>();
        private List<DisplayDevice> BSelectedDevice = new List<DisplayDevice>();

        public List<DisplayDevice> BluetoothDeviceList = new List<DisplayDevice>();
        public List<DisplayDevice> VirtualDeviceList = new List<DisplayDevice>();
        public List<DisplayDevice> ViewAllDevicesList = new List<DisplayDevice>();

        private bool IsVirtualBattery = false;
        private bool ProceedForPermission = false;
        private double Designwidth = 0;
        private double Designheight = 0;
        private double RemoveHeight = 0;
        private double RemoveHeightLandscap = 0;
        private string PopUpStatus = null;
        bool bProccedVConnection = true;
        bool ProceedForconnect = false;
        bool ErrorOccureWhileConnecting = false;
        int EightsCount = 0;
        bool ViewAllDevice = false;
        DevicePlatform platform = DeviceInfo.Platform;
        Version version = DeviceInfo.Version;
        Version targetVersion = new Version(12, 0);

        public bool PasswordCheck06CalledFrom_Please_Enter_Password = false;
        public bool PasswordCheck06CalledFrom_Reset_Password = false;
        public bool PasswordCheck06CalledFrom_IntialLoad = false;
        public string ConnectedDeviceMacAddress = string.Empty;
        public bool PasswordEnterSucces = false;
        public bool PasswordProcessCancel = false;
        public string BarcodeValue = string.Empty;

        public bool CalledPasswordConfirmationBox = false;
        int iPassCount = 1;

        DeviceInformation objDeviceInformation = new DeviceInformation();
        Regex cleanRegex = new Regex("[^0-9a-zA-Z:,]+", RegexOptions.Compiled);
        IProtocol objProtocol;

        public bool bIsSendNExt = false;
        public bool isProccesBegin = false;
        public bool CalibrationUpdateInProgress = false;
        public bool ProceedForLoadDefaultSettings = false;
        public string strDefaultValues = string.Empty;
        public int iReadRegister = 0;
        public bool IsSFKH2Device = false;
        public bool bPasswordRest = false;

        string RestoreMacAddress = string.Empty;

        bool IsDeviceConnected = false;

        int ResponceCount = 0;

        bool IsErrorOccured = false;
        bool ProceedAhead = false;
        public bool RemoteSessionDisplayMessageOK = false;
        bool IsNotificationMute = false;
        private readonly IAudioManager _audioManager;
        private IAudioPlayer _player;
        public static MainPage MainPageInstance { get; private set; }
        public bool bHasPassword = false;

        #endregion Variable Declaration

        #region Main Code
        public MainPage(bool isProcced = true)
        {
            try
            {
                InitializeComponent();
                MainPageInstance = this;
                _audioManager = Plugin.Maui.Audio.AudioManager.Current;

                Initialization(isProcced);
            }
            catch (Exception ex)
            {
                ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        public async void Initialization(bool isProcced)
        {
            await SetBusyState(true, "Please Wait...");

            ResetValues();

            ProceedForconnect = false;

            if (string.IsNullOrWhiteSpace(RemoteSessionDetails.RemoteSessionDeviceGUID))
            {
                ServiceHelper.StopService();

                await Task.Delay(1000);

                ServiceHelper.StartService();
            }
            await SetBusyState(false);
        }
        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            try
            {
                await SetBusyState(true, "Scanning...");

                await ResetValues();

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.Scan, null);

                ResponceCount = 0;
                while (ResponceCount < 30)
                {
                    await Task.Delay(1000);
                    if (BluetoothDeviceList != null && BluetoothDeviceList.Count > 0)
                    {
                        break;
                    }
                    ResponceCount++;
                }

                if (BluetoothDeviceList != null && BluetoothDeviceList.Count > 0)
                {
                    lstDeviceList.ItemsSource = BluetoothDeviceList.ToArray();

                    lstDeviceList.IsVisible = true;
                    lblNoDeviceListFound.IsVisible = false;
                }
                else
                {
                    lstDeviceList.IsVisible = false;
                    lblNoDeviceListFound.IsVisible = true;
                }

                if (VirtualDeviceList != null && VirtualDeviceList.Count > 0)
                {
                    lstVirtualBatteryList.ItemsSource = VirtualDeviceList.ToArray();

                    lstVirtualBatteryList.IsVisible = true;
                    lblNoVirtualBatteryFound.IsVisible = false;
                }
                else
                {
                    lstVirtualBatteryList.IsVisible = false;
                    lblNoVirtualBatteryFound.IsVisible = true;
                }

                if (VirtualDeviceList.Count <= 0 && BluetoothDeviceList.Count <= 0)
                {
                    stackAvailableDataInner.IsVisible = false;
                }
                else
                {
                    stackAvailableDataInner.IsVisible = true;

                    brdDevieListTab.BackgroundColor = Color.FromHex("#F0C000");
                    brdVirtualBatteryListTab.BackgroundColor = Color.FromHex("#E3E3E3");

                    stackDeviceList.IsVisible = true;
                    stackVirtualBatteryList.IsVisible = false;
                }

                await DesignChanges(Designwidth, Designheight);

                await SetBusyState(false);

                lblPrivacyPolicy.HorizontalOptions = LayoutOptions.EndAndExpand;
                lblViewAllDevices.IsVisible = true;
                ProceedForPermission = false;

                await CheckAndRequestLocationPermission();
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Trim().Contains("android.permission.bluetooth_scan"))
                {

                }
                else
                {
                    await ShowDisplayPopup("Error", ex.Message.ToString());
                }
            }
        }
        private async void btnQuite_Clicked(object sender, EventArgs e)
        {
            try
            {
                await DisconnectDevices();

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.RemoteSessionEnd, "CONNECTION_END");

                await Task.Delay(1000);

                ResponceCount = 0;
                while (ResponceCount < 5)
                {
                    await Task.Delay(1000);
                    ResponceCount++;
                }

                await StopRemoteSession("ADMIN");

                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex) { System.Diagnostics.Process.GetCurrentProcess().Kill(); }
        }
        public async void onPrivacyTapped(object sender, EventArgs args)
        {
            try
            {
                await Launcher.OpenAsync(new System.Uri("https://www.sunfunkits.com/content/3018/privacy-policy"));
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        private async void ClearDeviceDataTapped(object sender, TappedEventArgs e)
        {
            Preferences.Set("DeviceData_Cached", string.Empty);
            await ShowDisplayPopup("Success", "Cache cleared successfully.");
        }
        public async void ViewAllDevicesTapped(object sender, EventArgs args)
        {
            try
            {
                await SetBusyState(true, "Scanning...");

                lstDeviceList.ItemsSource = null;
                SelectedDevice = new List<DisplayDevice>();
                BSelectedDevice = new List<DisplayDevice>();
                if (lblViewAllDevices.Text == "View All Devices")
                {
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ViewAllDevice, null);

                    stackClickableLink.Spacing = 10;
                    lblViewAllDevices.Text = "View SFK Devices";
                    ViewAllDevice = true;

                    ResponceCount = 0;
                    while (ResponceCount < 10)
                    {
                        await Task.Delay(1000);
                        if (ViewAllDevicesList != null && ViewAllDevicesList.Count > 0)
                        {
                            break;
                        }
                        ResponceCount++;
                    }

                    if (ViewAllDevicesList != null && ViewAllDevicesList.Count > 0)
                    {
                        lstDeviceList.ItemsSource = ViewAllDevicesList.ToArray();

                        await SetBusyState(false);
                    }
                }
                else
                {
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ViewAllDevice, null);

                    lblViewAllDevices.Text = "View All Devices";
                    stackClickableLink.Spacing = 20;
                    ViewAllDevice = false;

                    if ((BluetoothDeviceList != null && BluetoothDeviceList.Count > 0))
                    {
                        if (BluetoothDeviceList != null && BluetoothDeviceList.Count > 0)
                        {
                            lstDeviceList.ItemsSource = BluetoothDeviceList.ToArray();
                        }
                        else
                        {
                            //lstDeviceList.ItemsSource = USBDeviceList.ToArray();
                        }
                    }
                    else
                    {
                        lstDeviceList.IsVisible = false;
                        lblNoDeviceListFound.IsVisible = true;
                    }
                }

                await DesignChanges(Designwidth, Designheight);

                await SetBusyState(false);
            }
            catch { }
        }
        private async void btnConnect_Clicked(object sender, EventArgs e)
        {
            try
            {
                stackPasswordBusy.IsVisible = EnterPassword.IsVisible = false;

                await SetBusyState(true, "Connecting...");

                await PerformConnectionLogicAsync();

                await SetBusyState(false);
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        private async Task PerformConnectionLogicAsync()
        {
            CalledPasswordConfirmationBox = false;
            if (!ProceedForconnect)
            {
                SelectedDevice = BSelectedDevice;
            }

            if (SelectedDevice != null && SelectedDevice.Count() > 0 && !ErrorOccureWhileConnecting)
            {
                EightsCount = SelectedDevice.Count(d => DeviceDefaultValues.BMS8Cells.Any(prefix => cleanRegex.Replace(d.DeviceName?.ToLower()?.Trim() ?? "", "").StartsWith(prefix)));

                if (EightsCount > 0 && SelectedDevice.Count != EightsCount)
                {
                    await ShowDisplayPopup("Information", "Multiview does not support displaying 12V and 24V batteries at the same time.");
                    await ResetConnectionStateAsync();
                    return;
                }
            }

            if (IsVirtualBattery)
            {
                await ConnectViaVirtualBattery();
            }
            else
            {
                await ConnectViaBluetooth();
            }
        }
        private async Task ConnectViaBluetooth()
        {
            bHasPassword = false;
            bool bProceedToConnect = false;
            if (SelectedDevice != null && SelectedDevice.Count > 0)
            {
                if (SelectedDevice.Count <= 6)
                {
                    IsErrorOccured = false;
                    List<string> lstMacAddress = new List<string>();
                    foreach (var device in SelectedDevice)
                    {
                        lstMacAddress.Add(device.MacAddress);
                    }

                    if (SelectedDevice.Count == 1)
                    {
                        CurrentSelectedDevice = SelectedDevice[0];

                        await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ConnectDevices, lstMacAddress);

                        ResponceCount = 0;
                        while (ResponceCount < 20)
                        {
                            await Task.Delay(1000);
                            if (IsDeviceConnected)
                            {
                                break;
                            }
                            ResponceCount++;
                        }

                        if (IsDeviceConnected)
                        {
                            bProceedToConnect = await CheckForDefaultPasswordForDevice();

                            if (bProceedToConnect && !PasswordProcessCancel)
                            {
                                if (!IsErrorOccured)
                                {
                                    await SetBusyState(true);

                                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ProceedForDetailPage, null);

                                    ProceedAhead = false;
                                    while (true)
                                    {
                                        if (ProceedAhead)
                                        {
                                            RemoteSessionDetails._remote.OnReceive -= MainPageReceive;

                                            DeviceDetails Page = new DeviceDetails(CurrentSelectedDevice, sPassword);
                                            await Navigation.PushAsync(Page);
                                            break;
                                        }
                                        await Task.Delay(1000);
                                    }
                                }
                            }
                            else
                            {
                                if (!EnterPassword.IsVisible)
                                {
                                    await ShowDisplayPopup("Error", $"Error connecting device.");
                                }
                            }
                        }
                    }
                    else if (SelectedDevice.Count > 1)
                    {
                        if (!ProceedForconnect && SelectedDevice.Count > 4)
                        {
                            await ShowDisplayPopup("Warning", "Warning: Connecting more than 4 BLE devices may cause connection instability and is not supported on all devices.");
                        }
                        else
                        {
                            ProceedForconnect = true;
                        }

                        if (ProceedForconnect)
                        {
                            await SetBusyState(true);

                            await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ConnectDevices, lstMacAddress);

                            ResponceCount = 0;
                            while (ResponceCount < 30)
                            {
                                await Task.Delay(1000);
                                if (IsDeviceConnected)
                                {
                                    break;
                                }
                                ResponceCount++;
                            }

                            if (IsDeviceConnected)
                            {
                                try
                                {
                                    if (!IsErrorOccured)
                                    {
                                        await SetBusyState(true);

                                        await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ProceedForMultiView, null);

                                        ProceedAhead = false;
                                        while (true)
                                        {
                                            if (ProceedAhead)
                                            {
                                                RemoteSessionDetails._remote.OnReceive -= MainPageReceive;

                                                CalledPasswordConfirmationBox = true;
                                                PasswordConfirmationBox Page = new PasswordConfirmationBox(SelectedDevice);
                                                await PopupNavigation.Instance.PushAsync(Page);
                                                break;
                                            }
                                            await Task.Delay(1000);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    await ShowDisplayPopup("Error", ex.Message.ToString());
                                    ErrorOccureWhileConnecting = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    await ShowDisplayPopup("Error", "You can select a maximum of 6 devices only.");
                }
            }
            else
            {
                await ShowDisplayPopup("Error", "No device selected. Please choose a device to connect.");
                await ResetConnectionStateAsync();
            }
        }
        private async Task ConnectViaVirtualBattery()
        {
            SelectedDevice = VSelectedDevice;
            //if (SelectedDevice != null && SelectedDevice.Count > 0)
            //{
            //    if (SelectedDevice.Count > 1)
            //    {
            //        bool ProceedToConnect = false;
            //        foreach (var Device in SelectedDevice)
            //        {
            //            if (selectedItem.State == DeviceState.Connected)
            //            {
            //                ProceedToConnect = true;
            //            }
            //            else
            //            {
            //                try
            //                {
            //                    var connectParameters = new ConnectParameters(false, true);
            //                    await _bluetoothAdapter.ConnectToDeviceAsync(selectedItem, connectParameters);
            //                    ProceedToConnect = true;
            //                }
            //                catch (Exception ex)
            //                {
            //                    await ShowDisplayPopup("Error", $"Error connecting to SFK device: {selectedItem.Name ?? "N/A"}");
            //                    ProceedToConnect = false;
            //                    break;
            //                }
            //            }
            //        }
            //        if (ProceedToConnect)
            //        {
            //            try
            //            {
            //                // MultiDeviceDetails Page = new MultiDeviceDetails(SelectedDevice, true);
            //                // await Navigation.PushAsync(Page);
            //            }
            //            catch (Exception ex)
            //            {
            //                await ShowDisplayPopup("Error", ex.Message.ToString());
            //            }
            //        }
            //    }
            //    else
            //    {
            //        await ShowDisplayPopup("Error", "Unable to find the virtual battery device.");
            //        await ResetConnectionStateAsync();
            //    }
            //}
            //else
            //{
            //    await ShowDisplayPopup("Error", "Unable to find the virtual battery device.");
            //    await ResetConnectionStateAsync();
            //}
        }
        private async Task<bool> CheckForDefaultPasswordForDevice()
        {
            bool bProceedConnect = false;
            try
            {
                await SetBusyState(true, "Checking PIN...");


                PasswordProcessCancel = false;

                ConnectedDeviceMacAddress = CurrentSelectedDevice.MacAddress;

                await Start();

                ResponceCount = 0;
                while (ResponceCount < 20)
                {
                    await Task.Delay(1000);
                    if (objProtocol != null)
                    {
                        break;
                    }
                    ResponceCount++;
                }

                await ReadCellInfo04Command();

                await Task.Delay(200);

                await ReadBMSDetails03Command();

                await Task.Delay(100);

                await ClearPINCommand();

                await Task.Delay(500);

                if (!IsSFKH2Device) // Need to remove
                {
                    await CheckForDefaultPassword();

                    int iTry = 0;
                    while (!PasswordEnterSucces && iTry < 15)
                    {
                        if (PasswordProcessCancel)
                        {
                            bProceedConnect = true;
                            break;
                        }
                        await Task.Delay(1000);
                        iTry++;
                    }

                    if (PasswordEnterSucces)
                    {
                        bProceedConnect = true;
                    }
                }
                else
                {
                    bProceedConnect = true;
                }
            }
            catch { }
            return bProceedConnect;
        }
        private async Task ResetConnectionStateAsync()
        {
            BSelectedDevice.Clear();
            lstDeviceList.ItemsSource = null;
            lstVirtualBatteryList.ItemsSource = null;

            lstDeviceList.ItemsSource = BluetoothDeviceList?.ToArray();

            lstVirtualBatteryList.ItemsSource = VirtualDeviceList?.ToArray();

            IsVirtualBattery = false;
            EightsCount = 0;
            ErrorOccureWhileConnecting = false;
        }
        private async void CheckBox_CheckChanged(object sender, EventArgs e)
        {
            try
            {
                InputKit.Shared.Controls.CheckBox objcheckbox = (InputKit.Shared.Controls.CheckBox)sender;
                if (objcheckbox != null)
                {
                    if (!string.IsNullOrWhiteSpace(objcheckbox.ClassId))
                    {
                        int count = objcheckbox.ClassId.Count(f => f == ':');
                        if (count == 5 && objcheckbox.ClassId.Length == 17)
                        {
                            string SelectedMacAddres = objcheckbox.ClassId;

                            var Blutoothdevice = BluetoothDeviceList.FirstOrDefault(d =>
                            {
                                return d.MacAddress == SelectedMacAddres;
                            });

                            if (Blutoothdevice != null)
                            {
                                if (objcheckbox.IsChecked)
                                {
                                    BSelectedDevice.Add(Blutoothdevice);

                                    SelectedMacAddres = SelectedMacAddres + "^" + true;
                                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.CheckBox_Check, SelectedMacAddres);

                                    if (IsVirtualBattery)
                                    {
                                        await ShowDisplayPopup("Information", "Only one of a device or a virtual battery can be connected at a time.");
                                    }
                                }
                                else
                                {
                                    BSelectedDevice.Remove(Blutoothdevice);

                                    SelectedMacAddres = SelectedMacAddres + "^" + false;
                                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.CheckBox_Check, SelectedMacAddres);
                                }
                            }
                        }
                        else if (objcheckbox.ClassId.Contains("_VB"))
                        {
                            if (objcheckbox.IsChecked)
                            {
                                BSelectedDevice.Clear();
                                if (BluetoothDeviceList != null && BluetoothDeviceList.Count > 0)
                                {
                                    lstDeviceList.ItemsSource = BluetoothDeviceList.ToArray();
                                }

                                if (IsVirtualBattery)
                                {
                                    VSelectedDevice.Clear();
                                    if (VirtualDeviceList != null && VirtualDeviceList.Count > 0)
                                    {
                                        lstVirtualBatteryList.ItemsSource = VirtualDeviceList.ToArray();
                                    }
                                    IsVirtualBattery = false;
                                    await ShowDisplayPopup("Information", "You can connect only one virtual battery at a time.");
                                }
                                else
                                {
                                    if (BSelectedDevice.Count > 0)
                                    {
                                        await ShowDisplayPopup("Information", "You can connect either a device or a virtual battery at a time");
                                    }
                                    else
                                    {
                                        IsVirtualBattery = true;
                                        ConnectToVrtualBattery(objcheckbox.ClassId);
                                    }
                                }
                            }
                            else
                            {
                                VSelectedDevice.Clear();
                                IsVirtualBattery = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        protected override bool OnBackButtonPressed()
        {
            DisconnectDevices();

            System.Diagnostics.Process.GetCurrentProcess().Kill();

            return true;
        }
        private async void ConnectToVrtualBattery(string VirtualBatteryName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(VirtualBatteryName))
                {
                    VirtualBatteryName = VirtualBatteryName.Replace("_VB", "");
                    string BatteryData = Preferences.Get(VirtualBatteryName, string.Empty);
                    List<string> VDeviceList = new List<string>();
                    string BatterySetupSetting = string.Empty;
                    if (!string.IsNullOrWhiteSpace(BatteryData))
                    {
                        VDeviceList = BatteryData.Split('|').ToList();
                        int iCount = VDeviceList.Count();
                        for (int i = 1; i <= iCount; i++)
                        {
                            if (VDeviceList[0].Count(c => c == ':') != 5 && VDeviceList[0].Count(c => c == '-') != 4)
                            {
                                VDeviceList.RemoveAt(0);
                            }
                        }

                        if (VDeviceList.Count == 2)
                        {
                            BatterySetupSetting = "BatteryConnectionSetupforTwo";
                        }
                        else if (VDeviceList.Count == 3)
                        {
                            BatterySetupSetting = "BatteryConnectionSetupforThree";
                        }
                        else if (VDeviceList.Count == 4)
                        {
                            BatterySetupSetting = "BatteryConnectionSetupforFour";
                        }
                        else if (VDeviceList.Count == 5)
                        {
                            BatterySetupSetting = "BatteryConnectionSetupforFive";
                        }
                        else if (VDeviceList.Count == 6)
                        {
                            BatterySetupSetting = "BatteryConnectionSetupforSix";
                        }
                        else if (VDeviceList.Count == 7)
                        {
                            BatterySetupSetting = "BatteryConnectionSetupforSeven";
                        }
                        else if (VDeviceList.Count == 8)
                        {
                            BatterySetupSetting = "BatteryConnectionSetupforEight";
                        }

                        Preferences.Set(BatterySetupSetting, BatteryData.Split('|')[0]);
                        Preferences.Set("AmpsDrawValue", BatteryData.Split('|')[1]);
                        Preferences.Set("VoltsDrawValue", BatteryData.Split('|')[2]);
                        Preferences.Set("BalanceTemperatureLimitValue", Convert.ToInt32(BatteryData.Split('|')[3]));
                        Preferences.Set("LowTempActivation", Convert.ToInt32(BatteryData.Split('|')[4]));

                        if (VDeviceList != null && VDeviceList.Count > 0)
                        {
                            bool AllUSBDevice = VDeviceList.All(x => x.Length == 8);
                            if (!AllUSBDevice)
                            {
                                //foreach (var gattitem in _gattDevices)
                                //{
                                //    foreach (var item in VDeviceList)
                                //    {
                                //        var obj = gattitem.NativeDevice;
                                //        PropertyInfo propInfo = obj.GetType().GetProperty("Address");
                                //        string MacAddress = (string)propInfo.GetValue(obj, null);
                                //        if (item == MacAddress)
                                //        {
                                //            VSelectedDevice.Add(gattitem);
                                //            break;
                                //        }
                                //    }

                                //    if (VDeviceList.Count() == VSelectedDevice.Count())
                                //    {
                                //        break;
                                //    }
                                //}

                                //if (VDeviceList.Count() != VSelectedDevice.Count())
                                //{
                                //    VSelectedDevice.Clear();
                                //}
                            }
                            Preferences.Set("SelectedVirtualBatteryName", VirtualBatteryName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        public async Task ShowDisplayPopup(string Status, string Message)
        {
            try
            {
                stackCancelAndYesButton.IsVisible = false;
                lblContinue.Text = "Continue";
                lblDecline.Text = "Decline";
                PopUpStatus = Status;
                lblRetryAndOK.Text = "Ok";
                sbbtn1.ClassId = "Procced";
                if (Status != "Permission Required")
                {
                    stkDisplayMessagePopUp.IsVisible = true;
                }
                else
                {
                    stkPermissionRequiredPopUp.IsVisible = true;
                }
                stkDisplayMessagePopUpBusy.IsVisible = true;

                if (Status == "Success")
                {
                    imgDisplay.Source = "settingsupdated.png";
                    btnRetryAndOK.IsVisible = true;
                    lblText.Text = Message;
                }
                else if (Status == "Error")
                {
                    imgDisplay.Source = "error.png";
                    btnRetryAndOK.IsVisible = true;
                    lblText.Text = Message;
                }
                else if (Status == "Warning")
                {
                    imgDisplay.Source = "information.png";
                    lblText.Text = Message;
                    lblText.HorizontalTextAlignment = TextAlignment.Start;
                    stackCancelAndYesButton.IsVisible = true;
                    lblContinue.Text = "Continue";
                    lblDecline.Text = "Cancel";
                    btnRetryAndOK.IsVisible = false;
                    sbbtn1.ClassId = "Continue";
                }
                else if (Status == "Bluetooth")
                {
                    imgDisplay.Source = "blutooth.png";
                    btnRetryAndOK.IsVisible = true;
                    lblText.Text = Message;
                }
                else if (Status == "Information")
                {
                    imgDisplay.Source = "information.png";
                    btnRetryAndOK.IsVisible = true;
                    lblText.Text = Message;
                }
                else if (Status == "Location Permission")
                {
                    imgDisplay.Source = "location.png";
                    btnRetryAndOK.IsVisible = true;
                    lblText.Text = Message;
                }
                else if (Status == "Load Default Setting")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    stackCancelAndYesButton.IsVisible = true;
                    btnRetryAndOK.IsVisible = false;
                    lblText.Text = Message;
                }

                await ShowDisplayPopupDesign(Designwidth, Designheight);

                string data = Status + '^' + Message;
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ShowDisplayPopup, data);
            }
            catch (Exception ex)
            {

            }
        }
        private async void DisplayPopup_Clicked(object sender, EventArgs e)
        {
            try
            {
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DisplayPopup_Clicked, null);

                stkDisplayMessagePopUp.IsVisible = false;
                stkDisplayMessagePopUpBusy.IsVisible = false;

                stkPermissionRequiredPopUp.IsVisible = false;
                stkDisplayMessagePopUpBusy.IsVisible = false;

                SetBusyState(false);

                if (RemoteSessionDisplayMessageOK)
                {
                    await SetBusyState(true, "Please wait..");
                    MainPage Page = new MainPage(false);
                    await Navigation.PushAsync(Page);
                }
            }
            catch (Exception ex) { }
        }
        private async void DisplayPopupYes_Clicked(object sender, EventArgs e)
        {
            if (!ProceedForPermission)
            {
                stkPermissionRequiredPopUp.IsVisible = false;
                stkDisplayMessagePopUpBusy.IsVisible = false;
                ProceedForPermission = true;
                btnScan_Clicked(null, null);
            }
        }
        private async Task DisconnectDevices()
        {
            try
            {
                SelectedDevice.Clear();
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnCancelAutoConnect_Clicked(object sender, EventArgs e)
        {
            try
            {
                await DisconnectDevices();

                SelectedDevice.Clear();
                bProccedVConnection = false;
                btnCancelAutoConnect.IsVisible = false;
                await Task.Delay(6000);
                MainPage Page = new MainPage(false);
                await Navigation.PushAsync(Page);
                await SetBusyState(false);
            }
            catch (Exception ex)
            {

            }
        }
        private async Task CheckAndRequestLocationPermission()
        {
            if (platform == DevicePlatform.Android)
            {
                if (version.CompareTo(targetVersion) <= 0)
                {
                    // Check the status of the permission
                    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                    if (status != PermissionStatus.Granted)
                    {
                        //await ShowDisplayPopup("Location Permission", "Sun Fun Kits BMS App requires the Location Permission (Precise) and Bluetooth Permission to be granted in order to communicate via Bluetooth. These permissions are also needed for the Auto Dashboard feature to work properly. Please ensure Location Services are enabled on your device, grant these permissions from the app settings, and then restart the application.");
                        await SetBusyState(false);
                        return;
                    }
                }
            }
        }
        public async Task SendWriteOrReadStartCommandToBMS()
        {
            try
            {
                var Start = objProtocol.StartandEndCommand("start");
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.byteCommand, Start);
            }
            catch (Exception ex)
            {

            }
        }
        public async Task SendWriteOrReadEndCommandToBMS()
        {
            try
            {
                var End = objProtocol.StartandEndCommand("end");
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.byteCommand, End);
            }
            catch (Exception ex)
            {
            }
        }
        private async Task ResetValues()
        {
            lblViewAllDevices.IsVisible = false;
            lstDeviceList.ItemsSource = null;
            lstVirtualBatteryList.ItemsSource = null;

            BluetoothDeviceList.Clear();
            VirtualDeviceList.Clear();
            SelectedDevice.Clear();
            BSelectedDevice.Clear();
            VSelectedDevice.Clear();
        }
        private async Task SetBusyState(bool isBusy, string Label = null)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (!string.IsNullOrWhiteSpace(Label))
                {
                    lblBusyIndicator.Text = Label;
                }
                sfbIsBusyIndicator.IsVisible = stkBusy.IsVisible = isBusy;

                if (RemoteSessionDetails._remote != null && RemoteSessionDetails._remote.IsConnected)
                {
                    string data = Convert.ToString(isBusy) + '^' + Label;
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.BusyIndicator, data);
                }
            });
            await Task.Delay(100);
        }
        private async Task ReadCellInfo04Command()
        {
            try
            {
                var bytes04 = objProtocol.CreateReadCommand(RegisterEnum.REG_CELL);
                await SendCommandToBMS(bytes04);
            }
            catch (Exception ex)
            {
            }
        }
        private async Task ReadBMSDetails03Command()
        {
            try
            {
                var bytes03 = objProtocol.CreateReadCommand(RegisterEnum.REG_GENERAL);
                await SendCommandToBMS(bytes03);
            }
            catch (Exception ex)
            {

            }
        }
        public async Task SendCommandToBMS(byte[] SendCommand)
        {
            try
            {
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.byteCommand, SendCommand);
            }
            catch (Exception ex)
            {

            }
        }
        public static async Task<string> GetIPAddress()
        {
            string IpAddress = string.Empty;
            try
            {
                string[] PublicIpApis = new[] { "https://api.ipify.org", "https://ifconfig.me", "https://ipinfo.io/ip", "https://icanhazip.com", "https://checkip.amazonaws.com" };
                using var httpClient = new HttpClient();
                foreach (var api in PublicIpApis)
                {
                    try
                    {
                        string publicIp = await httpClient.GetStringAsync(api);
                        if (!string.IsNullOrWhiteSpace(publicIp))
                        {
                            IpAddress = publicIp.Trim();
                            break;
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex) { }
            return IpAddress;
        }
        private async void ListTabChange_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                await SetBusyState(true, "Loading...");
                Border border = (Border)sender;

                brdDevieListTab.BackgroundColor = Color.FromHex("#e3e3e3");
                brdVirtualBatteryListTab.BackgroundColor = Color.FromHex("#e3e3e3");

                stackDeviceList.IsVisible = false;
                stackVirtualBatteryList.IsVisible = false;

                if (border.ClassId == "DeviceList")
                {
                    brdDevieListTab.BackgroundColor = Color.FromHex("#F0C000");
                    stackDeviceList.IsVisible = true;

                    if (BluetoothDeviceList?.Count > 0)
                    {
                        for (int i = 0; i < BluetoothDeviceList.Count; i++)
                        {
                            BluetoothDeviceList[i].IsChecked = false;
                        }

                        lstDeviceList.ItemsSource = BluetoothDeviceList;
                    }
                }
                else if (border.ClassId == "VirtualBatteryList")
                {
                    brdVirtualBatteryListTab.BackgroundColor = Color.FromHex("#F0C000");
                    stackVirtualBatteryList.IsVisible = true;

                    if (VirtualDeviceList?.Count > 0)
                    {
                        for (int i = 0; i < VirtualDeviceList.Count; i++)
                        {
                            VirtualDeviceList[i].IsChecked = false;
                        }

                        lstVirtualBatteryList.ItemsSource = VirtualDeviceList;
                    }
                }
                await SetBusyState(false);
            }
            catch (Exception ex) { }
        }

        private void lstDeviceList_Scrolled(object sender, ScrolledEventArgs e)
        {
            SendScroll(lstDeviceList, "lstDeviceList", e);
        }
        private void lstVirtualBatteryList_Scrolled(object sender, ScrolledEventArgs e)
        {
            SendScroll(lstVirtualBatteryList, "lstVirtualBatteryList", e);
        }
        private CancellationTokenSource _scrollCts = new();
        private int _lastSentIndex = -1;

        private void SendScroll(ListView listView, string type, ScrolledEventArgs e)
        {
            try
            {
                int firstVisibleIndex = Math.Max(0, (int)(e.ScrollY / 50));

                // Skip if index hasn't changed
                if (firstVisibleIndex == _lastSentIndex) return;
                _lastSentIndex = firstVisibleIndex;

                // Cancel previous pending send
                _scrollCts.Cancel();
                _scrollCts.Dispose();
                _scrollCts = new CancellationTokenSource();
                CancellationToken token = _scrollCts.Token;

                // Debounce: wait 80ms before actually sending
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(80, token);
                        if (token.IsCancellationRequested) return;

                        string data = $"{type}^{firstVisibleIndex.ToString(CultureInfo.InvariantCulture)}";
                        _ = RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ScrollEvent, data);
                    }
                    catch (OperationCanceledException) { }
                    catch { }
                }, token);
            }
            catch { }
        }

        #endregion Main Code

        #region Responsive Design
        protected async override void OnSizeAllocated(double width, double height)
        {
            try
            {
                base.OnSizeAllocated(width, height);

                RemoveHeight = Convert.ToInt32(stackHeader.Height + stackButtons.Height) + 10;
                RemoveHeightLandscap = Convert.ToInt32(stackHeader.Height) + 10;

                Designwidth = width;
                Designheight = height;

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    // lstRemoteConnection.HeightRequest = Designheight - stacksfklogo.HeightRequest - btnClearAll.HeightRequest - 150;

                }
                else
                {
                    // lstRemoteConnection.HeightRequest = Designheight - stacksfklogo.HeightRequest - btnClearAll.HeightRequest - 170;
                }

                if (width > height)
                {
                    stackButtons.HorizontalOptions = LayoutOptions.CenterAndExpand;

                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        stkPermissionRequiredPopUp.WidthRequest = (width - 200);
                    }
                    else
                    {
                        stkPermissionRequiredPopUp.HeightRequest = (height / 100) * 55;
                        stkPermissionRequiredPopUp.WidthRequest = (width - 200);
                    }
                }
                else if (height > width)
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        stackButtons.WidthRequest = ((width / 100) * 90);

                        stackButtons.HorizontalOptions = LayoutOptions.FillAndExpand;

                        stkPermissionRequiredPopUp.HeightRequest = (height / 100) * 70;
                        stkPermissionRequiredPopUp.WidthRequest = (width / 100) * 85;
                    }
                    else
                    {
                        stackButtons.WidthRequest = ((width / 100) * 90);

                        stkPermissionRequiredPopUp.HeightRequest = (height / 100) * 55;
                        stkPermissionRequiredPopUp.WidthRequest = (width / 100) * 85;
                    }
                }
                await DesignChanges(width, height);

                if (stkDisplayMessagePopUp.IsVisible)
                {
                    await ShowDisplayPopupDesign(width, height);
                }
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        private async Task DesignChanges(double width, double height)
        {
            try
            {
                if (width > height)
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        await LandscapDesignForMobile(width, height);
                    }
                    else if (Device.Idiom == TargetIdiom.Tablet)
                    {
                        await LandscapDesignForTab(width, height);
                    }
                }
                else if (height > width)
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        await PotraitDesignForMobile(width, height);
                    }
                    else if (Device.Idiom == TargetIdiom.Tablet)
                    {
                        await PotraitDesignForTab(width, height);
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        private async Task PotraitDesignForTab(double width, double height)
        {
            try
            {
                imgBatteryScanVertical.IsVisible = true;
                imgBatteryScanHorizontal.IsVisible = false;

                stackButtons.VerticalOptions = LayoutOptions.End;

                stackButtons.RowDefinitions.Clear();
                stackButtons.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                stackButtons.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

                Grid.SetRow(btnRemoteSupportNew, 0);
                Grid.SetRow(stackButtonInner, 1);

                Grid.SetRow(grdButtonInner, 1);
                Grid.SetRow(stackClickableLinkMain, 0);

                Grid.SetRow(lblViewAllDevices, 0);
                Grid.SetRow(stackClickableLink, 1);

                stackAvailableDataInner.Margin = new Thickness(40, 20, 40, 5);

                stackMainStackLayout.Orientation = StackOrientation.Vertical;

                stackAvailableData.HorizontalOptions = LayoutOptions.FillAndExpand;
                stackButtons.HorizontalOptions = LayoutOptions.FillAndExpand;

                stackAvailableData.VerticalOptions = LayoutOptions.FillAndExpand;
                stackButtons.VerticalOptions = LayoutOptions.End;

                stackButtonInner.Orientation = StackOrientation.Horizontal;

                grdScanButtonNew.Orientation = StackOrientation.Horizontal;
                grdConnectButtonNew.Orientation = StackOrientation.Horizontal;
                grdQuitButtonNew.Orientation = StackOrientation.Horizontal;

                grdButtonInner.Margin = new Thickness(15, 5, 15, 20);
                stackButtons.RowSpacing = 0;
            }
            catch (Exception ex)
            {
            }
        }
        private async Task LandscapDesignForTab(double width, double height)
        {
            try
            {
                imgBatteryScanVertical.IsVisible = false;
                imgBatteryScanHorizontal.IsVisible = true;

                stackMainStackLayout.Orientation = StackOrientation.Horizontal;

                stackButtons.RowDefinitions.Clear();
                // stackButtons.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                stackButtons.RowDefinitions.Add(new RowDefinition { Height = 130 });
                stackButtons.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                stackButtons.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

                grdButtonInner.RowDefinitions.Clear();
                grdButtonInner.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                grdButtonInner.RowDefinitions.Add(new RowDefinition { Height = btnRemoteSupportNew.HeightRequest + 10 });

                Grid.SetRow(btnRemoteSupportNew, 1);
                Grid.SetRow(stackButtonInner, 0);

                Grid.SetRow(imgBatteryScanHorizontal, 0);
                Grid.SetRow(grdButtonInner, 1);
                Grid.SetRow(stackClickableLinkMain, 2);

                Grid.SetRow(lblViewAllDevices, 1);
                Grid.SetRow(stackClickableLink, 0);

                stackAvailableData.WidthRequest = ((width / 100) * 65);
                stackButtons.WidthRequest = ((width / 100) * 35);

                stackAvailableData.HorizontalOptions = LayoutOptions.Fill;
                stackButtons.HorizontalOptions = LayoutOptions.End;

                stackAvailableData.VerticalOptions = LayoutOptions.FillAndExpand;
                stackButtons.VerticalOptions = LayoutOptions.Center;

                stackButtonInner.Orientation = StackOrientation.Vertical;

                grdScanButtonNew.Orientation = StackOrientation.Horizontal;
                grdConnectButtonNew.Orientation = StackOrientation.Horizontal;
                grdQuitButtonNew.Orientation = StackOrientation.Horizontal;

                grdButtonInner.Margin = new Thickness(15, 5, 15, 5);

                stackButtons.RowSpacing = 15;
            }
            catch (Exception ex)
            {
            }
        }
        private async Task PotraitDesignForMobile(double width, double height)
        {
            try
            {
                imgBatteryScanVertical.IsVisible = true;
                imgBatteryScanHorizontal.IsVisible = false;

                stackButtons.VerticalOptions = LayoutOptions.End;

                stackButtons.RowDefinitions.Clear();
                stackButtons.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                stackButtons.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

                grdButtonInner.RowDefinitions.Clear();
                grdButtonInner.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                grdButtonInner.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

                stackAvailableDataInner.Margin = new Thickness(20, 10, 20, 10);

                Grid.SetRow(btnRemoteSupportNew, 0);
                Grid.SetRow(stackButtonInner, 1);

                Grid.SetRow(grdButtonInner, 1);
                Grid.SetRow(stackClickableLinkMain, 0);

                Grid.SetRow(lblViewAllDevices, 0);
                Grid.SetRow(stackClickableLink, 1);

                stackMainStackLayout.Orientation = StackOrientation.Vertical;

                stackAvailableData.HorizontalOptions = LayoutOptions.FillAndExpand;
                stackButtons.HorizontalOptions = LayoutOptions.FillAndExpand;

                stackAvailableData.VerticalOptions = LayoutOptions.FillAndExpand;
                stackButtons.VerticalOptions = LayoutOptions.End;

                stackButtonInner.Orientation = StackOrientation.Horizontal;

                grdScanButtonNew.Orientation = StackOrientation.Vertical;
                grdConnectButtonNew.Orientation = StackOrientation.Vertical;
                grdQuitButtonNew.Orientation = StackOrientation.Vertical;
                stackButtons.RowSpacing = 0;
            }
            catch (Exception ex)
            {
            }
        }
        private async Task LandscapDesignForMobile(double width, double height)
        {
            try
            {
                EnterPassword.WidthRequest = (width / 100) * 60;

                imgLock.HeightRequest = 50;
            }
            catch (Exception ex)
            {
            }
        }
        private async Task ShowDisplayPopupDesign(double width, double height)
        {
            try
            {
                if (width > height) //landscape
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        if (PopUpStatus == "Location Permission")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 95;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 85;
                        }
                        else
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 90;
                            stkDisplayMessagePopUp.WidthRequest = (width / 2);
                        }
                    }
                    else
                    {
                        if (PopUpStatus == "Location Permission")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 60;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 65;
                        }
                        else
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 60;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 50;
                        }
                    }
                }
                else //portrait
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        if (PopUpStatus == "Location Permission")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 60;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 85;
                        }
                        else if (PopUpStatus == "Warning")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 50;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 85;
                        }
                        else
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 50;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 85;
                        }
                    }
                    else
                    {
                        if (PopUpStatus == "Location Permission")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 60;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 80;
                        }
                        else if (PopUpStatus == "Warning")
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 35;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 70;
                        }
                        else
                        {
                            stkDisplayMessagePopUp.HeightRequest = (height / 100) * 45;
                            stkDisplayMessagePopUp.WidthRequest = (width / 100) * 80;
                        }
                    }
                }

            }
            catch (Exception ex) { }
        }

        #endregion Responsive Design        

        #region Restore Default Values

        private async void frmRestoreDefault_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                bool bGetLiveData = false;
                await SetBusyState(true, "Restoring...");
                ProceedForLoadDefaultSettings = false;
                if (!string.IsNullOrWhiteSpace(strDefaultValues))
                {
                    if (strDefaultValues.StartsWith("DefaultData|"))
                    {
                        await RestoreDefaultValueConnection();
                        return;
                    }
                    else
                    {
                        bGetLiveData = true;
                    }
                }
                else
                {
                    bGetLiveData = true;
                }

                if (bGetLiveData)
                {
                    Frame objbtn = (Frame)sender;
                    var strSelectedMACAddress = objbtn.ClassId;
                    if (ViewAllDevicesList != null)
                    {
                        foreach (var item in ViewAllDevicesList)
                        {
                            string MacAddress = item.MacAddress;
                            if (MacAddress == strSelectedMACAddress)
                            {
                                CurrentSelectedDevice = item;
                                strDefaultValues = await GetDefaultValueData();
                                if (!string.IsNullOrWhiteSpace(strDefaultValues))
                                {
                                    await SetBusyState(false);
                                    if (strDefaultValues.StartsWith("DefaultData|"))
                                    {
                                        await RestoreDefaultValueConnection(MacAddress);
                                        break;
                                    }
                                    else if (strDefaultValues.StartsWith("DEVICENOTSUPPORTED"))
                                    {
                                        await ShowDisplayPopup("Information", "Sorry, your device does not support restoring default settings.");
                                    }
                                    else if (strDefaultValues.StartsWith("DATANOTFOUND"))
                                    {
                                        await ShowDisplayPopup("Information", "Sorry, your device was not found.");
                                    }
                                    else if (strDefaultValues.StartsWith("ERROR") || strDefaultValues.StartsWith("EXERROR"))
                                    {
                                        await ShowDisplayPopup("Error", "Something went wrong!");
                                    }
                                }
                                else
                                {
                                    await ShowDisplayPopup("Error", "Please Try Again.");
                                    await SetBusyState(false);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { await SetBusyState(false); }
        }
        private async void btnConfirmORDecline_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                Border brd = (Border)sender;
                stkDisplayMessagePopUp.IsVisible = false;
                stkDisplayMessagePopUpBusy.IsVisible = false;

                if (!ProceedForconnect && Convert.ToString(brd.ClassId) == "Continue")
                {
                    ProceedForconnect = true;
                    btnConnect_Clicked(null, null);
                }

                if (Convert.ToString(brd.ClassId) == "Procced")
                {
                    ProceedForLoadDefaultSettings = true;
                    if (ProceedForLoadDefaultSettings)
                    {
                        await RestoreDefaultValueConnection(RestoreMacAddress);
                    }
                }
                else if (Convert.ToString(brd.ClassId) == "Cancel")
                {
                    ProceedForLoadDefaultSettings = false;
                }
            }
            catch (Exception ex) { }
        }
        public async Task<string> GetDefaultValueData()
        {
            string saAllLines = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                string SiteURL = "https://www.sunfunkits.com/app/GetDefaultValueData";

                string MacAddress = RestoreMacAddress;
                // Remove
                MacAddress = "70 3e 97 07 ab 5e";

                client.DefaultRequestHeaders.Add("MAC_ADDRESS", MacAddress.Replace(":", " "));
                var httpResponseMessage = await client.GetStringAsync(SiteURL);

                if (httpResponseMessage != null)
                {
                    saAllLines = httpResponseMessage.Split("|^|")[1];
                }
            }
            catch (HttpRequestException ex)
            {
                await ShowDisplayPopup("Error", "Unable to connect to the server. Please check your internet connection.");
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", "An unexpected error occurred. Please try again later or Contact sunfunkits sales.");
            }
            return saAllLines;
        }
        public async Task RestoreDefaultValueConnection(string MacAddress = "")
        {
            try
            {
                if (!ProceedForLoadDefaultSettings)
                {
                    if (!string.IsNullOrWhiteSpace(MacAddress))
                    {
                        RestoreMacAddress = MacAddress;
                    }
                    await ShowDisplayPopup("Load Default Setting", "Looks like we found a matching device, click continue to restore back to factory settings.");
                }

                if (ProceedForLoadDefaultSettings)
                {
                    await SetBusyState(true);

                    if (!string.IsNullOrWhiteSpace(MacAddress))
                    {
                        await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.RestoreOrForgotPINDeviceConnect, MacAddress);
                    }

                    ResponceCount = 0;
                    while (ResponceCount < 10)
                    {
                        await Task.Delay(1000);
                        if (IsDeviceConnected)
                        {
                            break;
                        }
                        ResponceCount++;
                    }

                    if (IsDeviceConnected)
                    {
                        await Start();
                        await Task.Run(() => RestoreDefaultValuesAsync());
                    }

                    await SetBusyState(false);
                }
            }
            catch (Exception ex) { }
        }
        public async Task Start()
        {
            try
            {
                var BLEDeviceName = Regex.Replace(Convert.ToString(CurrentSelectedDevice.DeviceName).ToLower().Trim(), "[^0-9a-zA-Z:,]+", "");
                if (!string.IsNullOrEmpty(BLEDeviceName))
                {
                    objDeviceInformation = await DeviceDefaultValues.GetDeviceData(BLEDeviceName.ToLower());

                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.GetProtocolType, CurrentSelectedDevice.MacAddress);
                }
            }
            catch (Exception ex) { }
        }
        public async Task Stop(string MacAddress = "")
        {
            try
            {
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.Disconnect, MacAddress);
            }
            catch (Exception ex)
            {
            }
        }
        public async Task RestoreDefaultValuesAsync()
        {
            try
            {
                isProccesBegin = true;
                CalibrationUpdateInProgress = true;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    stackProgressBarMain.IsVisible = stkDisplayMessagePopUpBusy.IsVisible = ProceedForLoadDefaultSettings;
                });

                if (ProceedForLoadDefaultSettings)
                {
                    strDefaultValues = strDefaultValues.Replace("DefaultData|", "");
                    string[] strValueArray = strDefaultValues.Split("||");
                    if (strValueArray.Length > 0)
                    {
                        int iPercentage = 0;
                        int totalItems = strValueArray.Length;
                        for (int i = 0; i < totalItems; i++)
                        {
                            int iTry = 0;
                            var item = strValueArray[i];
                            string register = Convert.ToString(item.Trim().Split(' ')[2]);
                            iReadRegister = Convert.ToInt32(register, 16);

                            byte[] byteArray = item.Trim().Split(' ').Select(hex => Convert.ToByte(hex, 16)).ToArray();

                            while (iTry < 3)
                            {
                                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.byteCommand, byteArray);

                                WaitForResponse();

                                if (bIsSendNExt) { break; }
                                iTry++;
                            }

                            await Task.Delay(100);

                            iPercentage = (int)((i + 1) / (double)totalItems * 100);
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                ProgressBarPointer.Progress = iPercentage;
                                lblProgressBar.Text = $"{iPercentage}%";
                            });
                        }
                    }

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (bIsSendNExt)
                        {
                            ProceedForLoadDefaultSettings = false;
                            await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.Disconnect, CurrentSelectedDevice.MacAddress);
                            stackProgressBarMain.IsVisible = stkDisplayMessagePopUpBusy.IsVisible = ProceedForLoadDefaultSettings;
                            await ShowDisplayPopup("Success", "Device settings restored, please turn off Bluetooth on your battery and phone tablet and then turn back on after 30 seconds.Close the SFK app and launch it again to scan for available devices.");
                        }
                        else
                        {
                            ProceedForLoadDefaultSettings = false;
                            await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.Disconnect, CurrentSelectedDevice.MacAddress);
                            stackProgressBarMain.IsVisible = stkDisplayMessagePopUpBusy.IsVisible = ProceedForLoadDefaultSettings;
                            await ShowDisplayPopup("Error", "Unable to restore default settings.");
                        }
                    });
                }

                isProccesBegin = false;
                CalibrationUpdateInProgress = false;

                await SetBusyState(true);
            }
            catch (Exception ex) { }
        }
        private async Task WaitForResponse()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
                if (bIsSendNExt == true)
                    break;
            }
        }

        #endregion Restore Default Values

        #region Forgot PIN
        private async void frmForgotPIN_Tapped(object sender, TappedEventArgs e)
        {
            string MacAddress = string.Empty;
            try
            {
                await SetBusyState(true, "Clearing PIN..");

                bool PermissionGranted = false;
                Frame objbtn = (Frame)sender;
                var strSelectedMACAddress = objbtn.ClassId;
                if (ViewAllDevicesList != null)
                {
                    foreach (var item in ViewAllDevicesList)
                    {
                        MacAddress = item.MacAddress;
                        if (MacAddress == strSelectedMACAddress)
                        {
                            CurrentSelectedDevice = item;
                            string BarcodeID = item.DeviceName;

                            HttpClient client = new HttpClient();
                            string SiteURL = "https://www.sunfunkits.com/app/GetPermissionForClearPIN";

                            client.DefaultRequestHeaders.Add("Bms_ID", BarcodeID);

                            string response = await client.GetStringAsync(SiteURL);
                            string cleaned = response.Split(' ').Last().Trim();
                            PermissionGranted = bool.TryParse(cleaned, out bool parsedResult) && parsedResult;

                            if (PermissionGranted)
                            {
                                IsDeviceConnected = false;
                                if (!string.IsNullOrWhiteSpace(MacAddress))
                                {
                                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.RestoreOrForgotPINDeviceConnect, MacAddress);
                                }

                                ResponceCount = 0;
                                while (ResponceCount < 10)
                                {
                                    await Task.Delay(1000);
                                    if (IsDeviceConnected)
                                    {
                                        break;
                                    }
                                    ResponceCount++;
                                }

                                if (IsDeviceConnected)
                                {
                                    await Start();

                                    ResponceCount = 0;
                                    while (ResponceCount < 10)
                                    {
                                        await Task.Delay(1000);
                                        if (objProtocol != null)
                                        {
                                            break;
                                        }
                                    }

                                    await ClearPINCommand();

                                    int iTry = 0;
                                    while (iTry < 5)
                                    {
                                        await WaitForResponsePassword();
                                        if (bPasswordRest == true) { break; }

                                        if (iTry == 2)
                                        {
                                            await ClearPINCommand();
                                        }

                                        iTry++;
                                    }

                                    if (bPasswordRest)
                                    {
                                        HttpClient InnerClient = new HttpClient();
                                        string strSiteURL = "https://www.sunfunkits.com/app/ClearDevicePIN";
                                        string IpAddress = await GetIPAddress();

                                        InnerClient.DefaultRequestHeaders.Add("Bms_ID", Convert.ToString(BarcodeID));
                                        InnerClient.DefaultRequestHeaders.Add("Ip_Address", IpAddress);
                                        string httpResponseMsg = await InnerClient.GetStringAsync(strSiteURL);
                                        string ResponseMsg = httpResponseMsg.Split(' ').Last().Trim();

                                        if (!string.IsNullOrWhiteSpace(ResponseMsg))
                                        {
                                            if (ResponseMsg == "UPDATED")
                                            {
                                                await ShowDisplayPopup("Success", "The PIN was successfully cleared.");
                                            }
                                            else
                                            {
                                                await ShowDisplayPopup("Error", "Something went wrong!.Please try again.");
                                            }
                                        }
                                        else
                                        {
                                            await ShowDisplayPopup("Error", "Something went wrong!");
                                        }
                                        await Stop(MacAddress);
                                        IsDeviceConnected = false;
                                    }
                                    else
                                    {
                                        await Stop(MacAddress);
                                        await ShowDisplayPopup("Error", "Unable to clear PIN, Please try again.");
                                    }
                                }
                            }
                            else
                            {
                                await ShowDisplayPopup("Error", "Unable to clear PIN, contact SFK support");
                            }
                        }
                    }
                }
                await SetBusyState(false);
            }
            catch (HttpRequestException ex)
            {
                await Stop(MacAddress);
                await SetBusyState(false);
                await ShowDisplayPopup("Error", "Unable to connect to the server. Please check your internet connection.");
            }
            catch (Exception ex)
            {
                await Stop(MacAddress);
                await SetBusyState(false);
                await ShowDisplayPopup("Error", "An unexpected error occurred. Please try again later or contact SFK sales.");
            }
        }
        public async Task ClearPINCommand()
        {
            try
            {
                var SetPassword = objProtocol.PrepareDefaultPin09();

                await SendCommandToBMS(SetPassword);
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        private async Task WaitForResponsePassword()
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(20);
                if (IsDeviceConnected == true) { break; }
            }
        }

        #endregion Forgot PIN

        #region Check For Device Pin        
        public async Task CheckForDefaultPassword()
        {
            try
            {
                PasswordCheck06CalledFrom_IntialLoad = true;
                PasswordCheck06CalledFrom_Please_Enter_Password = false;
                PasswordCheck06CalledFrom_Reset_Password = false;

                // var DefaultPassword = new byte[] { 0xdd, 0x5a, 0x06, 0x07, 0x06, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0xff, 0xde, 0x77 };
                // if (objDeviceInformation.Protocol.ToLower().Contains("v4"))
                // {
                //     DefaultPassword = new byte[] { 0xcc, 0xcd, 0x06, 0x07, 0x06, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0xff, 0xde, 0xcf };
                // }                
                var DefaultPassword = objProtocol.PrepareCustomerPassword("012345");
                await SendCommandToBMS(DefaultPassword);
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", ex.Message.ToString());
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
        private async void btnConfirmUserPassword_Clicked(object sender, EventArgs e)
        {
            try
            {
                await SetBusyState(true);

                if (!string.IsNullOrWhiteSpace(txtBatteryPassword.PINValue))
                {
                    sPassword = txtBatteryPassword.PINValue;
                    if (CheckValidPassword(txtBatteryPassword.PINValue.Trim()))
                    {
                        var SubmitPassword = objProtocol.PrepareCustomerPassword(txtBatteryPassword.PINValue.Trim());
                        await SendCommandToBMS(SubmitPassword);

                        PasswordCheck06CalledFrom_Please_Enter_Password = true;
                        PasswordCheck06CalledFrom_IntialLoad = false;
                        PasswordCheck06CalledFrom_Reset_Password = false;

                        Preferences.Set((ConnectedDeviceMacAddress + "_Old_Password"), Convert.ToString(txtBatteryPassword.PINValue));

                        if (chkRememberSettingsForSingle.IsChecked)
                        {
                            string Password = Convert.ToString(txtBatteryPassword.PINValue);
                            if (string.IsNullOrWhiteSpace(Password))
                            {
                                Password = "012345";
                                Preferences.Set((ConnectedDeviceMacAddress + "_Password"), Password);
                            }
                            else
                            {
                                Preferences.Set((ConnectedDeviceMacAddress + "_Password"), Password);
                            }
                        }

                    }
                    else
                    {
                        stackIncorrectPasswordMsg.IsVisible = true;
                        lblPasswordMessage.Text = "Invalid Pin, It must be 6 digit";
                    }
                }
                else
                {
                    stackIncorrectPasswordMsg.IsVisible = true;
                    lblPasswordMessage.Text = "Invalid Pin";
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async void btnCancelEnterPassword_Clicked(object sender, EventArgs e)
        {
            await Stop();
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                SetBusyState(false);
                PasswordProcessCancel = true;
                EnterPassword.IsVisible = false;
                stackPasswordBusy.IsVisible = false;
                txtBatteryPassword.PINValue = "";
                lblPasswordMessage.Text = "";
                stackIncorrectPasswordMsg.IsVisible = false;
            });
            await Task.Delay(100);
        }
        #endregion Check For Device Pin

        #region Remote Session

        List<RemoteSession> lstRemoteSessionDeviceInfo = new List<RemoteSession>();
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _player = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("ringingbell.mp3"));

            if (MainActivity.MainActivityInstance != null)
            {
                MainActivity.MainActivityInstance.ApplyStatusBarStyle();
            }

            RemoteSessionDetails._remote = App.Services.GetService<RemoteSessionService>();
            RemoteSessionDetails._remote.OnReceive += MainPageReceive;

            if (!RemoteSessionDetails._remote.IsConnected)
            {
                MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await SetBusyState(true);
                    stackRemoteSessonList.IsVisible = true;
                    gridMain.IsEnabled = false;
                });
            }
        }
        public async void MainPageReceive(string fromDevice, string Datatype, string jsondata)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    if (Datatype == RemoteSessionReceiveDataTypes.VirtualDeviceList.ToString())
                    {
                        VirtualDeviceList = Serializer.Deserialize<List<DisplayDevice>>(jsondata);
                    }
                    else if (Datatype == RemoteSessionReceiveDataTypes.BluetoothDeviceList.ToString())
                    {
                        BluetoothDeviceList = Serializer.Deserialize<List<DisplayDevice>>(jsondata);
                    }
                    else if (Datatype == RemoteSessionReceiveDataTypes.ViewAllDevicesList.ToString())
                    {
                        ViewAllDevicesList = Serializer.Deserialize<List<DisplayDevice>>(jsondata);
                    }
                    else if (Datatype == RemoteSessionReceiveDataTypes.DeviceConnectionStatus.ToString())
                    {
                        IsDeviceConnected = false;
                        IsDeviceConnected = Serializer.Deserialize<bool>(jsondata);
                    }
                    else if (Datatype == RemoteSessionReceiveDataTypes.SuccessDetailPage.ToString())
                    {
                        ProceedAhead = Serializer.Deserialize<bool>(jsondata);
                    }
                    else if (Datatype == RemoteSessionReceiveDataTypes.SuccessMultiPage.ToString())
                    {
                        ProceedAhead = Serializer.Deserialize<bool>(jsondata);
                    }
                    else if (Datatype == RemoteSessionReceiveDataTypes.ProtocolType.ToString())
                    {
                        try
                        {
                            IsSFKH2Device = Serializer.Deserialize<bool>(jsondata);
                        }
                        catch (Exception ex) { IsSFKH2Device = false; }
                        if (objDeviceInformation != null)
                        {
                            objProtocol = await DeviceDefaultValues.GetProtocolDetails(IsSFKH2Device ? "SFKV4" : "SFKV1");
                        }
                    }
                    else if (Datatype == RemoteSessionReceiveDataTypes.byteCommand.ToString())
                    {
                        byte[] receivedBytes = Serializer.Deserialize<byte[]>(jsondata);
                        if (receivedBytes != null && ((byte[])receivedBytes).Length > 1 && !CalledPasswordConfirmationBox)
                        {
                            if ((Convert.ToInt32(((byte[])receivedBytes)[1]) == iReadRegister))
                            {
                                bIsSendNExt = true;
                            }
                            else if ((((byte[])receivedBytes)[0].ToString() == "221" || ((byte[])receivedBytes)[0].ToString() == "204") && ((((byte[])receivedBytes)[1].ToString() == "06") || (((byte[])receivedBytes)[1].ToString() == "6")))
                            {
                                string sResult = BitConverter.ToString(receivedBytes);
                                sResult = sResult.Replace("-", "").ToLower();

                                if (sResult.StartsWith("dd060000000077") || sResult.StartsWith("cc0600000000"))
                                {
                                    if (PasswordCheck06CalledFrom_IntialLoad)
                                    {
                                        PasswordEnterSucces = true;
                                        stackPasswordBusy.IsVisible = EnterPassword.IsVisible = false;
                                    }
                                    else if (PasswordCheck06CalledFrom_Please_Enter_Password)
                                    {
                                        PasswordEnterSucces = true;
                                        stackPasswordBusy.IsVisible = EnterPassword.IsVisible = false;
                                    }
                                }
                                else
                                {
                                    await SetBusyState(true);
                                    bHasPassword = true;
                                    if (PasswordCheck06CalledFrom_IntialLoad)
                                    {
                                        var Password = Preferences.Get(ConnectedDeviceMacAddress + "_Password", string.Empty);

                                        if (!string.IsNullOrWhiteSpace(Password))
                                        {
                                            var SubmitPassword = objProtocol.PrepareCustomerPassword(Convert.ToString(Password.Split('_')[0]));
                                            await SendCommandToBMS(SubmitPassword);

                                            if (iPassCount == SelectedDevice.Count())
                                            {
                                                PasswordCheck06CalledFrom_IntialLoad = false;
                                                PasswordCheck06CalledFrom_Please_Enter_Password = true;
                                                PasswordCheck06CalledFrom_Reset_Password = false;
                                                iPassCount = 1;
                                            }
                                            else
                                            {
                                                iPassCount++;
                                            }

                                            PasswordEnterSucces = true;
                                            stackPasswordBusy.IsVisible = EnterPassword.IsVisible = false;
                                        }
                                        else
                                        {

                                            PasswordEnterSucces = false;
                                            stackPasswordBusy.IsVisible = EnterPassword.IsVisible = true;
                                            txtBatteryPassword.PINValue = "";
                                        }
                                    }
                                    else if (PasswordCheck06CalledFrom_Please_Enter_Password)
                                    {
                                        PasswordEnterSucces = false;
                                        stackPasswordBusy.IsVisible = EnterPassword.IsVisible = true;

                                        stackIncorrectPasswordMsg.IsVisible = true;
                                        lblPasswordMessage.Text = "Invalid Pin";
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
                            else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "9" || ((byte[])receivedBytes)[1].ToString() == "09") && (((byte[])receivedBytes)[2].ToString() == "0" || ((byte[])receivedBytes)[2].ToString() == "00"))
                            {
                                bPasswordRest = true;
                            }
                        }
                    }
                    else if (Datatype == RemoteSessionReceiveDataTypes.Error.ToString())
                    {
                        string strError = Serializer.Deserialize<string>(jsondata);
                        IsErrorOccured = true;
                        await ShowDisplayPopup("Error", strError);
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
                            await StopRemoteSession("USER");
                        }
                    }
                }
                catch (Exception ex) { }
            });
        }
        public async Task StopRemoteSession(string CommandBy)
        {
            try
            {
                await RemoteSessionDetails.StopRemoteSessionService();

                RemoteSessionDisplayMessageOK = true;

                MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (CommandBy == "ADMIN")
                    {
                        await ShowDisplayPopup("Information", "The remote session has ended.");
                    }
                    else if (CommandBy == "USER")
                    {
                        await ShowDisplayPopup("Information", "The remote session has ended by " + RemoteSessionDetails.CustomerName);
                    }
                });
            }
            catch (Exception ex) { }
        }
        public async Task GetDeviceDetailRemoteSessionInfoInServer()
        {
            try
            {
                await SetBusyState(true);

                string Result = string.Empty; APIExtensions_Response ApiActionResponse = new APIExtensions_Response(); HttpResponseMessage response = new HttpResponseMessage(); bool ExecuteOneTime = false;
                
                while (string.IsNullOrWhiteSpace(RemoteSessionDetails.RemoteSessionDeviceGUID))
                {
                    string ApiURL = RemoteSessionService.SERVER_URL + "/APIExtensions/Execute/ComrogenAPI_Extensions_SFKRemoteAPP/GetRemoteSession";

                    ArrayList objParam = new ArrayList();

                    response = await RemoteSessionDetails.PostApiAsync(ApiURL, objParam);

                    if (!ExecuteOneTime)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await SetBusyState(true);
                            stackRemoteSessonList.IsVisible = true;
                            gridMain.IsEnabled = false;
                        });

                        ExecuteOneTime = true;
                    }

                    if (response != null && response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();

                        ApiActionResponse = JsonConvert.DeserializeObject<APIExtensions_Response>(data);

                        if (ApiActionResponse != null && ApiActionResponse.ActionResponse != null && ApiActionResponse.ActionResponse.Response != null)
                        {
                            if (ApiActionResponse.ActionResponse.Response.ResultType == ResultType.Success)
                            {
                                if (lstRemoteConnection.ItemsSource == null && !lblNotFound.IsVisible)
                                {
                                    await SetBusyState(true);
                                }

                                if (!string.IsNullOrWhiteSpace(ApiActionResponse.ActionResponse.Response.message))
                                {
                                    Result = ApiActionResponse.ActionResponse.Response.message;

                                    if (Result.StartsWith("Success,"))
                                    {
                                        Result = Result.Replace("Success,", string.Empty);

                                        lstRemoteSessionDeviceInfo = JsonConvert.DeserializeObject<List<RemoteSession>>(Result);

                                        if (lstRemoteSessionDeviceInfo != null)
                                        {
                                            lstRemoteSessionDeviceInfo = lstRemoteSessionDeviceInfo.Where(x => x.CreatedDate >= DateTime.Now.Date.AddDays(-5)).OrderByDescending(x => x.CreatedDate).ToList();
                                        }

                                        if (lstRemoteSessionDeviceInfo != null && lstRemoteSessionDeviceInfo.Count > 0)
                                        {
                                            await LoadNotificationImages();

                                            await MainThread.InvokeOnMainThreadAsync(() =>
                                            {
                                                lstRemoteConnection.ItemsSource = lstRemoteSessionDeviceInfo.OrderByDescending(x => x.ID).ToArray();
                                                lstRemoteConnection.IsVisible = true;
                                                lblNotFound.IsVisible = false;
                                                btnClearAll.IsVisible = true;
                                                btnMuteAll.IsVisible = true;
                                            });

                                            await SetBusyState(false);
                                        }
                                        else
                                        {
                                            await MainThread.InvokeOnMainThreadAsync(() =>
                                            {
                                                lstRemoteConnection.ItemsSource = null;
                                                lstRemoteConnection.IsVisible = false;
                                                btnClearAll.IsVisible = false;
                                                btnMuteAll.IsVisible = false;
                                                lblNotFound.IsVisible = true;
                                            });

                                            await SetBusyState(false);
                                        }
                                    }
                                }
                                else
                                {
                                    Result = "Fail";
                                }
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(RemoteSessionDetails.RemoteSessionDeviceGUID))
                    {
                        if (lstRemoteSessionDeviceInfo != null)
                        {
                            foreach (var item in lstRemoteSessionDeviceInfo)
                            {
                                if (item.SessionStatus != null && item.SessionStatus.ToLower() == "verified")
                                {
                                    string prefKey = "REMOTE_NOTIFY_" + item.DeviceID;

                                    if (!IsAlreadyNotified(prefKey))
                                    {
                                        IsNotificationMute = false;

                                        PlayNotificationSound();

                                        await SendNotification("New Remote Session Request", item.CustomerName);

                                        MarkAsNotified(prefKey);
                                    }
                                }
                            }
                        }
                    }

                    await Task.Delay(10000);
                }
            }
            catch { }
        }

        private async void PlayNotificationSound()
        {
            try
            {
                int count = 0;
                int TimeCounter = 10;
                //int TimeCounter = 60;
                //if (!App.IsAppActive)
                //{
                //    TimeCounter = 30;
                //}
                while (!IsNotificationMute)
                {
                    if (count <= TimeCounter && !_player.IsPlaying)
                    {
                        _player.Stop();
                        _player.Play();
                    }
                    else if (count > TimeCounter && _player.IsPlaying)
                    {
                        _player.Stop();
                        break;
                    }

                    await Task.Delay(2000);
                    count += 2;
                }
            }
            catch (Exception ex) { }
        }
        private Task LoadNotificationImages(string Command = "MAIN")
        {
            try
            {
                if (Command == "MAIN")
                {
                    foreach (var item in lstRemoteSessionDeviceInfo)
                    {
                        if (item.SessionStatus.ToLower() == "verified")
                        {
                            //if (!IsNotificationMute)
                            //{
                            //    item.ClickEnable = true;
                            //    item.EnableImage = "enable.png";
                            //    item.DisableImage = "mute.png";
                            //    //item.DisableImage = "disable.png";
                            //}
                            //else
                            //{
                            //    item.ClickEnable = true;
                            //    item.EnableImage = "enable.png";
                            //    item.DisableImage = "disable.png";
                            //}

                            item.ClickEnable = true;
                            item.EnableImage = "enable.png";
                            item.DisableImage = "disable.png";
                        }
                        else
                        {
                            item.ClickEnable = false;
                            item.EnableImage = "yesdisabled.png";
                            item.DisableImage = "nodisabled.png";
                        }
                    }
                }
                else
                {
                    foreach (var item in lstRemoteSessionDeviceInfo)
                    {
                        if (item.SessionStatus.ToLower() == "verified")
                        {
                            item.ClickEnable = true;
                            item.DisableImage = "enable.png";
                            item.DisableImage = "disable.png";
                        }
                        else
                        {
                            item.ClickEnable = false;
                            item.EnableImage = "yesdisabled.png";
                            item.DisableImage = "nodisabled.png";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Task.CompletedTask;
        }
        private async void btnClearAll_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                await SetBusyState(true);
                await DeleteAllRemoteSessionList();
                await SetBusyState(false);
            }
            catch (Exception ex) { await SetBusyState(false); }
        }
        public async Task DeleteAllRemoteSessionList()
        {
            string Result = string.Empty; APIExtensions_Response ApiActionResponse = new APIExtensions_Response(); HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                string ApiURL = RemoteSessionService.SERVER_URL + "/APIExtensions/Execute/ComrogenAPI_Extensions_SFKRemoteAPP/DeleteAllRemoteSession";
                ArrayList objParam = new ArrayList();
                response = await RemoteSessionDetails.PostApiAsync(ApiURL, objParam);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync(); ApiActionResponse = JsonConvert.DeserializeObject<APIExtensions_Response>(data);
                    if (ApiActionResponse != null && ApiActionResponse.ActionResponse != null && ApiActionResponse.ActionResponse.Response != null)
                    {
                        if (ApiActionResponse.ActionResponse.Response.ResultType == ResultType.Success)
                        {
                            if (!string.IsNullOrWhiteSpace(ApiActionResponse.ActionResponse.Response.message))
                            {
                                Result = ApiActionResponse.ActionResponse.Response.message;
                                if (Result.StartsWith("Success,"))
                                {
                                    lstRemoteConnection.ItemsSource = null;
                                    lstRemoteConnection.IsVisible = false;
                                    btnClearAll.IsVisible = false;
                                    btnMuteAll.IsVisible = false;
                                    lblNotFound.IsVisible = true;
                                    await SetBusyState(false);
                                    await ShowDisplayPopup("Success", "Remote Session Cleared");
                                }
                            }
                            else
                            {
                                Result = "Fail";
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Result = "Fail"; }
        }
        private async void imgbtnAcceptRequest_Clicked(object sender, EventArgs e)
        {
            try
            {
                await SetBusyState(true);

                IsNotificationMute = true;

                _player.Stop();

                ImageButton objButton = sender as ImageButton;

                if (objButton == null)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(objButton.ClassId))
                {
                    return;
                }

                RemoteSessionDetails.RemoteSessionDeviceGUID = objButton.ClassId;

                RemoteSessionService remoteService = App.Services.GetService<RemoteSessionService>();

                if (remoteService == null)
                {
                    await ShowDisplayPopup("Error", "Remote service unavailable.");
                    return;
                }

                RemoteSessionDetails._remote = remoteService;

                if (remoteService.IsConnected)
                {
                    await remoteService.StopAsync();

                    await Task.Delay(1000);
                }

                bool isConnected = false;

                int maxAttempts = 2;

                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {

                    try
                    {
                        await remoteService.StartAsync(RemoteSessionDetails.RemoteSessionDeviceGUID);
                    }
                    catch (Exception ex)
                    {
                        await ShowDisplayPopup("Error", ex.Message);
                        return;
                    }


                    await Task.Delay(500);

                    await remoteService.SendPingAsync(RemoteSessionSendCommandTypes.Ping, null);

                    for (int i = 0; i < 30; i++)
                    {
                        if (remoteService.IsConnected && !remoteService.IsPeerDisconnected)
                        {
                            isConnected = true;
                            break;
                        }

                        await Task.Delay(250);
                    }

                    if (isConnected)
                    {
                        break;
                    }
                }

                await Task.Delay(250);

                if (isConnected)
                {
                    stackRemoteSessonList.IsVisible = false;
                    gridMain.IsEnabled = true;

                    await RemoteSessionDetails.UpdateRemoteSession("START");

                    _ = Task.Run(() =>
                    {
                        CheckRemoteSessionPingStatus();
                    });

                    ServiceHelper.StopService();

                    await ShowDisplayPopup("Information", "The remote session has started.");
                }
                else
                {
                    stackRemoteSessonList.IsVisible = true;
                    gridMain.IsEnabled = false;

                    await ShowDisplayPopup("Information", "Please try again.");
                }
            }
            catch (Exception)
            {
                await ShowDisplayPopup("Error", "Unexpected error occurred.");
            }
            finally
            {
                await SetBusyState(false);
            }
        }
        private async void imgbtnDeclineRequest_Clicked(object sender, EventArgs e)
        {
            try
            {
                ImageButton objButton = (ImageButton)sender;
                if (objButton != null)
                {
                    string imageName = string.Empty;
                    if (objButton.Source is FileImageSource fileSource)
                    {
                        imageName = fileSource.File;
                    }

                    //if (imageName == "mute.png")
                    //{
                    //    await LoadNotificationImages("MUTE");
                    //    lstRemoteConnection.ItemsSource = lstRemoteSessionDeviceInfo.OrderByDescending(x => x.ID).ToArray();
                    //    IsNotificationMute = true;
                    //    _player.Stop();
                    //}
                    //else 
                    if (imageName == "disable.png")
                    {
                        await SetBusyState(true);

                        if (!string.IsNullOrWhiteSpace(objButton.ClassId))
                        {
                            RemoteSessionDetails.RemoteSessionDeviceGUID = objButton.ClassId;
                        }

                        RemoteSessionDetails._remote = App.Services.GetService<RemoteSessionService>();

                        if (RemoteSessionDetails._remote.IsConnected)
                        {
                            await RemoteSessionDetails._remote.StopAsync();
                        }

                        await Task.Delay(1000);

                        await RemoteSessionDetails._remote.StartAsync(RemoteSessionDetails.RemoteSessionDeviceGUID);

                        await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DeclineRequest, null);

                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await RemoteSessionDetails.UpdateRemoteSession("START");

                            await Task.Delay(200);

                            await RemoteSessionDetails.StopRemoteSessionService();

                            MainPage page = new MainPage(false);

                            await Navigation.PushAsync(page);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void btnMuteAll_Tapped(object sender, TappedEventArgs e)
        {
            IsNotificationMute = true;
            _player.Stop();
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

                        if (currentPage is MainPage)
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
                await ShowDisplayPopup("Information", "Remote Session Connection Lost.");
            });
        }
        private async void btnEndRemoteSupprt_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                await SetBusyState(true, "Please Wait...");
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.RemoteSessionEnd, "CONNECTION_END");
                await StopRemoteSession("ADMIN");
            }
            catch (Exception ex) { }
            finally
            {
                await SetBusyState(false);
            }
        }

        #region Notification Action
        private int GetBadgeCount()
        {
            return Preferences.Get("APP_BADGE_COUNT", 0);
        }
        private int IncrementBadgeCount()
        {
            int count = Preferences.Get("APP_BADGE_COUNT", 0);

            count = count + 1;

            Preferences.Set("APP_BADGE_COUNT", count);

            return count;
        }
        private bool IsAlreadyNotified(string key)
        {
            return Preferences.ContainsKey(key);
        }
        private void MarkAsNotified(string key)
        {
            Preferences.Set(key, true);
        }
        public async Task SendNotification(string NotificationMessage, string Description)
        {
            try
            {
                NotificationRequest _PushAppNotificationRequest = new();

                string Key = GenerateShortKey(DateTime.Now);

                int SFKNotificationID = GenerateNotificationKey(Key);

                int badgeCount = IncrementBadgeCount();

                NotificationImage _image = new NotificationImage
                {
                    ResourceName = "sunfunkitsappicon.png",
                    FilePath = "sunfunkitsappicon.png"
                };

                _PushAppNotificationRequest.BadgeNumber = badgeCount;
                _PushAppNotificationRequest.NotificationId = SFKNotificationID;
                _PushAppNotificationRequest.Title = NotificationMessage;
                _PushAppNotificationRequest.Description = Description;
                _PushAppNotificationRequest.Image = _image;
                _PushAppNotificationRequest.Sound = "default";
                _PushAppNotificationRequest.Android = new AndroidOptions
                {
                    ChannelId = "default",
                    IconSmallName = new AndroidIcon("icon_notification")
                };

                await LocalNotificationCenter.Current.Show(_PushAppNotificationRequest);

                Badge.SetCount((uint)badgeCount);

                await Task.Delay(100);
            }
            catch (Exception ex)
            {
            }
        }
        public static string GenerateShortKey(DateTime createdDate)
        {
            string rawKey = string.Empty;

            rawKey = $"{createdDate:yyyyMMddHHmmss}";

            using (SHA256 sha = SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawKey));
                // Take first 8 characters of the hash
                return BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 8);
            }
        }
        public static int GenerateNotificationKey(string Key)
        {
            int iKey = 0;
            using (var sha = SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Key));
                iKey = BitConverter.ToInt32(hashBytes, 0); // Still possible but extremely unlikely to collide
            }
            return iKey;
        }
        #endregion Notification Action

        #endregion Remote Session
    }
}