
using Android.Bluetooth;
using Android.Net.Wifi;
using Microsoft.Maui.Controls.Shapes;
using SFKBle;
using SFKBle.Models;
using SFKBle_Admin.SFK_Protocol;
using Syncfusion.Maui.DataSource.Extensions;
using Syncfusion.Maui.Gauges;
using Syncfusion.Maui.Sliders;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Android.Provider.ContactsContract.CommonDataKinds;

namespace SFKBle_Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceDetails : ContentPage
    {
        #region Variable Declaration

        private readonly DisplayDevice _connectedDevice;
        private List<byte[]> arr03RecBytes;
        private List<byte[]> arr04RecBytes;
        private List<byte[]> arrErrorCountRecBytes;
        private List<byte[]> arrD7RecBytes;
        private List<byte[]> arrE8RecBytes;
        public CellModel objCellmodel;
        public BusyIndicatorViewModel objBusyIndicator;
        public BusyIndicatorReconnectionViewModel objIndicator;

        public RadioButtonViewModel objRadioButtonViewModel;
        public List<FileViewModel> objFileViewModelList;
        public BLEDevice objbleDevice;
        public ValidReadValues objReadValues;
        public Capacity_ViewModel objCapacity;

        public decimal temprature1;
        public decimal temprature2;
        public decimal temprature3;
        public bool LowTemperatureHeatingSupport = false;
        public string ModelDeviceName;
        public string BLEDeviceName = string.Empty;

        private int nWriteCounter = 0;
        private int nReadCounter = 0;
        private int PullingRequestTime = 3000;
        private int EstablishConnectionDatAttemps = 10;

        public bool PasswordCheck06CalledFrom_Please_Enter_Password = false;
        public bool PasswordCheck06CalledFrom_Reset_Password = false;
        public bool PasswordCheck06CalledFrom_IntialLoad = false;

        public bool isOldPinWrong = false;

        public bool isAlreadyIntialLoadDecidedtheDefaultPassword = false;
        public bool isSystemisSettingDefaultPin = false;

        public bool isProccesBegin = false;
        public bool IsCalibrationTabOpened = false;
        public bool RunMainPageWhileLoop = true;
        public bool BreakWhileLoop = false;
        public bool IsBMSConnected = true;
        public string DeviceOldPassword = string.Empty;

        public static bool IsFahrenheit = false;
        public string sCelsiusorFahrenheit = string.Empty;
        private double _oldWidth, _oldHeight;
        private bool bEnableAutoRotate = true;

        bool IsScantabSelect = false;
        bool IsBenchmarktabSelect = false;
        bool ProceedForCalibrateCapacity = false;
        bool ProceedForBanchmarkTest = false;
        bool ProceedForFaultReleaseUpdate = false;
        bool FaultReleasePopup = false;
        string logFilePath = string.Empty;
        public bool BechmarkTestRunning = false;
        public string strTextData = null;
        public int IntervalTime = 0;
        public int BenchmarkTestCount = 0;
        public int Firmware_Version = 0;

        public double DeviceHeight = 0;
        public double DeviceWidth = 0;

        public string PopUpStatus = string.Empty;

        public string BarcodeValue = string.Empty;

        public int ChecksumFails = 0;
        public bool bProccedFails = false;

        public bool Is24VBattery = false;
        public bool hasCaseTemp2 = true;

        public bool CallFunctionOneTime = true;

        public bool bManufatureDate = true;
        public bool IsReconnectFailed = false;
        public bool ReconnectExecuted = false;

        public ObservableCollection<VoltageCalibration> ListVoltageCalibration { get; set; }
        public ObservableCollection<TemperatureCalibration> ListTemperatureCalibration { get; set; }

        public string[] strVoltageCalibrationValues = new string[2];
        public string[] strTemperatureCalibrationValues = new string[2];

        public string sCalibrationPassword = string.Empty;
        public bool CalibrationUpdateInProgress = false;
        bool bSameCell = true;

        Frame frmLastSelectedCell = new Frame();
        Frame frmLastSelectedTemp = new Frame();

        public int TempratureCount = 0;
        DisplayDevice _Device = new DisplayDevice();
        public bool UsedNewFirmware = false;
        public double TempsliderLowTempValue = 0;


        bool bTheftReportStarted = false;
        bool bPasswordRest = false;
        bool bResetLogFaults = false;
        bool bResetCycleCount = false;
        private bool _isInitialized = false;

        DeviceInformation objDeviceInformation = new DeviceInformation();

        IProtocol objProtocol;
        string sPassword = string.Empty;

        TimeSpan tsDiffrence = new TimeSpan();

        bool IsBleGain = false;

        bool ProceedForHeatingMode = false;
        string CurrentPopup = string.Empty;

        int CurrentBattType = 0;

        bool bBmsUsageLogClear = false;

        bool IsTimeInterval = false;

        bool bProceedMaxMinSOC = false;
        bool IsDeviceConnectionStatus = false;

        int ResponceCount = 0;

        bool IsH2SettingUpdating = false;

        bool IsMaxSOCPopLoaded = false;
        public bool RemoteSessionDisplayMessageOK = false;

        int countRTCUpdateTime = 0;

        bool bProccedBatteryType = false;

        bool bProccedRestartESP = false;

        bool bSinceFullChargeBlink = false;

        bool bRestoreDefaultValue = false;

        bool TimeSyncronized = false;

        bool bFullyChargedDate = false;

        public bool bIsSendNExt = false;

        public bool _ReadOnlyOncesForDevices = true;
        public int CounterFor09Command = 0;
        public bool FirstTimeDetailLoad = true;

        public int CounterForActiveBalancerCommand = 15;
        public int CounterForFullyChargedDateCommand = 30;
        public int CounterForReadRegulateSOCActiveStatusCommand = 60;
        string strH2FirmwareData = string.Empty;
        bool ProceedForFirmwareUpgradeForH2 = false;
        bool bFirmwareUptodate = false;
        int MtuSize = 0;
        bool H2FirmwareUpgradeCompleted = false;

        Dictionary<int, int> Tempmap = new Dictionary<int, int>() { { 1, 35 }, { 4, 40 }, { 7, 45 }, { 10, 50 }, { 13, 55 } };
        List<FirmwareInfo> firmwareList = new List<FirmwareInfo>();
        string FirmwareVersion = string.Empty;
        string FirmwareType = string.Empty;
        bool bFirmwareUpdated = false;
        #endregion Variable Declaration

        #region Main Code 
        public DeviceDetails(DisplayDevice BLEDevice, string strPassword = "")
        {
            try
            {
                InitializeComponent();
                sPassword = strPassword;

                _connectedDevice = BLEDevice;
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
            RemoteSessionDetails._remote.OnReceive += (from, type, json) => { _ = Task.Run(async () => { await DeviceDetailReceive(from, type, json); }); };

            Task.Run(() => CheckRemoteSessionPingStatus());

            if (!_isInitialized)
            {
                _isInitialized = true;

                Dispatcher.Dispatch(async () =>
                {
                    await Initialization();
                });
            }
        }
        public async Task Initialization()
        {
            try
            {
                await ShowBusyindicatior(true);

                DynamicBinding();

                if (_connectedDevice != null)
                {
                    StartExecutingCommand();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public async Task DynamicBinding()
        {
            objCellmodel = new CellModel();
            objbleDevice = new BLEDevice();
            objReadValues = new ValidReadValues();
            objCapacity = new Capacity_ViewModel();
            objReadValues.VoltageCalibrationValue = new List<decimal>();
            SKFTabView.IsVisible = true;
            Benchmark.IsVisible = false;
            Networking.IsVisible = false;

            lblMacAddressID.Text = _connectedDevice.MacAddress;
            bool isValidMac = Regex.IsMatch(_connectedDevice.DeviceDetail, @"^([0-9A-Fa-f]{2}:){5}[0-9A-Fa-f]{2}$");
            if (isValidMac)
            {
                ModelDeviceName = _connectedDevice.DeviceName.Trim();
                BLEDeviceName = Regex.Replace(Convert.ToString(_connectedDevice.DeviceName).ToLower().Trim(), "[^0-9a-zA-Z:,]+", "");
            }
            else
            {
                ModelDeviceName = _connectedDevice.DeviceDetail.Substring(0, _connectedDevice.DeviceDetail.Length - 17);
                BLEDeviceName = Regex.Replace(Convert.ToString(ModelDeviceName).ToLower().Trim(), "[^0-9a-zA-Z:,]+", "");
            }

            if (!string.IsNullOrEmpty(BLEDeviceName))
            {
                BLEDeviceName = BLEDeviceName.Replace("-", "-");
            }

            await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.GetProtocolType, _connectedDevice.MacAddress);

            ResponceCount = 0;
            while (ResponceCount < 40)
            {
                await Task.Delay(1000);
                if (objDeviceInformation != null && objDeviceInformation.CustomCollectionHeatingPadTimer != null && objDeviceInformation.CustomCollectionHeatingPadTimer.Count() > 0)
                {
                    break;
                }

                if (ResponceCount == 20)
                {
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.GetProtocolType, _connectedDevice.MacAddress);
                }

                ResponceCount++;
            }

            //if (!objReadValues._IsSFKH2Device)
            //{
            //    FirmwareUpdate.IsVisible = false;
            //}

            sfbDetailsPower.BackgroundColor = Color.FromHex("#f0c000");
            lblDetailsPower.TextColor = Color.FromHex("#ffffff");

            SKFTabView.SelectedIndex = 1;

            temprature1 = System.Decimal.Zero;
            temprature2 = System.Decimal.Zero;
            temprature3 = System.Decimal.Zero;
            arr03RecBytes = new List<byte[]>();
            arr04RecBytes = new List<byte[]>();
            arrErrorCountRecBytes = new List<byte[]>();
            arrD7RecBytes = new List<byte[]>();
            arrE8RecBytes = new List<byte[]>();

            rangesliderMaxChargeAmps.BindingContext = objDeviceInformation;
            rangesliderMaxChargeAmps.Minimum = objDeviceInformation.MaxChargeAmpsStart;
            rangesliderMaxChargeAmps.Maximum = objDeviceInformation.MaxChargeAmpsEnd;
            rangesliderMaxChargeAmps.Interval = rangesliderMaxChargeAmps.StepSize = objDeviceInformation.MaxChargeAmpsInterval;

            rangesliderMaxDisChargeAmps.BindingContext = objDeviceInformation;
            rangesliderMaxDisChargeAmps.Minimum = objDeviceInformation.MaxDisChargeAmpsStart;
            rangesliderMaxDisChargeAmps.Maximum = objDeviceInformation.MaxDisChargeAmpsEnd;
            rangesliderMaxDisChargeAmps.Interval = rangesliderMaxDisChargeAmps.StepSize = objDeviceInformation.MaxDisChargeAmpsInterval;

            rangesliderFullCapacity.BindingContext = objDeviceInformation;
            rangesliderFullCapacity.Minimum = objDeviceInformation.CustomCollectionFullCapacity[0].Value;
            rangesliderFullCapacity.Maximum = objDeviceInformation.CustomCollectionFullCapacity[objDeviceInformation.CustomCollectionFullCapacity.Count - 1].Value;
            rangesliderFullCapacity.Interval = rangesliderFullCapacity.StepSize = objDeviceInformation.CapacityInterval;

            rangesliderLowVoltageCutOff.BindingContext = objDeviceInformation;
            rangesliderLowVoltageCutOff.Minimum = objDeviceInformation.CustomCollectionLowVoltageCutOff[0].Value;
            rangesliderLowVoltageCutOff.Maximum = objDeviceInformation.CustomCollectionLowVoltageCutOff[objDeviceInformation.CustomCollectionLowVoltageCutOff.Count - 1].Value;

            NF_rangesliderMaxSOC.BindingContext = objDeviceInformation;
            NF_rangesliderMaxSOC.Minimum = objDeviceInformation.CustomCollectionMaxSoc[0].Value;
            NF_rangesliderMaxSOC.Maximum = objDeviceInformation.CustomCollectionMaxSoc[objDeviceInformation.CustomCollectionMaxSoc.Count - 1].Value;

            NF_rangesliderMinSOC.BindingContext = objDeviceInformation;
            NF_rangesliderMinSOC.Minimum = objDeviceInformation.NF_CustomCollectionMinSoc[0].Value;
            NF_rangesliderMinSOC.Maximum = objDeviceInformation.NF_CustomCollectionMinSoc[objDeviceInformation.NF_CustomCollectionMinSoc.Count - 1].Value;

            grdOverAmpTimeOut.IsVisible = objDeviceInformation.OverAmpTimeOut;

            rangeCalibrateSOC.BindingContext = objDeviceInformation;
            rangeCalibrateSOC.Minimum = objDeviceInformation.CustomCollectionCalibrateSOC[0].Value;
            rangeCalibrateSOC.Maximum = objDeviceInformation.CustomCollectionCalibrateSOC[objDeviceInformation.CustomCollectionCalibrateSOC.Count - 1].Value;
            rangeCalibrateSOC.Interval = rangesliderFullCapacity.StepSize = objDeviceInformation.CalibrateSOCInterval;

            rangesliderHeatingPadTimer.BindingContext = objDeviceInformation;
            rangesliderHeatingPadTimer.Minimum = objDeviceInformation.CustomCollectionHeatingPadTimer[0].Value;
            rangesliderHeatingPadTimer.Maximum = objDeviceInformation.CustomCollectionHeatingPadTimer[objDeviceInformation.CustomCollectionHeatingPadTimer.Count - 1].Value;

            stackLoadDefaultValues.IsVisible = objDeviceInformation.RestoreDefaultValue;

            if (objReadValues._IsSFKH2Device)
            {
                rangesliderActivationVoltage.BindingContext = objDeviceInformation;
                rangesliderActivationVoltage.Minimum = objDeviceInformation.CustomCollectionActivationVoltage[0].Value;
                rangesliderActivationVoltage.Maximum = objDeviceInformation.CustomCollectionActivationVoltage[objDeviceInformation.CustomCollectionActivationVoltage.Count - 1].Value;
                rangesliderActivationVoltage.Interval = rangesliderActivationVoltage.StepSize = objDeviceInformation.ActivationVoltageInterval;

                rangesliderDeviationLimit.BindingContext = objDeviceInformation;
                rangesliderDeviationLimit.Minimum = objDeviceInformation.CustomCollectionDeviationLimit[0].Value;
                rangesliderDeviationLimit.Maximum = objDeviceInformation.CustomCollectionDeviationLimit[objDeviceInformation.CustomCollectionDeviationLimit.Count - 1].Value;
                rangesliderDeviationLimit.Interval = rangesliderDeviationLimit.StepSize = objDeviceInformation.DeviationLimitInterval;

                rangesliderTimeInterval.BindingContext = objDeviceInformation;
                rangesliderTimeInterval.Minimum = objDeviceInformation.CustomCollectionTimeInterval[0].Value;
                rangesliderTimeInterval.Maximum = objDeviceInformation.CustomCollectionTimeInterval[objDeviceInformation.CustomCollectionTimeInterval.Count - 1].Value;
                rangesliderTimeInterval.Interval = rangesliderTimeInterval.StepSize = objDeviceInformation.SOCTimeInterval;

                rangesliderHighSOCLimit.BindingContext = objDeviceInformation;
                rangesliderHighSOCLimit.Minimum = objDeviceInformation.CustomCollectionHighSOCLimit[0].Value;
                rangesliderHighSOCLimit.Maximum = objDeviceInformation.CustomCollectionHighSOCLimit[objDeviceInformation.CustomCollectionHighSOCLimit.Count - 1].Value;
                rangesliderHighSOCLimit.Interval = rangesliderHighSOCLimit.StepSize = objDeviceInformation.HighSOCLimitInterval;

                rangesliderChargePulse.BindingContext = objDeviceInformation;
                rangesliderChargePulse.Minimum = objDeviceInformation.CustomCollectionChargePulse[0].Value;
                rangesliderChargePulse.Maximum = objDeviceInformation.CustomCollectionChargePulse[objDeviceInformation.CustomCollectionChargePulse.Count - 1].Value;
                rangesliderChargePulse.Interval = rangesliderChargePulse.StepSize = objDeviceInformation.ChargePulseInterval;

                rangesliderBleGain.BindingContext = objDeviceInformation;
                rangesliderBleGain.Minimum = objDeviceInformation.CustomCollectionBleGain[0].Value;
                rangesliderBleGain.Maximum = objDeviceInformation.CustomCollectionBleGain[objDeviceInformation.CustomCollectionBleGain.Count - 1].Value;
                rangesliderBleGain.Interval = rangesliderBleGain.StepSize = objDeviceInformation.BleGainInterval;

                stackBMSLogs.IsVisible = true;
                Networking.IsVisible = true;
                grdFourthrow.IsVisible = true;
                grdFourthrowcln.IsVisible = true;

                stackSyncTime.IsVisible = true;

                stackVariableInterval.IsVisible = objReadValues._IsSFKH2Device;
                stackDeviceSettingsMain.IsVisible = objReadValues._IsSFKH2Device;
                stackSocAndTempLogClear.IsVisible = objReadValues._IsSFKH2Device;

                stackH2CalibrationInformation.IsVisible = objReadValues._IsSFKH2Device;

                stackRestartESP.IsVisible = objReadValues._IsSFKH2Device;

                imgBle.Source = "bleactiveicon.png";
                stackBleMain.IsVisible = true;
                lblBluetooth.TextColor = Color.FromHex("#0082FC");

                stackEnableBluetoothRestart.IsVisible = true;
                stackEnableDebugMode.IsVisible = true;
            }

            bool ShowFullCapacityPart = false;

            if (objDeviceInformation != null)
            {
                lblAboutModelName.Text = objDeviceInformation.ModelName;
                lblAboutModelNameInner.Text = objDeviceInformation.ModelName;
                ShowFullCapacityPart = objDeviceInformation.ShowFullCapacityPart;
                LowTemperatureHeatingSupport = objDeviceInformation.LowTemperatureHeatingSupport;
                LowTempStackLayout.IsVisible = objDeviceInformation.LowTemperatureHeatingSupport;
                Benchmark.IsVisible = objDeviceInformation.Benchmark;
                hasCaseTemp2 = objDeviceInformation.HasCaseTemp2;
                Is24VBattery = objDeviceInformation.Is24VBattery;
                lblWarranty.IsVisible = objDeviceInformation.ShowWarranty;

                if (objReadValues._IsSFKH2Device)
                {
                    stackH2BoardSettings.IsVisible = true;
                    stackActiveBalancerMain.IsVisible = true;
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        stackSinceFullChargeMainTab.IsVisible = false;
                        stackSinceFullChargeMainMBL.IsVisible = true;
                        stackRemaingAhMain.Orientation = StackOrientation.Horizontal;
                        stackRemaingAhMain.HorizontalOptions = LayoutOptions.StartAndExpand;
                        stackRemaingAhMain.Margin = new Thickness(7, 0, 0, 0);
                        stackCycleCountMain.Orientation = StackOrientation.Horizontal;
                        stackCycleCountMain.HorizontalOptions = LayoutOptions.StartAndExpand;
                        stackCycleCountMain.Margin = new Thickness(5, 17, 0, 0);
                        brdHomeStatus.HorizontalOptions = LayoutOptions.StartAndExpand;
                        lblCycleCount.HorizontalOptions = LayoutOptions.StartAndExpand;
                        lblCycleCount.Margin = new Thickness(5, 0, 0, 0);
                        lblRemainingL.HorizontalOptions = LayoutOptions.StartAndExpand;
                    }
                    else
                    {
                        stackCycleCountMain.Margin = new Thickness(5, 0, 0, 0);
                        stackRemaingAhMain.Margin = new Thickness(7, 0, 0, 0);

                        stackSinceFullChargeMainTab.IsVisible = true;
                        stackSinceFullChargeMainMBL.IsVisible = false;
                        stackCycleCountMain.Margin = new Thickness(5, 0, 0, 0);
                    }
                }
                else
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        stackRemaingAhMain.Orientation = StackOrientation.Horizontal;
                        stackRemaingAhMain.HorizontalOptions = LayoutOptions.StartAndExpand;
                        stackRemaingAhMain.Margin = new Thickness(7, 0, 0, 0);
                        stackCycleCountMain.Orientation = StackOrientation.Horizontal;
                        stackCycleCountMain.HorizontalOptions = LayoutOptions.StartAndExpand;
                        stackCycleCountMain.Margin = new Thickness(5, 20, 0, 0);
                        brdHomeStatus.HorizontalOptions = LayoutOptions.StartAndExpand;
                        lblCycleCount.HorizontalOptions = LayoutOptions.CenterAndExpand;
                        lblCycleCount.Margin = new Thickness(0, 0, 0, 0);
                        lblRemainingL.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    }
                    else
                    {
                        stackCycleCountMain.Margin = new Thickness(5, 0, 0, 0);
                    }
                }
            }

            if (LowTemperatureHeatingSupport)
            {
                Grid.SetRow(LowTempStackLayout, 3);
                LowTempStackLayout.RowDefinitions.Clear();

                LowTempStackLayout.RowDefinitions.Add(new RowDefinition { Height = 30 });
                LowTempStackLayout.RowDefinitions.Add(new RowDefinition { Height = 80 });
                LowTempStackLayout.RowDefinitions.Add(new RowDefinition { Height = 60 });
            }
            else
            {
                Grid.SetRow(LowTempStackLayout, 0);
            }

            if (Is24VBattery)
            {
                MainscaleValue.Minimum = 24;
                MainscaleValue.Maximum = 32;

                ScaleRangeValue1.StartValue = 24;
                ScaleRangeValue1.EndValue = 25;
                ScaleRangeValue1.Fill = Colors.Red;

                ScaleRangeValue2.StartValue = 25.01;
                ScaleRangeValue2.EndValue = 26;
                ScaleRangeValue2.Fill = Colors.Orange;

                ScaleRangeValue3.StartValue = 26.01;
                ScaleRangeValue3.EndValue = 27;
                ScaleRangeValue3.Fill = Colors.Yellow;

                ScaleRangeValue4.StartValue = 27.01;
                ScaleRangeValue4.EndValue = 32;
                ScaleRangeValue4.Fill = Colors.Green;
            }
            else
            {
                MainscaleValue.Minimum = 10;
                MainscaleValue.Maximum = 16;

                ScaleRangeValue1.StartValue = 10;
                ScaleRangeValue1.EndValue = 11;
                ScaleRangeValue1.Fill = Colors.Red;

                ScaleRangeValue2.StartValue = 11.01;
                ScaleRangeValue2.EndValue = 12;
                ScaleRangeValue2.Fill = Colors.Orange;

                ScaleRangeValue3.StartValue = 12.01;
                ScaleRangeValue3.EndValue = 13;
                ScaleRangeValue3.Fill = Colors.Yellow;

                ScaleRangeValue4.StartValue = 13.01;
                ScaleRangeValue4.EndValue = 16;
                ScaleRangeValue4.Fill = Colors.Green;
            }

            stackFullCapacityPart.IsVisible = stackFullCapacityHeaderPart.IsVisible = ShowFullCapacityPart;

            ListVoltageCalibration = new ObservableCollection<VoltageCalibration>();

            ListVoltageCalibration.Add(new VoltageCalibration { CellName = "Cell 1", CellVoltage = 3101, TextColor = Colors.White, BatteryImage = "batteryiconcalibration.png", BackgroundColor = Color.FromHex("#6E6E6E") });
            ListVoltageCalibration.Add(new VoltageCalibration { CellName = "Cell 2", CellVoltage = 3101, TextColor = Colors.White, BatteryImage = "batteryiconcalibration.png", BackgroundColor = Color.FromHex("#6E6E6E") });
            ListVoltageCalibration.Add(new VoltageCalibration { CellName = "Cell 3", CellVoltage = 3101, TextColor = Colors.White, BatteryImage = "batteryiconcalibration.png", BackgroundColor = Color.FromHex("#6E6E6E") });
            ListVoltageCalibration.Add(new VoltageCalibration { CellName = "Cell 4", CellVoltage = 3101, TextColor = Colors.White, BatteryImage = "batteryiconcalibration.png", BackgroundColor = Color.FromHex("#6E6E6E") });
            ListVoltageCalibration.Add(new VoltageCalibration { CellName = "Cell 5", CellVoltage = 3101, TextColor = Colors.White, BatteryImage = "batteryiconcalibration.png", BackgroundColor = Color.FromHex("#6E6E6E") });
            ListVoltageCalibration.Add(new VoltageCalibration { CellName = "Cell 6", CellVoltage = 3101, TextColor = Colors.White, BatteryImage = "batteryiconcalibration.png", BackgroundColor = Color.FromHex("#6E6E6E") });
            ListVoltageCalibration.Add(new VoltageCalibration { CellName = "Cell 7", CellVoltage = 3101, TextColor = Colors.White, BatteryImage = "batteryiconcalibration.png", BackgroundColor = Color.FromHex("#6E6E6E") });
            ListVoltageCalibration.Add(new VoltageCalibration { CellName = "Cell 8", CellVoltage = 3101, TextColor = Colors.White, BatteryImage = "batteryiconcalibration.png", BackgroundColor = Color.FromHex("#6E6E6E") });

            lstVoltageCalibration.ItemsSource = ListVoltageCalibration.ToArray();

            ListTemperatureCalibration = new ObservableCollection<TemperatureCalibration>();

            ListTemperatureCalibration.Add(new TemperatureCalibration { TempName = "BMS T.", TempValue = 0M, TextColor = Colors.White, TempratureImage = "bmst.png", BackgroundColor = Color.FromHex("#6E6E6E") });
            ListTemperatureCalibration.Add(new TemperatureCalibration { TempName = "Case T. 1", TempValue = 0M, TextColor = Colors.White, TempratureImage = "bmst.png", BackgroundColor = Color.FromHex("#6E6E6E") });
            ListTemperatureCalibration.Add(new TemperatureCalibration { TempName = "Case T. 2", TempValue = 0M, TextColor = Colors.White, TempratureImage = "bmst.png", BackgroundColor = Color.FromHex("#6E6E6E") });

            lstTemperatureCalibration.ItemsSource = ListTemperatureCalibration;

            OnSizeAllocated(DeviceWidth, DeviceHeight);
        }
        public async Task DeviceDetailReceive(string fromDevice, string Datatype, string jsondata)
        {
            try
            {
                string MacAddress = lblMacAddressID.Text + "_Old_Password";
                DeviceOldPassword = Preferences.Get(MacAddress, string.Empty);
                if (string.IsNullOrWhiteSpace(DeviceOldPassword))
                {
                    DeviceOldPassword = "012345";
                }
                nWriteCounter = 0;

                if (Datatype == RemoteSessionReceiveDataTypes.byteCommand.ToString())
                {
                    byte[] receivedBytes = Serializer.Deserialize<byte[]>(jsondata);
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await UpdateGUI(receivedBytes);
                        receivedBytes = null;
                    });
                }
                else if (Datatype == RemoteSessionReceiveDataTypes.DeviceConnectionStatus.ToString())
                {
                    IsDeviceConnectionStatus = false;
                    IsDeviceConnectionStatus = Serializer.Deserialize<bool>(jsondata);
                }
                else if (Datatype == RemoteSessionReceiveDataTypes.ReEstablishConnection.ToString())
                {
                    bool strRecconection = Serializer.Deserialize<bool>(jsondata);
                    await ReEstablishConnection(strRecconection);

                }
                else if (Datatype == RemoteSessionReceiveDataTypes.DisconnectDevice.ToString())
                {
                    IsReconnectFailed = true;
                    await ReEstablishConnection(true);
                    //await DisconnectDevice();
                }
                else if (Datatype == RemoteSessionReceiveDataTypes.ProtocolType.ToString())
                {
                    objReadValues._IsSFKH2Device = Serializer.Deserialize<bool>(jsondata);

                    objDeviceInformation = await DeviceDefaultValues.GetDeviceData(BLEDeviceName.ToLower());

                    if (objDeviceInformation != null)
                    {
                        objProtocol = await DeviceDefaultValues.GetProtocolDetails(objReadValues._IsSFKH2Device ? "SFKV4" : "SFKV1");
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
                else if (Datatype == RemoteSessionReceiveDataTypes.bFirmwareUpdated.ToString())
                {
                    bFirmwareUpdated = Serializer.Deserialize<bool>(jsondata);
                }
            }
            catch (Exception ex)
            {
                IsBMSConnected = false;
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        public async Task DisconnectDevice()
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    IsBusyIndicatorForReconnecing.IsVisible = false;
                    stackBusy.IsVisible = false;

                    await ShowDisplayPopup("Information", "Unable to connect battery.");
                });

                await Task.Delay(2000);

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Navigation.PushAsync(new MainPage(false));
                });
            }
            catch (Exception ex)
            {

            }
        }

        private async Task UpdateIconBlink()
        {
            try
            {
                imgUpdateIconForDevice.IsVisible = true;
                await Task.Delay(500);
                imgUpdateIconForDevice.IsVisible = false;
            }
            catch (Exception ex)
            {
            }
        }
        private async Task StartExecutingCommand()
        {
            try
            {
                await ShowBusyindicatior(true);

                await ReadBindUnbindStatus(RegisterEnum.CHIPID_STATUS);

                while (nWriteCounter < EstablishConnectionDatAttemps && !BreakWhileLoop)
                {
                    if (RunMainPageWhileLoop)
                    {
                        UpdateIconBlink();

                        if (SKFTabView.SelectedIndex == 1 || SKFTabView.SelectedIndex == 2 || SKFTabView.SelectedIndex == 4 || SKFTabView.SelectedIndex == 6)
                        {
                            await ReadBMSDetails03Command();

                            await Task.Delay(100);
                        }

                        if (!IsBMSConnected) { continue; }

                        if (SKFTabView.SelectedIndex == 2 || SKFTabView.SelectedIndex == 4 || SKFTabView.SelectedIndex == 6)
                        {
                            await ReadCellCount04Command();

                            await Task.Delay(100);
                        }

                        if (SKFTabView.SelectedIndex == 1 && objReadValues._IsSFKH2Device && UsedNewFirmware && CounterForActiveBalancerCommand >= 15)
                        {
                            await ReadActiveBalancer();

                            CounterForActiveBalancerCommand = 0;
                        }

                        if (SKFTabView.SelectedIndex == 1 && objReadValues.BatteryType == 2 && objReadValues._IsSFKH2Device && UsedNewFirmware && CounterFor09Command >= 15)
                        {
                            await ReadSOCAndHeatingMode();

                            CounterFor09Command = 0;
                        }

                        if (SKFTabView.SelectedIndex == 1 && objReadValues._IsSFKH2Device && CounterForFullyChargedDateCommand >= 30)
                        {
                            await ReadDaySinceFullCharge("date");

                            await Task.Delay(50);

                            await ReadDaySinceFullCharge();

                            CounterForFullyChargedDateCommand = 0;
                        }

                        if (SKFTabView.SelectedIndex == 1 && objReadValues.IsHighSOCMonitorOn && CounterForReadRegulateSOCActiveStatusCommand >= 60)
                        {
                            await ReadRegulateSOCActiveStatus();

                            CounterForReadRegulateSOCActiveStatusCommand = 0;
                        }

                        if (_ReadOnlyOncesForDevices)
                        {
                            _ReadOnlyOncesForDevices = false;

                            if (!string.IsNullOrWhiteSpace(sPassword))
                            {
                                CheckForDefaultPassword(sPassword);
                            }
                            else
                            {
                                CheckForDefaultPassword();
                            }

                            ReadHomeTab();

                            await OnHomeTabLoad();
                        }

                        int intervalSeconds = PullingRequestTime / 1000;

                        CounterFor09Command += intervalSeconds;
                        CounterForActiveBalancerCommand += intervalSeconds;
                        CounterForFullyChargedDateCommand += intervalSeconds;
                        CounterForReadRegulateSOCActiveStatusCommand += intervalSeconds;
                        await Task.Delay(PullingRequestTime);
                    }

                    if (nWriteCounter >= EstablishConnectionDatAttemps)
                    {
                        string NickName = Preferences.Get(lblMacAddressID.Text.ToString(), string.Empty);
                        if (string.IsNullOrWhiteSpace(NickName))
                        {
                            NickName = _connectedDevice.DeviceName.Trim();
                        }
                        await ShowDisplayPopup("Error", "Unable to connect : " + NickName);
                        MainPage Page = new MainPage(false);
                        await Navigation.PushAsync(Page);
                    }
                }

                await ShowBusyindicatior(false);
            }
            catch (Exception ex)
            {

            }
        }
        private async Task UpdateGUI(byte[] receivedBytes)
        {
            try
            {
                if (await VerifyByteUsingChecksum(receivedBytes) || CalibrationUpdateInProgress || BechmarkTestRunning)
                {
                    if ((Convert.ToInt32(((byte[])receivedBytes)[1]) == iReadRegister) && bRestoreDefaultValue)
                    {
                        bIsSendNExt = true;
                    }
                    if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "211"))
                    {
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01"))
                        {
                            objReadValues.IsSOCParamSet = true;
                        }
                        else if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "0" || ((byte[])receivedBytes)[3].ToString() == "00"))
                        {
                            objReadValues.IsSOCParamSet = false;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "212"))
                    {
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01"))
                        {
                            objReadValues.IsHighSOCMonitorOn = true;
                        }
                        else if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "0" || ((byte[])receivedBytes)[3].ToString() == "00"))
                        {
                            objReadValues.IsHighSOCMonitorOn = false;
                            NF_grdMaximumSOC.IsVisible = true;
                        }

                        NewFirmwareSocSetting.IsVisible = true;
                        if (objReadValues.BatteryType == 2)
                        {
                            if (objReadValues.IsHighSOCMonitorOn && objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = false;
                                NF_grdMinimumSOC.IsVisible = false;
                                NewFirmwareSocSetting.IsVisible = false;
                            }
                            else if (!objReadValues.IsHighSOCMonitorOn && !objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = true;
                                NF_grdMinimumSOC.IsVisible = true;
                            }
                            else if (objReadValues.IsHighSOCMonitorOn && !objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = false;
                                NF_grdMinimumSOC.IsVisible = true;
                            }
                            else if (!objReadValues.IsHighSOCMonitorOn && objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = true;
                                NF_grdMinimumSOC.IsVisible = false;
                            }

                            if (objReadValues.IsHighSOCMonitorOn)
                            {
                                imgHomeMaxSOC.Source = "regulatehighsoc.png";
                            }
                            else
                            {
                                imgHomeMaxSOC.Source = "socupperlimit.png";
                            }
                        }
                        else
                        {
                            NF_grdMaximumSOC.IsVisible = true;
                            NF_grdMinimumSOC.IsVisible = true;
                            imgHomeMaxSOC.Source = "socupperlimit.png";
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "245"))
                    {
                        objReadValues.IsActiveBalancerOn = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01"))
                        {
                            objReadValues.IsActiveBalancerOn = true;
                        }
                        checkBalancerOnOff.IsChecked = objReadValues.IsActiveBalancerOn;
                        //TwentyWarning.IsVisible = objReadValues.IsActiveBalancerOn;
                        //imgTwentyWarning.Source = "activebalancer.png";
                        //lblTwentyWarning.Text = "Active \nBalancer";
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "217"))
                    {
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01"))
                        {
                            objReadValues.IsActivationVoltage = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "243"))
                    {
                        objReadValues.IsChargePulseOn = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01") && rangesliderChargePulse.Value != 0)
                        {
                            objReadValues.IsChargePulseOn = true;
                        }
                        else if (((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "0" || ((byte[])receivedBytes)[3].ToString() == "00")) && rangesliderChargePulse.Value == 0)
                        {
                            objReadValues.IsChargePulseOn = false;
                        }

                        NewFirmwareSocSetting.IsVisible = true;
                        if (objReadValues.BatteryType == 2)
                        {
                            if (objReadValues.IsHighSOCMonitorOn && objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = false;
                                NF_grdMinimumSOC.IsVisible = false;
                                NewFirmwareSocSetting.IsVisible = false;
                            }
                            else if (!objReadValues.IsHighSOCMonitorOn && !objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = true;
                                NF_grdMinimumSOC.IsVisible = true;
                            }
                            else if (objReadValues.IsHighSOCMonitorOn && !objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = false;
                                NF_grdMinimumSOC.IsVisible = true;
                            }
                            else if (!objReadValues.IsHighSOCMonitorOn && objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = true;
                                NF_grdMinimumSOC.IsVisible = false;
                            }

                            if (objReadValues.IsChargePulseOn && rangesliderChargePulse.Value > 0)
                            {
                                imgHomeMinSOC.Source = "chargepulsealerts.png";
                            }
                            else
                            {
                                imgHomeMinSOC.Source = "soclowerlimit.png";
                            }
                        }
                        else
                        {
                            NF_grdMaximumSOC.IsVisible = true;
                            NF_grdMinimumSOC.IsVisible = true;
                            imgHomeMinSOC.Source = "soclowerlimit.png";
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "244"))
                    {
                        objReadValues.IsChargePulseTimeSet = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "01" || ((byte[])receivedBytes)[3].ToString() == "1"))
                        {
                            objReadValues.IsChargePulseTimeSet = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "193"))
                    {
                        objReadValues.IsActiveBalancerOn = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01"))
                        {
                            objReadValues.IsActiveBalancerOn = true;
                        }
                        TwentyWarning.IsVisible = objReadValues.IsActiveBalancerOn;
                        imgTwentyWarning.Source = "activebalancer.png";
                        lblTwentyWarning.Text = "Active \nBalancer";

                        // checkBalancerOnOff.IsChecked = objReadValues.IsActiveBalancerOn;
                        // stackBalancerSettings.IsVisible = objReadValues.IsActiveBalancerOn;
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "194"))
                    {
                        bBmsUsageLogClear = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01"))
                        {
                            bBmsUsageLogClear = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "232"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[0].ToString() == "204" && ((byte[])receivedBytes)[receivedBytes.Length - 1].ToString() == "207")
                            {
                                await CommandE8DisplayData(receivedBytes);
                            }
                            else
                            {
                                arrE8RecBytes.Clear();
                                if (arrE8RecBytes.Count == 0)
                                {
                                    arrE8RecBytes.Add(receivedBytes);
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "233"))
                    {
                        string strNicID = receivedBytes[3].ToString("X") + receivedBytes[4].ToString("X") + receivedBytes[5].ToString("X");
                        lblNicID.Text = Convert.ToString(Convert.ToInt64(strNicID, 16));
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "215"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[0].ToString() == "204" && ((byte[])receivedBytes)[receivedBytes.Length - 1].ToString() == "207")
                            {
                                await CommandD7DisplayData(receivedBytes);
                            }
                            else
                            {
                                arrD7RecBytes.Clear();
                                if (arrD7RecBytes.Count == 0)
                                {
                                    arrD7RecBytes.Add(receivedBytes);
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "213"))
                    {
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01"))
                        {
                            if (!TimeSyncronized)
                            {
                                TimeSyncronized = true;
                                await ShowDisplayPopup("Success", "Time synchronized sucessfully.");
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "214"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string hex = BitConverter.ToString(receivedBytes);
                            string[] subs = hex.Split('-');

                            if (subs.Length > 10)
                            {
                                int iYear = Convert.ToInt32((subs[3] + subs[4]), 16);
                                int iMonth = Convert.ToInt32(subs[5], 16);
                                int iDate = Convert.ToInt32(subs[6], 16);

                                int iHour = Convert.ToInt32(subs[7], 16);
                                int iMin = Convert.ToInt32(subs[8], 16);
                                int iSec = Convert.ToInt32(subs[9], 16);

                                string sBmsTime = iDate.ToString().PadLeft(2, '0') + "/" + iMonth.ToString().PadLeft(2, '0') + "/" + iYear.ToString().PadLeft(4, '0') + " " + iHour.ToString().PadLeft(2, '0') + ":" + iMin.ToString().PadLeft(2, '0') + ":" + iSec.ToString().PadLeft(2, '0');

                                DateTime start = new DateTime();
                                try
                                {
                                    start = DateTime.ParseExact(sBmsTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);
                                    stackBMSTime.IsVisible = true;
                                    lblBMSTime.Text = start.ToString("yyyy/MM/dd hh:mm:ss tt");
                                }
                                catch (Exception ex)
                                {
                                    stackBMSTime.IsVisible = true;
                                    lblBMSTime.Text = start.ToString("yyyy/MM/dd hh:mm:ss tt");
                                }

                                try
                                {
                                    DateTime end = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    tsDiffrence = end - start;
                                }
                                catch (Exception ex)
                                {
                                    tsDiffrence = DateTime.Now - start;
                                }

                                if (tsDiffrence.TotalMinutes >= 1 || tsDiffrence.TotalMinutes <= -1)
                                {
                                    if (!TimeSyncronized && CurrentPopup != "ComfirmationPopUpRTCSync")
                                    {
                                        //await ShowDisplayPopup("ComfirmationPopUpRTCSync", "You battery's clock needs to be synchronized with your device, click proceed to synchronize it.");
                                    }
                                }
                                else
                                {
                                    TimeSyncronized = true;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "241"))
                    {
                        if (receivedBytes.Count() >= 7)
                        {
                            int iDays = 0;
                            iDays = ((byte[])receivedBytes)[3];

                            objReadValues.DaySinceFullCharge = iDays;

                            if (iDays >= 0 && iDays <= 7)
                            {
                                frmSinceFullChargeTab.BackgroundColor = Color.FromHex("#14B729");
                                frmSinceFullChargeMbl.BackgroundColor = Color.FromHex("#14B729");
                            }
                            else if (iDays >= 8 && iDays <= 15)
                            {
                                frmSinceFullChargeTab.BackgroundColor = Color.FromHex("#F0C000");
                                frmSinceFullChargeMbl.BackgroundColor = Color.FromHex("#F0C000");
                            }
                            else if (iDays >= 15 && iDays <= 22)
                            {
                                frmSinceFullChargeTab.BackgroundColor = Color.FromHex("#F99C00");
                                frmSinceFullChargeMbl.BackgroundColor = Color.FromHex("#F99C00");
                            }
                            else if (iDays >= 23)
                            {
                                frmSinceFullChargeTab.BackgroundColor = Color.FromHex("#DC2624");
                                frmSinceFullChargeMbl.BackgroundColor = Color.FromHex("#DC2624");
                            }

                            if (lblFullyChargedDate.Text == "N/A")
                            {
                                frmSinceFullChargeTab.BackgroundColor = Color.FromHex("#14B729");
                                frmSinceFullChargeMbl.BackgroundColor = Color.FromHex("#14B729");

                                txtDaySinceFullCharge.Text = "?";
                                lblSinceFullChargeTab.Text = "?";
                                lblSinceFullChargeMBL.Text = "?";
                                lblDaySinceFullChargeValue.Text = "?" + " Days";
                            }
                            else
                            {
                                txtDaySinceFullCharge.Text = Convert.ToString(iDays);
                                lblSinceFullChargeTab.Text = Convert.ToString(iDays);
                                lblSinceFullChargeMBL.Text = Convert.ToString(iDays);
                                lblDaySinceFullChargeValue.Text = iDays + " Days";

                                if (!bSinceFullChargeBlink && iDays >= 23)
                                {
                                    SinceFullChargeBlink();
                                }
                                else if (bSinceFullChargeBlink && iDays < 23)
                                {
                                    frmSinceFullChargeTab.IsVisible = frmSinceFullChargeMbl.IsVisible = true;
                                    bSinceFullChargeBlink = false;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "246"))
                    {
                        objReadValues.IsBleGain = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "0" || ((byte[])receivedBytes)[3].ToString() == "00"))
                        {
                            objReadValues.IsBleGain = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "247"))
                    {
                        objReadValues.IsBalancerCutOff = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "0" || ((byte[])receivedBytes)[3].ToString() == "00"))
                        {
                            objReadValues.IsBalancerCutOff = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "248"))
                    {
                        objReadValues.H2FirmwareVersion = receivedBytes[3] + "." + receivedBytes[4] + receivedBytes[5];
                        lblH2FirmwareVersion.Text = "v" + objReadValues.H2FirmwareVersion;
                        if (Convert.ToDouble(objReadValues.H2FirmwareVersion) >= 6.4)
                        {
                            UsedNewFirmware = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "194"))
                    {
                        bResetLogFaults = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "01" || ((byte[])receivedBytes)[3].ToString() == "1"))
                        {
                            bResetLogFaults = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "249"))
                    {
                        objReadValues.BatteryType = Convert.ToInt32(((byte[])receivedBytes)[3].ToString(), 16);
                        CurrentBattType = objReadValues.BatteryType;
                        if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "01" || ((byte[])receivedBytes)[3].ToString() == "1"))
                        {
                            rdTypeOne.IsChecked = true;
                            stackActiveBalancerMain.IsVisible = stackH2BoardSettings.IsVisible = stackBMSLogs.IsVisible = stackActiveBalancerCutOffMain.IsVisible = false;
                            if (objDeviceInformation.ModelName.Contains("SFK315EX"))
                            {
                                stackBMSLogs.IsVisible = true;
                            }
                        }
                        else if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "02" || ((byte[])receivedBytes)[3].ToString() == "2"))
                        {
                            rdTypeTwo.IsChecked = true;
                            stackActiveBalancerMain.IsVisible = stackH2BoardSettings.IsVisible = stackBMSLogs.IsVisible = stackActiveBalancerCutOffMain.IsVisible = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "209"))
                    {
                        IsTimeInterval = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "01" || ((byte[])receivedBytes)[3].ToString() == "1"))
                        {
                            IsTimeInterval = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "178"))
                    {
                        objReadValues.IsDeviceBind = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "01" || ((byte[])receivedBytes)[3].ToString() == "1"))
                        {
                            objReadValues.IsDeviceBind = true;
                            CheckBoxBindUnbind.IsChecked = true;
                        }
                        else if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "02" || ((byte[])receivedBytes)[3].ToString() == "2"))
                        {
                            objReadValues.IsDeviceBind = false;
                            CheckBoxBindUnbind.IsChecked = false;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "181"))
                    {
                        BreakWhileLoop = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "01" || ((byte[])receivedBytes)[3].ToString() == "1"))
                        {
                            SKFTabView.IsVisible = true;
                        }
                        else if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "00" || ((byte[])receivedBytes)[3].ToString() == "0"))
                        {
                            BreakWhileLoop = true;
                            SKFTabView.IsVisible = false;
                            await ShowBusyindicatior(false);
                            await ShowDisplayPopup("BmsMismatch", "Warning BMS Mismatch!\nUnable to receive device data.");
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "177"))
                    {
                        if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3"))
                        {
                            string strResetCount = ((byte[])receivedBytes)[3].ToString();
                            stackBMSResetCount.IsVisible = true;
                            lblBMSResetCount.Text = strResetCount;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "161"))
                    {
                        if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "00" || ((byte[])receivedBytes)[3].ToString() == "0"))
                        {
                            objReadValues.IsDebugModeOn = false;
                        }
                        else if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "01" || ((byte[])receivedBytes)[3].ToString() == "1"))
                        {
                            objReadValues.IsDebugModeOn = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "195"))
                    {
                        if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "00" || ((byte[])receivedBytes)[3].ToString() == "0"))
                        {

                        }
                        else if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "01" || ((byte[])receivedBytes)[3].ToString() == "1"))
                        {

                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "198"))
                    {
                        stackCalibrationAccessed.IsVisible = true;
                        lblCalibrationAccessed.Text = ((byte[])receivedBytes)[3].ToString();
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "199"))
                    {
                        if (receivedBytes.Count() > 7)
                        {
                            string sFullChargeDate = string.Empty;

                            string year = ((byte[])receivedBytes)[3].ToString("X").PadLeft(2, '0') + ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string month = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');
                            string day = ((byte[])receivedBytes)[6].ToString("X").PadLeft(2, '0');

                            sFullChargeDate = Convert.ToString(Convert.ToInt32(year, 16)) + "/" + Convert.ToString(Convert.ToInt32(month, 16)) + "/" + Convert.ToString(Convert.ToInt32(day, 16));

                            TwentyTwoWarning.IsVisible = false;
                            if (Convert.ToInt32(year, 16) > 0 && Convert.ToInt32(month, 16) > 0 && Convert.ToInt32(day, 16) > 0)
                            {
                                lblFullyChargedDate.Text = sFullChargeDate;
                            }
                            else
                            {
                                lblFullyChargedDate.Text = "N/A";

                                frmSinceFullChargeTab.BackgroundColor = Color.FromHex("#14B729");
                                frmSinceFullChargeMbl.BackgroundColor = Color.FromHex("#14B729");

                                txtDaySinceFullCharge.Text = "?";
                                lblSinceFullChargeTab.Text = "?";
                                lblSinceFullChargeMBL.Text = "?";
                                lblDaySinceFullChargeValue.Text = "?" + " Days";

                                TwentyTwoWarning.IsVisible = true;
                                imgTwentyTwoWarning.Source = "fullchargerecommendation.png";
                                lblTwentyTwoWarning.Text = "Calibrate \nSOC";

                                if (!bFullyChargedDate)
                                {
                                    bFullyChargedDate = true;
                                    if (!IsCalibrationTabOpened)
                                    {
                                        //await ShowDisplayPopup("Warning", "Your battery needs to be charged to full voltage and SOC to set a fully charged date.");
                                    }
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "200"))
                    {
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "0" || ((byte[])receivedBytes)[3].ToString() == "00"))
                        {
                            objReadValues.IsEnableBLERestart = false;
                        }
                        else if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01"))
                        {
                            objReadValues.IsEnableBLERestart = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "201"))
                    {
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "0" || ((byte[])receivedBytes)[3].ToString() == "00"))
                        {
                            objReadValues.IsEnableBLERestart = false;
                        }
                        else if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01"))
                        {
                            objReadValues.IsEnableBLERestart = true;
                        }
                        CheckBoxEnableBluetoothRestart.IsChecked = objReadValues.IsEnableBLERestart;
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "227"))
                    {
                        if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "1" || ((byte[])receivedBytes)[3].ToString() == "01"))
                        {
                            // Regulate high soc running status
                            NineteenWarning.IsVisible = true;
                            imgNineteenWarning.Source = "regulatehighsoc.png";
                            lblNineteenWarning.Text = "RSOC\nActive";
                        }
                        else if ((((byte[])receivedBytes)[2].ToString() == "3" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "0" || ((byte[])receivedBytes)[3].ToString() == "00"))
                        {
                            NineteenWarning.IsVisible = false;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && (((byte[])receivedBytes)[1].ToString() == "163"))
                    {
                        objReadValues.IsDebugModeOn = false;
                        if ((((byte[])receivedBytes)[2].ToString() == "03" || ((byte[])receivedBytes)[2].ToString() == "3") && (((byte[])receivedBytes)[3].ToString() == "01" || ((byte[])receivedBytes)[3].ToString() == "1"))
                        {
                            objReadValues.IsDebugModeOn = true;
                        }
                        CheckBoxEnableDebugMode.IsChecked = objReadValues.IsDebugModeOn;
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "204" && ((byte[])receivedBytes)[1].ToString() == "9" || ((byte[])receivedBytes)[1].ToString() == "09")
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();

                        bPasswordRest = false;
                        if (sResult.StartsWith("dd090000000077") || sResult.StartsWith("cc0900000000"))
                        {
                            bPasswordRest = true;
                        }
                    }


                    if ((((byte[])receivedBytes)[0].ToString() != "221" && ((byte[])receivedBytes)[0].ToString() != "204") && ((byte[])receivedBytes)[receivedBytes.Length - 1].ToString() != "119")
                    {
                        if (((byte[])receivedBytes)[0].ToString() != "221" && ((byte[])receivedBytes)[receivedBytes.Length - 1].ToString() != "119")
                        {
                            if (arrD7RecBytes.Count() > 0)
                            {
                                arrD7RecBytes.Add(receivedBytes);
                            }

                            if (arrE8RecBytes.Count() > 0)
                            {
                                arrE8RecBytes.Add(receivedBytes);
                            }
                        }

                        if (arrD7RecBytes != null && arrD7RecBytes.Count >= 1 && ((byte[])arrD7RecBytes[0])[1].ToString() == "215")
                        {
                            byte[] BMSFinalDataD7 = new byte[arrD7RecBytes.Count()];
                            BMSFinalDataD7 = arrD7RecBytes[0].Concat(arrD7RecBytes[1]).ToArray();

                            await CommandD7DisplayData(BMSFinalDataD7);
                        }
                        if (arrE8RecBytes != null && arrE8RecBytes.Count >= 1 && ((byte[])arrE8RecBytes[0])[1].ToString() == "232")
                        {
                            byte[] BMSFinalDataE8 = new byte[arrE8RecBytes.Count()];
                            BMSFinalDataE8 = arrE8RecBytes[0].Concat(arrE8RecBytes[1]).ToArray();

                            await CommandE8DisplayData(BMSFinalDataE8);
                        }
                    }

                    if ((((byte[])receivedBytes)[0].ToString() == "204" || ((byte[])receivedBytes)[0].ToString() == "221") && ((((byte[])receivedBytes)[1].ToString() == "06") || (((byte[])receivedBytes)[1].ToString() == "6")))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();

                        if (sResult.StartsWith("dd060000000077") || sResult.StartsWith("cc0600000000"))
                        {
                            isAlreadyIntialLoadDecidedtheDefaultPassword = false;
                            if (PasswordCheck06CalledFrom_IntialLoad)
                            {
                                if (!string.IsNullOrWhiteSpace(sPassword) && sPassword != "123456" && sPassword != "012345")
                                {
                                    PasswordCheck06CalledFrom_IntialLoad = false;
                                    PasswordCheck06CalledFrom_Please_Enter_Password = true;
                                    PasswordCheck06CalledFrom_Reset_Password = false;

                                    checkEnablePin.IsChecked = true;
                                    grdEnablePinrowmain.Height = 150;
                                    checkEnablePin.IsChecked = regionResetPin.IsVisible = true;
                                }
                                else
                                {
                                    DeviceOldPassword = "012345";
                                    //intial load ma nakki thai gyu k password 012345 che
                                    isAlreadyIntialLoadDecidedtheDefaultPassword = true;
                                    checkEnablePin.IsChecked = false;
                                }

                            }
                            else if (PasswordCheck06CalledFrom_Please_Enter_Password)
                            {
                                checkEnablePin.IsChecked = true;
                            }
                            else if (PasswordCheck06CalledFrom_Reset_Password)
                            {
                                isOldPinWrong = false;
                                //no problem and go for setting new password 
                                SetNewPasswordforUser();
                            }
                        }
                        else
                        {
                            await ShowBusyindicatior(true);

                            if (PasswordCheck06CalledFrom_IntialLoad)
                            {
                                string Password = sPassword;
                                if (string.IsNullOrWhiteSpace(sPassword))
                                {
                                    Password = Preferences.Get((lblMacAddressID.Text + "_Password"), string.Empty);
                                }

                                if (!string.IsNullOrWhiteSpace(Password))
                                {
                                    var SubmitPassword = objProtocol.PrepareCustomerPassword(Convert.ToString(Password.Split('_')[0]));
                                    await SendCommandToBMS(SubmitPassword);

                                    PasswordCheck06CalledFrom_IntialLoad = false;
                                    PasswordCheck06CalledFrom_Please_Enter_Password = true;
                                    PasswordCheck06CalledFrom_Reset_Password = false;

                                    checkEnablePin.IsChecked = true;

                                    grdEnablePinrowmain.Height = 150;

                                    checkEnablePin.IsChecked = regionResetPin.IsVisible = true;
                                }
                                else
                                {
                                    checkEnablePin.IsChecked = true;

                                    grdEnablePinrowmain.Height = 150;

                                    checkEnablePin.IsChecked = regionResetPin.IsVisible = true;
                                }
                            }
                            else if (PasswordCheck06CalledFrom_Reset_Password)
                            {
                                lblMessageResetPassword.TextColor = Colors.Red;
                                lblMessageResetPassword.Text = "Old Pin is wrong";
                                isOldPinWrong = true;
                            }
                        }
                    }
                    else if ((((byte[])receivedBytes)[0].ToString() == "204" || ((byte[])receivedBytes)[0].ToString() == "221") && ((((byte[])receivedBytes)[1].ToString() == "07") || (((byte[])receivedBytes)[1].ToString() == "7")))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();
                        if (sResult == "dd070000000077" || sResult.StartsWith("cc0700000000"))
                        {
                            if (!isSystemisSettingDefaultPin)
                            {
                                lblMessageResetPassword.IsVisible = true;
                                lblMessageResetPassword.TextColor = Colors.Green;
                                lblMessageResetPassword.Text = "";
                                lblMessageResetPassword.Text = "";

                                ShowDisplayPopup("Password", "Pin has been updated");

                                DeviceOldPassword = txtEnterNewPassword.Text;
                                txtEnterNewPassword.Text = txtEnterConfirmPassword.Text = string.Empty;
                            }
                        }
                        else
                        {
                            lblMessageResetPassword.IsVisible = true;
                            lblMessageResetPassword.TextColor = Colors.Red;
                            lblMessageResetPassword.Text = "Something went wrong";
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "4" || ((byte[])receivedBytes)[1].ToString() == "04"))
                    {
                        try
                        {
                            int Count = Convert.ToInt32((((byte[])receivedBytes)[2].ToString("X") + ((byte[])receivedBytes)[3].ToString("X")), 16) / 2;
                            if (Count >= 8)
                            {
                                if (((byte[])receivedBytes)[0].ToString() == "221" && ((byte[])receivedBytes)[receivedBytes.Length - 1].ToString() == "119")
                                {
                                    await Command04DisplayData(receivedBytes);
                                }
                                else
                                {
                                    arr04RecBytes.Clear();
                                    if (arr04RecBytes.Count == 0)
                                    {
                                        arr04RecBytes.Add(receivedBytes);
                                    }
                                }
                            }
                            else
                            {
                                byte[] data = receivedBytes;
                                string hex = BitConverter.ToString(data);
                                string[] subs = hex.Split('-');
                                CellModel TempobjCellmodel = new CellModel();
                                TempobjCellmodel = objCellmodel;
                                TempCellViewModel eobjCellmodel = new TempCellViewModel();
                                eobjCellmodel = ConvertToTempCellViewModel(objCellmodel);
                                objReadValues.VoltageCalibrationValue = new List<decimal>();

                                if (ListVoltageCalibration != null && ListVoltageCalibration.Count() > 4)
                                {
                                    while (ListVoltageCalibration.Count > 4)
                                    {
                                        ListVoltageCalibration.RemoveAt(ListVoltageCalibration.Count - 1);
                                    }
                                    lstVoltageCalibration.ItemsSource = null;
                                    lstVoltageCalibration.ItemsSource = ListVoltageCalibration.ToArray();
                                }

                                if (subs != null && subs.Length > 0 && data[0].ToString() == "221")
                                {
                                    if (subs.Length > 4)
                                    {
                                        try
                                        {
                                            string cellCount = subs[2] + subs[3];
                                            int ncellCount = Convert.ToInt32(cellCount, 16);
                                            ncellCount = ncellCount / 2;
                                            if (ncellCount > 0)
                                            {
                                                if (!bSameCell)
                                                {
                                                    objCellmodel = new CellModel();
                                                }

                                                for (int i = 0; i < ncellCount; i++)
                                                {
                                                    int ntemp = (i * 2) + 4;

                                                    string hex_value = subs[ntemp] + subs[ntemp + 1];

                                                    //converting hex to integer
                                                    int int_value = Convert.ToInt32(hex_value, 16);

                                                    decimal dcellvolts = ((decimal)int_value / 1000.00M);
                                                    if (dcellvolts >= (decimal)1.5 && dcellvolts <= (decimal)4.5)
                                                    {
                                                        objCellmodel.Data[i].CellVolts = Math.Round(dcellvolts, 2);
                                                        objReadValues.VoltageCalibrationValue.Add(Math.Round(dcellvolts, 2));
                                                        ListVoltageCalibration[i].CellVoltage = int_value;

                                                        #region cellcolor 

                                                        if (dcellvolts >= 0 && dcellvolts <= 2.749M)
                                                        {
                                                            objCellmodel.Colors[i] = Colors.Red;
                                                        }
                                                        else if (dcellvolts >= 2.750M && dcellvolts <= 2.999M)
                                                        {
                                                            objCellmodel.Colors[i] = Colors.Orange;
                                                            objCellmodel.TextColors[i] = Colors.Black;
                                                        }
                                                        else if (dcellvolts >= 3M && dcellvolts <= 3.099M)
                                                        {
                                                            objCellmodel.Colors[i] = Colors.Yellow;
                                                            objCellmodel.TextColors[i] = Colors.Black;
                                                        }
                                                        else if (dcellvolts >= 3.1M && dcellvolts <= 4M)
                                                        {
                                                            objCellmodel.Colors[i] = Colors.Green;
                                                        }
                                                        else
                                                        {
                                                            objCellmodel.Colors[i] = Colors.Green;
                                                        }
                                                        #endregion cellcolor

                                                        if (i == 0)
                                                        {
                                                            objbleDevice.Cell1volt = dcellvolts;
                                                        }
                                                        else if (i == 1)
                                                        {
                                                            objbleDevice.Cell2volt = dcellvolts;
                                                        }
                                                        else if (i == 2)
                                                        {
                                                            objbleDevice.Cell3volt = dcellvolts;
                                                        }
                                                        else if (i == 3)
                                                        {
                                                            objbleDevice.Cell4volt = dcellvolts;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        objCellmodel = TempobjCellmodel;
                                                        break;
                                                    }
                                                }
                                                if (Device.Idiom == TargetIdiom.Phone)
                                                {
                                                    if (DeviceHeight > DeviceWidth)
                                                    {
                                                        lstViewSpanCount.SpanCount = 2;
                                                        lstTCViewSpanCount.SpanCount = 2;
                                                    }
                                                    else
                                                    {
                                                        lstViewSpanCount.SpanCount = 1;
                                                        lstTCViewSpanCount.SpanCount = 1;
                                                    }
                                                }
                                                else
                                                {
                                                    lstViewSpanCount.SpanCount = 1;
                                                    lstTCViewSpanCount.SpanCount = 1;
                                                }

                                                if (CallFunctionOneTime)
                                                {
                                                    SetDesignforVoltageCalibration();
                                                    CallFunctionOneTime = false;
                                                }

                                                if (ncellCount == 4)
                                                {
                                                    for (int i = ncellCount; i < 8;)
                                                    {
                                                        if (objCellmodel.Data.Count() > 4)
                                                        {
                                                            objCellmodel.Data.RemoveAt(i);
                                                            objCellmodel.Colors.RemoveAt(i);
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                            }

                                            chartItemSource.ItemsSource = objCellmodel.Data;
                                            chartItemSource.PaletteBrushes = objCellmodel.Colors;

                                            for (int i = 0; i < objCellmodel.Data.Count(); i++)
                                            {
                                                if (objCellmodel.Data[i].CellVolts == eobjCellmodel.Data[i].CellVolts)
                                                {
                                                    bSameCell = true;
                                                }
                                                else
                                                {
                                                    bSameCell = false;
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                            }
                            await ShowBusyindicatior(false);
                        }
                        catch { }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && ((byte[])receivedBytes)[1].ToString() == "3" || ((byte[])receivedBytes)[1].ToString() == "03")
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[0].ToString() == "221" && ((byte[])receivedBytes)[receivedBytes.Length - 1].ToString() == "119")
                            {
                                await Command03DisplayData(receivedBytes);
                            }
                            else
                            {
                                arr03RecBytes.Clear();
                                if (arr03RecBytes.Count == 0)
                                {
                                    arr03RecBytes.Add(receivedBytes);
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "10"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            UsedNewFirmware = true;
                            lblMCUID.Text = "";
                            for (int i = 4; i < receivedBytes.Count() - 3; i++)
                            {
                                lblMCUID.Text += Convert.ToString(receivedBytes[i]);
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && ((byte[])receivedBytes)[1].ToString() == "9" || ((byte[])receivedBytes)[1].ToString() == "09")
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string hex = BitConverter.ToString(receivedBytes);
                            string[] subs = hex.Split('-');
                            if (subs != null)
                            {
                                //HeatingStartTemp  - 10 C 
                                string temp1Vhex = subs[5] + subs[6];
                                int int_temp1Avalue = Convert.ToInt32(temp1Vhex, 16);
                                decimal dec_temp1 = ((decimal)int_temp1Avalue / 10) - 273.15M;
                                int intTempValue = Convert.ToInt32(dec_temp1);
                                objReadValues.HeatingStartTempValue = intTempValue;

                                if (Tempmap.ContainsKey(intTempValue))
                                {
                                    int value = IsFahrenheit ? Tempmap[intTempValue] : intTempValue;
                                    TempsliderLowTempValue = value;
                                }

                                //HeatingStopTemp   - 15 C
                                string temp2Vhex = subs[7] + subs[8];
                                int int_temp2Avalue = Convert.ToInt32(temp2Vhex, 16);
                                decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;
                                objReadValues.HeatingStopTempValue = dec_temp2;

                                //HeatingMode  -  2 Mode

                                int Mode = Convert.ToInt32(subs[9], 16);
                                if (Mode == 1 || Mode == 01)
                                {
                                    rdLowTempcharging.IsChecked = true;
                                    imgHeatingMode.Source = "lowtempchargingheatingmode.png";
                                    grdHeaterTest.IsVisible = true;
                                }
                                else if (Mode == 2 || Mode == 02)
                                {
                                    rdStandbyHeating.IsChecked = true;
                                    imgHeatingMode.Source = "standbyheating.png";
                                    grdHeaterTest.IsVisible = true;
                                }
                                else if (Mode == 0 || Mode == 00)
                                {
                                    rdDisabledORLowTempProtectionOnly.IsChecked = true;
                                    imgHeatingMode.Source = "disabledheatingmode.png";
                                    grdHeaterTest.IsVisible = false;
                                }
                                await SetLowTempHeatingMode(Mode);

                                objReadValues.HeatingModeValue = Mode;

                                // Soc Protection Bit -  01 
                                objReadValues.SocProtectionBitValue = Convert.ToInt32(subs[10], 16);

                                //Soc Max Value -  95% 
                                objReadValues.SocMaxValue = Convert.ToInt32(subs[11], 16);
                                lblSOCUpperLimit.Text = Convert.ToString(objReadValues.SocMaxValue) + "%";
                                NF_rangesliderMaxSOC.Value = objReadValues.SocMaxValue;

                                //Soc Min Value  -  50%   
                                objReadValues.SocMinValue = Convert.ToInt32(subs[12], 16);
                                lblSOCLowerLimit.Text = Convert.ToString(objReadValues.SocMinValue) + "%";
                                NF_rangesliderMinSOC.Value = objReadValues.SocMinValue;

                                if (objReadValues.HeatingModeValue == 2)
                                {
                                    int value = IsFahrenheit ? Tempmap[intTempValue] : intTempValue;
                                    string unit = IsFahrenheit ? "°F" : "°C";

                                    lblHomeHeatingMode.Text = value.ToString() + unit;
                                    NF_rangesliderLowTemp.Value = value;
                                }

                                if (!IsMaxSOCPopLoaded)
                                {
                                    if (objReadValues.SocMaxValue > 0 && objReadValues.SocMaxValue < 100 && !objReadValues.IsHighSOCMonitorOn)
                                    {
                                        //await ShowDisplayPopup("Warning", "Maximum SOC below 100% should only be used if batteries are in storage and not actively being used.");
                                    }
                                    IsMaxSOCPopLoaded = true;
                                }
                            }
                        }
                        else if ((((byte[])receivedBytes)[0].ToString() == "204" || ((byte[])receivedBytes)[0].ToString() == "221") && (((byte[])receivedBytes)[1].ToString() == "9" || ((byte[])receivedBytes)[1].ToString() == "09") && (((byte[])receivedBytes)[2].ToString() == "0" || ((byte[])receivedBytes)[2].ToString() == "00"))
                        {
                            //string[] arrayInt = new string[] { "221", "09", "00", "00", "00", "00", "77" };
                            //for (int i = 0; i < arrayInt.Length; i++)
                            //{
                            //    if (((byte[])receivedBytes)[i].ToString() == arrayInt[i]) { bPasswordRest = true; }
                            //    else { bPasswordRest = false; }
                            //}
                            bPasswordRest = true;
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
                            if (!string.IsNullOrWhiteSpace(BarcodeValue))
                            {
                                txtSerialNumber.Text = BarcodeValue;
                                lblModel.Text = BarcodeValue;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "36"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            objReadValues.MaxSOCReadValue = 0;
                            objRadioButtonViewModel = new RadioButtonViewModel();
                            objRadioButtonViewModel.MaximumSocData = new bool[3];

                            if (((byte[])receivedBytes)[4].ToString() == "12" && ((byte[])receivedBytes)[5].ToString() == "188")
                            {
                                objRadioButtonViewModel.MaximumSocData[0] = true;
                                objRadioButtonViewModel.MaximumSocData[1] = false;
                                objRadioButtonViewModel.MaximumSocData[2] = false;
                                objReadValues.MaxSOCReadValue = 60;

                                objReadValues.IsvalidMaxSOCReadValue = 3.26;

                                UpdateSOCModeImage("Service");

                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "12" && ((byte[])receivedBytes)[5].ToString() == "223")
                            {
                                objRadioButtonViewModel.MaximumSocData[0] = false;
                                objRadioButtonViewModel.MaximumSocData[1] = true;
                                objRadioButtonViewModel.MaximumSocData[2] = false;
                                objReadValues.MaxSOCReadValue = 70;

                                objReadValues.IsvalidMaxSOCReadValue = 3.295;

                                UpdateSOCModeImage("Storage");
                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "14" && ((byte[])receivedBytes)[5].ToString() == "66")
                            {
                                objRadioButtonViewModel.MaximumSocData[0] = false;
                                objRadioButtonViewModel.MaximumSocData[1] = false;
                                objRadioButtonViewModel.MaximumSocData[2] = true;
                                objReadValues.MaxSOCReadValue = 100;
                                objReadValues.IsvalidMaxSOCReadValue = 3.65;

                                UpdateSOCModeImage("Normal");
                            }
                            else
                            {
                                objRadioButtonViewModel.MaximumSocData[0] = false;
                                objRadioButtonViewModel.MaximumSocData[1] = false;
                                objRadioButtonViewModel.MaximumSocData[2] = true;
                                objReadValues.MaxSOCReadValue = 100;

                                objReadValues.IsvalidMaxSOCReadValue = 0;

                                UpdateSOCModeImage("Normal");
                            }

                            if (objReadValues.MaxSOCReadValue > 0)
                            {
                                rbMaximumSOCService.BindingContext = objRadioButtonViewModel;
                                rbMaximumSOCService.IsChecked = objRadioButtonViewModel.MaximumSocData[0];

                                rbMaximumSOCStorage.BindingContext = objRadioButtonViewModel;
                                rbMaximumSOCStorage.IsChecked = objRadioButtonViewModel.MaximumSocData[1];

                                rbMaximumSOCNormal.BindingContext = objRadioButtonViewModel;
                                rbMaximumSOCNormal.IsChecked = objRadioButtonViewModel.MaximumSocData[2];
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "37"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {

                            if (((byte[])receivedBytes)[4].ToString() == "12" && ((byte[])receivedBytes)[5].ToString() == "178")
                            {
                                objReadValues.IsvalidMaxSOCReadValue = 3.25;
                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "12" && ((byte[])receivedBytes)[5].ToString() == "198")
                            {
                                objReadValues.IsvalidMaxSOCReadValue = 3.27;
                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "13" && ((byte[])receivedBytes)[5].ToString() == "172")
                            {
                                objReadValues.IsvalidMaxSOCReadValue = 3.5;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "32"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (Is24VBattery)
                            {
                                if (((byte[])receivedBytes)[4].ToString() == "10" && ((byte[])receivedBytes)[5].ToString() == "48")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 26.08;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "10" && ((byte[])receivedBytes)[5].ToString() == "76")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 26.36;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "11" && ((byte[])receivedBytes)[5].ToString() == "104")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 29.2;
                                }
                            }
                            else
                            {
                                if (((byte[])receivedBytes)[4].ToString() == "5" && ((byte[])receivedBytes)[5].ToString() == "24")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 13.04;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "5" && ((byte[])receivedBytes)[5].ToString() == "38")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 13.18;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "5" && ((byte[])receivedBytes)[5].ToString() == "180")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 14.6;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "33"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (Is24VBattery)
                            {
                                if (((byte[])receivedBytes)[4].ToString() == "10" && ((byte[])receivedBytes)[5].ToString() == "40")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 26;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "10" && ((byte[])receivedBytes)[5].ToString() == "56")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 26.16;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "10" && ((byte[])receivedBytes)[5].ToString() == "240")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 28;
                                }
                            }
                            else
                            {
                                if (((byte[])receivedBytes)[4].ToString() == "5" && ((byte[])receivedBytes)[5].ToString() == "20")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 13;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "5" && ((byte[])receivedBytes)[5].ToString() == "28")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 13.08;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "5" && ((byte[])receivedBytes)[5].ToString() == "120")
                                {
                                    objReadValues.IsvalidMaxSOCReadValue = 14;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "23"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string cycleCount1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string cycleCount2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string cycleCountHexValue = cycleCount1 + cycleCount2;

                            int int_cycleCountvalue = Convert.ToInt32(cycleCountHexValue, 16);

                            lblCycleCount.Text = Convert.ToString(int_cycleCountvalue);
                        }
                        else if (bResetCycleCount)
                        {
                            if (((byte[])receivedBytes)[4].ToString() != "0" && (((byte[])receivedBytes)[5].ToString() != "0") && (((byte[])receivedBytes)[6].ToString() != "119"))
                            {
                                bResetCycleCount = false;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "170"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[0].ToString() == "221" && ((byte[])receivedBytes)[receivedBytes.Length - 1].ToString() == "119")
                            {
                                await CommandErrorCountDisplayData(receivedBytes);
                            }
                            else
                            {
                                arrErrorCountRecBytes.Clear();
                                if (arrErrorCountRecBytes.Count == 0)
                                {
                                    arrErrorCountRecBytes.Add(receivedBytes);
                                }
                            }
                        }
                    }
                    else if ((((byte[])receivedBytes)[0].ToString() == "221" || ((byte[])receivedBytes)[0].ToString() == "204") && (((byte[])receivedBytes)[1].ToString() == "21"))
                    {
                        string sResult = BitConverter.ToString(receivedBytes);
                        sResult = sResult.Replace("-", "").ToLower();

                        bManufatureDate = false;
                        if (sResult.StartsWith("dd150000") || sResult.StartsWith("cc150000"))
                        {
                            bManufatureDate = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "24"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string tempDectoHex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string tempDectoHex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string tempHexValue = tempDectoHex1 + tempDectoHex2;

                            int int_temp2Avalue = Convert.ToInt32(tempHexValue, 16);
                            decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;

                            int intTempValue = Convert.ToInt32(dec_temp2);

                            if (intTempValue == 45)
                            {
                                objReadValues.chargeMaxBatteryTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    objReadValues.chargeMaxBatteryTempValue = 113;
                                }
                                objReadValues.IsvalidChargeMaxBatteryTempValue = intTempValue;
                                rangesliderMaxBatteryTemp.Value = objReadValues.chargeMaxBatteryTempValue;
                            }
                            else if (intTempValue == 50)
                            {
                                objReadValues.chargeMaxBatteryTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    objReadValues.chargeMaxBatteryTempValue = 122;
                                }
                                objReadValues.IsvalidChargeMaxBatteryTempValue = intTempValue;
                                rangesliderMaxBatteryTemp.Value = objReadValues.chargeMaxBatteryTempValue;
                            }
                            else if (intTempValue == 55)
                            {
                                objReadValues.chargeMaxBatteryTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    objReadValues.chargeMaxBatteryTempValue = 131;
                                }
                                objReadValues.IsvalidChargeMaxBatteryTempValue = intTempValue;
                                rangesliderMaxBatteryTemp.Value = objReadValues.chargeMaxBatteryTempValue;
                            }
                            else if (intTempValue == 60)
                            {
                                objReadValues.chargeMaxBatteryTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    objReadValues.chargeMaxBatteryTempValue = 140;
                                }
                                objReadValues.IsvalidChargeMaxBatteryTempValue = intTempValue;
                                rangesliderMaxBatteryTemp.Value = objReadValues.chargeMaxBatteryTempValue;
                            }
                            else
                            {
                                objReadValues.IsvalidChargeMaxBatteryTempValue = 0;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "25"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string tempDectoHex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string tempDectoHex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string tempHexValue = tempDectoHex1 + tempDectoHex2;

                            int int_temp2Avalue = Convert.ToInt32(tempHexValue, 16);
                            decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;

                            int intTempValue = Convert.ToInt32(dec_temp2);

                            if (intTempValue == 35)
                            {
                                objReadValues.IsvalidChargeMaxBatteryTempValue = intTempValue;
                            }
                            else if (intTempValue == 40)
                            {
                                objReadValues.IsvalidChargeMaxBatteryTempValue = intTempValue;
                            }
                            else if (intTempValue == 45)
                            {
                                objReadValues.IsvalidChargeMaxBatteryTempValue = intTempValue;
                            }
                            else if (intTempValue == 50)
                            {
                                objReadValues.IsvalidChargeMaxBatteryTempValue = intTempValue;
                            }
                            else
                            {
                                objReadValues.IsvalidChargeMaxBatteryTempValue = 0;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "26"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string tempDectoHex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string tempDectoHex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string tempHexValue = tempDectoHex1 + tempDectoHex2;

                            int int_temp2Avalue = Convert.ToInt32(tempHexValue, 16);
                            decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;

                            int intTempValue = Convert.ToInt32(dec_temp2);

                            if (intTempValue == 1)
                            {
                                objReadValues.IsvalidLowTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    intTempValue = 35;
                                }
                                rangesliderLowTemp.Value = objReadValues.LowTempValue = intTempValue;
                            }
                            else if (intTempValue == 4)
                            {
                                objReadValues.IsvalidLowTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    intTempValue = 40;
                                }
                                rangesliderLowTemp.Value = objReadValues.LowTempValue = intTempValue;
                            }
                            else if (intTempValue == 7)
                            {
                                objReadValues.IsvalidLowTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    intTempValue = 45;
                                }
                                rangesliderLowTemp.Value = objReadValues.LowTempValue = intTempValue;
                            }
                            else if (intTempValue == 10)
                            {
                                objReadValues.IsvalidLowTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    intTempValue = 50;
                                }
                                rangesliderLowTemp.Value = objReadValues.LowTempValue = intTempValue;
                            }
                            else if (intTempValue == 13)
                            {
                                objReadValues.IsvalidLowTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    intTempValue = 55;
                                }
                                rangesliderLowTemp.Value = objReadValues.LowTempValue = intTempValue;
                            }

                            if (objReadValues.HeatingModeValue == 0 || objReadValues.HeatingModeValue == 1)
                            {
                                int value = intTempValue;
                                string unit = IsFahrenheit ? "°F" : "°C";

                                lblHomeHeatingMode.Text = value.ToString() + unit;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "27"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string tempDectoHex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string tempDectoHex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string tempHexValue = tempDectoHex1 + tempDectoHex2;

                            int int_temp2Avalue = Convert.ToInt32(tempHexValue, 16);
                            decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;

                            int intTempValue = Convert.ToInt32(dec_temp2);

                            objReadValues.IsvalidLowTempValue = intTempValue;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "28"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string tempDectoHex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string tempDectoHex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string tempHexValue = tempDectoHex1 + tempDectoHex2;

                            int int_temp2Avalue = Convert.ToInt32(tempHexValue, 16);
                            decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;

                            int intTempValue = Convert.ToInt32(dec_temp2);

                            if (intTempValue == 60)
                            {
                                objReadValues.LowTempDischargeValue = intTempValue;
                                objReadValues.IsValidLowTempDischargeValue = intTempValue;

                                objReadValues.dischargeMaxBatteryTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    objReadValues.dischargeMaxBatteryTempValue = 140;
                                }
                                objReadValues.IsvalidDischargeMaxBatteryTempValue = intTempValue;
                            }
                            else if (intTempValue == 55)
                            {
                                objReadValues.dischargeMaxBatteryTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    objReadValues.dischargeMaxBatteryTempValue = 131;
                                }
                                objReadValues.IsvalidDischargeMaxBatteryTempValue = intTempValue;
                            }
                            else if (intTempValue == 50)
                            {
                                objReadValues.dischargeMaxBatteryTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    objReadValues.dischargeMaxBatteryTempValue = 122;
                                }
                                objReadValues.IsvalidDischargeMaxBatteryTempValue = intTempValue;
                            }
                            else if (intTempValue == 45)
                            {
                                objReadValues.dischargeMaxBatteryTempValue = intTempValue;
                                if (IsFahrenheit)
                                {
                                    objReadValues.dischargeMaxBatteryTempValue = 113;
                                }
                                objReadValues.IsvalidDischargeMaxBatteryTempValue = intTempValue;
                            }
                            else
                            {
                                objReadValues.IsValidLowTempDischargeValue = 0;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "29"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string tempDectoHex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string tempDectoHex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string tempHexValue = tempDectoHex1 + tempDectoHex2;

                            int int_temp2Avalue = Convert.ToInt32(tempHexValue, 16);
                            decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;

                            int intTempValue = Convert.ToInt32(dec_temp2);

                            if (intTempValue == 50)
                            {
                                objReadValues.LowTempDischargeValue = intTempValue;
                                objReadValues.IsValidLowTempDischargeValue = intTempValue;

                                objReadValues.IsvalidDischargeMaxBatteryTempValue = intTempValue;
                            }
                            else if (intTempValue == 35)
                            {
                                objReadValues.IsvalidDischargeMaxBatteryTempValue = intTempValue;
                            }
                            else if (intTempValue == 40)
                            {
                                objReadValues.IsvalidDischargeMaxBatteryTempValue = intTempValue;
                            }
                            else if (intTempValue == 45)
                            {
                                objReadValues.IsvalidDischargeMaxBatteryTempValue = intTempValue;
                            }
                            else
                            {
                                objReadValues.IsValidLowTempDischargeValue = 0;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "30"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string tempDectoHex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string tempDectoHex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string tempHexValue = tempDectoHex1 + tempDectoHex2;

                            int int_temp2Avalue = Convert.ToInt32(tempHexValue, 16);
                            decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;

                            int intTempValue = Convert.ToInt32(dec_temp2);

                            if (intTempValue == -20 || intTempValue == -1)
                            {
                                objReadValues.LowTempDischargeValue = intTempValue;
                                objReadValues.IsValidLowTempDischargeValue = intTempValue;

                                if (intTempValue == -20)
                                {
                                    CheckBoxLowTempDischarg.IsChecked = true;
                                }
                                else
                                {
                                    CheckBoxLowTempDischarg.IsChecked = false;
                                }
                            }
                            else
                            {
                                CheckBoxLowTempDischarg.IsChecked = false;
                                objReadValues.IsValidLowTempDischargeValue = 0;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "31"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string tempDectoHex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string tempDectoHex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string tempHexValue = tempDectoHex1 + tempDectoHex2;

                            int int_temp2Avalue = Convert.ToInt32(tempHexValue, 16);
                            decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;

                            int intTempValue = Convert.ToInt32(dec_temp2);

                            if (intTempValue == -15 || intTempValue == 2)
                            {
                                objReadValues.LowTempDischargeValue = intTempValue;
                                objReadValues.IsValidLowTempDischargeValue = intTempValue;
                            }
                            else
                            {
                                objReadValues.IsValidLowTempDischargeValue = 0;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "38"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if ((((byte[])receivedBytes)[4].ToString() == "9" || ((byte[])receivedBytes)[4].ToString() == "09") && ((byte[])receivedBytes)[5].ToString() == "196")
                            {
                                rangesliderLowVoltageCutOff.Value = 10;
                                objReadValues.IsvalidMinSOCReadValue = 2.5;
                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "10" && ((byte[])receivedBytes)[5].ToString() == "140")
                            {
                                rangesliderLowVoltageCutOff.Value = 11;
                                objReadValues.IsvalidMinSOCReadValue = 2.7;
                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "11" && ((byte[])receivedBytes)[5].ToString() == "84")
                            {
                                rangesliderLowVoltageCutOff.Value = 12;
                                objReadValues.IsvalidMinSOCReadValue = 2.9;
                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "11" && ((byte[])receivedBytes)[5].ToString() == "184")
                            {
                                rangesliderLowVoltageCutOff.Value = 13;
                                objReadValues.IsvalidMinSOCReadValue = 3.0;
                            }
                            else
                            {
                                rangesliderLowVoltageCutOff.Value = 12;
                                objReadValues.IsvalidMinSOCReadValue = 0.0;
                            }
                            objReadValues.MinSOCReadValue = (int)rangesliderLowVoltageCutOff.Value;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "39"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "10" && ((byte[])receivedBytes)[5].ToString() == "140")
                            {
                                objReadValues.IsvalidMinSOCReadValue = 2.7;
                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "11" && ((byte[])receivedBytes)[5].ToString() == "84")
                            {
                                objReadValues.IsvalidMinSOCReadValue = 2.9;
                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "12" && ((byte[])receivedBytes)[5].ToString() == "28")
                            {
                                objReadValues.IsvalidMinSOCReadValue = 3.1;
                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "11" && ((byte[])receivedBytes)[5].ToString() == "184")
                            {
                                objReadValues.IsvalidMinSOCReadValue = 3.0;
                            }
                            else if (((byte[])receivedBytes)[4].ToString() == "11" && ((byte[])receivedBytes)[5].ToString() == "234")
                            {
                                objReadValues.IsvalidMinSOCReadValue = 3.05;
                            }
                            else
                            {
                                objReadValues.IsvalidMinSOCReadValue = 0.0;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "34"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (Is24VBattery)
                            {
                                if (((byte[])receivedBytes)[4].ToString() == "7" && ((byte[])receivedBytes)[5].ToString() == "208")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 20.0;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "8" && ((byte[])receivedBytes)[5].ToString() == "112")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 21.6;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "9" && ((byte[])receivedBytes)[5].ToString() == "16")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 23.2;
                                }
                                else if ((((byte[])receivedBytes)[4].ToString() == "9" || ((byte[])receivedBytes)[4].ToString() == "09") && ((byte[])receivedBytes)[5].ToString() == "96")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 24;
                                }
                                else
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 0.0;
                                }
                            }
                            else
                            {
                                if (((byte[])receivedBytes)[4].ToString() == "3" && ((byte[])receivedBytes)[5].ToString() == "232")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 10.0;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "4" && ((byte[])receivedBytes)[5].ToString() == "56")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 10.8;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "4" && ((byte[])receivedBytes)[5].ToString() == "136")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 11.6;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "4" && ((byte[])receivedBytes)[5].ToString() == "176")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 12;
                                }
                                else
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 0.0;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "35"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (Is24VBattery)
                            {
                                if (((byte[])receivedBytes)[4].ToString() == "8" && ((byte[])receivedBytes)[5].ToString() == "112")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 21.6;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "9" && ((byte[])receivedBytes)[5].ToString() == "16")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 23.2;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "9" && ((byte[])receivedBytes)[5].ToString() == "96")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 24.0;
                                }
                                else if ((((byte[])receivedBytes)[4].ToString() == "09" || ((byte[])receivedBytes)[4].ToString() == "9") && ((byte[])receivedBytes)[5].ToString() == "176")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 24.8;
                                }
                                else
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 0.0;
                                }
                            }
                            else
                            {
                                if (((byte[])receivedBytes)[4].ToString() == "4" && ((byte[])receivedBytes)[5].ToString() == "56")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 10.8;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "4" && ((byte[])receivedBytes)[5].ToString() == "136")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 11.6;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "4" && ((byte[])receivedBytes)[5].ToString() == "176")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 12.0;
                                }
                                else if (((byte[])receivedBytes)[4].ToString() == "4" && ((byte[])receivedBytes)[5].ToString() == "196")
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 12.2;
                                }
                                else
                                {
                                    objReadValues.IsvalidMinSOCReadValue = 0.0;
                                }
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "40"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            rangesliderMaxChargeAmps.Value = (Convert.ToInt32((((byte[])receivedBytes)[4] << 8) + (((byte[])receivedBytes)[5] & 255)) / 100);
                            objReadValues.MaxChargeAmpsReadValue = (int)rangesliderMaxChargeAmps.Value;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "41"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString().Length > 2)
                            {
                                string value = ((byte[])receivedBytes)[4].ToString("X") + ((byte[])receivedBytes)[5].ToString("X");
                                int iValue = Convert.ToInt32(value, 16);
                                iValue = 65536 - iValue;
                                rangesliderMaxDisChargeAmps.Value = (System.Math.Abs(iValue) / 100);
                            }
                            else { rangesliderMaxDisChargeAmps.Value = (Convert.ToInt32((((byte[])receivedBytes)[4] << 8) + (((byte[])receivedBytes)[5] & 255)) / 100); }
                            objReadValues.MaxDischargeAmpsReadValue = (int)rangesliderMaxDisChargeAmps.Value;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "63"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            objReadValues.OverAmpTimeOutReadValue = ((byte[])receivedBytes)[4];
                            rangesliderOverAmpTimeOut.Value = ((byte[])receivedBytes)[4];
                            if (((byte[])receivedBytes)[4].ToString() == "10" && ((byte[])receivedBytes)[5].ToString() == "32")
                            {
                                objReadValues.MaxDischargeDelayReadValue = "10-32";
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "45"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string sFaultRelease = (receivedBytes[4]).ToString("X") + (receivedBytes[5]).ToString("X");
                            int FaultRelease = Convert.ToInt32(sFaultRelease.PadLeft(4, '0'), 16);

                            List<int> bits = new List<int>();
                            for (int i = 0; i <= 15; i++)
                            {
                                bits.Add(((int)((FaultRelease >> i) & 0x01)));
                            }


                            objReadValues.FaultReleaseValue = 0;
                            objRadioButtonViewModel = new RadioButtonViewModel();
                            objRadioButtonViewModel.FaultReleaseData = new bool[2];

                            if (bits != null && bits.Count > 0)
                            {
                                if (bits[1] == 1)
                                {
                                    objRadioButtonViewModel.FaultReleaseData[0] = true;
                                    objRadioButtonViewModel.FaultReleaseData[1] = false;
                                    objReadValues.FaultReleaseValue = 1;

                                    objReadValues.IsValidFaultReleaseValue = 1;
                                }
                                else
                                {
                                    objRadioButtonViewModel.FaultReleaseData[0] = false;
                                    objRadioButtonViewModel.FaultReleaseData[1] = true;
                                    objReadValues.FaultReleaseValue = 0;

                                    objReadValues.IsValidFaultReleaseValue = 0;
                                }
                            }

                            rdManual.BindingContext = objRadioButtonViewModel;
                            rdManual.IsChecked = objRadioButtonViewModel.FaultReleaseData[0];

                            rdAutomatic.BindingContext = objRadioButtonViewModel;
                            rdAutomatic.IsChecked = objRadioButtonViewModel.FaultReleaseData[1];
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "16"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            rangesliderFullCapacity.Value = (Convert.ToInt32((((byte[])receivedBytes)[4] << 8) + (((byte[])receivedBytes)[5] & 255)) / 100);
                            objReadValues.FullCapacityValue = (int)rangesliderFullCapacity.Value;
                            objReadValues.IsvalidFullCapacityValue = objReadValues.FullCapacityValue;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "17"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            objReadValues.IsvalidFullCapacityValue = (Convert.ToInt32((((byte[])receivedBytes)[4] << 8) + (((byte[])receivedBytes)[5] & 255)) / 100);
                            // objReadValues.IsvalidFullCapacityValue = objReadValues.FullCapacityValue;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "18"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "13" && (((byte[])receivedBytes)[5].ToString() == "172"))
                            {
                                objCapacity.SingleFullCapacityValue = 3.500;
                                objCapacity.isValidSingleFull = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "19"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "10" && (((byte[])receivedBytes)[5].ToString() == "140"))
                            {
                                objCapacity.SingleCutOffCapacityValue = 2.700;
                                objCapacity.isValidSingleCutOff = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "71"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "13" && (((byte[])receivedBytes)[5].ToString() == "72"))
                            {
                                objCapacity.Capacity100Value = 3.400;
                                objCapacity.isValid100Capacity = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "66"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "13" && (((byte[])receivedBytes)[5].ToString() == "2"))
                            {
                                objCapacity.Capacity90Value = 3.330;
                                objCapacity.isValid90Capacity = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "50"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "13" && (((byte[])receivedBytes)[5].ToString() == "1"))
                            {
                                objCapacity.Capacity80Value = 3.329;
                                objCapacity.isValid80Capacity = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "67"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "13" && (((byte[])receivedBytes)[5].ToString() == "0"))
                            {
                                objCapacity.Capacity70Value = 3.328;
                                objCapacity.isValid70Capacity = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "51"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "12" && (((byte[])receivedBytes)[5].ToString() == "250"))
                            {
                                objCapacity.Capacity60Value = 3.322;
                                objCapacity.isValid60Capacity = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "68"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "12" && (((byte[])receivedBytes)[5].ToString() == "222"))
                            {
                                objCapacity.Capacity50Value = 3.294;
                                objCapacity.isValid50Capacity = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "52"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "12" && (((byte[])receivedBytes)[5].ToString() == "219"))
                            {
                                objCapacity.Capacity40Value = 3.291;
                                objCapacity.isValid40Capacity = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "69"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "12" && (((byte[])receivedBytes)[5].ToString() == "217"))
                            {
                                objCapacity.Capacity30Value = 3.289;
                                objCapacity.isValid30Capacity = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "53"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "12" && (((byte[])receivedBytes)[5].ToString() == "194"))
                            {
                                objCapacity.Capacity20Value = 3.266;
                                objCapacity.isValid20Capacity = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "70"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            if (((byte[])receivedBytes)[4].ToString() == "12" && (((byte[])receivedBytes)[5].ToString() == "143"))
                            {
                                objCapacity.Capacity10Value = 3.215;
                                objCapacity.isValid10Capacity = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "208"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string tempDectoHex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string tempDectoHex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string tempHexValue = tempDectoHex1 + tempDectoHex2;

                            int int_temp2Avalue = Convert.ToInt32(tempHexValue, 16);
                            decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;

                            objReadValues.CalibrateTempReadValue = Convert.ToInt32(dec_temp2);
                            objReadValues.IsvalidCalibrateTempValue = objReadValues.CalibrateTempReadValue;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "209"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string tempDectoHex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string tempDectoHex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string tempHexValue = tempDectoHex1 + tempDectoHex2;

                            int int_temp2Avalue = Convert.ToInt32(tempHexValue, 16);
                            decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;

                            objReadValues.CalibrateTempReadValue = Convert.ToInt32(dec_temp2);
                            objReadValues.IsvalidCalibrateTempValue = objReadValues.CalibrateTempReadValue;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && (((byte[])receivedBytes)[1].ToString() == "224"))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            string Hex1 = ((byte[])receivedBytes)[4].ToString("X").PadLeft(2, '0');
                            string Hex2 = ((byte[])receivedBytes)[5].ToString("X").PadLeft(2, '0');

                            string HexValue = Hex1 + Hex2;

                            int value = Convert.ToInt32(HexValue, 16);

                            objReadValues.CalibrateSOCValue = Convert.ToInt32(value);
                            objReadValues.IsvalidCalibrateSOC = objReadValues.CalibrateTempReadValue;
                        }
                        else
                        {
                            string sResult = BitConverter.ToString(receivedBytes);
                            sResult = sResult.Replace("-", "").ToLower();

                            bPasswordRest = false;
                            if (sResult.StartsWith("dde000000000") || sResult.StartsWith("cce000000000"))
                            {
                                objReadValues.bCalibrateSOCSuccess = true;
                            }
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && ((byte[])receivedBytes)[1].ToString() == "1" || ((byte[])receivedBytes)[1].ToString() == "01")
                    {
                        if (((byte[])receivedBytes)[4].ToString() == "0" && (((byte[])receivedBytes)[5].ToString() == "0") && (((byte[])receivedBytes)[6].ToString() == "119"))
                        {
                            bResetLogFaults = true;
                        }
                    }
                    else if (((byte[])receivedBytes)[0].ToString() == "221" && ((((byte[])receivedBytes)[1].ToString() == "07") || (((byte[])receivedBytes)[1].ToString() == "7")))
                    {
                        if (((byte[])receivedBytes).Length > 7)
                        {
                            //lblreadpassword.Text = receivedBytes.ToString();
                        }
                    }

                    if ((((byte[])receivedBytes)[0].ToString() != "221" && ((byte[])receivedBytes)[0].ToString() != "204") && ((byte[])receivedBytes)[receivedBytes.Length - 1].ToString() != "207")
                    {
                        if (arr03RecBytes.Count >= 1)
                        {
                            if (((byte[])receivedBytes)[0].ToString() != "221")
                            {
                                arr03RecBytes.Add(receivedBytes);
                            }

                            if (arr03RecBytes != null && arr03RecBytes.Count >= 1 && (((byte[])arr03RecBytes[0])[1].ToString() == "3" || ((byte[])arr03RecBytes[0])[1].ToString() == "03"))
                            {
                                byte[] BMSFinalData03 = new byte[arr03RecBytes.Count()];
                                if (arr03RecBytes.Count() >= 3)
                                {
                                    BMSFinalData03 = arr03RecBytes[0].Concat(arr03RecBytes[1]).Concat(arr03RecBytes[2]).ToArray();
                                }
                                else
                                {
                                    BMSFinalData03 = arr03RecBytes[0].Concat(arr03RecBytes[1]).ToArray();
                                }

                                await Command03DisplayData(BMSFinalData03);
                            }
                        }
                        else if (arr04RecBytes.Count == 1)
                        {
                            if (((byte[])receivedBytes)[0].ToString() != "221")
                            {
                                arr04RecBytes.Add(receivedBytes);
                            }
                            if (arr04RecBytes != null && arr04RecBytes.Count >= 1 && (((byte[])arr04RecBytes[0])[1].ToString() == "4" || ((byte[])arr04RecBytes[0])[1].ToString() == "04"))
                            {
                                byte[] BMSfinalData04 = new byte[arr04RecBytes.Count()];
                                BMSfinalData04 = arr04RecBytes[0].Concat(arr04RecBytes[1]).ToArray();

                                await Command04DisplayData(BMSfinalData04);
                            }
                        }
                        else
                        {
                            if (((byte[])receivedBytes)[0].ToString() != "221")
                            {
                                arrErrorCountRecBytes.Add(receivedBytes);
                            }
                            if (arrErrorCountRecBytes != null && arrErrorCountRecBytes.Count >= 1 && ((byte[])arrErrorCountRecBytes[0])[1].ToString() == "170")
                            {
                                try
                                {
                                    byte[] BMSfinalDataErrorCount = new byte[arrErrorCountRecBytes.Count()];
                                    BMSfinalDataErrorCount = arrErrorCountRecBytes[0].Concat(arrErrorCountRecBytes[1]).ToArray();

                                    await CommandErrorCountDisplayData(BMSfinalDataErrorCount);
                                    BMSfinalDataErrorCount = new byte[0];
                                }
                                catch
                                {
                                }
                                arrErrorCountRecBytes.Clear();
                            }
                            else
                            {
                                arrErrorCountRecBytes.Clear();
                            }
                        }
                    }
                }
                else
                {
                    await CheckSumFailer();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task CommandD7DisplayData(byte[] BMSFinalDataD7)
        {
            try
            {
                if (BMSFinalDataD7[BMSFinalDataD7.Length - 1].ToString() == "207")
                {
                    string hex = BitConverter.ToString(BMSFinalDataD7);
                    string[] subs = hex.Split('-');

                    if (subs.Count() > 4)
                    {
                        string packVhex = subs[3] + subs[4];
                        // int Time = (Convert.ToInt32(packVhex, 16) / 60);
                        int Time = Convert.ToInt32(packVhex, 16);
                        lblHourHighTemperatureValue.Text = Convert.ToString(Time);

                        if (Time > 0)
                        {
                            EleventhWarning.IsVisible = false;
                            imgEleventhWarning.Source = "loghourhightemperature.png";
                            lblEleventhWarning.Text = "High temp \n(Hour)";
                        }
                        else
                        {
                            if (!EleventhWarning.IsVisible)
                            {
                                EleventhWarning.IsVisible = false;
                            }
                        }
                    }

                    if (subs.Count() > 6)
                    {
                        string packVhex = subs[5] + subs[6];
                        // int Time = (Convert.ToInt32(packVhex, 16) / 60);
                        int Time = Convert.ToInt32(packVhex, 16);
                        lblHourLowTemperatureValue.Text = Convert.ToString(Time);

                        if (Time > 0)
                        {
                            TwelfthWarning.IsVisible = false;
                            imgTwelfthWarning.Source = "loghourlowtemperature.png";
                            lblTwelfthWarning.Text = "Low temp \n(Hour)";
                        }
                        else
                        {
                            if (!TwelfthWarning.IsVisible)
                            {
                                TwelfthWarning.IsVisible = false;
                            }
                        }
                    }

                    if (subs.Count() > 8)
                    {
                        string packVhex = subs[7] + subs[8];
                        // int Time = (Convert.ToInt32(packVhex, 16) / 60);
                        int Time = Convert.ToInt32(packVhex, 16);
                        lblHourHighSocValue.Text = Convert.ToString(Time);

                        if (Time > 0)
                        {
                            ThirteenthWarning.IsVisible = false;
                            imgThirteenthWarning.Source = "loghourhighsoc.png";
                            lblThirteenthWarning.Text = "High SOC \n(Hour)";
                        }
                        else
                        {
                            if (!ThirteenthWarning.IsVisible)
                            {
                                ThirteenthWarning.IsVisible = false;
                            }
                        }
                    }

                    if (subs.Count() > 10)
                    {
                        string packVhex = subs[9] + subs[10];
                        // int Time = (Convert.ToInt32(packVhex, 16) / 60);
                        int Time = Convert.ToInt32(packVhex, 16);
                        lblHourLowSocValue.Text = Convert.ToString(Time);

                        if (Time > 0)
                        {
                            FourteenthWarning.IsVisible = false;
                            imgFourteenthWarning.Source = "loghourlowsoc.png";
                            lblFourteenthWarning.Text = "Low SOC \n(Hour)";
                        }
                        else
                        {
                            if (!FourteenthWarning.IsVisible)
                            {
                                FourteenthWarning.IsVisible = false;
                            }
                        }
                    }

                    if (subs.Count() > 12)
                    {
                        string packVhex = subs[11] + subs[12];
                        // int Time = (Convert.ToInt32(packVhex, 16) / 60);
                        int Time = Convert.ToInt32(packVhex, 16);
                        lblCriticallyHourHighTemperatureValue.Text = Convert.ToString(Time);

                        if (Time > 0)
                        {
                            FifteenthWarning.IsVisible = false;
                            imgFifteenthWarning.Source = "logcriticallyhourhightemperature.png";
                            lblFifteenthWarning.Text = "Critically high\ntemp (Hour)";
                        }
                        else
                        {
                            if (!FifteenthWarning.IsVisible)
                            {
                                FifteenthWarning.IsVisible = false;
                            }
                        }
                    }

                    if (subs.Count() > 14)
                    {
                        string packVhex = subs[13] + subs[14];
                        // int Time = (Convert.ToInt32(packVhex, 16) / 60);
                        int Time = Convert.ToInt32(packVhex, 16);
                        lblCriticallyHourLowTemperatureValue.Text = Convert.ToString(Time);

                        if (Time > 0)
                        {
                            SixteenthWarning.IsVisible = false;
                            imgSixteenthWarning.Source = "logcriticallyhourlowtemperature.png";
                            lblSixteenthWarning.Text = "Critically low\ntemp (Hour)";
                        }
                        else
                        {
                            if (!SixteenthWarning.IsVisible)
                            {
                                SixteenthWarning.IsVisible = false;
                            }
                        }
                    }

                    if (subs.Count() > 16)
                    {
                        string packVhex = subs[15] + subs[16];
                        // int Time = (Convert.ToInt32(packVhex, 16) / 60);
                        int Time = Convert.ToInt32(packVhex, 16);
                        lblCriticallyLowSOCValue.Text = Convert.ToString(Time);

                        if (Time > 0)
                        {
                            SeventeenthWarning.IsVisible = false;
                            imgSeventeenthWarning.Source = "logcriticallylowsoc.png";
                            lblSeventeenthWarning.Text = "Critically Low\nSOC (Hour)";
                        }
                        else
                        {
                            if (!SeventeenthWarning.IsVisible)
                            {
                                SeventeenthWarning.IsVisible = false;
                            }
                        }
                    }

                    if (subs.Count() > 19)
                    {
                        string Time = Convert.ToInt32(subs[19], 16) + ":" + Convert.ToInt32(subs[18], 16);
                        lblAMPBoost.Text = "AMP Boost\n";
                        lblAMPBoostValue.Text = Time;

                        if (!string.IsNullOrWhiteSpace(Time) || true)
                        {
                            SeventeenthWarning.IsVisible = false;
                            imgSeventeenthWarning.Source = "logampboost.png";
                            lblSeventeenthWarning.Text = "AMP \nBoost";
                        }
                        else
                        {
                            if (!SeventeenthWarning.IsVisible)
                            {
                                SeventeenthWarning.IsVisible = false;
                            }
                        }
                    }

                    if (!EleventhWarning.IsVisible && !TwelfthWarning.IsVisible && !ThirteenthWarning.IsVisible && !FourteenthWarning.IsVisible && !FifteenthWarning.IsVisible && !SixteenthWarning.IsVisible && !SeventeenthWarning.IsVisible && !EighteenthWarning.IsVisible)
                    {
                        FirstWarning.IsVisible = true;

                        imgFirstWarning.Source = "normalcell.png";
                        lblFirstWarning.Text = "Normal";
                    }

                    if (Device.Idiom == TargetIdiom.Tablet)
                    {
                        if (DeviceHeight > DeviceWidth) // potrait
                        {
                            if (DeviceHeight > 1000)
                            {
                                await WarningAlertDesign(4);
                            }
                            else
                            {
                                await WarningAlertDesign(4);
                            }
                        }
                        else if (DeviceHeight < DeviceWidth) // landscape
                        {
                            await WarningAlertDesign(4);
                        }
                    }
                    else if (Device.Idiom == TargetIdiom.Phone)
                    {
                        await WarningAlertDesign(4);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task CommandE8DisplayData(byte[] BMSFinalDataE8)
        {
            try
            {
                if (BMSFinalDataE8[BMSFinalDataE8.Length - 1].ToString() == "207")
                {
                    string hex = BitConverter.ToString(BMSFinalDataE8);
                    string[] subs = hex.Split('-');

                    if (subs.Length > 4)
                    {
                        string packVhex = subs[3] + subs[4];
                        int DeviationLimit = Convert.ToInt32(packVhex, 16);
                        rangesliderDeviationLimit.Value = DeviationLimit;
                    }

                    if (subs.Length > 7)
                    {
                        string packVhex = subs[7] + subs[8];
                        int ActivationVoltage = Convert.ToInt32(packVhex, 16);

                        switch (ActivationVoltage)
                        {
                            case 3325:
                                rangesliderActivationVoltage.Value = 1;
                                break;
                            case 3350:
                                rangesliderActivationVoltage.Value = 2;
                                break;
                            case 3375:
                                rangesliderActivationVoltage.Value = 3;
                                break;
                            case 3400:
                                rangesliderActivationVoltage.Value = 4;
                                break;
                            case 3425:
                                rangesliderActivationVoltage.Value = 5;
                                break;
                            default:
                                rangesliderActivationVoltage.Value = 1;
                                break;
                        }
                    }

                    if (subs.Length > 11)
                    {
                        // Soc Time limit [11]
                        int SocTimeLimit = Convert.ToInt32(subs[11], 16);
                        rangesliderTimeInterval.Value = SocTimeLimit;
                    }

                    if (subs.Length > 12)
                    {
                        // Soc Limit [12]
                        int SocLimit = Convert.ToInt32(subs[12], 16);
                        rangesliderHighSOCLimit.Value = SocLimit;
                    }

                    if (subs.Length > 14)
                    {
                        // Soc On-Off
                        objReadValues.IsHighSOCMonitorOn = false;
                        if (subs[14] == "01" || subs[14] == "1")
                        {
                            objReadValues.IsHighSOCMonitorOn = true;
                            checkSOCOnOff.IsChecked = stackSOCParameter.IsVisible = objReadValues.IsHighSOCMonitorOn;
                        }

                        NewFirmwareSocSetting.IsVisible = true;
                        if (objReadValues.BatteryType == 2)
                        {
                            if (objReadValues.IsHighSOCMonitorOn && objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = false;
                                NF_grdMinimumSOC.IsVisible = false;
                                NewFirmwareSocSetting.IsVisible = false;
                            }
                            else if (!objReadValues.IsHighSOCMonitorOn && !objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = true;
                                NF_grdMinimumSOC.IsVisible = true;
                            }
                            else if (objReadValues.IsHighSOCMonitorOn && !objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = false;
                                NF_grdMinimumSOC.IsVisible = true;
                            }
                            else if (!objReadValues.IsHighSOCMonitorOn && objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = true;
                                NF_grdMinimumSOC.IsVisible = false;
                            }
                            if (objReadValues.IsHighSOCMonitorOn)
                            {
                                imgHomeMaxSOC.Source = "regulatehighsoc.png";
                            }
                            else
                            {
                                imgHomeMaxSOC.Source = "socupperlimit.png";
                            }
                        }
                        else
                        {
                            NF_grdMaximumSOC.IsVisible = true;
                            NF_grdMinimumSOC.IsVisible = true;
                            imgHomeMaxSOC.Source = "socupperlimit.png";
                        }
                    }

                    if (subs.Length > 19)
                    {
                        // Pulse Soc On-Off
                        objReadValues.IsChargePulseOn = false;
                        if (subs[18] == "01" || subs[18] == "1")
                        {
                            objReadValues.IsChargePulseOn = true;
                        }

                        rangesliderChargePulse.Value = 0;
                        if (objReadValues.IsChargePulseOn)
                        {
                            // Charge Pulse Hours
                            int ChargePulseHR = Convert.ToInt32(subs[19], 16);
                            rangesliderChargePulse.Value = ChargePulseHR;
                        }

                        NewFirmwareSocSetting.IsVisible = true;
                        if (objReadValues.BatteryType == 2)
                        {
                            if (objReadValues.IsHighSOCMonitorOn && objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = false;
                                NF_grdMinimumSOC.IsVisible = false;
                                NewFirmwareSocSetting.IsVisible = false;
                            }
                            else if (!objReadValues.IsHighSOCMonitorOn && !objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = true;
                                NF_grdMinimumSOC.IsVisible = true;
                            }
                            else if (objReadValues.IsHighSOCMonitorOn && !objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = false;
                                NF_grdMinimumSOC.IsVisible = true;
                            }
                            else if (!objReadValues.IsHighSOCMonitorOn && objReadValues.IsChargePulseOn)
                            {
                                NF_grdMaximumSOC.IsVisible = true;
                                NF_grdMinimumSOC.IsVisible = false;
                            }

                            if (objReadValues.IsChargePulseOn && rangesliderChargePulse.Value > 0)
                            {
                                imgHomeMinSOC.Source = "chargepulsealerts.png";
                            }
                            else
                            {
                                imgHomeMinSOC.Source = "soclowerlimit.png";
                            }
                        }
                        else
                        {
                            NF_grdMaximumSOC.IsVisible = true;
                            NF_grdMinimumSOC.IsVisible = true;
                            NewFirmwareSocSetting.IsVisible = true;
                            imgHomeMinSOC.Source = "soclowerlimit.png";
                        }
                    }

                    if (subs.Length > 17)
                    {
                        int iConnected = Convert.ToInt32(subs[16]);
                        int iDisconnected = Convert.ToInt32(subs[17]);

                        rangeAppConnectedInterval.Value = iConnected;
                        rangeAppDisconnectedInterval.Value = iDisconnected;
                    }

                    if (subs.Length > 20)
                    {
                        objReadValues.IsActiveBalancerOn = false;
                        if (subs[20] == "1" || subs[20] == "01")
                        {
                            objReadValues.IsActiveBalancerOn = true;
                        }
                        checkBalancerOnOff.IsChecked = objReadValues.IsActiveBalancerOn;
                        stackBalancerSettings.IsVisible = objReadValues.IsActiveBalancerOn;
                    }

                    if (subs.Length > 21)
                    {
                        // DB Gain  50 63 89                        
                        if (subs[21] == "5" || subs[21] == "05")
                        {
                            rangesliderBleGain.Value = 0;
                        }
                        else if (subs[21] == "6" || subs[21] == "06")
                        {
                            rangesliderBleGain.Value = 3;
                        }
                        else if (subs[21] == "7" || subs[21] == "07")
                        {
                            rangesliderBleGain.Value = 6;
                        }
                        else if (subs[21] == "8" || subs[21] == "08")
                        {
                            rangesliderBleGain.Value = 9;
                        }
                    }

                    if (subs.Length > 24)
                    {
                        string packVhex = subs[23] + subs[24];
                        int BalancerCutOff = Convert.ToInt32(packVhex, 16);
                        txtActiveBalancerCutOff.Text = Convert.ToString(BalancerCutOff);
                    }

                    if (subs.Length > 25)
                    {
                        CurrentBattType = Convert.ToInt32(subs[25]);
                        objReadValues.BatteryType = CurrentBattType;
                        if (CurrentBattType == 1)
                        {
                            rdTypeOne.IsChecked = true;
                            Networking.IsVisible = false;
                            stackActiveBalancerMain.IsVisible = stackH2BoardSettings.IsVisible = stackBMSLogs.IsVisible = stackActiveBalancerCutOffMain.IsVisible = false;
                            if (objDeviceInformation.ModelName.Contains("SFK315EX"))
                            {
                                stackBMSLogs.IsVisible = true;
                            }
                        }
                        else if (CurrentBattType == 2)
                        {
                            rdTypeTwo.IsChecked = true;
                            Networking.IsVisible = true;
                            stackActiveBalancerMain.IsVisible = stackH2BoardSettings.IsVisible = stackBMSLogs.IsVisible = stackActiveBalancerCutOffMain.IsVisible = true;
                        }
                    }

                }
                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    if (DeviceHeight > DeviceWidth) // portrait
                    {
                        if (DeviceHeight > 1000)
                        {
                            await WarningAlertDesign(4);
                        }
                        else
                        {
                            await WarningAlertDesign(4);
                        }
                    }
                    else if (DeviceHeight < DeviceWidth) // landscape
                    {
                        await WarningAlertDesign(4);
                    }
                }
                else if (Device.Idiom == TargetIdiom.Phone)
                {
                    await WarningAlertDesign(4);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task Command03DisplayData(byte[] BMSFinalData03)
        {
            try
            {
                if (BMSFinalData03[BMSFinalData03.Length - 1].ToString() == "119")
                {
                    string hex = BitConverter.ToString(BMSFinalData03);
                    string[] subs = hex.Split('-');
                    int SumOfByte = 0;
                    for (int i = 2; i < (subs.Count() - 3); i++)
                    {
                        SumOfByte = SumOfByte + Convert.ToInt32(subs[i], 16);
                    }
                    string CheckSum = (65536 - SumOfByte).ToString("X");

                    if (CheckSum == Convert.ToString(subs[subs.Count() - 3] + subs[subs.Count() - 2]) || CalibrationUpdateInProgress || BechmarkTestRunning)
                    {
                        arr03RecBytes.Clear();
                        try
                        {
                            decimal dVoltage = System.Decimal.Zero;
                            decimal dfinalAmp = System.Decimal.Zero;
                            decimal dFinalWatts = System.Decimal.Zero;
                            if (subs != null && subs.Length > 0 && BMSFinalData03[0].ToString() == "221")
                            {
                                if (subs.Length > 5)
                                {
                                    string packVhex = subs[4] + subs[5];
                                    int int_packvvalue = Convert.ToInt32(packVhex, 16);
                                    dVoltage = ((decimal)int_packvvalue / 100);
                                    lblpackVL.Text = Convert.ToString((decimal)int_packvvalue / 100);
                                    #region colors conditionally
                                    decimal dlblpackVL = ((decimal)int_packvvalue / 100);

                                    #endregion colors conditionally

                                    MainscaleValue.Pointers[0].Value = (double)Math.Round(dVoltage, 2);
                                    lblgaugeVolts.Text = Convert.ToString((double)Math.Round(dVoltage, 2)) + " Volts";
                                    objbleDevice.Voltes = Convert.ToDecimal(lblpackVL.Text);
                                }

                                if (subs.Length > 7)
                                {
                                    string currentAVhex = subs[6] + subs[7];
                                    int int_currentAvalue = Convert.ToInt32(currentAVhex, 16);

                                    int temp_amp1 = BMSFinalData03[6];
                                    if ((temp_amp1 & 0x80) == 0)
                                    {

                                    }
                                    else
                                    {
                                        temp_amp1 = (byte)(~(temp_amp1 - 0x01)) * -1;
                                    }
                                    temp_amp1 = temp_amp1 * 16 * 16;
                                    int temp_amp2 = BMSFinalData03[7];
                                    dfinalAmp = (decimal)(temp_amp1 + temp_amp2) / 100;


                                    lblAmpsL.Text = Convert.ToString(Math.Round(dfinalAmp, 1));
                                    objbleDevice.Amps = Convert.ToDecimal(dfinalAmp);
                                    lblidleCurrentValue.Text = Convert.ToString(Math.Round(Convert.ToDecimal(dfinalAmp) * 1000, 00));
                                    #region colors conditionally
                                    decimal dfinalampsRounded = Math.Round(dfinalAmp, 1);
                                    #endregion colors conditionally

                                    decimal dwattsfinalforColor = System.Decimal.Zero;

                                    dFinalWatts = (dfinalAmp * dVoltage);
                                    if (dFinalWatts < 0)
                                    {
                                        lblWattsL.Text = Convert.ToString(Math.Round((dFinalWatts * -1), 2));
                                        dwattsfinalforColor = Math.Round((dFinalWatts * -1), 2);
                                        lblgaugePowerHeader.Text = Convert.ToString(Math.Round((dFinalWatts * -1), 2)) + " Watts";
                                        lblpowergauge.Pointers[0].Value = (double)Math.Round((dFinalWatts * -1), 2);
                                        objbleDevice.Watts = Convert.ToDecimal((dFinalWatts * -1));
                                    }
                                    else
                                    {
                                        lblWattsL.Text = Convert.ToString(Math.Round(dFinalWatts, 2));
                                        dwattsfinalforColor = Math.Round(dFinalWatts, 2);
                                        lblgaugePowerHeader.Text = Convert.ToString(Math.Round(dFinalWatts, 2)) + " Watts";
                                        lblpowergauge.Pointers[0].Value = (double)Math.Round(dFinalWatts, 2);
                                        objbleDevice.Watts = Convert.ToDecimal(dFinalWatts);
                                    }

                                    lblgaugeAmsPositive.Text = Convert.ToString(Math.Round(dfinalAmp, 2)) + " Amps";
                                    lblgaugeAmsNegative.Text = Convert.ToString(Math.Round(dfinalAmp, 2)) + " Amps";

                                    gaugePositiveAms.IsVisible = gaugePositiveAms.IsEnabled = true;

                                    guageRadialAxisPositiveAms.Pointers[0].Value = (double)Math.Round(dfinalAmp, 2);
                                }

                                if (subs.Length > 9)
                                {
                                    var temp = BMSFinalData03[6];
                                    objReadValues.CalibrateSOCValue = Convert.ToInt32(temp);
                                    objReadValues.IsvalidCalibrateSOC = objReadValues.CalibrateSOCValue;
                                }

                                if (subs.Length > 28)
                                {
                                    TempratureCount = Convert.ToInt32(subs[26], 16);

                                    string temp1Vhex = subs[27] + subs[28];
                                    int int_temp1Avalue = Convert.ToInt32(temp1Vhex, 16);
                                    decimal dec_temp1 = ((decimal)int_temp1Avalue / 10) - 273.15M;
                                    decimal Calibration_temp1 = dec_temp1;
                                    if (IsFahrenheit)
                                    {
                                        dec_temp1 = (dec_temp1 * 9) / 5 + 32; //covert to feranite
                                    }
                                    decimal temprature1 = System.Decimal.Zero;
                                    temprature1 = dec_temp1;
                                    objbleDevice.BMSTempreture = Convert.ToDecimal(temprature1);

                                    lblBmsTemp.Text = Convert.ToString(temprature1);
                                    if (IsFahrenheit)
                                    {
                                        lblTempreatureL.Text = Convert.ToString(temprature1) + "°F";
                                    }
                                    else
                                    {
                                        lblTempreatureL.Text = Convert.ToString(temprature1) + "°C";
                                    }

                                    rangeSliderTempBMS.Pointers[0].Value = (double)temprature1;
                                    lblBmsTempScale.Pointers[0].Value = (double)temprature1;
                                    lblGaugeTempBMSHeader.Text = lblTempreatureL.Text;

                                    ListTemperatureCalibration[0].TempValue = Calibration_temp1;
                                    #region colors conditionally

                                    if ((temprature1 >= 20 && temprature1 <= 34 && IsFahrenheit) || (temprature1 >= 0 && temprature1 <= 1))
                                    {
                                        pointBmsTemp.Fill = Colors.DarkBlue;
                                    }
                                    else if ((temprature1 >= 34.01M && temprature1 <= 50 && IsFahrenheit) || (temprature1 >= 1.01M && temprature1 <= 10))
                                    {
                                        pointBmsTemp.Fill = Colors.LightBlue;
                                    }
                                    else if ((temprature1 >= 50.01M && temprature1 <= 60 && IsFahrenheit) || (temprature1 >= 10.01M && temprature1 <= 15))
                                    {
                                        pointBmsTemp.Fill = Colors.SeaGreen;
                                    }
                                    else if ((temprature1 >= 60.01M && temprature1 <= 85 && IsFahrenheit) || (temprature1 >= 15.01M && temprature1 <= 32))
                                    {
                                        pointBmsTemp.Fill = Colors.Green;
                                    }
                                    else if ((temprature1 >= 85.01M && temprature1 <= 95 && IsFahrenheit) || (temprature1 >= 32.01M && temprature1 <= 37))
                                    {
                                        pointBmsTemp.Fill = Colors.Orange;
                                    }
                                    else if ((temprature1 >= 95.01M && temprature1 <= 105 && IsFahrenheit) || (temprature1 >= 37.01M && temprature1 <= 48))
                                    {
                                        pointBmsTemp.Fill = Colors.DarkOrange;
                                    }
                                    else if ((temprature1 >= 105.01M && temprature1 <= 120 && IsFahrenheit) || (temprature1 >= 48.01M && temprature1 <= 60))
                                    {
                                        pointBmsTemp.Fill = Colors.OrangeRed;
                                    }
                                    #endregion colors conditionally
                                }

                                if (subs.Length > 30)
                                {
                                    string temp2Vhex = subs[29] + subs[30];
                                    int int_temp2Avalue = Convert.ToInt32(temp2Vhex, 16);
                                    decimal dec_temp2 = ((decimal)int_temp2Avalue / 10) - 273.15M;
                                    decimal Calibration_temp2 = dec_temp2;
                                    if (IsFahrenheit)
                                    {
                                        dec_temp2 = (dec_temp2 * 9) / 5 + 32; //covert to feranite
                                    }
                                    temprature2 = dec_temp2;
                                    objbleDevice.CaseTempreture = Convert.ToDecimal(temprature2);

                                    lblCaseTemp.Text = temprature2.ToString();
                                    if (IsFahrenheit)
                                    {
                                        lblTempreature2L.Text = temprature2.ToString() + "°F";
                                    }
                                    else
                                    {
                                        lblTempreature2L.Text = temprature2.ToString() + "°C";
                                    }

                                    rangeSliderTempCase.Pointers[0].Value = (double)temprature2;
                                    lblCaseTempScale.Pointers[0].Value = (double)temprature2;
                                    lblGaugeTempCaseHeader.Text = lblTempreature2L.Text;

                                    ListTemperatureCalibration[1].TempValue = Calibration_temp2;

                                    #region colors conditionally

                                    if ((temprature2 >= 20 && temprature2 <= 34 && IsFahrenheit) || (temprature2 >= -6 && temprature2 <= 1))
                                    {
                                        pointCaseTemp.Fill = Colors.DarkBlue;
                                    }
                                    else if ((temprature2 >= 34.01M && temprature2 <= 50 && IsFahrenheit) || (temprature2 >= 1.01M && temprature2 <= 10))
                                    {
                                        pointCaseTemp.Fill = Colors.LightBlue;
                                    }
                                    else if ((temprature2 >= 50.01M && temprature2 <= 60 && IsFahrenheit) || (temprature2 >= 10.01M && temprature2 <= 15))
                                    {
                                        pointCaseTemp.Fill = Colors.SeaGreen;
                                    }
                                    else if ((temprature2 >= 60.01M && temprature2 <= 85 && IsFahrenheit) || (temprature2 >= 15.01M && temprature2 <= 32))
                                    {
                                        pointCaseTemp.Fill = Colors.Green;
                                    }
                                    else if ((temprature2 >= 85.01M && temprature2 <= 95 && IsFahrenheit) || (temprature2 >= 32.01M && temprature2 <= 37))
                                    {
                                        pointCaseTemp.Fill = Colors.Orange;
                                    }
                                    else if ((temprature2 >= 95.01M && temprature2 <= 105 && IsFahrenheit) || (temprature2 >= 37.01M && temprature2 <= 48))
                                    {
                                        pointCaseTemp.Fill = Colors.DarkOrange;
                                    }
                                    else if ((temprature2 >= 105.01M && temprature2 <= 120 && IsFahrenheit) || (temprature2 >= 48.01M && temprature2 <= 60))
                                    {
                                        pointCaseTemp.Fill = Colors.OrangeRed;
                                    }

                                    #endregion colors conditionally
                                }

                                if (subs.Length > 32)
                                {
                                    string temp3Vhex = subs[31] + subs[32];
                                    int int_temp3Avalue = Convert.ToInt32(temp3Vhex, 16);
                                    decimal dec_temp3 = ((decimal)int_temp3Avalue / 10) - 273.15M;
                                    decimal Calibration_temp3 = dec_temp3;

                                    if (IsFahrenheit)
                                    {
                                        dec_temp3 = (dec_temp3 * 9) / 5 + 32; //covert to feranite
                                    }
                                    temprature3 = dec_temp3;
                                    lblCaseTempScale2.Pointers[0].Value = (double)temprature3;
                                    lblCaseTemp2.Text = temprature3.ToString();
                                    if (hasCaseTemp2)
                                    {
                                        if ((Calibration_temp3 > -100 && Calibration_temp3 < 100) && TempratureCount >= 3)
                                        {
                                            if (IsFahrenheit)
                                            {
                                                lblTempreature3L.Text = temprature3.ToString() + "°F";
                                            }
                                            else
                                            {
                                                lblTempreature3L.Text = temprature3.ToString() + "°C";
                                            }

                                            objbleDevice.CaseTempreture2 = Convert.ToDecimal(temprature3);
                                            ListTemperatureCalibration[2].TempValue = Calibration_temp3;
                                        }
                                        else
                                        {
                                            lblTempreature3L.Text = "N/A";

                                            ListTemperatureCalibration[2].TempValue = 0;
                                        }
                                    }
                                    else
                                    {
                                        if ((Calibration_temp3 > -100 && Calibration_temp3 < 100) && TempratureCount >= 3)
                                        {
                                            if (IsFahrenheit)
                                            {
                                                lblTempreature3L.Text = temprature3.ToString() + "°F";
                                            }
                                            else
                                            {
                                                lblTempreature3L.Text = temprature3.ToString() + "°C";
                                            }

                                            objbleDevice.CaseTempreture2 = Convert.ToDecimal(temprature3);
                                            ListTemperatureCalibration[2].TempValue = Calibration_temp3;
                                        }
                                        else
                                        {
                                            lblTempreature3L.Text = "N/A";
                                        }
                                    }
                                    #region colors conditionally

                                    if ((temprature3 >= 20 && temprature3 <= 34 && IsFahrenheit) || (temprature3 >= -6 && temprature3 <= 1))
                                    {
                                        pointCaseTemp2.Fill = Colors.DarkBlue;
                                    }
                                    else if ((temprature3 >= 34.01M && temprature3 <= 50 && IsFahrenheit) || (temprature3 >= 1.01M && temprature3 <= 10))
                                    {
                                        pointCaseTemp2.Fill = Colors.LightBlue;
                                    }
                                    else if ((temprature3 >= 50.01M && temprature3 <= 60 && IsFahrenheit) || (temprature3 >= 10.01M && temprature3 <= 15))
                                    {
                                        pointCaseTemp2.Fill = Colors.SeaGreen;
                                    }
                                    else if ((temprature3 >= 60.01M && temprature3 <= 85 && IsFahrenheit) || (temprature3 >= 15.01M && temprature3 <= 32))
                                    {
                                        pointCaseTemp2.Fill = Colors.Green;
                                    }
                                    else if ((temprature3 >= 85.01M && temprature3 <= 95 && IsFahrenheit) || (temprature3 >= 32.01M && temprature3 <= 37))
                                    {
                                        pointCaseTemp2.Fill = Colors.Orange;
                                    }
                                    else if ((temprature3 >= 95.01M && temprature3 <= 105 && IsFahrenheit) || (temprature3 >= 37.01M && temprature3 <= 48))
                                    {
                                        pointCaseTemp2.Fill = Colors.DarkOrange;
                                    }
                                    else if ((temprature3 >= 105.01M && temprature3 <= 120 && IsFahrenheit) || (temprature3 >= 48.01M && temprature3 <= 60))
                                    {
                                        pointCaseTemp2.Fill = Colors.OrangeRed;
                                    }

                                    #endregion colors conditionally
                                }

                                if (subs.Length > 11)
                                {
                                    string remaininghex = subs[8] + subs[9];
                                    int int_remainingvalue = Convert.ToInt32(remaininghex, 16);

                                    string fullhex = subs[10] + subs[11];
                                    int int_fullvalue = Convert.ToInt32(fullhex, 16);

                                    if (int_remainingvalue > 0)
                                    {
                                        lblRemainingL.Text = Convert.ToString(Math.Round((decimal)int_remainingvalue / 100, 2)) + " (AH)";
                                    }
                                    else
                                    {
                                        lblRemainingL.Text = Convert.ToString(Math.Round(0.0, 2)) + " (AH)";
                                    }

                                    if (dfinalAmp > 0)
                                    {
                                        if (!Convert.ToString(imgBatteryStatus.Source).ToLower().Replace("file: ", "").Trim().StartsWith("chargingenable"))
                                        {
                                            imgBatteryStatusMbl.Source = imgBatteryStatus.Source = "chargingenable.gif";
                                        }
                                        frmHomeBatteryStatusMbl.BackgroundColor = frmHomeBatteryStatusTab.BackgroundColor = Color.FromHex("#39B54A");
                                        frmHomeBatteryStatusMbl.BorderColor = frmHomeBatteryStatusTab.BorderColor = Color.FromHex("#39B54A");
                                    }
                                    else if (dfinalAmp < 0)
                                    {
                                        if (!Convert.ToString(imgBatteryStatus.Source).ToLower().Replace("file: ", "").Trim().StartsWith("dischargingenable"))
                                        {
                                            imgBatteryStatusMbl.Source = imgBatteryStatus.Source = "dischargingenable.gif";
                                        }
                                        frmHomeBatteryStatusMbl.BackgroundColor = frmHomeBatteryStatusTab.BackgroundColor = Color.FromHex("#DC2624");
                                        frmHomeBatteryStatusMbl.BorderColor = frmHomeBatteryStatusTab.BorderColor = Color.FromHex("#DC2624");
                                    }
                                    else if (dfinalAmp == 0)
                                    {
                                        if (!Convert.ToString(imgBatteryStatus.Source).ToLower().Replace("file: ", "").Trim().StartsWith("standby"))
                                        {
                                            imgBatteryStatusMbl.Source = imgBatteryStatus.Source = "standby.png";
                                        }
                                        frmHomeBatteryStatusMbl.BackgroundColor = frmHomeBatteryStatusTab.BackgroundColor = Color.FromHex("#979797");
                                        frmHomeBatteryStatusMbl.BorderColor = frmHomeBatteryStatusTab.BorderColor = Color.FromHex("#979797");
                                    }
                                }

                                if (subs.Length > 15)
                                {
                                    string productiondate = subs[14] + subs[15];
                                    int nproductiondate = Convert.ToInt32(productiondate, 16);

                                    int a = nproductiondate;

                                    int yy = a >> 9;
                                    yy = yy + 2000;

                                    int mm = ((a >> 5) & 15);

                                    int dd = (a & 31);
                                    lblProductionDate.Text = Convert.ToString(yy) + "-" + Convert.ToString(mm) + "-" + Convert.ToString(dd);
                                    if (!IsCalibrationTabOpened)
                                    {
                                        txtManufactureDate.Date = Convert.ToDateTime(lblProductionDate.Text);
                                    }
                                }

                                if (subs.Length > 24)
                                {
                                    //Soc Heating Status
                                    string hexvalue = subs[24];
                                    int decimalValue = Convert.ToInt32(hexvalue, 16);
                                    string binaryString = Convert.ToString(decimalValue, 2).PadLeft(5, '0');
                                    string[] bytes = new string[binaryString.Length];

                                    for (int i = 0; i < binaryString.Length; i++)
                                    {
                                        bytes[i] = Convert.ToString(binaryString[i]);
                                    }
                                    if (bytes != null && bytes.Length > 0)
                                    {
                                        if (bytes[0] == "1")
                                        {
                                            TenthWarning.IsVisible = true;

                                            imgTenthWarning.Source = "soclimit.png";
                                            lblTenthWarning.Text = "SOC Limit";
                                        }
                                        else
                                        {
                                            TenthWarning.IsVisible = false;
                                        }

                                        if (bytes[1] == "1")
                                        {
                                            NinethWarning.IsVisible = true;

                                            imgNinethWarning.Source = "heatingpadson.png";
                                            lblNinethWarning.Text = "Heating \npads on";
                                        }
                                        else
                                        {
                                            NinethWarning.IsVisible = false;
                                        }

                                        // Charging / Discharging

                                        // Discharging 
                                        if (bytes[3] == "1")
                                        {
                                            //discharge chalu                                                        
                                            frmHomeDischargeMbl.BackgroundColor = frmHomeDischargeTab.BackgroundColor = Color.FromHex("#39B54A");
                                            frmHomeDischargeMbl.BorderColor = frmHomeDischargeTab.BorderColor = Color.FromHex("#39B54A");

                                            objbleDevice.DischargingMOSFET = true;
                                            CheckBoxDisChargingOnOff.IsChecked = true;
                                        }
                                        else
                                        {
                                            //discharge bandh                                                        
                                            frmHomeDischargeMbl.BackgroundColor = frmHomeDischargeTab.BackgroundColor = Color.FromHex("#DC2624");
                                            frmHomeDischargeMbl.BorderColor = frmHomeDischargeTab.BorderColor = Color.FromHex("#DC2624");

                                            objbleDevice.DischargingMOSFET = false;
                                            CheckBoxDisChargingOnOff.IsChecked = false;
                                        }


                                        // Charging
                                        if (bytes[4] == "1")
                                        {
                                            //charging chalu                                                        
                                            frmHomeChargeMbl.BackgroundColor = frmHomeChargeTab.BackgroundColor = Color.FromHex("#39B54A");
                                            frmHomeChargeMbl.BorderColor = frmHomeChargeTab.BorderColor = Color.FromHex("#39B54A");

                                            objbleDevice.ChargingMOSFET = true;
                                            CheckBoxChargingOnOff.IsChecked = true;
                                        }
                                        else
                                        {
                                            //charging bandh
                                            frmHomeChargeMbl.BackgroundColor = frmHomeChargeTab.BackgroundColor = Color.FromHex("#DC2624");
                                            frmHomeChargeMbl.BorderColor = frmHomeChargeTab.BorderColor = Color.FromHex("#DC2624");

                                            objbleDevice.ChargingMOSFET = false;
                                            CheckBoxChargingOnOff.IsChecked = false;
                                        }
                                    }
                                }

                                if (subs.Length > 20)
                                {
                                    try
                                    {
                                        string sprotectionStatus = subs[20] + subs[21];
                                        int protectionStatus1 = Convert.ToInt32(sprotectionStatus, 16);

                                        List<int> bits = new List<int>();
                                        for (int i = 0; i <= 15; i++)
                                        {
                                            bits.Add(((int)((protectionStatus1 >> i) & 0x01)));
                                        }

                                        for (int i = 0; i <= 15; i++)
                                        {
                                            if (i == 0)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    FirstWarning.IsVisible = true;
                                                    imgFirstWarning.Source = "cellovervolt.png";
                                                    lblFirstWarning.Text = "Cell \novervolt";
                                                }
                                                else
                                                {
                                                    FirstWarning.IsVisible = false;
                                                }
                                            }
                                            else if (i == 1)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    FirstWarning.IsVisible = true;
                                                    imgFirstWarning.Source = "cellundervolt.png";
                                                    lblFirstWarning.Text = "Cell \nundervolt";
                                                }
                                                else
                                                {
                                                    if (!FirstWarning.IsVisible)
                                                    {
                                                        FirstWarning.IsVisible = false;
                                                    }
                                                }
                                            }
                                            else if (i == 2)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    SecoundWarning.IsVisible = true;
                                                    imgSecoundWarning.Source = "packovervolt.png";
                                                    lblSecoundWarning.Text = "Pack \novervolt";
                                                }
                                                else
                                                {
                                                    SecoundWarning.IsVisible = false;
                                                }
                                            }
                                            else if (i == 3)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    SecoundWarning.IsVisible = true;
                                                    imgSecoundWarning.Source = "packundervolt.png";
                                                    lblSecoundWarning.Text = "Pack \nundervolt";
                                                }
                                                else
                                                {
                                                    if (!SecoundWarning.IsVisible)
                                                    {
                                                        SecoundWarning.IsVisible = false;
                                                    }
                                                }
                                            }
                                            else if (i == 4)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    ThirdWarning.IsVisible = true;
                                                    imgThirdWarning.Source = "chargeovertemp.png";
                                                    lblThirdWarning.Text = "Charge \novertemp";
                                                }
                                                else
                                                {
                                                    ThirdWarning.IsVisible = false;
                                                }
                                            }
                                            else if (i == 5)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    ThirdWarning.IsVisible = true;
                                                    imgThirdWarning.Source = "chargeundertemp.png";
                                                    lblThirdWarning.Text = "Charge \nundertemp";

                                                    if (!UsedNewFirmware)
                                                    {
                                                        if (objDeviceInformation.HeatingPadWarning)
                                                        {
                                                            NinethWarning.IsVisible = objDeviceInformation.HeatingPadWarning;
                                                            imgNinethWarning.Source = "heatingpadson.png";
                                                            lblNinethWarning.Text = "Heating on";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (!ThirdWarning.IsVisible)
                                                    {
                                                        ThirdWarning.IsVisible = false;
                                                    }
                                                }
                                            }
                                            else if (i == 6)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    FourthWarning.IsVisible = true;
                                                    imgFourthWarning.Source = "dischargeovertemp.png";
                                                    lblFourthWarning.Text = "Discharge \novertemp";

                                                }
                                                else
                                                {
                                                    FourthWarning.IsVisible = false;
                                                }
                                            }
                                            else if (i == 7)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    FourthWarning.IsVisible = true;
                                                    imgFourthWarning.Source = "dischargeundertemp.png";
                                                    lblFourthWarning.Text = "Discharge \nundertemp";
                                                }
                                                else
                                                {
                                                    if (!FourthWarning.IsVisible)
                                                    {
                                                        FourthWarning.IsVisible = false;
                                                    }
                                                }
                                            }
                                            else if (i == 8)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    FifthWarning.IsVisible = true;
                                                    imgFifthWarning.Source = "chargeovercurrent.png";
                                                    lblFifthWarning.Text = "Charge \novercurrent";
                                                }
                                                else
                                                {
                                                    FifthWarning.IsVisible = false;
                                                }
                                            }
                                            else if (i == 9)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    FifthWarning.IsVisible = true;
                                                    imgFifthWarning.Source = "dischargeovercurrent.png";
                                                    lblFifthWarning.Text = "Discharge \novercurrent";
                                                }
                                                else
                                                {
                                                    if (!FifthWarning.IsVisible)
                                                    {
                                                        FifthWarning.IsVisible = false;
                                                    }
                                                }
                                            }
                                            else if (i == 10)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    SixthWarning.IsVisible = true;
                                                    imgSixthWarning.Source = "shortcircuit.png";
                                                    lblSixthWarning.Text = "Short Circuit";
                                                }
                                                else
                                                {
                                                    SixthWarning.IsVisible = false;
                                                }
                                            }
                                            else if (i == 11)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    SeventhWarning.IsVisible = true;
                                                    imgSeventhWarning.Source = "frontendicerror.png";
                                                    lblSeventhWarning.Text = "Frontend \nIC error";
                                                }
                                                else
                                                {
                                                    SeventhWarning.IsVisible = false;
                                                }
                                            }
                                            else if (i == 12)
                                            {
                                                if (bits[i] == 1)
                                                {
                                                    EightWarning.IsVisible = true;
                                                    imgEightWarning.Source = "locking.png";
                                                    lblEightWarning.Text = "Locked";
                                                }
                                                else
                                                {
                                                    EightWarning.IsVisible = false;
                                                }
                                            }
                                        }


                                        if (!SecoundWarning.IsVisible && !ThirdWarning.IsVisible && !FourthWarning.IsVisible && !FifthWarning.IsVisible && !SixthWarning.IsVisible && !SeventhWarning.IsVisible && !EightWarning.IsVisible && !NinethWarning.IsVisible && !TenthWarning.IsVisible)
                                        {
                                            FirstWarning.IsVisible = true;

                                            imgFirstWarning.Source = "normalcell.png";
                                            lblFirstWarning.Text = "Normal";
                                        }

                                        if (Device.Idiom == TargetIdiom.Tablet)
                                        {
                                            if (DeviceHeight > DeviceWidth) // portrait
                                            {
                                                if (DeviceHeight > 1000)
                                                {
                                                    await WarningAlertDesign(4);
                                                }
                                                else
                                                {
                                                    await WarningAlertDesign(4);
                                                }
                                            }
                                            else if (DeviceHeight < DeviceWidth) // landscape
                                            {
                                                await WarningAlertDesign(4);
                                            }
                                        }
                                        else if (Device.Idiom == TargetIdiom.Phone)
                                        {
                                            await WarningAlertDesign(4);
                                        }
                                    }
                                    catch
                                    {
                                        SecoundWarning.IsVisible = ThirdWarning.IsVisible = FourthWarning.IsVisible = FifthWarning.IsVisible = SixthWarning.IsVisible = SeventhWarning.IsVisible = EightWarning.IsVisible = false;

                                        FirstWarning.IsVisible = true;

                                        imgFirstWarning.Source = "normalcell.png";
                                        lblFirstWarning.Text = "Normal";

                                        await WarningAlertDesign(3);
                                    }
                                }

                                if (subs.Length > 22)
                                {
                                    Firmware_Version = Convert.ToInt32(subs[22]);
                                    lblBMSVersion.Text = "v" + subs[22];
                                }

                                if (subs.Length > 23)
                                {
                                    int batterystatuspercent = Convert.ToInt32(subs[23], 16);

                                    lblBatteryStatusPercent.Text = batterystatuspercent + "%";
                                    lblBenchmarkSOC.Text = batterystatuspercent + "%";
                                    objReadValues.CurrentSOC = batterystatuspercent;

                                    #region Soc Color

                                    if (batterystatuspercent >= 50)
                                    {
                                        lblBatteryStatusPercent.TextColor = Color.FromHex("#39B54A");
                                        lblBenchmarkSOC.TextColor = Color.FromHex("#39B54A");
                                    }
                                    else if (batterystatuspercent <= 50 && batterystatuspercent >= 20)
                                    {
                                        lblBatteryStatusPercent.TextColor = Color.FromHex("#F99C00");
                                        lblBenchmarkSOC.TextColor = Color.FromHex("#F99C00");
                                    }
                                    else if (batterystatuspercent <= 20)
                                    {
                                        lblBatteryStatusPercent.TextColor = Color.FromHex("#F34E06");
                                        lblBenchmarkSOC.TextColor = Color.FromHex("#F34E06");
                                    }

                                    #endregion Soc Color
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    else
                    {
                        arr03RecBytes.Clear();
                        await CheckSumFailer();
                    }
                    BMSFinalData03 = new byte[0];
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task Command04DisplayData(byte[] BMSfinalData04)
        {
            try
            {
                string hex04 = BitConverter.ToString(BMSfinalData04);
                string[] subs04 = hex04.Split('-');
                int SumOfByte = 0;
                for (int i = 2; i < (subs04.Count() - 3); i++)
                {
                    SumOfByte = SumOfByte + Convert.ToInt32(subs04[i], 16);
                }
                string CheckSum = (65536 - SumOfByte).ToString("X");

                if (CheckSum == Convert.ToString(subs04[subs04.Count() - 3] + subs04[subs04.Count() - 2]))
                {
                    CellModel TempobjCellmodel = new CellModel();
                    TempobjCellmodel = objCellmodel;
                    TempCellViewModel eobjCellmodel = new TempCellViewModel();
                    eobjCellmodel = ConvertToTempCellViewModel(objCellmodel);
                    objReadValues.VoltageCalibrationValue = new List<decimal>();

                    if (subs04 != null && subs04.Length > 0 && BMSfinalData04[0].ToString() == "221")
                    {
                        if (subs04.Length > 4)
                        {
                            try
                            {
                                string cellCount = subs04[2] + subs04[3];
                                int ncellCount = Convert.ToInt32(cellCount, 16);
                                ncellCount = ncellCount / 2;
                                if (ncellCount > 0)
                                {
                                    if (!bSameCell)
                                    {
                                        objCellmodel = new CellModel();
                                    }

                                    for (int i = 0; i < ncellCount; i++)
                                    {
                                        int ntemp = (i * 2) + 4;

                                        string hex_value = subs04[ntemp] + subs04[ntemp + 1];

                                        //converting hex to integer
                                        int int_value = Convert.ToInt32(hex_value, 16);

                                        decimal dcellvolts = ((decimal)int_value / 1000.00M);
                                        if (dcellvolts >= (decimal)1.5 && dcellvolts <= (decimal)4.5)
                                        {
                                            objCellmodel.Data[i].CellVolts = Math.Round(dcellvolts, 2);
                                            objReadValues.VoltageCalibrationValue.Add(Math.Round(dcellvolts, 2));
                                            ListVoltageCalibration[i].CellVoltage = int_value;

                                            #region cellcolor 

                                            if (dcellvolts >= 0 && dcellvolts <= 2.749M)
                                            {
                                                objCellmodel.Colors[i] = Colors.Red;
                                            }
                                            else if (dcellvolts >= 2.750M && dcellvolts <= 2.999M)
                                            {
                                                objCellmodel.Colors[i] = Colors.Orange;
                                                objCellmodel.TextColors[i] = Colors.Black;
                                            }
                                            else if (dcellvolts >= 3M && dcellvolts <= 3.099M)
                                            {
                                                objCellmodel.Colors[i] = Colors.Yellow;
                                                objCellmodel.TextColors[i] = Colors.Black;
                                            }
                                            else if (dcellvolts >= 3.1M && dcellvolts <= 4M)
                                            {
                                                objCellmodel.Colors[i] = Colors.Green;
                                            }
                                            else
                                            {
                                                objCellmodel.Colors[i] = Colors.Green;
                                            }

                                            #endregion cellcolor

                                            if (i == 0)
                                            {
                                                objbleDevice.Cell1volt = dcellvolts;
                                            }
                                            else if (i == 1)
                                            {
                                                objbleDevice.Cell2volt = dcellvolts;
                                            }
                                            else if (i == 2)
                                            {
                                                objbleDevice.Cell3volt = dcellvolts;
                                            }
                                            else if (i == 3)
                                            {
                                                objbleDevice.Cell4volt = dcellvolts;
                                            }
                                            else if (i == 4)
                                            {
                                                objbleDevice.Cell5volt = dcellvolts;
                                            }
                                            else if (i == 5)
                                            {
                                                objbleDevice.Cell6volt = dcellvolts;
                                            }
                                            else if (i == 6)
                                            {
                                                objbleDevice.Cell7volt = dcellvolts;
                                            }
                                            else if (i == 7)
                                            {
                                                objbleDevice.Cell8volt = dcellvolts;
                                            }
                                        }
                                        else
                                        {
                                            objCellmodel = TempobjCellmodel;
                                            break;
                                        }
                                    }

                                    if (Device.Idiom == TargetIdiom.Phone)
                                    {
                                        if (DeviceHeight > DeviceWidth)
                                        {

                                            lstViewSpanCount.SpanCount = 4;
                                            lstTCViewSpanCount.SpanCount = 2;
                                        }
                                        else
                                        {

                                            lstViewSpanCount.SpanCount = 2;
                                            lstTCViewSpanCount.SpanCount = 1;
                                        }
                                    }
                                    else
                                    {
                                        lstViewSpanCount.SpanCount = 2;
                                        lstTCViewSpanCount.SpanCount = 1;
                                    }

                                    chartItemSource.ItemsSource = objCellmodel.Data;
                                    chartItemSource.PaletteBrushes = objCellmodel.Colors;

                                    for (int i = 0; i < objCellmodel.Data.Count(); i++)
                                    {
                                        if (objCellmodel.Data[i].CellVolts == eobjCellmodel.Data[i].CellVolts)
                                        {
                                            bSameCell = true;
                                        }
                                        else
                                        {
                                            bSameCell = false;
                                        }
                                    }

                                    if (CallFunctionOneTime)
                                    {
                                        SetDesignforVoltageCalibration();
                                        CallFunctionOneTime = false;
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    arr04RecBytes.Clear();
                    BMSfinalData04 = new byte[0];
                }
                else
                {
                    await CheckSumFailer();
                    arr04RecBytes.Clear();
                    BMSfinalData04 = new byte[0];
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task CommandErrorCountDisplayData(byte[] BMSfinalDataErrorCount)
        {
            try
            {
                string hex = BitConverter.ToString(BMSfinalDataErrorCount);
                string[] subs = hex.Split('-');

                if (subs != null && subs.Length > 0 && BMSfinalDataErrorCount[0].ToString() == "221")
                {
                    string Value1 = subs[4] + subs[5];
                    lblShortCircuiteValue.Text = Convert.ToString(Convert.ToInt32(Value1, 16));

                    string Value2 = subs[6] + subs[7];
                    lblChargingovercurrentValue.Text = Convert.ToString(Convert.ToInt32(Value2, 16));

                    string Value3 = subs[8] + subs[9];
                    lblDischargingovercurrentValue.Text = Convert.ToString(Convert.ToInt32(Value3, 16));

                    string Value4 = subs[10] + subs[11];
                    lblCellovervoltageValue.Text = Convert.ToString(Convert.ToInt32(Value4, 16));

                    string Value5 = subs[12] + subs[13];
                    lblCellundervoltageValue.Text = Convert.ToString(Convert.ToInt32(Value5, 16));

                    string Value6 = subs[14] + subs[15];
                    lblChargingovertemperatureValue.Text = Convert.ToString(Convert.ToInt32(Value6, 16));

                    string Value7 = subs[16] + subs[17];
                    lblChargingundertemperatureValue.Text = Convert.ToString(Convert.ToInt32(Value7, 16));

                    string Value8 = subs[18] + subs[19];
                    lblDischargingovertemperatureValue.Text = Convert.ToString(Convert.ToInt32(Value8, 16));

                    string Value9 = subs[20] + subs[21];
                    lblDischargingundertemperatureValue.Text = Convert.ToString(Convert.ToInt32(Value9, 16));

                    string Value10 = subs[22] + subs[23];
                    lblPackovervoltValue.Text = Convert.ToString(Convert.ToInt32(Value10, 16));

                    string Value11 = subs[24] + subs[25];
                    lblPackundervoltValue.Text = Convert.ToString(Convert.ToInt32(Value11, 16));
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void SKFTabView_SelectionChanged(object sender, Syncfusion.Maui.TabView.TabSelectionChangedEventArgs e)
        {
            try
            {
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.TabSwitch, e.NewIndex);

                #region Common Value

                Scan.ImageSource = "scanicon.png";
                Home.ImageSource = "homeicon.png";
                Details.ImageSource = "detailsicon.png";
                Tools.ImageSource = "toolicon.png";
                Benchmark.ImageSource = "benchmarkicon.png";
                Calibration.ImageSource = "calibrationicon.png";
                Logs.ImageSource = "logicon.png";
                Networking.ImageSource = "networkicon.png";
                FirmwareUpdate.ImageSource = "firmwareicon.png";
                About.ImageSource = "abouticon.png";

                Scan.TextColor = Color.FromHex("#000");
                Home.TextColor = Color.FromHex("#000");
                Details.TextColor = Color.FromHex("#000");
                Tools.TextColor = Color.FromHex("#000");
                Benchmark.TextColor = Color.FromHex("#000");
                Calibration.TextColor = Color.FromHex("#000");
                Logs.TextColor = Color.FromHex("#000");
                Networking.TextColor = Color.FromHex("#000");
                FirmwareUpdate.TextColor = Color.FromHex("#000");
                About.TextColor = Color.FromHex("#000");

                PasswordConfirmBoxSL.IsVisible = false;
                stackbIsBusyIndicator.IsVisible = sfbIsBusyIndicator.IsVisible = false;
                isProccesBegin = true;
                IsCalibrationTabOpened = false;
                await ShowAbsoluteLayout(false);

                #endregion Common Value

                if (e.NewIndex == 0)
                {
                    IsScantabSelect = true;
                    Scan.ImageSource = "scaniconselect.png";
                    Scan.TextColor = Color.FromHex("#f0c000");

                    if (BechmarkTestRunning)
                    {
                        await ShowDisplayPopup("Alert", "Going back to the scan screen will end the benchmark test in progress and disconnect current battery, are you sure you want to proceed?");
                    }
                    else
                    {
                        await ShowDisplayPopup("Alert", "Going back to the scan screen will disconnect the Bluetooth connection from the current battery, are you sure you want to proceed?");
                    }
                }
                else if (e.NewIndex == 1)
                {
                    Home.ImageSource = "homeiconselect.png";
                    Home.TextColor = Color.FromHex("#f0c000");

                    await OnHomeTabLoad();
                }
                else if (e.NewIndex == 2)
                {
                    if (FirstTimeDetailLoad)
                    {
                        await ShowBusyindicatior(true);
                        FirstTimeDetailLoad = false;
                    }
                    Details.ImageSource = "detailsiconselect.png";
                    Details.TextColor = Color.FromHex("#f0c000");
                }
                else if (e.NewIndex == 3)
                {
                    Tools.ImageSource = "tooliconselect.png";
                    Tools.TextColor = Color.FromHex("#f0c000");

                    await OnToolTabLoad();
                }
                else if (e.NewIndex == 4)
                {
                    Benchmark.ImageSource = "benchmarkiconselect.png";
                    Benchmark.TextColor = Color.FromHex("#f0c000");

                    IsBenchmarktabSelect = true;
                    ChargeDischargeComboBox.SelectedValue = "Discharge Test";

                    if (!BechmarkTestRunning)
                    {
                        btnAbortTest.BackgroundColor = Color.FromHex("#e3e3e3");
                        btnAbortTest.TextColor = Colors.Gray;
                    }
                    await ShowAllFiles();
                }
                else if (e.NewIndex == 5)
                {
                    Logs.ImageSource = "logiconselect.png";
                    Logs.TextColor = Color.FromHex("#f0c000");

                    await ProtectionStatusErrorCountRead();
                }
                else if (e.NewIndex == 6)
                {
                    IsCalibrationTabOpened = true;
                    Calibration.ImageSource = "calibrationiconselect.png";
                    Calibration.TextColor = Color.FromHex("#f0c000");

                    await VoltageCalibrationTab();
                }
                else if (e.NewIndex == 7)
                {
                    Networking.ImageSource = "networkiconselect.png";
                    Networking.TextColor = Color.FromHex("#f0c000");
                }
                else if (e.NewIndex == 8)
                {
                    FirmwareUpdate.ImageSource = "firmwareiconselect.png";
                    FirmwareUpdate.TextColor = Color.FromHex("#f0c000");

                    lblCurrentFirmwareVersion.Text = "v" + objReadValues.H2FirmwareVersion;
                }
                else if (e.NewIndex == 9)
                {
                    About.ImageSource = "abouticonselect.png";
                    About.TextColor = Color.FromHex("#f0c000");
                }
            }
            catch (Exception ex)
            {
                //New Added
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task OnHomeTabLoad()
        {
            try
            {
                if (UsedNewFirmware)
                {
                    await ReadSOCAndHeatingMode();
                }

                if (objReadValues._IsSFKH2Device)
                {
                    await ReadH2ParameterValues();

                    await Task.Delay(100);

                    await ReadActiveBalancer();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task OnToolTabLoad()
        {
            try
            {
                isProccesBegin = false;

                await LoadToolTabData();

                if (objReadValues._IsSFKH2Device)
                {
                    if (CurrentBattType != 1)
                    {
                        stackActiveBalancerCutOffMain.IsVisible = true;
                    }

                    await ReadH2ParameterValues();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void TapGestureRecognizer_Tapped_Segment(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);


                StackLayout objGrid = (StackLayout)sender;
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DeatilTabSegement, objGrid.ClassId);
                gridPowerGaugeContainer.IsVisible = false;
                gridAmpsGaugeContainer.IsVisible = false;
                gridVoltsGaugeContainer.IsVisible = false;
                gridTemperatureGaugeContainer.IsVisible = false;
                sfbDetailsPower.BackgroundColor = Colors.White;
                sfbDetailsAmps.BackgroundColor = Colors.White;
                sfbDetailsVolts.BackgroundColor = Colors.White;
                sfbDetailsTemp.BackgroundColor = Colors.White;
                lblDetailsPower.TextColor = Color.FromHex("#f0c000");
                lblDetailsAmps.TextColor = Color.FromHex("#f0c000");
                lblDetailsVolts.TextColor = Color.FromHex("#f0c000");
                lblDetailsTemp.TextColor = Color.FromHex("#f0c000");
                imgDetailsPower.Source = "power.png";
                imgDetailsAmps.Source = "amps.png";
                imgDetailsVolts.Source = "volts.png";
                imgDetailsTemp.Source = "temp.png";

                if (objGrid.ClassId == "Power")
                {
                    gridPowerGaugeContainer.IsVisible = true;
                    sfbDetailsPower.BackgroundColor = Color.FromHex("#f0c000");
                    lblDetailsPower.TextColor = Color.FromHex("#ffffff");
                    imgDetailsPower.Source = "powerselect.png";
                }
                else if (objGrid.ClassId == "Amps")
                {
                    gridAmpsGaugeContainer.IsVisible = true;
                    sfbDetailsAmps.BackgroundColor = Color.FromHex("#f0c000");
                    lblDetailsAmps.TextColor = Color.FromHex("#ffffff");
                    imgDetailsAmps.Source = "ampsselect.png";
                }
                else if (objGrid.ClassId == "Volts")
                {
                    gridVoltsGaugeContainer.IsVisible = true;
                    sfbDetailsVolts.BackgroundColor = Color.FromHex("#f0c000");
                    lblDetailsVolts.TextColor = Color.FromHex("#ffffff");
                    imgDetailsVolts.Source = "voltsselect.png";
                }
                else if (objGrid.ClassId == "Temp")
                {
                    gridTemperatureGaugeContainer.IsVisible = true;
                    sfbDetailsTemp.BackgroundColor = Color.FromHex("#f0c000");
                    lblDetailsTemp.TextColor = Color.FromHex("#ffffff");
                    imgDetailsTemp.Source = "tempselect.png";
                }
                await ShowBusyindicatior(false);
            }
            catch { }
        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Launcher.OpenAsync(new Uri("https://www.sunfunkits.com/content/3018/privacy-policy"));
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            try
            {
                string url = "https://sunfunkits.com/app/CheckModel?id=" + BarcodeValue + "&ModelName=" + ModelDeviceName + "&MacId=" + lblMacAddressID.Text;

                await Launcher.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private void btn_NavigateToLogs(object sender, EventArgs e)
        {
            try
            {
                //if (Benchmark.IsVisible)
                //{
                //    SKFTabView.SelectedIndex = 5;
                //    imgTabLogs.Source = "logcellundervolt.png";
                //    lblTabLogs.TextColor = Color.FromHex("#f0c000");
                //}
                //else
                //{
                //    SKFTabView.SelectedIndex = 4;
                //    imgTabLogs.Source = "logcellundervolt.png";
                //    lblTabLogs.TextColor = Color.FromHex("#f0c000");
                //}
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        protected override bool OnBackButtonPressed()
        {
            IsScantabSelect = true;
            if (BechmarkTestRunning)
            {
                ShowDisplayPopup("Alert", "Going back to the scan screen will end the benchmark test in progress and disconnect current battery, are you sure you want to proceed?");
            }
            else
            {
                ShowDisplayPopup("Alert", "Going back to the scan screen will disconnect current battery, are you sure you want to proceed?");
            }

            return true;
        }
        private async Task<bool> VerifyByteUsingChecksum(byte[] bytes)
        {
            bool bProceed = true;
            try
            {
                if ((bytes[0].ToString() == "221" && (bytes[1].ToString() == "7" || bytes[1].ToString() == "07")) || (bytes[0].ToString() == "221" && (bytes[1].ToString() == "3" || bytes[1].ToString() == "03")) || (bytes[0].ToString() == "221" && (bytes[1].ToString() == "4" || bytes[1].ToString() == "04")))
                {
                    return bProceed;
                }
                else if ((bytes[0].ToString() == "221" && (bytes[bytes.Length - 3].ToString() == "0" || bytes[bytes.Length - 3].ToString() == "00") && (bytes[bytes.Length - 2].ToString() == "0" || bytes[bytes.Length - 2].ToString() == "00") && bytes[bytes.Length - 1].ToString() == "119"))
                {
                    return bProceed;
                }

                if (bytes[0].ToString() == "221" && bytes[bytes.Length - 1].ToString() == "119")
                {
                    string hex = BitConverter.ToString(bytes);
                    string[] subs = hex.Split('-');
                    int SumOfByte = 0;
                    for (int i = 2; i < (subs.Count() - 3); i++)
                    {
                        SumOfByte = SumOfByte + Convert.ToInt32(subs[i], 16);
                    }
                    string CheckSum = (65536 - SumOfByte).ToString("X");

                    if (CheckSum != Convert.ToString(subs[subs.Count() - 3] + subs[subs.Count() - 2]))
                    {
                        bProceed = false;
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
            return bProceed;
        }
        private async Task CheckSumFailer()
        {
            try
            {
                imgUpdateIconForDevice.IsVisible = false;
                imgFailIconForDevice.IsVisible = true;
                await Task.Delay(1000);
                imgFailIconForDevice.IsVisible = false;
                ChecksumFails = ChecksumFails + 1;

                if (ChecksumFails >= 5)
                {
                    await ShowBusyindicatior(false);

                    ResponceCount = 0;

                    bProccedFails = true;
                    ChecksumFails = 0;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnguageTempCase_Tapped(object sender, EventArgs e)
        {
            try
            {
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.CaseTemperatureToggle, lblGaugeTempCaseHeaderText.Text);

                if (hasCaseTemp2)
                {
                    if (lblGaugeTempCaseHeaderText.Text == "Case T. 1")
                    {
                        rangeSliderTempCase.Pointers[0].Value = (double)temprature3;
                        lblGaugeTempCaseHeaderText.Text = "Case T. 2";
                        lblGaugeTempCaseHeader.Text = lblTempreature3L.Text;
                    }
                    else
                    {
                        rangeSliderTempCase.Pointers[0].Value = (double)temprature2;
                        lblGaugeTempCaseHeaderText.Text = "Case T. 1";
                        lblGaugeTempCaseHeader.Text = lblTempreature2L.Text;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public TempCellViewModel ConvertToTempCellViewModel(CellModel cellViewModel)
        {
            TempCellViewModel tempCellViewModel = new TempCellViewModel
            {
                Data = cellViewModel.Data.Select(cell => new CellInfo
                {
                    CellName = cell.CellName,
                    CellVolts = cell.CellVolts
                }).ToList(),
                Colors = cellViewModel.Colors.Select(color => color is SolidColorBrush solidColorBrush ? new SolidColorBrush(solidColorBrush.Color) : color).ToList()
            };

            return tempCellViewModel;
        }
        private async Task WaitForResponsePassword()
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(20);
                if (bPasswordRest == true) { break; }
            }
        }
        private async Task SinceFullChargeBlink()
        {
            try
            {
                frmSinceFullChargeTab.IsVisible = frmSinceFullChargeMbl.IsVisible = true;

                while (true)
                {
                    if (objReadValues.DaySinceFullCharge < 23)
                    {
                        frmSinceFullChargeTab.IsVisible = true;
                        frmSinceFullChargeMbl.IsVisible = true;
                        break;
                    }

                    bSinceFullChargeBlink = true;
                    frmSinceFullChargeTab.IsVisible = frmSinceFullChargeMbl.IsVisible = true;
                    await Task.Delay(700);

                    frmSinceFullChargeTab.IsVisible = frmSinceFullChargeMbl.IsVisible = false;
                    await Task.Delay(400);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private readonly object _lock = new object();

        private bool _isProcessing = false;

        private (ScrollView scrollView, string type, ScrolledEventArgs e)? _latestScroll;

        private void scrlTools_Scrolled(object sender, ScrolledEventArgs e)
        {
            SendScroll(scrlTools, "Tools", e);
        }
        private void scrWarning_Scrolled(object sender, ScrolledEventArgs e)
        {
            SendScroll(scrWarning, "Warnings", e);
        }
        private void scrlBenchmark_Scrolled(object sender, ScrolledEventArgs e)
        {
            SendScroll(scrlBenchmark, "Benchmark", e);
        }
        private void scrlLogs_Scrolled(object sender, ScrolledEventArgs e)
        {
            SendScroll(scrlLogs, "Logs", e);
        }
        private void scrlCalibration_Scrolled(object sender, ScrolledEventArgs e)
        {
            SendScroll(scrlCalibration, "Calibration", e);
        }
        private void SendScroll(ScrollView scrollView, string type, ScrolledEventArgs e)
        {
            try
            {
                lock (_lock)
                {
                    _latestScroll = (scrollView, type, e);

                    if (_isProcessing)
                    {
                        return;
                    }

                    _isProcessing = true;

                    Task.Run(async () =>
                    {
                        while (true)
                        {
                            (ScrollView scrollView, string type, ScrolledEventArgs e)? item;

                            lock (_lock)
                            {
                                item = _latestScroll;
                                _latestScroll = null;
                            }

                            if (item == null)
                            {
                                await Task.Delay(50);

                                lock (_lock)
                                {
                                    if (_latestScroll == null)
                                    {
                                        _isProcessing = false;
                                        return;
                                    }
                                }

                                continue;
                            }

                            try
                            {
                                double totalHeight = item.Value.scrollView.ContentSize.Height - item.Value.scrollView.Height;

                                if (totalHeight > 0)
                                {
                                    double percent = item.Value.e.ScrollY / totalHeight;

                                    if (percent < 0)
                                    {
                                        percent = 0;
                                    }

                                    if (percent > 1)
                                    {
                                        percent = 1;
                                    }

                                    string data = item.Value.type + '^' + percent.ToString("F4", CultureInfo.InvariantCulture);

                                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ScrollEvent, data);
                                }
                            }
                            catch { }

                            await Task.Delay(80);
                        }
                    });
                }
            }
            catch { }
        }

        #endregion Main Code 

        #region Responsive Design
        protected async override void OnSizeAllocated(double width, double height)
        {
            sfbDatailse.Padding = 10;

            VoltageCalibrationUpdate.Margin = new Thickness(0, 5, 0, 0);

            if (Device.Idiom == TargetIdiom.Phone)
            {
                stackHomeStatusInnerMbl.IsVisible = true;
                stackHomeStatusInnerTab.IsVisible = false;

                scrMainTab.IsEnabled = true;
                stackChargeDischarge.Orientation = StackOrientation.Horizontal;

                brdidleCurrent.WidthRequest = 100;
                lblidleCurrentValue.WidthRequest = 100;

                lblpowergauge.Interval = 400;

                btnCalibrateCapacity.Padding = btnTestHeatingPads.Padding = new Thickness(50, 0, 50, 0);

                Grid.SetRow(stackChargeDischarge, 1);
                Grid.SetColumn(stackChargeDischarge, 0);
                Grid.SetColumnSpan(stackChargeDischarge, 2);

                Grid.SetColumnSpan(brdDeviceNickName, 2);

                stackResetCapacity.Orientation = StackOrientation.Vertical;

                stackResetSocLog.Orientation = stackResetTempLog.Orientation = stackResetCycleCount.Orientation = stackResetLogFault.Orientation = StackOrientation.Vertical;
                stackSyncTime.Orientation = stackRestartESP.Orientation = StackOrientation.Vertical;
                stackLoadDefaultValues.Orientation = StackOrientation.Vertical;
                btnResetLogFaults.HorizontalOptions = btnResetSOCLogFaults.HorizontalOptions = btnResetTemperatureLogFaults.HorizontalOptions = btnResetCycleCount.HorizontalOptions = LayoutOptions.FillAndExpand;
                btnReSyncTime.HorizontalOptions = btnRestartESP32.HorizontalOptions = LayoutOptions.FillAndExpand;
                btnLoadDefaultValues.HorizontalOptions = LayoutOptions.End;
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                stackHomeStatusInnerMbl.IsVisible = false;
                stackHomeStatusInnerTab.IsVisible = true;

                scrMainTab.IsEnabled = true;

                stackChargeDischarge.Orientation = StackOrientation.Horizontal;

                brdidleCurrent.WidthRequest = 130;
                lblidleCurrentValue.WidthRequest = 130;

                brdChargeCurrentSCEdit.HeightRequest = 90;
                brdDisChargeCurrentSCEdit.HeightRequest = 90;

                lblpowergauge.Interval = 200;

                btnChargeCurrentSC.HorizontalOptions = LayoutOptions.StartAndExpand;
                btnDisChargeCurrentSC.HorizontalOptions = LayoutOptions.StartAndExpand;

                btnCalibrateCapacity.Padding = btnTestHeatingPads.Padding = new Thickness(90, 0, 90, 0);

                stackResetCapacity.Orientation = StackOrientation.Horizontal;

                stackResetSocLog.Orientation = stackResetTempLog.Orientation = stackResetCycleCount.Orientation = stackResetLogFault.Orientation = StackOrientation.Horizontal;
                stackSyncTime.Orientation = stackRestartESP.Orientation = StackOrientation.Horizontal;
                stackLoadDefaultValues.Orientation = StackOrientation.Horizontal;
                btnResetLogFaults.HorizontalOptions = btnResetSOCLogFaults.HorizontalOptions = btnResetTemperatureLogFaults.HorizontalOptions = btnResetCycleCount.HorizontalOptions = LayoutOptions.EndAndExpand;
                btnReSyncTime.HorizontalOptions = btnRestartESP32.HorizontalOptions = LayoutOptions.EndAndExpand;
                btnLoadDefaultValues.HorizontalOptions = LayoutOptions.End;
            }

            base.OnSizeAllocated(width, height);
            if (width != _oldWidth || height != _oldHeight)
            {
                DeviceWidth = width;
                DeviceHeight = height;

                _oldWidth = width;
                _oldHeight = height;

                if (bEnableAutoRotate)
                {
                    if (width > height) // landscape
                    {
                        stackBMSDetails.Margin = new Thickness(0, 0, 0, 30);

                        stackHomeTabInner.Orientation = StackOrientation.Horizontal;

                        SfComboBoxborder.Margin = new Thickness(0, -2, 0, -2);
                        btnBeginTest.Margin = new Thickness(0, 0, 0, 0);
                        stackBatteryPercentage.Orientation = StackOrientation.Horizontal;
                        // stackBenchmarkInnerTop.Orientation = StackOrientation.Vertical;

                        #region Details Tab 

                        gridDetailTabCellBars.MaximumHeightRequest = (height / 100) * 90;

                        gridDetailTabOuter.Orientation = StackOrientation.Horizontal;

                        gridDetailTabGauges.Padding = new Thickness(15, 0, 15, 5);

                        sfbDatailse.Margin = new Thickness(0, 5, 0, 5);

                        #endregion Details Tab 

                        if (Device.Idiom == TargetIdiom.Phone)
                        {
                            stackHomeStatusInnerTab.IsVisible = stackHomeStatusInnerTab.IsEnabled = false;

                            stackCycleRemaing.Margin = new Thickness(0, 0, 0, 0);

                            stackGaugeMainStackInner.Orientation = StackOrientation.Horizontal;
                            stackGaugeMainStack.Orientation = StackOrientation.Vertical;

                            sfbDatailse.Padding = 7;

                            await LandscapDesignForMobile(width, height);
                        }
                        else if (Device.Idiom == TargetIdiom.Tablet)
                        {
                            stackHomeStatusInnerTab.IsVisible = stackHomeStatusInnerTab.IsEnabled = true;

                            stackCycleRemaing.Margin = new Thickness(0, 10, 0, 0);

                            stackGaugeMainStackInner.Orientation = StackOrientation.Vertical;
                            stackGaugeMainStack.Orientation = StackOrientation.Horizontal;

                            await LandscapDesignForTab(width, height);
                        }
                    }
                    else //portrait
                    {
                        stackBMSDetails.Margin = new Thickness(0, 30, 0, 30);

                        stackMainBMSAlerts.Margin = 0;

                        stackHomeTabInner.Orientation = StackOrientation.Vertical;

                        SfComboBoxborder.Margin = new Thickness(10, -2, 0, -2);
                        btnBeginTest.Margin = new Thickness(10, 0, 0, 0);
                        stackBatteryPercentage.Orientation = StackOrientation.Vertical;
                        // stackBenchmarkInnerTop.Orientation = StackOrientation.Vertical;

                        #region Details Tab 

                        gaugePower.WidthRequest = ((width / 100) * 95);
                        gaugePositiveAms.WidthRequest = ((width / 100) * 95);
                        gaugeNegativeAms.WidthRequest = ((width / 100) * 95);
                        gaugeVolts.WidthRequest = ((width / 100) * 95);

                        gridDetailTabCellBars.MaximumHeightRequest = (height / 100) * 40;
                        gridDetailTabCellBars.MaximumHeightRequest = (height / 100) * 40;

                        gridDetailTabOuter.Orientation = StackOrientation.Vertical;
                        gridDetailTabGauges.Padding = new Thickness(0, 0, 0, 0);
                        gridDetailTabGauges.ColumnSpacing = 0;

                        #endregion Details Tab 

                        if (Device.Idiom == TargetIdiom.Phone)
                        {
                            gaugePower.WidthRequest = ((width / 100) * 95);
                            gaugePower.HeightRequest = (height / 2);
                            gaugePower.Margin = new Thickness(0, -65, 0, 0);

                            gaugePositiveAms.WidthRequest = ((width / 100) * 95);
                            gaugePositiveAms.HeightRequest = (height / 2);
                            gaugePositiveAms.Margin = new Thickness(0, -65, 0, 0);

                            gaugeNegativeAms.WidthRequest = ((width / 100) * 95);
                            gaugeNegativeAms.HeightRequest = (height / 2);
                            gaugeNegativeAms.Margin = new Thickness(0, -65, 0, 0);

                            gaugeVolts.WidthRequest = ((width / 100) * 95);
                            gaugeVolts.HeightRequest = (height / 2);
                            gaugeVolts.Margin = new Thickness(0, -65, 0, 0);
                            stackHomeStatusInnerTab.IsVisible = stackHomeStatusInnerTab.IsEnabled = false;

                            stackBMSDetails.Margin = new Thickness(0, 0, 0, 10);
                            brWarningOne.Margin = new Thickness(0, 5, 0, 0);

                            stackGaugeMainStackInner.Orientation = StackOrientation.Horizontal;
                            stackGaugeMainStack.Orientation = StackOrientation.Vertical;
                            brdHomeStatus.WidthRequest = ((width / 3) * 40);

                            stackBatteryPercentage.Orientation = StackOrientation.Horizontal;

                            await PotraitDesignForMobile(width, height);
                        }
                        else if (Device.Idiom == TargetIdiom.Tablet)
                        {
                            gaugePower.WidthRequest = ((width / 100) * 70);
                            gaugePower.HeightRequest = (height / 2.8) + 60;
                            gaugePower.Margin = new Thickness(0, -50, 0, 0);

                            gaugePositiveAms.WidthRequest = ((width / 100) * 70);
                            gaugePositiveAms.HeightRequest = (height / 2.8) + 60;
                            gaugePositiveAms.Margin = new Thickness(0, -50, 0, 0);

                            gaugeNegativeAms.WidthRequest = ((width / 100) * 70);
                            gaugeNegativeAms.HeightRequest = (height / 2.8) + 60;
                            gaugeNegativeAms.Margin = new Thickness(0, -50, 0, 0);

                            gaugeVolts.WidthRequest = ((width / 100) * 70);
                            gaugeVolts.HeightRequest = (height / 2.8) + 60;
                            gaugeVolts.Margin = new Thickness(0, -50, 0, 0);

                            stackHomeStatusInnerTab.IsVisible = stackHomeStatusInnerTab.IsEnabled = true;

                            stackGaugeMainStackInner.Orientation = StackOrientation.Horizontal;
                            stackGaugeMainStack.Orientation = StackOrientation.Horizontal;

                            await PotraitDesignForTab(width, height);
                        }
                    }

                    if (stkDisplayMessagePopUp.IsVisible)
                    {
                        await ShowDisplayPopupDesign(width, height);
                    }

                }
            }
        }
        private async Task PotraitDesignForTab(double width, double height)
        {
            try
            {
                #region Main Tab

                brWarningOne.StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(13)
                };

                stackCycleRemaing.Orientation = StackOrientation.Horizontal;
                stackCycleRemaing.HorizontalOptions = LayoutOptions.FillAndExpand;

                gridMainTabOuter.Padding = new Thickness(10);

                stackGaugeMainStack.WidthRequest = width - 30;
                stackFirstMain.WidthRequest = (width / 2) - 30;
                brdHomeStatus.WidthRequest = (width / 2) - 30;

                stackGaugeMainStack.VerticalOptions = LayoutOptions.Start;

                stackSinceFullChargeMainTab.HorizontalOptions = LayoutOptions.CenterAndExpand;

                stackSinceFullChargeMainTab.Margin = new Thickness(25, 0, 0, 0);

                #endregion Main Tab

                #region Details Tab 

                sfbDetailsPower.Padding = new Thickness(0, 10, 0, 0);
                sfbDetailsAmps.Padding = new Thickness(0, 10, 0, 0);
                sfbDetailsVolts.Padding = new Thickness(0, 10, 0, 0);
                sfbDetailsTemp.Padding = new Thickness(0, 10, 0, 0);

                Grid.SetRow(lblDetailsPower, 1);
                Grid.SetRow(lblDetailsAmps, 1);
                Grid.SetRow(lblDetailsVolts, 1);
                Grid.SetRow(lblDetailsTemp, 1);

                imgDetailsPower.IsVisible = imgDetailsAmps.IsVisible = imgDetailsVolts.IsVisible = imgDetailsTemp.IsVisible = true;

                sfbDatailse.Margin = new Thickness(40, 0, 40, 10);

                #endregion Details Tab 

                #region Tool Tab

                grdMaximumSOC.RowDefinitions.Clear();
                grdMaximumSOC.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdMaximumSOC.RowDefinitions.Add(new RowDefinition { Height = 50 });

                grdPoolRequest.RowDefinitions.Clear();

                grdPoolRequest.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grdPoolRequest.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                stackActiveBalancerInnerMain.Orientation = StackOrientation.Horizontal;
                stackRegulateHighSOCMain.Orientation = StackOrientation.Horizontal;
                stackResetPin.Orientation = StackOrientation.Horizontal;

                #region Battery Settings

                grdNickName.ColumnDefinitions.Clear();

                grdNickName.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdNickName.ColumnDefinitions.Add(new ColumnDefinition { Width = 140 });

                grdNickName.RowDefinitions.Clear();
                grdNickName.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdNickName.RowDefinitions.Add(new RowDefinition { Height = 50 });


                Grid.SetRow(lblNickName, 0);
                Grid.SetColumn(lblNickName, 0);
                Grid.SetColumnSpan(lblNickName, 2);

                Grid.SetRow(btnChangeName, 1);
                Grid.SetColumn(btnChangeName, 1);
                Grid.SetColumnSpan(btnChangeName, 1);

                grdBatteryOutput.ColumnDefinitions.Clear();

                grdBatteryOutput.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdBatteryOutput.ColumnDefinitions.Add(new ColumnDefinition { Width = 140 });

                grdBatteryOutput.RowDefinitions.Clear();
                grdBatteryOutput.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdBatteryOutput.RowDefinitions.Add(new RowDefinition { Height = 50 });

                Grid.SetRow(stackChargeDischarge, 1);
                Grid.SetColumn(stackChargeDischarge, 0);
                Grid.SetColumnSpan(stackChargeDischarge, 1);
                #endregion Battery Settings

                #region Temperature Settings

                grdAmbientTemp.RowDefinitions.Clear();
                grdAmbientTemp.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdAmbientTemp.RowDefinitions.Add(new RowDefinition { Height = 50 });

                grdAmbientTemp.ColumnDefinitions.Clear();
                grdAmbientTemp.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdAmbientTemp.ColumnDefinitions.Add(new ColumnDefinition { Width = 150 });

                grdTempUnit.RowDefinitions.Clear();
                grdTempUnit.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdTempUnit.RowDefinitions.Add(new RowDefinition { Height = 50 });

                grdBelowFreezing.RowDefinitions.Clear();
                grdBelowFreezing.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                rdCelsius.Margin = new Thickness(0, 0, 0, 0);
                rdFahrenheit.Margin = new Thickness(10, 0, 0, 0);

                Grid.SetRow(lblTempCalibrate, 0);
                Grid.SetColumn(lblTempCalibrate, 0);
                Grid.SetColumnSpan(lblTempCalibrate, 2);

                Grid.SetRow(stackTempCalibrate, 1);
                Grid.SetColumn(stackTempCalibrate, 0);
                Grid.SetColumnSpan(stackTempCalibrate, 1);

                Grid.SetRow(btnCalibrateTemp, 1);
                Grid.SetColumn(btnCalibrateTemp, 1);
                Grid.SetColumnSpan(btnCalibrateTemp, 1);

                Grid.SetRow(lblTemperatureUnits, 0);
                Grid.SetColumn(lblTemperatureUnits, 0);
                Grid.SetColumnSpan(lblTemperatureUnits, 2);

                Grid.SetRow(stackTemperatureUnits, 1);
                Grid.SetColumn(stackTemperatureUnits, 0);
                Grid.SetColumnSpan(stackTemperatureUnits, 1);

                Grid.SetRow(btnTemperatureUnitUpdate, 1);
                Grid.SetColumn(btnTemperatureUnitUpdate, 1);
                Grid.SetColumnSpan(btnTemperatureUnitUpdate, 1);


                Grid.SetRow(stackTempSetting, 0);
                Grid.SetColumn(stackTempSetting, 0);

                Grid.SetRow(lblTempSetting, 0);
                Grid.SetColumn(lblTempSetting, 1);

                Grid.SetRow(btnLowTempDischargingUpdate, 0);
                Grid.SetColumn(btnLowTempDischargingUpdate, 2);

                #endregion Temperature Settings

                #region SOC Setting

                rbMaximumSOCNormal.Margin = new Thickness(10, 0, 0, 0);
                rbMaximumSOCStorage.Margin = new Thickness(10, 0, 0, 0);
                rbMaximumSOCService.Margin = new Thickness(0, 0, 0, 0);

                grdMaximumSOCcln3.Width = 0;

                Grid.SetRow(lblMaximumSOC, 0);
                Grid.SetColumn(lblMaximumSOC, 0);
                Grid.SetColumnSpan(lblMaximumSOC, 2);

                Grid.SetRow(stackMaximumSOC, 1);
                Grid.SetColumn(stackMaximumSOC, 0);
                Grid.SetColumnSpan(stackMaximumSOC, 1);

                Grid.SetRow(btnMaxSocUpdateRadio, 1);
                Grid.SetColumn(btnMaxSocUpdateRadio, 1);
                Grid.SetColumnSpan(btnMaxSocUpdateRadio, 1);

                #endregion SOC Setting

                #region Other Setting

                grdEnablePinrowmain.Height = 80;

                if (regionResetPin.IsVisible)
                {
                    grdEnablePinrowmain.Height = 150;
                }

                Grid.SetRow(stackResetPin, 1);
                Grid.SetColumn(stackResetPin, 0);
                Grid.SetColumnSpan(stackResetPin, 1);

                Grid.SetRow(regionResetPin, 2);
                Grid.SetColumn(regionResetPin, 0);
                Grid.SetColumnSpan(regionResetPin, 1);


                Grid.SetRow(lblPoolRequest, 0);
                Grid.SetColumn(lblPoolRequest, 0);
                Grid.SetColumnSpan(lblPoolRequest, 2);

                Grid.SetRow(stackPoolRequest, 1);
                Grid.SetColumn(stackPoolRequest, 0);
                Grid.SetColumnSpan(stackPoolRequest, 1);

                Grid.SetRow(btnPullRequest, 1);
                Grid.SetColumn(btnPullRequest, 1);
                Grid.SetColumnSpan(btnPullRequest, 1);


                Grid.SetRow(lblEstablishConnection, 0);
                Grid.SetColumn(lblEstablishConnection, 0);
                Grid.SetColumnSpan(lblEstablishConnection, 2);

                Grid.SetRow(lblFaultRelease, 0);
                Grid.SetColumn(lblFaultRelease, 0);
                Grid.SetColumnSpan(lblFaultRelease, 2);

                Grid.SetRow(stackEstablishConnection, 1);
                Grid.SetColumn(stackEstablishConnection, 0);
                Grid.SetColumnSpan(stackEstablishConnection, 1);

                Grid.SetRow(stackFaultRelease, 1);
                Grid.SetColumn(stackFaultRelease, 0);
                Grid.SetColumnSpan(stackFaultRelease, 1);

                Grid.SetRow(btnEstablishConnection, 1);
                Grid.SetColumn(btnEstablishConnection, 1);
                Grid.SetColumnSpan(btnEstablishConnection, 1);

                Grid.SetRow(btnFaultRelease, 1);
                Grid.SetColumn(btnFaultRelease, 1);
                Grid.SetColumnSpan(btnFaultRelease, 1);

                #endregion Other Setting


                #endregion Tool Tab

                #region Benchmark Tab

                stackBenchmarkTwo.Orientation = StackOrientation.Vertical;

                stackBenchmarkInnerOne.WidthRequest = (width);

                grdListViewHeader.HeightRequest = 50;

                SfComboBoxborder.HeightRequest = 50;
                SfComboBoxborder.WidthRequest = (width / 3);

                if (objFileViewModelList != null && objFileViewModelList.Count > 0)
                {
                    stackBenchMarkFilesList.HeightRequest = lstBenchMarkFilesList.HeightRequest = ((objFileViewModelList.Count + 1) * 85);
                }
                else
                {
                    stackBenchMarkFilesList.HeightRequest = lstBenchMarkFilesList.HeightRequest = 100;
                }

                if (objFileViewModelList != null && objFileViewModelList.Count > 0)
                {
                    lstBenchMarkFilesList.HeightRequest = ((objFileViewModelList.Count + 1) * 80);
                }
                else
                {
                    await ShowAllFiles();
                }


                StackBenchmarkDetails.HeightRequest = 150;
                StackBenchmarkDetails.WidthRequest = StackBenchmarkSocAndGuage.WidthRequest = width;
                StackBenchmarkSocAndGuage.Orientation = StackOrientation.Vertical;
                #endregion Benchmark Tab

                #region Calibration Tab

                VoltageCalibrationUpdate.Margin = new Thickness(0, 5, 25, 0);

                grdAboutMainBox.Margin = new Thickness(40, 15, 40, 0);

                stackMainVoltageCalibration.Orientation = StackOrientation.Vertical;
                stackVoltageCalibrationValue.Orientation = StackOrientation.Horizontal;

                stacklowerVoltageCal.HorizontalOptions = LayoutOptions.FillAndExpand;

                stackTempCalibration.Orientation = StackOrientation.Vertical;

                stackTempCalibrationEdit.VerticalOptions = LayoutOptions.StartAndExpand;
                stackTempCalibration.HorizontalOptions = LayoutOptions.CenterAndExpand;

                brdVoltageCalibrationEdit.Stroke = Color.FromHex("#747474");

                lstVoltageCalibration.HorizontalOptions = LayoutOptions.CenterAndExpand;
                lstTemperatureCalibration.HorizontalOptions = LayoutOptions.CenterAndExpand;

                grdVoltageCalibrationValue.RowDefinitions.Clear();
                grdVoltageCalibrationValue.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grdVoltageCalibrationValue.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                grdVoltageCalibrationValue.ColumnDefinitions.Clear();
                grdVoltageCalibrationValue.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Grid.SetRow(stackVoltageCalibrationValueInnerOne, 0);
                Grid.SetRow(stackValueAndButtonMain, 1);

                Grid.SetColumn(stackVoltageCalibrationValueInnerOne, 0);
                Grid.SetColumn(stackValueAndButtonMain, 0);

                Grid.SetColumnSpan(stackVoltageCalibrationValueInnerOne, 1);
                Grid.SetColumnSpan(stackValueAndButtonMain, 1);

                brdVoltageCalibrationEdit.HorizontalOptions = LayoutOptions.CenterAndExpand;
                stackVoltageCalibrationValue.HorizontalOptions = LayoutOptions.StartAndExpand;

                stackTempCalibrationEdit.HorizontalOptions = LayoutOptions.CenterAndExpand;

                stackValueAndButtonMain.HorizontalOptions = LayoutOptions.FillAndExpand;
                stackVoltageCalibrationUpdate.HorizontalOptions = LayoutOptions.EndAndExpand;

                stackValueAndButtonMain.Orientation = StackOrientation.Horizontal;

                stackValueAndButtonMain.HorizontalOptions = LayoutOptions.CenterAndExpand;

                brdChargeCurrentSCEdit.HeightRequest = 150;
                brdDisChargeCurrentSCEdit.HeightRequest = 150;

                #endregion Calibration Tab                

                if (height > 1000)
                {
                    grdRowPopUpFirst.Height = 100;

                    #region Main Tab

                    stackHomeStatusInnerTab.Margin = new Thickness(0, 10, 0, 0);
                    stackCycleRemaing.Margin = new Thickness(0, 50, 0, 0);

                    brWarningOne.Margin = new Thickness(0, 25, 0, 0);

                    stackBatteryPercentage.Orientation = StackOrientation.Horizontal;

                    #endregion Main Tab

                    #region Details Tab
                    grdrowDetailTabSegmantedButton.Height = 200;

                    lblDetailsPower.FontSize = 25;
                    lblDetailsAmps.FontSize = 25;
                    lblDetailsVolts.FontSize = 25;
                    lblDetailsTemp.FontSize = 25;

                    lblgaugePowerHeader.FontSize = 35;

                    lblgaugeAmsPositive.FontSize = 35;

                    lblgaugeAmsNegative.FontSize = 35;

                    lblgaugeVolts.FontSize = 35;

                    lblGaugeTempCaseHeader.FontSize = 32;

                    lblGaugeTempBMSHeader.FontSize = 32;

                    #endregion

                    #region Tool Tab

                    grdBatteryOutputcln1.Width = (width / 4);
                    grdBatteryOutputcln3.Width = 130;

                    if (stackFullCapacityPart.IsVisible)
                    {
                        stackFullCapacityHeaderPart.RowDefinitions.Clear();

                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 50 });
                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 70 });
                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 50 });
                    }

                    grdTemperatureUnitscln1.Width = new GridLength(1, GridUnitType.Star);
                    grdTemperatureUnitscln3.Width = 140;

                    grdEstablishConnectioncln1.Width = new GridLength(1, GridUnitType.Star);
                    grdEstablishConnectioncln3.Width = 140;

                    grdFaultReleasecln1.Width = new GridLength(1, GridUnitType.Star);
                    grdFaultReleasecln3.Width = 140;

                    grdMaximumSOCcln1.Width = new GridLength(1, GridUnitType.Star);
                    grdMaximumSOCcln3.Width = 140;

                    grdPollRequestcln1.Width = new GridLength(1, GridUnitType.Star);
                    grdPollRequestcln3.Width = 140;

                    #endregion

                    #region Benchmark Tab

                    grdrowGaugeOne2.Height = 150;
                    grdrowGaugeOne3.Height = 60;

                    grdrowGaugeTwo2CT1.Height = 150;
                    grdrowGaugeTwo3CT1.Height = 60;

                    grdrowGaugeThree2CT2.Height = 150;
                    grdrowGaugeThree3CT2.Height = 60;

                    grdrowGaugeThree2CT2.Height = 150;
                    grdrowGaugeThree3CT2.Height = 60;

                    lblBmsTemp.FontSize = lblCaseTemp.FontSize = lblCaseTemp2.FontSize = 25;

                    #endregion

                    #region Log Tab

                    grdLogs.ColumnDefinitions.Clear();
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grdLogs.RowDefinitions.Clear();

                    for (int i = 0; i < 4; i++)
                    {
                        grdLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }

                    Grid.SetRow(brdShortCircuite, 0);
                    Grid.SetRow(brdChargingovercurrent, 0);
                    Grid.SetRow(brdDischargingovercurrent, 0);
                    Grid.SetRow(brdCellovervoltage, 1);
                    Grid.SetRow(brdCellundervoltage, 1);
                    Grid.SetRow(brdChargingovertemperature, 1);
                    Grid.SetRow(brdChargingundertemperature, 2);
                    Grid.SetRow(brdDischargingovertemperature, 2);
                    Grid.SetRow(brdDischargingundertemperature, 2);
                    Grid.SetRow(brdPackovervolt, 3);
                    Grid.SetRow(brdPackundervolt, 3);

                    Grid.SetColumn(brdShortCircuite, 0);
                    Grid.SetColumn(brdChargingovercurrent, 1);
                    Grid.SetColumn(brdDischargingovercurrent, 2);
                    Grid.SetColumn(brdCellovervoltage, 0);
                    Grid.SetColumn(brdCellundervoltage, 1);
                    Grid.SetColumn(brdChargingovertemperature, 2);
                    Grid.SetColumn(brdChargingundertemperature, 0);
                    Grid.SetColumn(brdDischargingovertemperature, 1);
                    Grid.SetColumn(brdDischargingundertemperature, 2);
                    Grid.SetColumn(brdPackovervolt, 0);
                    Grid.SetColumn(brdPackundervolt, 1);

                    grdBMSLogs.ColumnDefinitions.Clear();
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grdBMSLogs.RowDefinitions.Clear();
                    for (int i = 0; i < 3; i++)
                    {
                        grdBMSLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }

                    Grid.SetRow(brdHourHighTemperature, 0);
                    Grid.SetRow(brdHourLowTemperature, 0);
                    Grid.SetRow(brdHourHighSoc, 0);
                    Grid.SetRow(brdHourLowSoc, 1);
                    Grid.SetRow(brdCriticallyHourHighTemperature, 1);
                    Grid.SetRow(brdCriticallyHourLowTemperature, 1);
                    Grid.SetRow(brdCriticallyLowSOC, 2);
                    Grid.SetRow(brdAMPBoost, 2);
                    Grid.SetRow(brdDaySinceFullCharge, 2);

                    Grid.SetColumn(brdHourHighTemperature, 0);
                    Grid.SetColumn(brdHourLowTemperature, 1);
                    Grid.SetColumn(brdHourHighSoc, 2);
                    Grid.SetColumn(brdHourLowSoc, 0);
                    Grid.SetColumn(brdCriticallyHourHighTemperature, 1);
                    Grid.SetColumn(brdCriticallyHourLowTemperature, 2);
                    Grid.SetColumn(brdCriticallyLowSOC, 0);
                    Grid.SetColumn(brdAMPBoost, 1);
                    Grid.SetColumn(brdDaySinceFullCharge, 2);

                    #endregion Log Tab

                    #region Calibration

                    lstVoltageCalibration.ItemSize = 200;
                    lstVoltageCalibration.WidthRequest = lstVoltageCalibration.ItemSize * 4;
                    lstVoltageCalibration.HeightRequest = lstVoltageCalibration.MinimumHeightRequest = 130 * lstViewSpanCount.SpanCount;

                    lstTemperatureCalibration.ItemSize = 230;
                    lstTemperatureCalibration.WidthRequest = lstTemperatureCalibration.ItemSize * 3;
                    lstTemperatureCalibration.HeightRequest = lstTemperatureCalibration.MinimumHeightRequest = 110 * lstTCViewSpanCount.SpanCount;

                    brdVoltageCalibrationEdit.HeightRequest = 120;
                    brdVoltageCalibrationEdit.MinimumHeightRequest = 120;
                    brdVoltageCalibrationEdit.Padding = new Thickness(10, 10, 10, 10);

                    VoltageCalibrationUpdate.HorizontalOptions = LayoutOptions.EndAndExpand;

                    stackVoltageCalibrationValueInnerOne.HorizontalOptions = LayoutOptions.EndAndExpand;
                    stackVoltageCalibrationValueInnerTwo.HorizontalOptions = LayoutOptions.EndAndExpand;

                    stackMainChargeCurrent.Orientation = StackOrientation.Horizontal;
                    stackMainDischargeCurrent.Orientation = StackOrientation.Horizontal;

                    btnResetTemperatureLogFaults.WidthRequest = btnResetSOCLogFaults.WidthRequest = btnResetLogFaults.WidthRequest = (width / 2);
                    // btnLoadDefaultValues.WidthRequest = (width / 2);
                    btnRestartESP32.WidthRequest = (width / 2);
                    btnReSyncTime.WidthRequest = (width / 2);
                    btnResetCycleCount.WidthRequest = (width / 2);

                    #endregion Calibration
                }
                else if (height < 1000)
                {
                    grdRowPopUpFirst.Height = 90;

                    #region Main Tab 

                    stackBMSDetails.Margin = new Thickness(0, 30, 0, 30);

                    stackCycleRemaing.Margin = new Thickness(0, 30, 0, 0);

                    brWarningOne.Margin = new Thickness(0, 20, 0, 0);

                    stackBatteryPercentage.Orientation = StackOrientation.Vertical;

                    #endregion Main Tab

                    #region Details Tab
                    grdrowDetailTabSegmantedButton.Height = 130;
                    gridTemperatureGaugeContainer.Margin = new Thickness(0, 0, 0, 5);

                    lblDetailsPower.FontSize = 22;
                    lblDetailsAmps.FontSize = 22;
                    lblDetailsVolts.FontSize = 22;
                    lblDetailsTemp.FontSize = 22;

                    lblgaugePowerHeader.FontSize = 25;

                    lblgaugeAmsPositive.FontSize = 25;

                    lblgaugeAmsNegative.FontSize = 25;

                    lblgaugeVolts.FontSize = 25;

                    lblGaugeTempCaseHeader.FontSize = 21;

                    lblGaugeTempBMSHeader.FontSize = 21;

                    imgDetailsPower.HeightRequest = imgDetailsAmps.HeightRequest = imgDetailsVolts.HeightRequest = imgDetailsTemp.HeightRequest = 40;
                    imgDetailsPower.WidthRequest = imgDetailsAmps.WidthRequest = imgDetailsVolts.WidthRequest = imgDetailsTemp.WidthRequest = 40;
                    #endregion

                    #region Tool Tab

                    #region Battery Settings
                    grdNickName.ColumnDefinitions.Clear();

                    grdNickName.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdNickName.ColumnDefinitions.Add(new ColumnDefinition { Width = 100 });


                    grdBatteryOutput.ColumnDefinitions.Clear();

                    grdBatteryOutput.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdBatteryOutput.ColumnDefinitions.Add(new ColumnDefinition { Width = 100 });
                    #endregion Battery Settings

                    grdTemperatureUnitscln1.Width = new GridLength(1, GridUnitType.Star);
                    grdTemperatureUnitscln3.Width = 110;

                    grdEstablishConnectioncln1.Width = new GridLength(1, GridUnitType.Star);
                    grdEstablishConnectioncln3.Width = 100;

                    grdFaultReleasecln1.Width = new GridLength(1, GridUnitType.Star);
                    grdFaultReleasecln3.Width = 100;

                    grdMaximumSOCcln1.Width = new GridLength(1, GridUnitType.Star);
                    grdMaximumSOCcln3.Width = 100;

                    stackFullCapacityHeaderPart.RowDefinitions.Clear();
                    if (stackFullCapacityPart.IsVisible)
                    {
                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 50 });
                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 70 });
                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 50 });
                    }
                    else
                    {
                        stackResetCapacity.Margin = new Thickness(0, 0, 0, 0);
                    }

                    #endregion

                    #region Benchmark Tab

                    grdrowGaugeOne2.Height = 120;
                    grdrowGaugeOne3.Height = 60;

                    grdrowGaugeTwo2CT1.Height = 120;
                    grdrowGaugeTwo3CT1.Height = 60;

                    grdrowGaugeThree2CT2.Height = 120;
                    grdrowGaugeThree3CT2.Height = 60;

                    SfComboBoxborder.HeightRequest = 45;

                    lblBmsTemp.FontSize = lblCaseTemp.FontSize = lblCaseTemp2.FontSize = 22;

                    StackBenchmarkDetails.HeightRequest = 130;

                    #endregion

                    #region Log Tab

                    grdLogs.ColumnDefinitions.Clear();
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grdLogs.RowDefinitions.Clear();

                    for (int i = 0; i < 6; i++)
                    {
                        grdLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }

                    Grid.SetRow(brdShortCircuite, 0);
                    Grid.SetRow(brdChargingovercurrent, 0);
                    Grid.SetRow(brdDischargingovercurrent, 1);
                    Grid.SetRow(brdCellovervoltage, 1);
                    Grid.SetRow(brdCellundervoltage, 2);
                    Grid.SetRow(brdChargingovertemperature, 2);
                    Grid.SetRow(brdChargingundertemperature, 3);
                    Grid.SetRow(brdDischargingovertemperature, 3);
                    Grid.SetRow(brdDischargingundertemperature, 4);
                    Grid.SetRow(brdPackovervolt, 4);
                    Grid.SetRow(brdPackundervolt, 5);

                    Grid.SetColumn(brdShortCircuite, 0);
                    Grid.SetColumn(brdChargingovercurrent, 1);
                    Grid.SetColumn(brdDischargingovercurrent, 0);
                    Grid.SetColumn(brdCellovervoltage, 1);
                    Grid.SetColumn(brdCellundervoltage, 0);
                    Grid.SetColumn(brdChargingovertemperature, 1);
                    Grid.SetColumn(brdChargingundertemperature, 0);
                    Grid.SetColumn(brdDischargingovertemperature, 1);
                    Grid.SetColumn(brdDischargingundertemperature, 0);
                    Grid.SetColumn(brdPackovervolt, 1);
                    Grid.SetColumn(brdPackundervolt, 0);


                    grdBMSLogs.ColumnDefinitions.Clear();
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grdBMSLogs.RowDefinitions.Clear();
                    for (int i = 0; i < 5; i++)
                    {
                        grdBMSLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }

                    Grid.SetRow(brdHourHighTemperature, 0);
                    Grid.SetRow(brdHourLowTemperature, 0);
                    Grid.SetRow(brdHourHighSoc, 1);
                    Grid.SetRow(brdHourLowSoc, 1);
                    Grid.SetRow(brdCriticallyHourHighTemperature, 2);
                    Grid.SetRow(brdCriticallyHourLowTemperature, 2);
                    Grid.SetRow(brdCriticallyLowSOC, 3);
                    Grid.SetRow(brdAMPBoost, 3);
                    Grid.SetRow(brdDaySinceFullCharge, 4);

                    Grid.SetColumn(brdHourHighTemperature, 0);
                    Grid.SetColumn(brdHourLowTemperature, 1);
                    Grid.SetColumn(brdHourHighSoc, 0);
                    Grid.SetColumn(brdHourLowSoc, 1);
                    Grid.SetColumn(brdCriticallyHourHighTemperature, 0);
                    Grid.SetColumn(brdCriticallyHourLowTemperature, 1);
                    Grid.SetColumn(brdCriticallyLowSOC, 0);
                    Grid.SetColumn(brdAMPBoost, 1);
                    Grid.SetColumn(brdDaySinceFullCharge, 0);

                    #endregion Log Tab

                    #region Calibration                  

                    lstVoltageCalibration.ItemSize = 150;
                    lstVoltageCalibration.WidthRequest = (lstVoltageCalibration.ItemSize * 4) + 40;

                    lstVoltageCalibration.HeightRequest = lstVoltageCalibration.MinimumHeightRequest = 110 * lstViewSpanCount.SpanCount;

                    lstVoltageCalibration.Margin = new Thickness(27, 0, 10, 0);

                    stacklowerVoltageCal.HorizontalOptions = LayoutOptions.CenterAndExpand;

                    brdVoltageCalibrationEdit.HeightRequest = 120;
                    brdVoltageCalibrationEdit.MinimumHeightRequest = 120;
                    brdVoltageCalibrationEdit.Padding = new Thickness(10, 10, 10, 10);

                    VoltageCalibrationUpdate.HorizontalOptions = LayoutOptions.EndAndExpand;

                    lstTemperatureCalibration.ItemSize = 200;
                    lstTemperatureCalibration.WidthRequest = lstTemperatureCalibration.ItemSize * 3;
                    lstTemperatureCalibration.HeightRequest = 110;
                    lstTemperatureCalibration.MinimumHeightRequest = 110;

                    stackVoltageCalibrationValueInnerOne.Orientation = StackOrientation.Vertical;

                    grdVoltageCalibrationValue.RowDefinitions.Clear();
                    grdVoltageCalibrationValue.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grdVoltageCalibrationValue.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                    grdVoltageCalibrationValue.ColumnDefinitions.Clear();
                    grdVoltageCalibrationValue.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    stackValueAndButtonMain.Margin = new Thickness(0, 0, 0, 0);

                    stackPrecisionValueBox.Margin = new Thickness(0, 0, 0, 0);

                    stackVoltageCalibrationValueInnerOne.HorizontalOptions = LayoutOptions.StartAndExpand;
                    stackVoltageCalibrationValueInnerTwo.HorizontalOptions = LayoutOptions.Center;

                    stackTempCalibrationEdit.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    btnTempCalibrate.Margin = new Thickness(20, 5, 0, 0);
                    stackVoltageCalibrationValueInnerTwo.Margin = new Thickness(0, 5, 0, 0);

                    stackVoltageCalibrationUpdate.Margin = new Thickness(15, 0, 0, 0);

                    stackSerialNumber.Orientation = StackOrientation.Vertical;
                    stackManufatureDate.Orientation = StackOrientation.Vertical;

                    stackVoltageCalibrationValueInnerOne.Orientation = StackOrientation.Horizontal;


                    lblPrecisionValue.HorizontalOptions = LayoutOptions.StartAndExpand;

                    stackMainChargeCurrent.Orientation = StackOrientation.Vertical;
                    stackMainDischargeCurrent.Orientation = StackOrientation.Vertical;

                    btnResetTemperatureLogFaults.WidthRequest = btnResetSOCLogFaults.WidthRequest = btnResetLogFaults.WidthRequest = ((width / 100) * 70);
                    // btnLoadDefaultValues.WidthRequest = ((width / 100) * 70);
                    btnRestartESP32.WidthRequest = ((width / 100) * 70);
                    btnReSyncTime.WidthRequest = ((width / 100) * 70);
                    btnResetCycleCount.WidthRequest = ((width / 100) * 70);

                    #endregion Calibration
                }
            }
            catch { }
        }
        private async Task LandscapDesignForTab(double width, double height)
        {
            try
            {
                #region Main Tab

                gridMainTabOuter.Padding = new Thickness(10);

                gridMainTabOuter.Orientation = StackOrientation.Horizontal;

                stackCycleRemaing.Orientation = StackOrientation.Horizontal;
                stackCycleRemaing.HorizontalOptions = LayoutOptions.FillAndExpand;

                stackGaugeMainStackInner.Orientation = StackOrientation.Vertical;
                stackGaugeMainStack.Orientation = StackOrientation.Vertical;

                stackGaugeMainStack.VerticalOptions = LayoutOptions.StartAndExpand;

                stackGaugeMainStack.WidthRequest = ((width / 100) * 32);
                MainStackTwo.WidthRequest = ((width / 100) * 58);
                stackSinceFullChargeMainTab.HorizontalOptions = LayoutOptions.Fill;

                stackSinceFullChargeMainTab.Margin = new Thickness(25, 10, 0, 0);

                #endregion Main Tab               

                #region Details Tab 
                sfbDetailsPower.Padding = new Thickness(0, 10, 0, 0);
                sfbDetailsAmps.Padding = new Thickness(0, 10, 0, 0);
                sfbDetailsVolts.Padding = new Thickness(0, 10, 0, 0);
                sfbDetailsTemp.Padding = new Thickness(0, 10, 0, 0);

                Grid.SetRow(lblDetailsPower, 1);
                Grid.SetRow(lblDetailsAmps, 1);
                Grid.SetRow(lblDetailsVolts, 1);
                Grid.SetRow(lblDetailsTemp, 1);

                imgDetailsPower.IsVisible = imgDetailsAmps.IsVisible = imgDetailsVolts.IsVisible = imgDetailsTemp.IsVisible = true;
                #endregion Details Tab 

                #region Tool Tab

                grdPoolRequest.RowDefinitions.Clear();
                grdPoolRequest.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grdPoolRequest.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                stackActiveBalancerInnerMain.Orientation = StackOrientation.Horizontal;
                stackRegulateHighSOCMain.Orientation = StackOrientation.Horizontal;
                stackResetPin.Orientation = StackOrientation.Horizontal;

                #region Battery Settings

                grdNickName.ColumnDefinitions.Clear();

                grdNickName.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdNickName.ColumnDefinitions.Add(new ColumnDefinition { Width = 150 });

                grdNickName.RowDefinitions.Clear();

                grdNickName.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdNickName.RowDefinitions.Add(new RowDefinition { Height = 60 });

                Grid.SetRow(lblNickName, 0);
                Grid.SetColumn(lblNickName, 0);
                Grid.SetColumnSpan(lblNickName, 2);

                Grid.SetRow(btnChangeName, 1);
                Grid.SetColumn(btnChangeName, 1);
                Grid.SetColumnSpan(btnChangeName, 1);

                grdBatteryOutput.ColumnDefinitions.Clear();

                grdBatteryOutput.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdBatteryOutput.ColumnDefinitions.Add(new ColumnDefinition { Width = 150 });

                grdBatteryOutput.RowDefinitions.Clear();

                grdBatteryOutput.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdBatteryOutput.RowDefinitions.Add(new RowDefinition { Height = 60 });

                Grid.SetRow(stackSaveChargingDischarging, 1);
                Grid.SetColumn(stackSaveChargingDischarging, 1);
                Grid.SetColumnSpan(stackSaveChargingDischarging, 1);

                #endregion Battery Settings

                #region Temperature Settings

                grdAmbientTemp.RowDefinitions.Clear();
                grdAmbientTemp.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdAmbientTemp.RowDefinitions.Add(new RowDefinition { Height = 50 });

                grdTempUnit.RowDefinitions.Clear();
                grdTempUnit.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdTempUnit.RowDefinitions.Add(new RowDefinition { Height = 50 });

                grdBelowFreezing.RowDefinitions.Clear();
                grdBelowFreezing.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                rdCelsius.Margin = new Thickness(0, 0, 0, 0);
                rdFahrenheit.Margin = new Thickness(10, 0, 0, 0);

                Grid.SetRow(lblTempCalibrate, 0);
                Grid.SetColumn(lblTempCalibrate, 0);
                Grid.SetColumnSpan(lblTempCalibrate, 1);

                Grid.SetRow(stackTempCalibrate, 1);
                Grid.SetColumn(stackTempCalibrate, 0);
                Grid.SetColumnSpan(stackTempCalibrate, 1);

                Grid.SetRow(btnCalibrateTemp, 1);
                Grid.SetColumn(btnCalibrateTemp, 1);
                Grid.SetColumnSpan(btnCalibrateTemp, 1);

                Grid.SetRow(lblTemperatureUnits, 0);
                Grid.SetColumn(lblTemperatureUnits, 0);
                Grid.SetColumnSpan(lblTemperatureUnits, 2);

                Grid.SetRow(stackTemperatureUnits, 1);
                Grid.SetColumn(stackTemperatureUnits, 0);
                Grid.SetColumnSpan(stackTemperatureUnits, 1);

                Grid.SetRow(btnTemperatureUnitUpdate, 1);
                Grid.SetColumn(btnTemperatureUnitUpdate, 1);
                Grid.SetColumnSpan(btnTemperatureUnitUpdate, 1);

                Grid.SetRow(stackTempSetting, 0);
                Grid.SetColumn(stackTempSetting, 0);

                Grid.SetRow(lblTempSetting, 0);
                Grid.SetColumn(lblTempSetting, 1);

                Grid.SetRow(btnLowTempDischargingUpdate, 0);
                Grid.SetColumn(btnLowTempDischargingUpdate, 2);

                #endregion Temperature Settings

                #region SOC Setting

                grdMaximumSOC.RowDefinitions.Clear();
                grdMaximumSOC.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdMaximumSOC.RowDefinitions.Add(new RowDefinition { Height = 50 });

                rbMaximumSOCNormal.Margin = new Thickness(10, 0, 0, 0);
                rbMaximumSOCStorage.Margin = new Thickness(10, 0, 0, 0);
                rbMaximumSOCService.Margin = new Thickness(0, 0, 0, 0);

                Grid.SetRow(lblMaximumSOC, 0);
                Grid.SetColumn(lblMaximumSOC, 0);
                Grid.SetColumnSpan(lblMaximumSOC, 2);

                Grid.SetRow(stackMaximumSOC, 1);
                Grid.SetColumn(stackMaximumSOC, 0);
                Grid.SetColumnSpan(stackMaximumSOC, 1);

                Grid.SetRow(btnMaxSocUpdateRadio, 1);
                Grid.SetColumn(btnMaxSocUpdateRadio, 1);
                Grid.SetColumnSpan(btnMaxSocUpdateRadio, 1);

                #endregion SOC Setting

                #region Other Setting
                Grid.SetRow(lblPoolRequest, 0);
                Grid.SetColumn(lblPoolRequest, 0);
                Grid.SetColumnSpan(lblPoolRequest, 2);

                Grid.SetRow(stackPoolRequest, 1);
                Grid.SetColumn(stackPoolRequest, 0);
                Grid.SetColumnSpan(stackPoolRequest, 1);

                Grid.SetRow(btnPullRequest, 1);
                Grid.SetColumn(btnPullRequest, 1);
                Grid.SetColumnSpan(btnPullRequest, 1);


                Grid.SetRow(lblEstablishConnection, 0);
                Grid.SetColumn(lblEstablishConnection, 0);
                Grid.SetColumnSpan(lblEstablishConnection, 2);

                Grid.SetRow(lblFaultRelease, 0);
                Grid.SetColumn(lblFaultRelease, 0);
                Grid.SetColumnSpan(lblFaultRelease, 2);

                Grid.SetRow(stackEstablishConnection, 1);
                Grid.SetColumn(stackEstablishConnection, 0);
                Grid.SetColumnSpan(stackEstablishConnection, 1);

                Grid.SetRow(stackFaultRelease, 1);
                Grid.SetColumn(stackFaultRelease, 0);
                Grid.SetColumnSpan(stackFaultRelease, 1);

                Grid.SetRow(btnEstablishConnection, 1);
                Grid.SetColumn(btnEstablishConnection, 1);
                Grid.SetColumnSpan(btnEstablishConnection, 1);

                Grid.SetRow(btnFaultRelease, 1);
                Grid.SetColumn(btnFaultRelease, 1);
                Grid.SetColumnSpan(btnFaultRelease, 1);

                #endregion Other Setting

                grdEnablePinrowmain.Height = 80;

                if (regionResetPin.IsVisible == true)
                {
                    grdEnablePinrowmain.Height = 150;
                }

                #endregion Tool Tab

                #region Benchmark Tab

                stackBenchmarkTwo.Orientation = StackOrientation.Horizontal;

                stackBenchmarkInnerOne.WidthRequest = (width - (width / 3));

                if (objFileViewModelList != null && objFileViewModelList.Count > 0)
                {
                    stackBenchMarkFilesList.HeightRequest = lstBenchMarkFilesList.HeightRequest = ((objFileViewModelList.Count + 1) * 80);
                }
                else
                {
                    stackBenchMarkFilesList.HeightRequest = lstBenchMarkFilesList.HeightRequest = 100;
                }
                sfbDatailse.Margin = new Thickness(0, -5, 0, 10);

                StackBenchmarkDetails.HeightRequest = 200;
                StackBenchmarkDetails.WidthRequest = StackBenchmarkSocAndGuage.WidthRequest = width;
                StackBenchmarkSocAndGuage.Orientation = StackOrientation.Horizontal;

                #endregion Benchmark Tab

                #region About Tab

                grdAboutMainBox.Margin = new Thickness(40, 15, 40, 0);

                #endregion About Tab

                #region Calibration Tab

                VoltageCalibrationUpdate.Margin = new Thickness(0, 5, 25, 0);
                stackMainVoltageCalibration.Orientation = StackOrientation.Horizontal;
                stackVoltageCalibrationValue.Orientation = StackOrientation.Vertical;

                stacklowerVoltageCal.HorizontalOptions = LayoutOptions.EndAndExpand;

                stackTempCalibration.Orientation = StackOrientation.Horizontal;

                stackTempCalibrationEdit.VerticalOptions = LayoutOptions.CenterAndExpand;

                stackTempCalibration.HorizontalOptions = LayoutOptions.FillAndExpand;

                brdVoltageCalibrationEdit.Stroke = Colors.White;

                lstVoltageCalibration.HorizontalOptions = LayoutOptions.StartAndExpand;
                lstTemperatureCalibration.HorizontalOptions = LayoutOptions.StartAndExpand;

                grdVoltageCalibrationValue.RowDefinitions.Clear();
                grdVoltageCalibrationValue.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grdVoltageCalibrationValue.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                grdVoltageCalibrationValue.ColumnDefinitions.Clear();
                grdVoltageCalibrationValue.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Grid.SetRow(stackVoltageCalibrationValueInnerOne, 0);
                Grid.SetRow(stackVoltageCalibrationValueInnerTwo, 1);

                Grid.SetColumn(stackVoltageCalibrationValueInnerOne, 0);
                Grid.SetColumn(stackVoltageCalibrationValueInnerTwo, 0);

                brdVoltageCalibrationEdit.HorizontalOptions = LayoutOptions.EndAndExpand;
                stackVoltageCalibrationValue.HorizontalOptions = LayoutOptions.EndAndExpand;

                stackTempCalibrationEdit.HorizontalOptions = LayoutOptions.End;

                stackMainChargeCurrent.Orientation = StackOrientation.Horizontal;
                stackMainDischargeCurrent.Orientation = StackOrientation.Horizontal;

                brdChargeCurrentSCEdit.HeightRequest = 110;
                brdDisChargeCurrentSCEdit.HeightRequest = 110;

                #endregion Calibration Tab

                if (width > 1000)
                {
                    grdRowPopUpFirst.Height = 100;

                    #region Main Tab
                    if (width > 1200)
                    {
                        grdrowDetailTabSegmantedButton.Height = 180;
                        brWarningOne.Margin = new Thickness(0, 25, 0, 0);
                    }
                    else
                    {
                        brWarningOne.Margin = new Thickness(0, 25, 0, 0);

                        grdrowDetailTabSegmantedButton.Height = 150;
                    }

                    stackCycleRemaing.Margin = new Thickness(0, 30, 0, 0);

                    #endregion Main Tab

                    #region Tool Tab

                    rangesliderFullCapacity.Margin = new Thickness(0, -60, 0, -15);

                    if (stackFullCapacityPart.IsVisible)
                    {
                        stackFullCapacityHeaderPart.RowDefinitions.Clear();

                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 50 });
                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 70 });
                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 50 });
                    }

                    grdTemperatureUnitscln1.Width = new GridLength(1, GridUnitType.Star);
                    grdTemperatureUnitscln3.Width = 140;

                    grdMaximumSOCcln1.Width = new GridLength(1, GridUnitType.Star);
                    grdMaximumSOCcln3.Width = 140;

                    grdEstablishConnectioncln1.Width = new GridLength(1, GridUnitType.Star);
                    grdEstablishConnectioncln3.Width = 140;

                    grdFaultReleasecln1.Width = new GridLength(1, GridUnitType.Star);
                    grdFaultReleasecln3.Width = 140;

                    grdPollRequestcln1.Width = new GridLength(1, GridUnitType.Star);
                    grdPollRequestcln3.Width = 140;

                    #endregion Tool Tab                    

                    #region Benchmark Tab

                    lblBmsTemp.FontSize = lblCaseTemp.FontSize = lblCaseTemp2.FontSize = 25;

                    grdrowGaugeOne2.Height = 140;
                    grdrowGaugeOne3.Height = 60;

                    grdrowGaugeTwo2CT1.Height = 140;
                    grdrowGaugeTwo3CT1.Height = 60;

                    grdrowGaugeThree2CT2.Height = 140;
                    grdrowGaugeThree3CT2.Height = 60;

                    #endregion Benchmark Tab

                    #region Log Tab

                    grdLogs.ColumnDefinitions.Clear();
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grdLogs.RowDefinitions.Clear();
                    for (int i = 0; i < 3; i++)
                    {
                        grdLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }

                    Grid.SetRow(brdShortCircuite, 0);
                    Grid.SetRow(brdChargingovercurrent, 0);
                    Grid.SetRow(brdDischargingovercurrent, 0);
                    Grid.SetRow(brdCellovervoltage, 0);
                    Grid.SetRow(brdCellundervoltage, 1);
                    Grid.SetRow(brdChargingovertemperature, 1);
                    Grid.SetRow(brdChargingundertemperature, 1);
                    Grid.SetRow(brdDischargingovertemperature, 1);
                    Grid.SetRow(brdDischargingundertemperature, 2);
                    Grid.SetRow(brdPackovervolt, 2);
                    Grid.SetRow(brdPackundervolt, 2);

                    Grid.SetColumn(brdShortCircuite, 0);
                    Grid.SetColumn(brdChargingovercurrent, 1);
                    Grid.SetColumn(brdDischargingovercurrent, 2);
                    Grid.SetColumn(brdCellovervoltage, 3);
                    Grid.SetColumn(brdCellundervoltage, 0);
                    Grid.SetColumn(brdChargingovertemperature, 1);
                    Grid.SetColumn(brdChargingundertemperature, 2);
                    Grid.SetColumn(brdDischargingovertemperature, 3);
                    Grid.SetColumn(brdDischargingundertemperature, 0);
                    Grid.SetColumn(brdPackovervolt, 1);
                    Grid.SetColumn(brdPackundervolt, 2);

                    grdBMSLogs.ColumnDefinitions.Clear();
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grdBMSLogs.RowDefinitions.Clear();
                    for (int i = 0; i < 3; i++)
                    {
                        grdBMSLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }
                    Grid.SetRow(brdHourHighTemperature, 0);
                    Grid.SetRow(brdHourLowTemperature, 0);
                    Grid.SetRow(brdHourHighSoc, 0);
                    Grid.SetRow(brdHourLowSoc, 0);
                    Grid.SetRow(brdCriticallyHourHighTemperature, 1);
                    Grid.SetRow(brdCriticallyHourLowTemperature, 1);
                    Grid.SetRow(brdCriticallyLowSOC, 1);
                    Grid.SetRow(brdAMPBoost, 1);
                    Grid.SetRow(brdDaySinceFullCharge, 2);

                    Grid.SetColumn(brdHourHighTemperature, 0);
                    Grid.SetColumn(brdHourLowTemperature, 1);
                    Grid.SetColumn(brdHourHighSoc, 2);
                    Grid.SetColumn(brdHourLowSoc, 3);
                    Grid.SetColumn(brdCriticallyHourHighTemperature, 0);
                    Grid.SetColumn(brdCriticallyHourLowTemperature, 1);
                    Grid.SetColumn(brdCriticallyLowSOC, 2);
                    Grid.SetColumn(brdAMPBoost, 3);
                    Grid.SetColumn(brdDaySinceFullCharge, 0);

                    #endregion Log Tab

                    #region Calibration                  

                    stackVoltageCalibrationValueInnerOne.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    stackVoltageCalibrationValueInnerTwo.HorizontalOptions = LayoutOptions.CenterAndExpand;

                    VoltageCalibrationUpdate.Margin = new Thickness(0, 5, 25, 0);

                    lstVoltageCalibration.ItemSize = 200;
                    lstVoltageCalibration.WidthRequest = lstVoltageCalibration.ItemSize * 4;
                    lstVoltageCalibration.HeightRequest = lstVoltageCalibration.MinimumHeightRequest = 130 * lstViewSpanCount.SpanCount;

                    lstTemperatureCalibration.ItemSize = 230;
                    lstTemperatureCalibration.WidthRequest = lstTemperatureCalibration.ItemSize * 3;
                    lstTemperatureCalibration.HeightRequest = lstTemperatureCalibration.MinimumHeightRequest = 110 * lstTCViewSpanCount.SpanCount;

                    brdVoltageCalibrationEdit.Padding = new Thickness(0, 0, 0, 0);
                    VoltageCalibrationUpdate.HorizontalOptions = LayoutOptions.EndAndExpand;

                    brdVoltageCalibrationEdit.HeightRequest = 100;
                    brdVoltageCalibrationEdit.MinimumHeightRequest = 100;

                    btnResetTemperatureLogFaults.WidthRequest = btnResetSOCLogFaults.WidthRequest = btnResetLogFaults.WidthRequest = (width / 3);
                    // btnLoadDefaultValues.WidthRequest = (width / 3);
                    btnRestartESP32.WidthRequest = (width / 3);
                    btnReSyncTime.WidthRequest = (width / 3);
                    btnResetCycleCount.WidthRequest = (width / 3);

                    #endregion Calibration
                }
                else if (width < 1000)
                {
                    grdRowPopUpFirst.Height = 90;

                    #region Main Tab        

                    stackMainBMSAlerts.Margin = new Thickness(0, -25, 0, 0);

                    brWarningOne.Margin = new Thickness(0, 5, 0, 0);

                    Grid.SetRow(brWarningOne, 0);

                    #endregion Main Tab

                    #region Details Tab
                    gridTemperatureGaugeContainer.Margin = new Thickness(0, 0, 0, 0);
                    grdrowDetailTabSegmantedButton.Height = 160;

                    lblDetailsPower.FontSize = 22;
                    lblDetailsAmps.FontSize = 22;
                    lblDetailsVolts.FontSize = 22;
                    lblDetailsTemp.FontSize = 22;

                    lblgaugePowerHeader.FontSize = 25;

                    lblgaugeAmsPositive.FontSize = 25;

                    lblgaugeAmsNegative.FontSize = 25;

                    lblgaugeVolts.FontSize = 25;

                    lblGaugeTempCaseHeader.FontSize = 21;

                    lblGaugeTempBMSHeader.FontSize = 21;

                    grdrowDetailTabSegmantedButton.Height = 130;

                    #endregion

                    #region Tool Tab

                    grdNickNamecln1.Width = new GridLength(1, GridUnitType.Star);
                    grdNickNamecln3.Width = 100;

                    grdBatteryOutputcln1.Width = new GridLength(1, GridUnitType.Star);
                    grdBatteryOutputcln3.Width = 100;

                    grdTemperatureUnitscln1.Width = new GridLength(1, GridUnitType.Star);
                    grdTemperatureUnitscln3.Width = 100;

                    grdEstablishConnectioncln1.Width = new GridLength(1, GridUnitType.Star);
                    grdEstablishConnectioncln3.Width = 100;

                    grdFaultReleasecln1.Width = new GridLength(1, GridUnitType.Star);
                    grdFaultReleasecln3.Width = 100;

                    grdMaximumSOCcln1.Width = new GridLength(1, GridUnitType.Star);
                    grdMaximumSOCcln3.Width = 100;

                    if (stackFullCapacityPart.IsVisible)
                    {
                        stackFullCapacityHeaderPart.RowDefinitions.Clear();

                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 40 });
                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 50 });
                    }

                    #endregion

                    #region Benchmark Tab

                    lblBmsTemp.FontSize = lblCaseTemp.FontSize = lblCaseTemp2.FontSize = 22;

                    grdrowGaugeOne2.Height = 120;
                    grdrowGaugeOne3.Height = 60;

                    grdrowGaugeTwo2CT1.Height = 120;
                    grdrowGaugeTwo3CT1.Height = 60;

                    grdrowGaugeThree2CT2.Height = 120;
                    grdrowGaugeThree3CT2.Height = 60;

                    StackBenchmarkDetails.HeightRequest = 150;

                    #endregion Benchmark Tab

                    #region Log Tab

                    grdLogs.ColumnDefinitions.Clear();
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grdLogs.RowDefinitions.Clear();

                    for (int i = 0; i < 4; i++)
                    {
                        grdLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }

                    Grid.SetRow(brdShortCircuite, 0);
                    Grid.SetRow(brdChargingovercurrent, 0);
                    Grid.SetRow(brdDischargingovercurrent, 0);
                    Grid.SetRow(brdCellovervoltage, 1);
                    Grid.SetRow(brdCellundervoltage, 1);
                    Grid.SetRow(brdChargingovertemperature, 1);
                    Grid.SetRow(brdChargingundertemperature, 2);
                    Grid.SetRow(brdDischargingovertemperature, 2);
                    Grid.SetRow(brdDischargingundertemperature, 2);
                    Grid.SetRow(brdPackovervolt, 3);
                    Grid.SetRow(brdPackundervolt, 3);

                    Grid.SetColumn(brdShortCircuite, 0);
                    Grid.SetColumn(brdChargingovercurrent, 1);
                    Grid.SetColumn(brdDischargingovercurrent, 2);
                    Grid.SetColumn(brdCellovervoltage, 0);
                    Grid.SetColumn(brdCellundervoltage, 1);
                    Grid.SetColumn(brdChargingovertemperature, 2);
                    Grid.SetColumn(brdChargingundertemperature, 0);
                    Grid.SetColumn(brdDischargingovertemperature, 1);
                    Grid.SetColumn(brdDischargingundertemperature, 2);
                    Grid.SetColumn(brdPackovervolt, 0);
                    Grid.SetColumn(brdPackundervolt, 1);


                    grdBMSLogs.ColumnDefinitions.Clear();
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grdBMSLogs.RowDefinitions.Clear();
                    for (int i = 0; i < 3; i++)
                    {
                        grdBMSLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }
                    Grid.SetRow(brdHourHighTemperature, 0);
                    Grid.SetRow(brdHourLowTemperature, 0);
                    Grid.SetRow(brdHourHighSoc, 0);
                    Grid.SetRow(brdHourLowSoc, 1);
                    Grid.SetRow(brdCriticallyHourHighTemperature, 1);
                    Grid.SetRow(brdCriticallyHourLowTemperature, 1);
                    Grid.SetRow(brdCriticallyLowSOC, 2);
                    Grid.SetRow(brdAMPBoost, 2);
                    Grid.SetRow(brdDaySinceFullCharge, 2);

                    Grid.SetColumn(brdHourHighTemperature, 0);
                    Grid.SetColumn(brdHourLowTemperature, 1);
                    Grid.SetColumn(brdHourHighSoc, 2);
                    Grid.SetColumn(brdHourLowSoc, 0);
                    Grid.SetColumn(brdCriticallyHourHighTemperature, 1);
                    Grid.SetColumn(brdCriticallyHourLowTemperature, 2);
                    Grid.SetColumn(brdCriticallyLowSOC, 0);
                    Grid.SetColumn(brdAMPBoost, 1);
                    Grid.SetColumn(brdDaySinceFullCharge, 2);
                    #endregion Log Tab

                    #region Calibration                  

                    lstVoltageCalibration.ItemSize = 150;
                    lstVoltageCalibration.WidthRequest = (lstVoltageCalibration.ItemSize * 4) + 30;

                    lstVoltageCalibration.HeightRequest = lstVoltageCalibration.MinimumHeightRequest = 130 * lstViewSpanCount.SpanCount;

                    brdVoltageCalibrationEdit.HeightRequest = brdVoltageCalibrationEdit.MinimumHeightRequest = 180 * lstViewSpanCount.SpanCount;

                    brdVoltageCalibrationEdit.Padding = new Thickness(0, 0, 0, 0);
                    VoltageCalibrationUpdate.HorizontalOptions = LayoutOptions.EndAndExpand;

                    brdVoltageCalibrationEdit.HeightRequest = 145;
                    brdVoltageCalibrationEdit.MinimumHeightRequest = 100;

                    lstTemperatureCalibration.ItemSize = 200;
                    lstTemperatureCalibration.WidthRequest = lstTemperatureCalibration.ItemSize * 3;
                    lstTemperatureCalibration.HeightRequest = 110;
                    lstTemperatureCalibration.MinimumHeightRequest = height / 4.6;

                    stackVoltageCalibrationValueInnerOne.Orientation = StackOrientation.Vertical;
                    stackValueAndButtonMain.Orientation = StackOrientation.Horizontal;

                    stackValueAndButtonMain.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    stackVoltageCalibrationUpdate.HorizontalOptions = LayoutOptions.CenterAndExpand;

                    lblPrecisionValue.HorizontalOptions = LayoutOptions.StartAndExpand;
                    lblPrecisionValue.Padding = new Thickness(0, 0, 0, 5);

                    btnResetTemperatureLogFaults.WidthRequest = btnResetSOCLogFaults.WidthRequest = btnResetLogFaults.WidthRequest = ((width / 100) * 50);
                    // btnLoadDefaultValues.WidthRequest = ((width / 100) * 50);
                    btnRestartESP32.WidthRequest = ((width / 100) * 50);
                    btnReSyncTime.WidthRequest = ((width / 100) * 50);
                    btnResetCycleCount.WidthRequest = ((width / 100) * 50);

                    #endregion Calibration
                }
            }
            catch { }
        }
        private async Task PotraitDesignForMobile(double width, double height)
        {
            try
            {
                grdRowPopUpFirst.Height = 90;

                #region Main Tab

                stackFirstMain.WidthRequest = ((width / 3)) * 2 + 30;
                stackBMSIndicators.WidthRequest = ((width / 3)) * 2 + 30;
                brdHomeStatus.WidthRequest = stackCycleRemaing.WidthRequest = (width / 2.2);

                stackGaugeMainStack.WidthRequest = width;
                MainStackTwo.WidthRequest = width;

                MainStackTwo.HorizontalOptions = LayoutOptions.FillAndExpand;
                stackGaugeMainStack.HorizontalOptions = LayoutOptions.FillAndExpand;

                stackCycleRemaing.Orientation = StackOrientation.Vertical;
                stackCycleRemaing.HorizontalOptions = LayoutOptions.CenterAndExpand;

                stackCycleCountMain.Margin = new Thickness(0, 20, 0, 0);

                stackRemaingAhMain.Margin = new Thickness(7, 0, 0, 0);
                stackRemainglabel.Margin = new Thickness(15, 0, 0, 0);
                stackFirstMain.Margin = new Thickness(0, 0, 0, 0);
                gridMainTabOuter.Margin = new Thickness(10);

                brWarningOne.StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(7)
                };

                Grid.SetRow(brWarningOne, 2);

                stackGaugeMainStack.Margin = new Thickness(20, 0, 0, 0);
                stackBMSDetails.Margin = new Thickness(20, 10, 0, 10);

                #endregion Main Tab

                #region Details Tab  

                sfbDetailsPower.Padding = new Thickness(0, 10, 0, 0);
                sfbDetailsAmps.Padding = new Thickness(0, 10, 0, 0);
                sfbDetailsVolts.Padding = new Thickness(0, 10, 0, 0);
                sfbDetailsTemp.Padding = new Thickness(0, 10, 0, 0);

                Grid.SetRow(lblDetailsPower, 1);
                Grid.SetRow(lblDetailsAmps, 1);
                Grid.SetRow(lblDetailsVolts, 1);
                Grid.SetRow(lblDetailsTemp, 1);

                imgDetailsPower.IsVisible = imgDetailsAmps.IsVisible = imgDetailsVolts.IsVisible = imgDetailsTemp.IsVisible = true;

                grdrowDetailTabSegmantedButton.Height = 125;

                sfbDatailse.Margin = new Thickness(15, 0, 15, 5);

                lblDetailsPower.FontSize = 20;
                lblDetailsAmps.FontSize = 20;
                lblDetailsVolts.FontSize = 20;
                lblDetailsTemp.FontSize = 20;

                lblgaugePowerHeader.FontSize = 18;

                lblgaugeAmsNegative.FontSize = 18;

                lblgaugeAmsPositive.FontSize = 18;

                lblgaugeVolts.FontSize = 18;

                lblGaugeTempCaseHeader.FontSize = 18;

                lblGaugeTempBMSHeader.FontSize = 17;

                if (Is24VBattery)
                {
                    chartItemSource.DataLabelSettings.LabelStyle.Angle = 270;
                    chartItemSource.Width = 0.85;
                }
                else
                {
                    chartItemSource.DataLabelSettings.LabelStyle.Angle = 0;
                }

                #endregion Details Tab

                #region ToolTab

                rdHeatingModesHeight.Height = 150;

                stackActiveBalancerInnerMain.Orientation = StackOrientation.Vertical;
                stackRegulateHighSOCMain.Orientation = StackOrientation.Vertical;
                stackResetPin.Orientation = StackOrientation.Vertical;

                #region Battery Settings

                grdNickName.ColumnDefinitions.Clear();

                grdNickName.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdNickName.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                grdNickName.RowDefinitions.Clear();

                grdNickName.RowDefinitions.Add(new RowDefinition { Height = 30 });
                grdNickName.RowDefinitions.Add(new RowDefinition { Height = 50 });
                grdNickName.RowDefinitions.Add(new RowDefinition { Height = 50 });

                Grid.SetRow(lblNickName, 0);
                Grid.SetColumn(lblNickName, 0);
                Grid.SetColumnSpan(lblNickName, 2);

                Grid.SetRow(btnChangeName, 2);
                Grid.SetColumn(btnChangeName, 0);
                Grid.SetColumnSpan(btnChangeName, 2);

                Grid.SetRow(stackSaveChargingDischarging, 2);
                Grid.SetColumn(stackSaveChargingDischarging, 0);
                Grid.SetColumnSpan(stackSaveChargingDischarging, 2);

                grdBatteryOutput.ColumnDefinitions.Clear();
                grdBatteryOutput.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdBatteryOutput.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });


                grdBatteryOutput.RowDefinitions.Clear();
                grdBatteryOutput.RowDefinitions.Add(new RowDefinition { Height = 30 });
                grdBatteryOutput.RowDefinitions.Add(new RowDefinition { Height = 40 });
                grdBatteryOutput.RowDefinitions.Add(new RowDefinition { Height = 50 });



                #endregion Battery Settings

                #region Temperature Settings

                grdAmbientTemp.RowDefinitions.Clear();

                grdAmbientTemp.RowDefinitions.Add(new RowDefinition { Height = 50 });
                grdAmbientTemp.RowDefinitions.Add(new RowDefinition { Height = 50 });
                grdAmbientTemp.RowDefinitions.Add(new RowDefinition { Height = 45 });

                grdAmbientTemp.ColumnDefinitions.Clear();
                grdAmbientTemp.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Grid.SetRow(lblTempCalibrate, 0);
                Grid.SetColumn(lblTempCalibrate, 0);
                Grid.SetColumnSpan(lblTempCalibrate, 2);

                Grid.SetRow(stackTempCalibrate, 1);
                Grid.SetColumn(stackTempCalibrate, 0);
                Grid.SetColumnSpan(stackTempCalibrate, 2);

                Grid.SetRow(btnCalibrateTemp, 2);
                Grid.SetColumn(btnCalibrateTemp, 0);
                Grid.SetColumnSpan(btnCalibrateTemp, 2);


                grdTempUnit.ColumnDefinitions.Clear();
                grdTempUnit.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                grdTempUnit.RowDefinitions.Clear();
                grdTempUnit.RowDefinitions.Add(new RowDefinition { Height = 25 });
                grdTempUnit.RowDefinitions.Add(new RowDefinition { Height = 50 });
                grdTempUnit.RowDefinitions.Add(new RowDefinition { Height = 45 });

                Grid.SetRow(lblTempCalibrate, 0);
                Grid.SetColumn(lblTempCalibrate, 0);
                Grid.SetColumnSpan(lblTempCalibrate, 2);

                Grid.SetRow(stackTempCalibrate, 1);
                Grid.SetColumn(stackTempCalibrate, 0);
                Grid.SetColumnSpan(stackTempCalibrate, 2);

                Grid.SetRow(lblTemperatureUnits, 0);
                Grid.SetColumn(lblTemperatureUnits, 0);
                Grid.SetColumnSpan(lblTemperatureUnits, 2);

                Grid.SetRow(stackTemperatureUnits, 1);
                Grid.SetColumn(stackTemperatureUnits, 0);
                Grid.SetColumnSpan(stackTemperatureUnits, 2);

                Grid.SetRow(btnTemperatureUnitUpdate, 2);
                Grid.SetColumn(btnTemperatureUnitUpdate, 0);
                Grid.SetColumnSpan(btnTemperatureUnitUpdate, 2);


                Grid.SetRow(stackTempSetting, 0);
                Grid.SetColumn(stackTempSetting, 0);

                Grid.SetRow(lblTempSetting, 0);
                Grid.SetColumn(lblTempSetting, 1);

                Grid.SetRow(btnLowTempDischargingUpdate, 1);
                Grid.SetColumn(btnLowTempDischargingUpdate, 0);
                Grid.SetColumnSpan(btnLowTempDischargingUpdate, 2);

                #endregion Temperature Settings

                #region Capacity Setting

                stackFullCapacityHeaderPart.RowDefinitions.Clear();

                if (stackFullCapacityHeaderPart.IsVisible)
                {
                    stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 45 });
                    stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 70 });
                    stackFullCapacityHeaderPart.RowDefinitions.Add(new RowDefinition { Height = 60 });
                }

                #endregion Capacity Setting

                #region SOC Setting

                grdMaximumSOC.ColumnDefinitions.Clear();
                grdMaximumSOC.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                grdMaximumSOC.RowDefinitions.Clear();
                grdMaximumSOC.RowDefinitions.Add(new RowDefinition { Height = 30 });
                grdMaximumSOC.RowDefinitions.Add(new RowDefinition { Height = 30 });
                grdMaximumSOC.RowDefinitions.Add(new RowDefinition { Height = 60 });

                Grid.SetRow(lblMaximumSOC, 0);
                Grid.SetColumn(lblMaximumSOC, 0);
                Grid.SetColumnSpan(lblMaximumSOC, 1);

                Grid.SetRow(stackMaximumSOC, 1);
                Grid.SetColumn(stackMaximumSOC, 0);
                Grid.SetColumnSpan(stackMaximumSOC, 2);

                Grid.SetRow(btnMaxSocUpdateRadio, 2);
                Grid.SetColumn(btnMaxSocUpdateRadio, 0);
                Grid.SetColumnSpan(btnMaxSocUpdateRadio, 2);

                #endregion SOC Setting

                #region Other Setting

                grdEnablePinrowmain.Height = 90;
                if (regionResetPin.IsVisible)
                {
                    grdEnablePinrowmain.Height = 160;
                }

                grdPoolRequest.RowDefinitions.Clear();

                grdPoolRequest.RowDefinitions.Add(new RowDefinition { Height = 30 });
                grdPoolRequest.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                grdPoolRequest.RowDefinitions.Add(new RowDefinition { Height = 60 });

                grdPollRequestcln3.Width = 0;
                grdEstablishConnectioncln3.Width = 0;

                Grid.SetRow(lblPoolRequest, 0);
                Grid.SetColumn(lblPoolRequest, 0);
                Grid.SetColumnSpan(lblPoolRequest, 2);

                Grid.SetRow(stackPoolRequest, 1);
                Grid.SetColumn(stackPoolRequest, 0);
                Grid.SetColumnSpan(stackPoolRequest, 2);

                Grid.SetRow(btnPullRequest, 2);
                Grid.SetColumn(btnPullRequest, 0);
                Grid.SetColumnSpan(btnPullRequest, 2);

                Grid.SetRow(lblEstablishConnection, 0);
                Grid.SetColumn(lblEstablishConnection, 0);
                Grid.SetColumnSpan(lblEstablishConnection, 2);

                Grid.SetRow(lblFaultRelease, 0);
                Grid.SetColumn(lblFaultRelease, 0);
                Grid.SetColumnSpan(lblFaultRelease, 2);

                Grid.SetRow(stackEstablishConnection, 1);
                Grid.SetColumn(stackEstablishConnection, 0);
                Grid.SetColumnSpan(stackEstablishConnection, 2);

                Grid.SetRow(stackFaultRelease, 1);
                Grid.SetColumn(stackFaultRelease, 0);
                Grid.SetColumnSpan(stackFaultRelease, 2);

                Grid.SetRow(btnEstablishConnection, 2);
                Grid.SetColumn(btnEstablishConnection, 0);
                Grid.SetColumnSpan(btnEstablishConnection, 2);

                Grid.SetRow(btnFaultRelease, 2);
                Grid.SetColumn(btnFaultRelease, 0);
                Grid.SetColumnSpan(btnFaultRelease, 2);

                Grid.SetRow(stackResetPin, 1);
                Grid.SetColumn(stackResetPin, 0);
                Grid.SetColumnSpan(stackResetPin, 2);

                Grid.SetRow(regionResetPin, 2);
                Grid.SetColumn(regionResetPin, 0);
                Grid.SetColumnSpan(regionResetPin, 2);

                #endregion Other Setting

                #endregion

                #region Benchmark Tab

                if (objFileViewModelList != null && objFileViewModelList.Count > 0)
                {
                    stackBenchMarkFilesList.HeightRequest = lstBenchMarkFilesList.HeightRequest = ((objFileViewModelList.Count + 1) * 85);
                }
                else
                {
                    stackBenchMarkFilesList.HeightRequest = lstBenchMarkFilesList.HeightRequest = 100;
                }

                StackBenchmarkDetails.HeightRequest = 120;
                StackBenchmarkDetails.WidthRequest = StackBenchmarkSocAndGuage.WidthRequest = width;

                grdListViewHeader.HeightRequest = 45;
                SfComboBoxborder.HeightRequest = 45;
                SfComboBoxborder.WidthRequest = (width / 2);

                grdrowGaugeOne2.Height = 130;
                grdrowGaugeOne3.Height = 50;

                grdrowGaugeTwo2CT1.Height = 130;
                grdrowGaugeTwo3CT1.Height = 50;

                grdrowGaugeThree2CT2.Height = 130;
                grdrowGaugeThree3CT2.Height = 50;

                lblBmsTemp.FontSize = lblCaseTemp.FontSize = lblCaseTemp2.FontSize = 20;
                stackBenchmarkButton.WidthRequest = width;
                StackBenchmarkSocAndGuage.Orientation = StackOrientation.Vertical;
                #endregion Benchmark Tab

                #region Calibration Tab       

                brdChargeCurrentSCEdit.HeightRequest = 150;
                brdDisChargeCurrentSCEdit.HeightRequest = 150;

                btnChargeCurrentSC.HorizontalOptions = LayoutOptions.EndAndExpand;
                btnDisChargeCurrentSC.HorizontalOptions = LayoutOptions.EndAndExpand;
                if (Is24VBattery)
                {
                    lstViewSpanCount.SpanCount = 4;
                    lstTCViewSpanCount.SpanCount = 2;
                }
                else
                {
                    lstViewSpanCount.SpanCount = 2;
                    lstTCViewSpanCount.SpanCount = 2;
                }

                lstVoltageCalibration.ItemSize = (width / 2.4);
                lstVoltageCalibration.WidthRequest = lstVoltageCalibration.ItemSize * 2;

                lstVoltageCalibration.MinimumHeightRequest = 120 * lstViewSpanCount.SpanCount;
                lstVoltageCalibration.HeightRequest = 120 * lstViewSpanCount.SpanCount;

                stackVoltageCalibrationValue.Orientation = StackOrientation.Vertical;
                brdVoltageCalibrationEdit.HeightRequest = 110;
                brdVoltageCalibrationEdit.MinimumHeightRequest = 110;
                brdVoltageCalibrationEdit.Padding = new Thickness(10, 10, 10, 10);

                VoltageCalibrationUpdate.HorizontalOptions = LayoutOptions.EndAndExpand;

                lstTemperatureCalibration.ItemSize = (width / 2.1);
                lstTemperatureCalibration.WidthRequest = lstTemperatureCalibration.ItemSize * 2;
                lstTemperatureCalibration.MinimumHeightRequest = 120 * lstTCViewSpanCount.SpanCount;

                lstTemperatureCalibration.HeightRequest = 120 * lstTCViewSpanCount.SpanCount;

                brdVoltageCalibrationEdit.Stroke = Colors.White;

                stacklowerVoltageCal.HorizontalOptions = LayoutOptions.FillAndExpand;

                stacklowerVoltageCal.Orientation = StackOrientation.Vertical;

                VoltageCalibrationUpdate.Margin = new Thickness(0, 0, 10, 0);

                stacklowerVoltageCal.Margin = new Thickness(0, 0, 0, 0);

                lstVoltageCalibration.HorizontalOptions = LayoutOptions.CenterAndExpand;

                grdVoltageCalibrationValue.RowDefinitions.Clear();
                grdVoltageCalibrationValue.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grdVoltageCalibrationValue.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                grdVoltageCalibrationValue.ColumnDefinitions.Clear();
                grdVoltageCalibrationValue.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Grid.SetRow(stackVoltageCalibrationValueInnerOne, 0);
                Grid.SetRow(stackVoltageCalibrationValueInnerTwo, 1);

                Grid.SetColumn(stackVoltageCalibrationValueInnerOne, 0);
                Grid.SetColumn(stackVoltageCalibrationValueInnerTwo, 0);

                Grid.SetColumnSpan(stackVoltageCalibrationValueInnerOne, 1);
                Grid.SetColumnSpan(stackVoltageCalibrationValueInnerTwo, 1);

                brdVoltageCalibrationEdit.HorizontalOptions = LayoutOptions.FillAndExpand;
                stackVoltageCalibrationValue.HorizontalOptions = LayoutOptions.FillAndExpand;

                stackVoltageCalibrationValueInnerOne.HorizontalOptions = LayoutOptions.CenterAndExpand;
                stackVoltageCalibrationValueInnerTwo.HorizontalOptions = LayoutOptions.Center;

                stackVoltageCalibrationValueInnerTwo.Margin = new Thickness(0, 0, 0, 0);

                stackVoltageCalibrationUpdate.Margin = new Thickness(10, 0, 0, 0);

                stackVoltageCalibrationValueInnerOne.Orientation = StackOrientation.Horizontal;
                stackValueAndButtonMain.Orientation = StackOrientation.Horizontal;

                stackMainChargeCurrent.Orientation = StackOrientation.Vertical;
                stackMainDischargeCurrent.Orientation = StackOrientation.Vertical;

                #endregion Calibration Tab

                #region About Tab

                grdAboutMainBox.Margin = new Thickness(5, 15, 5, 0);

                #endregion About Tab

                #region Log Tab

                lblShortCircuite.Text = "Short\ncircuit";
                lblChargingovercurrent.Text = "Charging\novercurrent";
                lblDischargingovercurrent.Text = "Discharging\novercurrent";
                lblCellovervoltage.Text = "Cell\novervoltage";
                lblCellundervoltage.Text = "Cell\nundervoltage";
                lblChargingovertemperature.Text = "Charging\nover temp.";
                lblChargingundertemperature.Text = "Charging\nlow temp.";
                lblDischargingovertemperature.Text = "Discharging\nover temp.";
                lblDischargingundertemperature.Text = "Discharge\nlow temp.";
                lblPackovervolt.Text = "Pack\novervolt";
                lblPackundervolt.Text = "Pack\nundervolt";


                grdLogs.ColumnDefinitions.Clear();
                grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                grdLogs.RowDefinitions.Clear();

                for (int i = 0; i < 6; i++)
                {
                    grdLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }

                Grid.SetRow(brdShortCircuite, 0);
                Grid.SetRow(brdChargingovercurrent, 0);
                Grid.SetRow(brdDischargingovercurrent, 1);
                Grid.SetRow(brdCellovervoltage, 1);
                Grid.SetRow(brdCellundervoltage, 2);
                Grid.SetRow(brdChargingovertemperature, 2);
                Grid.SetRow(brdChargingundertemperature, 3);
                Grid.SetRow(brdDischargingovertemperature, 3);
                Grid.SetRow(brdDischargingundertemperature, 4);
                Grid.SetRow(brdPackovervolt, 4);
                Grid.SetRow(brdPackundervolt, 5);

                Grid.SetColumn(brdShortCircuite, 0);
                Grid.SetColumn(brdChargingovercurrent, 1);
                Grid.SetColumn(brdDischargingovercurrent, 0);
                Grid.SetColumn(brdCellovervoltage, 1);
                Grid.SetColumn(brdCellundervoltage, 0);
                Grid.SetColumn(brdChargingovertemperature, 1);
                Grid.SetColumn(brdChargingundertemperature, 0);
                Grid.SetColumn(brdDischargingovertemperature, 1);
                Grid.SetColumn(brdDischargingundertemperature, 0);
                Grid.SetColumn(brdPackovervolt, 1);
                Grid.SetColumn(brdPackundervolt, 0);

                grdBMSLogs.ColumnDefinitions.Clear();
                grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                grdBMSLogs.RowDefinitions.Clear();
                for (int i = 0; i < 5; i++)
                {
                    grdBMSLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }
                Grid.SetRow(brdHourHighTemperature, 0);
                Grid.SetRow(brdHourLowTemperature, 0);
                Grid.SetRow(brdHourHighSoc, 1);
                Grid.SetRow(brdHourLowSoc, 1);
                Grid.SetRow(brdCriticallyHourHighTemperature, 2);
                Grid.SetRow(brdCriticallyHourLowTemperature, 2);
                Grid.SetRow(brdCriticallyLowSOC, 3);
                Grid.SetRow(brdAMPBoost, 3);
                Grid.SetRow(brdDaySinceFullCharge, 4);

                Grid.SetColumn(brdHourHighTemperature, 0);
                Grid.SetColumn(brdHourLowTemperature, 1);
                Grid.SetColumn(brdHourHighSoc, 0);
                Grid.SetColumn(brdHourLowSoc, 1);
                Grid.SetColumn(brdCriticallyHourHighTemperature, 0);
                Grid.SetColumn(brdCriticallyHourLowTemperature, 1);
                Grid.SetColumn(brdCriticallyLowSOC, 0);
                Grid.SetColumn(brdAMPBoost, 1);
                Grid.SetColumn(brdDaySinceFullCharge, 0);

                #endregion Log Tab
            }
            catch { }

        }
        private async Task LandscapDesignForMobile(double width, double height)
        {
            try
            {
                #region Main Tab

                stackCycleRemaing.Orientation = StackOrientation.Vertical;
                stackCycleRemaing.HorizontalOptions = LayoutOptions.FillAndExpand;

                stackGaugeMainStack.WidthRequest = ((width / 100) * 40);
                MainStackTwo.WidthRequest = ((width / 100) * 50);

                stackCycleCountMain.Margin = new Thickness(0, 25, 0, 0);

                stackGaugeMainStack.HorizontalOptions = LayoutOptions.CenterAndExpand;
                MainStackTwo.HorizontalOptions = LayoutOptions.CenterAndExpand;

                gridMainTabOuter.Margin = new Thickness(20);
                scrMainTab.IsEnabled = true;

                gridMainTabOuter.Orientation = StackOrientation.Vertical;

                Grid.SetRow(brWarningOne, 0);

                brWarningOne.Margin = new Thickness(0, 10, 0, 0);

                stackGaugeMainStack.Margin = new Thickness(0, 0, 0, 0);
                stackBMSDetails.Margin = new Thickness(0, 10, 0, 10);

                stackFirstMain.Margin = new Thickness(0, 0, 0, 0);

                #endregion Main Tab

                #region Details Tab

                lblDetailsPower.FontSize = 20;
                lblDetailsAmps.FontSize = 20;
                lblDetailsVolts.FontSize = 20;
                lblDetailsTemp.FontSize = 20;

                lblgaugePowerHeader.FontSize = 18;

                lblgaugeAmsPositive.FontSize = 18;

                lblgaugeAmsNegative.FontSize = 18;

                lblgaugeVolts.FontSize = 18;

                lblGaugeTempCaseHeader.FontSize = 16;

                lblGaugeTempBMSHeader.FontSize = 16;

                sfbDetailsPower.Padding = new Thickness(0, 0, 0, 0);
                sfbDetailsAmps.Padding = new Thickness(0, 0, 0, 0);
                sfbDetailsVolts.Padding = new Thickness(0, 0, 0, 0);
                sfbDetailsTemp.Padding = new Thickness(0, 0, 0, 0);

                Grid.SetRow(lblDetailsPower, 0);
                Grid.SetRow(lblDetailsAmps, 0);
                Grid.SetRow(lblDetailsVolts, 0);
                Grid.SetRow(lblDetailsTemp, 0);

                imgDetailsPower.IsVisible = imgDetailsAmps.IsVisible = imgDetailsVolts.IsVisible = imgDetailsTemp.IsVisible = false;

                grdrowDetailTabSegmantedButton.Height = 65;

                #endregion Details Tab

                #region Benchmark Tab

                stackBenchmarkTwo.Orientation = StackOrientation.Horizontal;

                stackBenchmarkInnerOne.WidthRequest = (width - (width / 3));
                StackBenchmarkDetails.HeightRequest = 110;
                StackBenchmarkDetails.WidthRequest = StackBenchmarkSocAndGuage.WidthRequest = width;

                grdListViewHeader.HeightRequest = 45;
                SfComboBoxborder.HeightRequest = 45;
                SfComboBoxborder.WidthRequest = (width / 2.5);
                stackBenchmarkTwo.WidthRequest = width;
                grdrowGaugeOne2.Height = 120;
                grdrowGaugeOne3.Height = 30;

                grdrowGaugeTwo2CT1.Height = 120;
                grdrowGaugeTwo3CT1.Height = 30;

                grdrowGaugeThree2CT2.Height = 120;
                grdrowGaugeThree3CT2.Height = 30;

                lblBmsTemp.FontSize = lblCaseTemp.FontSize = lblCaseTemp2.FontSize = 20;

                if (objFileViewModelList != null && objFileViewModelList.Count > 0)
                {
                    stackBenchMarkFilesList.HeightRequest = lstBenchMarkFilesList.HeightRequest = ((objFileViewModelList.Count + 1) * 85);
                }
                else
                {
                    stackBenchMarkFilesList.HeightRequest = lstBenchMarkFilesList.HeightRequest = 100;
                }
                #endregion Benchmark Tab

                #region Calibration Tab

                VoltageCalibrationUpdate.Margin = new Thickness(0, 5, 25, 0);

                btnChargeCurrentSC.HorizontalOptions = LayoutOptions.StartAndExpand;
                btnDisChargeCurrentSC.HorizontalOptions = LayoutOptions.StartAndExpand;

                stackMainVoltageCalibration.Orientation = StackOrientation.Vertical;
                stackMainVoltageCalibration.HorizontalOptions = LayoutOptions.CenterAndExpand;

                if (Is24VBattery)
                {
                    lstViewSpanCount.SpanCount = 2;
                    lstTCViewSpanCount.SpanCount = 1;
                }
                else
                {
                    lstViewSpanCount.SpanCount = 1;
                    lstTCViewSpanCount.SpanCount = 1;
                }

                lstVoltageCalibration.HorizontalOptions = LayoutOptions.CenterAndExpand;

                lstVoltageCalibration.ItemSize = ((width / 100) * 22);
                lstVoltageCalibration.WidthRequest = lstVoltageCalibration.ItemSize * 4;

                lstVoltageCalibration.MinimumHeightRequest = 120 * lstViewSpanCount.SpanCount;
                lstVoltageCalibration.HeightRequest = 120 * lstViewSpanCount.SpanCount;


                grdVoltageCalibrationValue.RowDefinitions.Clear();
                grdVoltageCalibrationValue.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                grdVoltageCalibrationValue.ColumnDefinitions.Clear();
                grdVoltageCalibrationValue.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdVoltageCalibrationValue.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                stackVoltageCalibrationValueInnerOne.Orientation = StackOrientation.Horizontal;
                stackVoltageCalibrationValue.Orientation = StackOrientation.Horizontal;

                brdVoltageCalibrationEdit.HorizontalOptions = LayoutOptions.StartAndExpand;
                brdVoltageCalibrationEdit.HeightRequest = 45;
                brdVoltageCalibrationEdit.MinimumHeightRequest = 80;
                brdVoltageCalibrationEdit.Padding = new Thickness(10, 10, 10, 10);

                VoltageCalibrationUpdate.HorizontalOptions = LayoutOptions.EndAndExpand;

                lstTCViewSpanCount.SpanCount = 1;

                lstTemperatureCalibration.ItemSize = ((width / 100) * 22);
                lstTemperatureCalibration.WidthRequest = lstTemperatureCalibration.ItemSize * 3;
                lstTemperatureCalibration.MinimumHeightRequest = 120 * lstTCViewSpanCount.SpanCount;
                lstTemperatureCalibration.HeightRequest = 120 * lstTCViewSpanCount.SpanCount;

                brdVoltageCalibrationEdit.Stroke = Colors.White;

                stacklowerVoltageCal.Orientation = StackOrientation.Horizontal;

                VoltageCalibrationUpdate.Margin = new Thickness(0, 5, 0, 0);

                stacklowerVoltageCal.Margin = new Thickness(0, 0, 0, 0);

                Grid.SetRow(stackVoltageCalibrationValueInnerOne, 0);
                Grid.SetRow(stackVoltageCalibrationValueInnerTwo, 0);

                Grid.SetColumn(stackVoltageCalibrationValueInnerOne, 0);
                Grid.SetColumn(stackVoltageCalibrationValueInnerTwo, 1);

                brdVoltageCalibrationEdit.HorizontalOptions = LayoutOptions.EndAndExpand;
                stackVoltageCalibrationValue.HorizontalOptions = LayoutOptions.EndAndExpand;

                stackTempCalibrationEdit.HorizontalOptions = LayoutOptions.CenterAndExpand;

                #endregion Calibration Tab

                #region About Tab

                grdAboutMainBox.Margin = new Thickness(40, 15, 40, 0);
                #endregion About Tab

                #region Log Tab

                lblShortCircuite.Text = "Short circuit";
                lblChargingovercurrent.Text = "Charging overcurrent";
                lblDischargingovercurrent.Text = "Discharging overcurrent";
                lblCellovervoltage.Text = "Cell overvoltage";
                lblCellundervoltage.Text = "Cell undervoltage";
                lblChargingovertemperature.Text = "Charging over temp.";
                lblChargingundertemperature.Text = "Charging low temp.";
                lblDischargingovertemperature.Text = "Discharging over temp.";
                lblDischargingundertemperature.Text = "Discharge low temp.";
                lblPackovervolt.Text = "Pack overvolt";
                lblPackundervolt.Text = "Pack undervolt";

                grdLogs.ColumnDefinitions.Clear();
                grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                grdLogs.RowDefinitions.Clear();

                for (int i = 0; i < 4; i++)
                {
                    grdLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }

                Grid.SetRow(brdShortCircuite, 0);
                Grid.SetRow(brdChargingovercurrent, 0);
                Grid.SetRow(brdDischargingovercurrent, 0);
                Grid.SetRow(brdCellovervoltage, 1);
                Grid.SetRow(brdCellundervoltage, 1);
                Grid.SetRow(brdChargingovertemperature, 1);
                Grid.SetRow(brdChargingundertemperature, 2);
                Grid.SetRow(brdDischargingovertemperature, 2);
                Grid.SetRow(brdDischargingundertemperature, 2);
                Grid.SetRow(brdPackovervolt, 3);
                Grid.SetRow(brdPackundervolt, 3);

                Grid.SetColumn(brdShortCircuite, 0);
                Grid.SetColumn(brdChargingovercurrent, 1);
                Grid.SetColumn(brdDischargingovercurrent, 2);
                Grid.SetColumn(brdCellovervoltage, 0);
                Grid.SetColumn(brdCellundervoltage, 1);
                Grid.SetColumn(brdChargingovertemperature, 2);
                Grid.SetColumn(brdChargingundertemperature, 0);
                Grid.SetColumn(brdDischargingovertemperature, 1);
                Grid.SetColumn(brdDischargingundertemperature, 2);
                Grid.SetColumn(brdPackovervolt, 0);
                Grid.SetColumn(brdPackundervolt, 1);

                grdBMSLogs.ColumnDefinitions.Clear();
                grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grdBMSLogs.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                grdBMSLogs.RowDefinitions.Clear();
                for (int i = 0; i < 3; i++)
                {
                    grdBMSLogs.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }
                Grid.SetRow(brdHourHighTemperature, 0);
                Grid.SetRow(brdHourLowTemperature, 0);
                Grid.SetRow(brdHourHighSoc, 0);
                Grid.SetRow(brdHourLowSoc, 1);
                Grid.SetRow(brdCriticallyHourHighTemperature, 1);
                Grid.SetRow(brdCriticallyHourLowTemperature, 1);
                Grid.SetRow(brdCriticallyLowSOC, 2);
                Grid.SetRow(brdAMPBoost, 2);
                Grid.SetRow(brdDaySinceFullCharge, 2);

                Grid.SetColumn(brdHourHighTemperature, 0);
                Grid.SetColumn(brdHourLowTemperature, 1);
                Grid.SetColumn(brdHourHighSoc, 2);
                Grid.SetColumn(brdHourLowSoc, 0);
                Grid.SetColumn(brdCriticallyHourHighTemperature, 1);
                Grid.SetColumn(brdCriticallyHourLowTemperature, 2);
                Grid.SetColumn(brdCriticallyLowSOC, 0);
                Grid.SetColumn(brdAMPBoost, 1);
                Grid.SetColumn(brdDaySinceFullCharge, 2);

                #endregion Log Tab
            }
            catch { }
        }
        private async Task WarningAlertDesign(int count)
        {
            try
            {
                if (brWarningOne.Width > 0)
                {
                    clnInnerMainWarning1.Width = clnInnerMainWarning2.Width = clnInnerMainWarning3.Width = clnInnerMainWarning4.Width = clnInnerMainWarning5.Width = clnInnerMainWarning6.Width = clnInnerMainWarning7.Width = clnInnerMainWarning8.Width = clnInnerMainWarning9.Width = clnInnerMainWarning10.Width = 0;

                    if (FirstWarning.IsVisible)
                    {
                        clnInnerMainWarning1.Width = (brWarningOne.Width / count);
                    }
                    if (SecoundWarning.IsVisible)
                    {
                        clnInnerMainWarning2.Width = (brWarningOne.Width / count);
                    }
                    if (ThirdWarning.IsVisible)
                    {
                        clnInnerMainWarning3.Width = (brWarningOne.Width / count);
                    }
                    if (FourthWarning.IsVisible)
                    {
                        clnInnerMainWarning4.Width = (brWarningOne.Width / count);
                    }
                    if (FifthWarning.IsVisible)
                    {
                        clnInnerMainWarning5.Width = (brWarningOne.Width / count);
                    }
                    if (SixthWarning.IsVisible)
                    {
                        clnInnerMainWarning6.Width = (brWarningOne.Width / count);
                    }
                    if (SeventhWarning.IsVisible)
                    {
                        clnInnerMainWarning7.Width = (brWarningOne.Width / count);
                    }
                    if (EightWarning.IsVisible)
                    {
                        clnInnerMainWarning8.Width = (brWarningOne.Width / count);
                    }
                    if (NinethWarning.IsVisible)
                    {
                        clnInnerMainWarning9.Width = (brWarningOne.Width / count);
                    }
                    if (TenthWarning.IsVisible)
                    {
                        clnInnerMainWarning10.Width = (brWarningOne.Width / count);
                    }

                    if (EleventhWarning.IsVisible)
                    {
                        clnInnerMainWarning11.Width = (brWarningOne.Width / count);
                    }
                    if (TwelfthWarning.IsVisible)
                    {
                        clnInnerMainWarning12.Width = (brWarningOne.Width / count);
                    }
                    if (ThirteenthWarning.IsVisible)
                    {
                        clnInnerMainWarning13.Width = (brWarningOne.Width / count);
                    }
                    if (FourteenthWarning.IsVisible)
                    {
                        clnInnerMainWarning14.Width = (brWarningOne.Width / count);
                    }
                    if (FifteenthWarning.IsVisible)
                    {
                        clnInnerMainWarning15.Width = (brWarningOne.Width / count);
                    }
                    if (SixteenthWarning.IsVisible)
                    {
                        clnInnerMainWarning16.Width = (brWarningOne.Width / count);
                    }
                    if (SeventeenthWarning.IsVisible)
                    {
                        clnInnerMainWarning17.Width = (brWarningOne.Width / count);
                    }
                    if (EighteenthWarning.IsVisible)
                    {
                        clnInnerMainWarning18.Width = (brWarningOne.Width / count);
                    }

                    if (NineteenWarning.IsVisible)
                    {
                        clnInnerMainWarning19.Width = (brWarningOne.Width / count);
                    }
                    if (TwentyWarning.IsVisible)
                    {
                        clnInnerMainWarning20.Width = (brWarningOne.Width / count);
                    }
                    if (TwentyOneWarning.IsVisible)
                    {
                        clnInnerMainWarning21.Width = (brWarningOne.Width / count);
                    }
                    if (TwentyTwoWarning.IsVisible)
                    {
                        clnInnerMainWarning22.Width = (brWarningOne.Width / count);
                    }

                }
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
                        sbbtn1.Padding = new Thickness(5, 0, 5, 0);
                        sbbtn2.Padding = new Thickness(5, 0, 5, 0);

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

                            PasswordConfirmBoxSL.HeightRequest = (height / 100) * 90;
                            PasswordConfirmBoxSL.WidthRequest = (width / 100) * 100;
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

                            PasswordConfirmBoxSL.HeightRequest = (height / 100) * 70;
                            PasswordConfirmBoxSL.WidthRequest = (width / 100) * 100;
                        }
                    }
                }
                else //potrait
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        sbbtn1.Padding = new Thickness(5, 0, 5, 0);
                        sbbtn2.Padding = new Thickness(5, 0, 5, 0);

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
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 80;
                        }
                        else
                        {
                            stkDisplayMessagePopUp.HeightRequest = (DeviceHeight / 100) * 50;
                            stkDisplayMessagePopUp.WidthRequest = (DeviceWidth / 100) * 90;

                            PasswordConfirmBoxSL.HeightRequest = (height / 100) * 60;
                            PasswordConfirmBoxSL.WidthRequest = (width / 100) * 100;
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

                            PasswordConfirmBoxSL.HeightRequest = (height / 100) * 45;
                            PasswordConfirmBoxSL.WidthRequest = (width / 100) * 100;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        #endregion Responsive Design

        #region Send and Read Command to BMS

        private async Task ReadBMSDetails03Command()
        {
            try
            {
                var bytes1 = objProtocol.CreateReadCommand(RegisterEnum.REG_GENERAL);
                await SendCommandToBMS(bytes1);
            }
            catch (Exception ex)
            {
            }
        }
        private async Task ReadCellCount04Command()
        {
            try
            {
                var bytes2 = objProtocol.CreateReadCommand(RegisterEnum.REG_CELL);
                await SendCommandToBMS(bytes2);
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task ReadBMSCodeWith0ACommand()
        {
            try
            {
                int tCount = 0;

                while (tCount < 2) // Register 0A
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    // var bytes2 = new byte[] { 0xdd, 0xa5, 0x0a, 0x00, 0xff, 0xf6, 0x77 }; //BMS unique code reading
                    var bytes2 = objProtocol.CreateReadCommand(RegisterEnum.REG_MCUID);
                    await SendCommandToBMS(bytes2);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();

                    await ShowBusyindicatior(true);

                    if (!UsedNewFirmware)
                    {
                        tCount++;
                    }
                    else
                    {
                        break;
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
        private async Task ReadNicIdCommand()
        {
            try
            {
                int tCount = 0;
                while (tCount < 3) // Register E9
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(50);

                    // var bytes2 = new byte[] { 0xdd, 0xa5, 0x0a, 0x00, 0xff, 0xf6, 0x77 }; //BMS unique code reading
                    var bytes2 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_NICID, "03", 0);
                    await SendCommandToBMS(bytes2);

                    await Task.Delay(50);

                    await SendWriteOrReadEndCommandToBMS();

                    await ShowBusyindicatior(true);

                    if (!UsedNewFirmware || lblNicID.Text == "N/A")
                    {
                        tCount++;
                    }
                    else
                    {
                        break;
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
        public async Task SendCommandToBMS(byte[] SendCommand, bool SendToClient = true)
        {
            try
            {
                if (SendToClient)
                {
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.byteCommand, SendCommand);
                }
                else
                {
                    await Task.Delay(165);
                }
            }
            catch (Exception ex)
            {

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
        private async Task ReadRTCTimeInterval()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(150);

                // CC D6 03 00 FF FD CF
                var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_T_NTC_6, "03", 00);
                await SendCommandToBMS(bytes);

                await Task.Delay(150);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
            }
        }
        private async Task WriteRTCTimeInterval()
        {
            try
            {
                IsH2SettingUpdating = true;
                int count = 0;
                while (count < 3)
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    // CC D5 0A 00   00 3C 0C  17 0B 07 E8   FE 9D CF
                    var bytes2 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_T_NTC_5, "0A", 0, "RTC");
                    await SendCommandToBMS(bytes2);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(100);

                    await ReadRTCTimeInterval();

                    await Task.Delay(500);

                    if (TimeSyncronized)
                    {
                        break;
                    }
                }
                CurrentPopup = string.Empty;
                IsH2SettingUpdating = false;
            }
            catch (Exception ex)
            {
            }
        }

        #endregion Send Command to BMS

        #region Bind Slider Value
        private async void rangesliderLowTemp_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                sCelsiusorFahrenheit = Preferences.Get("CelsiusorFahrenheit", string.Empty);

                if (sCelsiusorFahrenheit == "" || sCelsiusorFahrenheit == null)
                {
                    sCelsiusorFahrenheit = "Fahrenheit";
                }

                if (sCelsiusorFahrenheit == "Fahrenheit")
                {
                    var item = objDeviceInformation.CustomCollectionLowTemp.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                    if (item != null)
                    {
                        e.Text = item.Label;
                    }
                }
                else
                {
                    var item = objDeviceInformation.CustomCollectionLowTempCelsius.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                    if (item != null)
                    {
                        e.Text = item.Label;
                    }
                }
            }
            catch { }
        }
        private async void rangesliderMaxBatteryTemp_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                sCelsiusorFahrenheit = Preferences.Get("CelsiusorFahrenheit", string.Empty);

                if (sCelsiusorFahrenheit == "" || sCelsiusorFahrenheit == null)
                {
                    sCelsiusorFahrenheit = "Fahrenheit";
                }

                if (sCelsiusorFahrenheit == "Fahrenheit")
                {
                    var item = objDeviceInformation.CustomCollectionMaxBatteryTemp.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                    if (item != null)
                    {
                        e.Text = item.Label;
                    }
                }
                else
                {
                    var item = objDeviceInformation.CustomCollectionMaxBatteryTempCelsius.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                    if (item != null)
                    {
                        e.Text = item.Label;
                    }
                }
            }
            catch { }
        }
        private async void NF_rangesliderLowTemp_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                sCelsiusorFahrenheit = Preferences.Get("CelsiusorFahrenheit", string.Empty);

                if (sCelsiusorFahrenheit == "" || sCelsiusorFahrenheit == null)
                {
                    sCelsiusorFahrenheit = "Fahrenheit";
                }

                if (sCelsiusorFahrenheit == "Fahrenheit")
                {
                    var item = objDeviceInformation.CustomCollectionLowTemp.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                    if (item != null)
                    {
                        e.Text = item.Label;
                    }
                }
                else
                {
                    int start = Convert.ToInt32(NF_rangesliderLowTemp.Minimum);
                    int end = Convert.ToInt32(NF_rangesliderLowTemp.Maximum);
                    int Interval = Convert.ToInt32(NF_rangesliderLowTemp.Interval);

                    objDeviceInformation.CustomCollectionLowTempCelsius = new ObservableCollection<RangeSliderItem>();
                    for (int i = start; i <= end; i = i + Interval)
                    {
                        objDeviceInformation.CustomCollectionLowTempCelsius.Add(new RangeSliderItem { Label = i + "°C", Value = i });
                    }

                    var item = objDeviceInformation.CustomCollectionLowTempCelsius.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                    if (item != null)
                    {
                        e.Text = item.Label;
                    }
                }
            }
            catch { }
        }
        private async void rangesliderLowVoltageCutOff_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionLowVoltageCutOff.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch { }
        }
        private void NF_rangesliderMaxSOC_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionMaxSoc.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch { }
        }
        private void NF_rangesliderMinSOC_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.NF_CustomCollectionMinSoc.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch { }
        }
        private void rangesliderMaxChargeAmps_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionMaxChargeAmps.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch { }
        }
        private void rangesliderMaxDisChargeAmps_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionMaxDisChargeAmps.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch { }
        }
        private void rangesliderFullCapacity_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionFullCapacity.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch { }
        }
        private void rangesliderActivationVoltage_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionActivationVoltage.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }

                rangesliderActivationVoltage.Value = 5;
            }
            catch { }
        }
        private void rangesliderDeviationLimit_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionDeviationLimit.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }

                rangesliderActivationVoltage.Value = 150;
            }
            catch { }
        }
        private void rangesliderTimeInterval_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionTimeInterval.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void rangesliderHighSOCLimit_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionHighSOCLimit.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void rangesliderChargePulse_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionChargePulse.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch { }
        }
        private void rangeCalibrateSOC_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionCalibrateSOC.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch { }
        }
        private void rangesliderHeatingPadTimer_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                switch (Convert.ToDouble(e.Text))
                {
                    case 1:
                        e.Text = "15";
                        break;
                    case 2:
                        e.Text = "60";
                        break;
                    case 3:
                        e.Text = "120";
                        break;
                    case 4:
                        e.Text = "240";
                        break;
                }
            }
            catch { }
        }
        private void rangeAppConnectedInterval_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {

        }
        private void rangeAppDisconnectedInterval_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {

        }

        #endregion Bind Slider Value

        #region Password
        static bool CheckValidPassword(string number)
        {
            bool a = false;
            if (number.Length > 0 && number.Length == 6 && number.All(Char.IsDigit))
            {
                a = true;
            }
            return a;
        }
        private async void btnResetPassword_Clicked(object sender, EventArgs e)
        {
            try
            {
                var keyboardService = App.Services.GetService<IKeyboardService>();
                keyboardService?.HideKeyboard();
                if (!string.IsNullOrWhiteSpace(txtEnterNewPassword.Text) || !string.IsNullOrWhiteSpace(txtEnterConfirmPassword.Text))
                {
                    if ((txtEnterNewPassword.Text.Trim().Length == 6) && (txtEnterConfirmPassword.Text.Trim().Length == 6))
                    {
                        if (txtEnterNewPassword.Text.Trim() == txtEnterConfirmPassword.Text.Trim())
                        {

                            if (txtEnterNewPassword.Text.Trim() == "012345" && txtEnterConfirmPassword.Text.Trim() == "012345")
                            {
                                lblMessageResetPassword.IsVisible = true;
                                lblMessageResetPassword.TextColor = Colors.Red;
                                lblMessageResetPassword.Text = "012345 can not be used as a PIN, please use another 6 digit PIN code.";
                            }
                            else if ((txtEnterNewPassword.Text.Trim() == "123456" && txtEnterConfirmPassword.Text.Trim() == "123456")
                             || (txtEnterNewPassword.Text.Trim() == "000000" && txtEnterConfirmPassword.Text.Trim() == "000000"))
                            {
                                lblMessageResetPassword.IsVisible = true;
                                lblMessageResetPassword.TextColor = Colors.Red;
                                lblMessageResetPassword.Text = "000000 or 123456 can not be used as a PIN.";
                            }
                            else
                            {
                                SetNewPasswordforUser();
                            }
                        }
                        else
                        {
                            lblMessageResetPassword.IsVisible = true;
                            lblMessageResetPassword.TextColor = Colors.Red;
                            lblMessageResetPassword.Text = "New and confirm Pin must be same";
                        }
                    }
                    else
                    {
                        lblMessageResetPassword.IsVisible = true;
                        lblMessageResetPassword.TextColor = Colors.Red;
                        lblMessageResetPassword.Text = "Pin must be of 6 digits";
                    }
                }
                else
                {
                    lblMessageResetPassword.IsVisible = true;
                    lblMessageResetPassword.TextColor = Colors.Red;
                    lblMessageResetPassword.Text = "Invalid Pin";
                }
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", "Please Enter Valid Pin");
            }
        }
        public async void SetNewPasswordforUser()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtEnterNewPassword.Text) && !string.IsNullOrWhiteSpace(txtEnterConfirmPassword.Text))
                {
                    if (CheckValidPassword(txtEnterNewPassword.Text.Trim()) && CheckValidPassword(txtEnterConfirmPassword.Text.Trim()))
                    {
                        if (txtEnterNewPassword.Text.Trim() == txtEnterConfirmPassword.Text.Trim())
                        {
                            if (txtEnterNewPassword.Text.Trim() == "012345" && txtEnterConfirmPassword.Text.Trim() == "012345")
                            {
                                lblMessageResetPassword.IsVisible = true;
                                lblMessageResetPassword.TextColor = Colors.Red;
                                lblMessageResetPassword.Text = "012345 can not be used as a PIN, please use another 6 digit PIN code.";
                            }
                            else
                            {
                                isAlreadyIntialLoadDecidedtheDefaultPassword = false;
                                var ResetPassword = objProtocol.PrepareCustomerPassword_Reset(DeviceOldPassword.Trim(), txtEnterNewPassword.Text.Trim());
                                await SendCommandToBMS(ResetPassword);

                                Preferences.Set((lblMacAddressID.Text + "_Password"), string.Empty);
                            }
                        }
                        else
                        {
                            lblMessageResetPassword.IsVisible = true;
                            //new and lbl are different 
                            lblMessageResetPassword.TextColor = Colors.Red;
                            lblMessageResetPassword.Text = "Both new and confirm pin should be same";
                        }
                    }
                    else
                    {
                        lblMessageResetPassword.IsVisible = true;
                        lblMessageResetPassword.TextColor = Colors.Red;
                        lblMessageResetPassword.Text = "Invalid Pin";
                    }
                }
                else
                {
                    lblMessageResetPassword.IsVisible = true;
                    lblMessageResetPassword.TextColor = Colors.Red;
                    lblMessageResetPassword.Text = "Invalid Pin";
                }

            }
            catch (Exception ex)
            {
                // await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        private async Task SetDefaultPin()
        {
            try
            {
                // var SetPassword = new byte[] { 0xdd, 0x5a, 0x09, 0x07, 0x06, 0x4a, 0x31, 0x42, 0x32, 0x44, 0x34, 0xfe, 0x83, 0x77 };
                // var SetPassword = new byte[] { 0xcc, 0xcd, 0x09, 0x07, 0x06, 0x4a, 0x31, 0x42, 0x32, 0x44, 0x34, 0xfe, 0x83, 0xcf };
                var SetPassword = objProtocol.PrepareDefaultPin09();
                await SendCommandToBMS(SetPassword);
            }
            catch (Exception ex)
            {
                // await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        public async Task CheckForDefaultPassword(string Password = "012345")
        {
            try
            {
                // var DefaultPassword = new byte[] { 0xdd, 0x5a, 0x06, 0x07, 0x06, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0xff, 0xde, 0x77 };
                var DefaultPassword = objProtocol.PrepareCustomerPassword(Password);
                await SendCommandToBMS(DefaultPassword);

                PasswordCheck06CalledFrom_IntialLoad = true;
                PasswordCheck06CalledFrom_Please_Enter_Password = false;
                PasswordCheck06CalledFrom_Reset_Password = false;

            }
            catch (Exception ex)
            {
                // await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }

        #endregion Password       

        #region ToolTab Update
        private async void btnMaxSocUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                int tCount = 0;
                bool IsValid = false;
                isProccesBegin = true;
                int NewMaxSOCValue = 0;
                int OldMaxSOCReadValue = objReadValues.MaxSOCReadValue;

                if (rbMaximumSOCService.IsChecked == true)  // 60% - 3.26
                {
                    NewMaxSOCValue = 60;
                    while (tCount < 2) // Register 24
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        // var setMaxSocReg24 = new byte[] { 0xdd, 0x5a, 0x24, 0x02, 0x0c, 0xbc, 0xff, 0x12, 0x77 };                        
                        var setMaxSocReg24 = objProtocol.CreateMvByte(RegisterEnum.REG_COVP, 3.26);
                        await SendCommandToBMS(setMaxSocReg24);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MaxSOCRead(RegisterEnum.REG_COVP);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidMaxSOCReadValue != 3.26)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidMaxSOCReadValue == 3.26)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 25
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            //var setMaxSocReg25 = new byte[] { 0xdd, 0x5a, 0x25, 0x02, 0x0c, 0xb2, 0xff, 0x1b, 0x77 };
                            var setMaxSocReg25 = objProtocol.CreateMvByte(RegisterEnum.REG_COVP_REL, 3.25);
                            await SendCommandToBMS(setMaxSocReg25);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxSOCRead(RegisterEnum.REG_COVP_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidMaxSOCReadValue != 3.25)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidMaxSOCReadValue == 3.25)
                        {
                            if (Is24VBattery)
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 20
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMaxSocReg20 = new byte[] { 0xdd, 0x5a, 0x20, 0x02, 0x0a, 0x30, 0xff, 0xa4, 0x77 };
                                    var setMaxSocReg20 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP, 26.08);
                                    await SendCommandToBMS(setMaxSocReg20);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MaxSOCRead(RegisterEnum.REG_POVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMaxSOCReadValue != 26.08)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMaxSOCReadValue == 26.08)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 21
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        //var setMaxSocReg21 = new byte[] { 0xdd, 0x5a, 0x21, 0x02, 0x0a, 0x28, 0xff, 0xab, 0x77 };
                                        var setMaxSocReg21 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP_REL, 26);
                                        await SendCommandToBMS(setMaxSocReg21);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MaxSOCRead(RegisterEnum.REG_POVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMaxSOCReadValue != 26)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 20
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMaxSocReg20 = new byte[] { 0xdd, 0x5a, 0x20, 0x02, 0x05, 0x18, 0xff, 0xc1, 0x77 };
                                    var setMaxSocReg20 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP, 13.04);
                                    await SendCommandToBMS(setMaxSocReg20);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MaxSOCRead(RegisterEnum.REG_POVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMaxSOCReadValue != 13.04)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMaxSOCReadValue == 13.04)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 21
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        //var setMaxSocReg21 = new byte[] { 0xdd, 0x5a, 0x21, 0x02, 0x05, 0x14, 0xff, 0xc4, 0x77 };
                                        var setMaxSocReg21 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP_REL, 13);
                                        await SendCommandToBMS(setMaxSocReg21);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MaxSOCRead(RegisterEnum.REG_POVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMaxSOCReadValue != 13)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (rbMaximumSOCStorage.IsChecked == true) // 70% - 3.295
                {
                    NewMaxSOCValue = 70;
                    while (tCount < 2) // Register 24
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        //var setMaxSocReg24 = new byte[] { 0xdd, 0x5a, 0x24, 0x02, 0x0c, 0xdf, 0xfe, 0xef, 0x77 };
                        var setMaxSocReg24 = objProtocol.CreateMvByte(RegisterEnum.REG_COVP, 3.295);
                        await SendCommandToBMS(setMaxSocReg24);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MaxSOCRead(RegisterEnum.REG_COVP);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidMaxSOCReadValue != 3.295)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidMaxSOCReadValue == 3.295)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 25
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            //var setMaxSocReg25 = new byte[] { 0xdd, 0x5a, 0x25, 0x02, 0x0c, 0xc6, 0xff, 0x07, 0x77 };
                            var setMaxSocReg25 = objProtocol.CreateMvByte(RegisterEnum.REG_COVP_REL, 3.27); // new byte[] { 0xdd, 0x5a, 0x25, 0x02, 0x0c, 0xc6, 0xff, 0x07, 0x77 };
                            await SendCommandToBMS(setMaxSocReg25);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxSOCRead(RegisterEnum.REG_COVP_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidMaxSOCReadValue != 3.27)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidMaxSOCReadValue == 3.27)
                        {
                            if (Is24VBattery)
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 20
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMaxSocReg20 = new byte[] { 0xdd, 0x5a, 0x20, 0x02, 0x0a, 0x4c, 0xff, 0x88, 0x77 };
                                    var setMaxSocReg20 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP, 26.36);
                                    await SendCommandToBMS(setMaxSocReg20);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MaxSOCRead(RegisterEnum.REG_POVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMaxSOCReadValue != 26.36)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMaxSOCReadValue == 26.36)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 21
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMaxSocReg21 = new byte[] { 0xdd, 0x5a, 0x21, 0x02, 0x0a, 0x38, 0xff, 0x9b, 0x77 };
                                        var setMaxSocReg21 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP_REL, 26.16);
                                        await SendCommandToBMS(setMaxSocReg21);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MaxSOCRead(RegisterEnum.REG_POVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMaxSOCReadValue != 26.16)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 20
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMaxSocReg20 = new byte[] { 0xdd, 0x5a, 0x20, 0x02, 0x05, 0x26, 0xff, 0xb3, 0x77 };
                                    var setMaxSocReg20 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP, 13.18);
                                    await SendCommandToBMS(setMaxSocReg20);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MaxSOCRead(RegisterEnum.REG_POVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMaxSOCReadValue != 13.18)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMaxSOCReadValue == 13.18)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 21
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMaxSocReg21 = new byte[] { 0xdd, 0x5a, 0x21, 0x02, 0x05, 0x1c, 0xff, 0xbc, 0x77 };
                                        var setMaxSocReg21 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP_REL, 13.08);
                                        await SendCommandToBMS(setMaxSocReg21);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MaxSOCRead(RegisterEnum.REG_POVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMaxSOCReadValue != 13.08)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (rbMaximumSOCNormal.IsChecked == true) //100% - 3.65
                {
                    NewMaxSOCValue = 100;
                    while (tCount < 2) // Register 24
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        //var setMaxSocReg24 = new byte[] { 0xdd, 0x5a, 0x24, 0x02, 0x0e, 0x42, 0xff, 0x8a, 0x77 };
                        var setMaxSocReg24 = objProtocol.CreateMvByte(RegisterEnum.REG_COVP, 3.65);
                        await SendCommandToBMS(setMaxSocReg24);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MaxSOCRead(RegisterEnum.REG_COVP);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidMaxSOCReadValue != 3.65)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidMaxSOCReadValue == 3.65)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 25
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            //var setMaxSocReg25 = new byte[] { 0xdd, 0x5a, 0x25, 0x02, 0x0d, 0xac, 0xff, 0x20, 0x77 };
                            var setMaxSocReg25 = objProtocol.CreateMvByte(RegisterEnum.REG_COVP_REL, 3.5);
                            await SendCommandToBMS(setMaxSocReg25);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxSOCRead(RegisterEnum.REG_COVP_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidMaxSOCReadValue != 3.5)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidMaxSOCReadValue == 3.5)
                        {

                            if (Is24VBattery)
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 20
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMaxSocReg20 = new byte[] { 0xdd, 0x5a, 0x20, 0x02, 0x0b, 0x68, 0xff, 0x6b, 0x77 };
                                    var setMaxSocReg20 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP, 29.2);
                                    await SendCommandToBMS(setMaxSocReg20);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MaxSOCRead(RegisterEnum.REG_POVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMaxSOCReadValue != 29.2)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMaxSOCReadValue == 29.2)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 21
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMaxSocReg21 = new byte[] { 0xdd, 0x5a, 0x21, 0x02, 0x0a, 0xf0, 0xfe, 0xe3, 0x77 };
                                        var setMaxSocReg21 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP_REL, 28);
                                        await SendCommandToBMS(setMaxSocReg21);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MaxSOCRead(RegisterEnum.REG_POVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMaxSOCReadValue != 28)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 20
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMaxSocReg20 = new byte[] { 0xdd, 0x5a, 0x20, 0x02, 0x05, 0xb4, 0xff, 0x25, 0x77 };
                                    var setMaxSocReg20 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP, 14.6);
                                    await SendCommandToBMS(setMaxSocReg20);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MaxSOCRead(RegisterEnum.REG_POVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMaxSOCReadValue != 14.6)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMaxSOCReadValue == 14.6)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 21
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMaxSocReg21 = new byte[] { 0xdd, 0x5a, 0x21, 0x02, 0x05, 0x78, 0xff, 0x60, 0x77 };
                                        var setMaxSocReg21 = objProtocol.CreateMv10Byte(RegisterEnum.REG_POVP_REL, 14);
                                        await SendCommandToBMS(setMaxSocReg21);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MaxSOCRead(RegisterEnum.REG_POVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMaxSOCReadValue != 14)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

                if (IsValid && (objReadValues.MaxSOCReadValue == NewMaxSOCValue))
                {
                    if (rbMaximumSOCService.IsChecked)
                    {
                        UpdateSOCModeImage("Service");
                    }
                    else if (rbMaximumSOCStorage.IsChecked)
                    {
                        UpdateSOCModeImage("Storage");
                    }
                    else
                    {
                        UpdateSOCModeImage("Normal");
                    }

                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnLowVoltageCutOff_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                int tCount = 0;
                isProccesBegin = true;
                bool IsValid = false;

                int NewMinSOCValue = (int)rangesliderLowVoltageCutOff.Value;

                if (Convert.ToString(rangesliderLowVoltageCutOff.Value) == "10")  // 12v - 10v, 24v - 20v
                {
                    NewMinSOCValue = 10;
                    while (tCount < 2) // Register 26
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        // var setMinSocReg26 = new byte[] { 0xcc, 0xcd, 0x26, 0x02, 0x09, 0xc4, 0xff, 0x0b, 0xcf }; // 2.500
                        // var setMinSocReg26 = new byte[] { 0xdd, 0x5a, 0x26, 0x02, 0x09, 0xc4, 0xff, 0x0b, 0x77 }; // 2.500
                        var setMinSocReg26 = objProtocol.CreateMvByte(RegisterEnum.REG_CUVP, 2.5);
                        await SendCommandToBMS(setMinSocReg26);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MinSocRead(RegisterEnum.REG_CUVP);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidMinSOCReadValue != 2.5)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidMinSOCReadValue == 2.5)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 27
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            //var setMinSocReg27 = new byte[] { 0xdd, 0x5a, 0x27, 0x02, 0x0a, 0x8c, 0xff, 0x41, 0x77 }; // 2.700
                            var setMinSocReg27 = objProtocol.CreateMvByte(RegisterEnum.REG_CUVP_REL, 2.7);
                            await SendCommandToBMS(setMinSocReg27);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MinSocRead(RegisterEnum.REG_CUVP_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidMinSOCReadValue != 2.7)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidMinSOCReadValue == 2.7)
                        {
                            if (Is24VBattery)
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 22
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMinSocReg22 = new byte[] { 0xdd, 0x5a, 0x22, 0x02, 0x07, 0xd0, 0xff, 0x05, 0x77 }; // 20.0
                                    var setMinSocReg22 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP, 20.0);
                                    await SendCommandToBMS(setMinSocReg22);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MinSocRead(RegisterEnum.REG_PUVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMinSOCReadValue != 20.0)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMinSOCReadValue == 20.0)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 23
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMinSocReg23 = new byte[] { 0xdd, 0x5a, 0x23, 0x02, 0x08, 0x70, 0xff, 0x63, 0x77 }; // 21.6
                                        var setMinSocReg23 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP_REL, 21.6);
                                        await SendCommandToBMS(setMinSocReg23);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MinSocRead(RegisterEnum.REG_PUVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMinSOCReadValue != 21.6)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 22
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMinSocReg22 = new byte[] { 0xdd, 0x5a, 0x22, 0x02, 0x03, 0xe8, 0xfe, 0xf1, 0x77 }; // 10.0
                                    var setMinSocReg22 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP, 10.0);
                                    await SendCommandToBMS(setMinSocReg22);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MinSocRead(RegisterEnum.REG_PUVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMinSOCReadValue != 10.0)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMinSOCReadValue == 10.0)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 23
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMinSocReg23 = new byte[] { 0xdd, 0x5a, 0x23, 0x02, 0x04, 0x38, 0xff, 0x9f, 0x77 }; // 10.8
                                        var setMinSocReg23 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP_REL, 10.8);
                                        await SendCommandToBMS(setMinSocReg23);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MinSocRead(RegisterEnum.REG_PUVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMinSOCReadValue != 10.8)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Convert.ToString(rangesliderLowVoltageCutOff.Value) == "11") // 12v - 10.8v, 24v - 21.6v
                {
                    NewMinSOCValue = 11;
                    while (tCount < 2) // Register 26
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        //var setMinSocReg26 = new byte[] { 0xdd, 0x5a, 0x26, 0x02, 0x0a, 0x8c, 0xff, 0x42, 0x77 }; // 2.700
                        var setMinSocReg26 = objProtocol.CreateMvByte(RegisterEnum.REG_CUVP, 2.7);
                        await SendCommandToBMS(setMinSocReg26);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MinSocRead(RegisterEnum.REG_CUVP);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidMinSOCReadValue != 2.7)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidMinSOCReadValue == 2.7)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 27
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            //var setMinSocReg27 = new byte[] { 0xdd, 0x5a, 0x27, 0x02, 0x0b, 0x54, 0xff, 0x78, 0x77 }; // 2.900
                            var setMinSocReg27 = objProtocol.CreateMvByte(RegisterEnum.REG_CUVP_REL, 2.9);
                            await SendCommandToBMS(setMinSocReg27);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MinSocRead(RegisterEnum.REG_CUVP_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidMinSOCReadValue != 2.9)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidMinSOCReadValue == 2.9)
                        {
                            if (Is24VBattery)
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 22
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMinSocReg22 = new byte[] { 0xdd, 0x5a, 0x22, 0x02, 0x08, 0x70, 0xff, 0x64, 0x77 }; // 21.6
                                    var setMinSocReg22 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP, 21.6);
                                    await SendCommandToBMS(setMinSocReg22);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MinSocRead(RegisterEnum.REG_PUVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMinSOCReadValue != 21.6)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMinSOCReadValue == 21.6)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 23
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMinSocReg23 = new byte[] { 0xdd, 0x5a, 0x23, 0x02, 0x09, 0x10, 0xff, 0xc2, 0x77 }; // 23.2
                                        var setMinSocReg23 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP_REL, 23.2);
                                        await SendCommandToBMS(setMinSocReg23);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MinSocRead(RegisterEnum.REG_PUVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMinSOCReadValue != 23.2)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 22
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMinSocReg22 = new byte[] { 0xdd, 0x5a, 0x22, 0x02, 0x04, 0x38, 0xff, 0xa0, 0x77 }; // 10.8
                                    var setMinSocReg22 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP, 10.8);
                                    await SendCommandToBMS(setMinSocReg22);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MinSocRead(RegisterEnum.REG_PUVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMinSOCReadValue != 10.8)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMinSOCReadValue == 10.8)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 23
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMinSocReg23 = new byte[] { 0xdd, 0x5a, 0x23, 0x02, 0x04, 0x88, 0xff, 0x4f, 0x77 }; // 11.6
                                        var setMinSocReg23 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP_REL, 11.6);
                                        await SendCommandToBMS(setMinSocReg23);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MinSocRead(RegisterEnum.REG_PUVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMinSOCReadValue != 11.6)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Convert.ToString(rangesliderLowVoltageCutOff.Value) == "12") // 12V - 11.6v , 24v - 23.2v
                {
                    NewMinSOCValue = 12;
                    while (tCount < 2) // Register 26
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        //var setMinSocReg26 = new byte[] { 0xdd, 0x5a, 0x26, 0x02, 0x0b, 0x54, 0xff, 0x79, 0x77 }; // 2.900
                        var setMinSocReg26 = objProtocol.CreateMvByte(RegisterEnum.REG_CUVP, 2.9);
                        await SendCommandToBMS(setMinSocReg26);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MinSocRead(RegisterEnum.REG_CUVP);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidMinSOCReadValue != 2.9)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidMinSOCReadValue == 2.9)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 27
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            //var setMinSocReg27 = new byte[] { 0xdd, 0x5a, 0x27, 0x02, 0x0b, 0xb8, 0xff, 0x14, 0x77 }; // 3.0
                            var setMinSocReg27 = objProtocol.CreateMvByte(RegisterEnum.REG_CUVP_REL, 3.0);
                            await SendCommandToBMS(setMinSocReg27);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MinSocRead(RegisterEnum.REG_CUVP_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidMinSOCReadValue != 3.0)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidMinSOCReadValue == 3.0)
                        {
                            if (Is24VBattery)
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 22
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMinSocReg22 = new byte[] { 0xdd, 0x5a, 0x22, 0x02, 0x09, 0x10, 0xff, 0xc3, 0x77 }; // 23.2
                                    var setMinSocReg22 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP, 23.2);
                                    await SendCommandToBMS(setMinSocReg22);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MinSocRead(RegisterEnum.REG_PUVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMinSOCReadValue != 23.2)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMinSOCReadValue == 23.2)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 23
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMinSocReg23 = new byte[] { 0xdd, 0x5a, 0x23, 0x02, 0x09, 0x60, 0xff, 0x72, 0x77 }; // 24.0
                                        var setMinSocReg23 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP_REL, 24.0);
                                        await SendCommandToBMS(setMinSocReg23);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MinSocRead(RegisterEnum.REG_PUVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMinSOCReadValue != 24.0)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 22
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMinSocReg22 = new byte[] { 0xdd, 0x5a, 0x22, 0x02, 0x04, 0x88, 0xff, 0x50, 0x77 }; // 11.6
                                    var setMinSocReg22 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP, 11.6);
                                    await SendCommandToBMS(setMinSocReg22);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MinSocRead(RegisterEnum.REG_PUVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMinSOCReadValue != 11.6)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMinSOCReadValue == 11.6)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 23
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMinSocReg23 = new byte[] { 0xdd, 0x5a, 0x23, 0x02, 0x04, 0xb0, 0xff, 0x27, 0x77 }; // 12.0
                                        var setMinSocReg23 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP_REL, 12.0);
                                        await SendCommandToBMS(setMinSocReg23);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MinSocRead(RegisterEnum.REG_PUVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMinSOCReadValue != 12.0)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Convert.ToString(rangesliderLowVoltageCutOff.Value) == "13") // 12V - 12v , 24V - 24V
                {
                    NewMinSOCValue = 13;
                    while (tCount < 2) // Register 26
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        //var setMinSocReg26 = new byte[] { 0xdd, 0x5a, 0x26, 0x02, 0x0b, 0xb8, 0xff, 0x15, 0x77 }; //3.0 
                        var setMinSocReg26 = objProtocol.CreateMvByte(RegisterEnum.REG_CUVP, 3.0);
                        await SendCommandToBMS(setMinSocReg26);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MinSocRead(RegisterEnum.REG_CUVP);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidMinSOCReadValue != 3.0)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidMinSOCReadValue == 3.0)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 27
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            //var setMinSocReg27 = new byte[] { 0xdd, 0x5a, 0x27, 0x02, 0x0b, 0xea, 0xfe, 0xe2, 0x77 }; // 3.05
                            var setMinSocReg27 = objProtocol.CreateMvByte(RegisterEnum.REG_CUVP_REL, 3.1);
                            await SendCommandToBMS(setMinSocReg27);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MinSocRead(RegisterEnum.REG_CUVP_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidMinSOCReadValue != 3.1)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidMinSOCReadValue == 3.1)
                        {
                            if (Is24VBattery)
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 22
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMinSocReg22 = new byte[] { 0xdd, 0x5a, 0x22, 0x02, 0x5d, 0xc0, 0xfe, 0xbf, 0x77 }; // 24
                                    var setMinSocReg22 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP, 24);
                                    await SendCommandToBMS(setMinSocReg22);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MinSocRead(RegisterEnum.REG_PUVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMinSOCReadValue != 24)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMinSOCReadValue == 24)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 23
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMinSocReg23 = new byte[] { 0xdd, 0x5a, 0x23, 0x02, 0x09, 0x74, 0xff, 0x5e, 0x77 }; // 24.2
                                        var setMinSocReg23 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP_REL, 24.8);
                                        await SendCommandToBMS(setMinSocReg23);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MinSocRead(RegisterEnum.REG_PUVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMinSOCReadValue != 24.8)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tCount = 0;
                                while (tCount < 2) // Register 22
                                {
                                    await SendWriteOrReadStartCommandToBMS();

                                    await Task.Delay(100);

                                    //var setMinSocReg22 = new byte[] { 0xdd, 0x5a, 0x22, 0x02, 0x04, 0xb0, 0xff, 0x28, 0x77 }; // 12
                                    var setMinSocReg22 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP, 12);
                                    await SendCommandToBMS(setMinSocReg22);

                                    await Task.Delay(100);

                                    await SendWriteOrReadEndCommandToBMS();

                                    await ShowBusyindicatior(true);

                                    await Task.Delay(100);

                                    await MinSocRead(RegisterEnum.REG_PUVP);

                                    await ShowBusyindicatior(true);

                                    if (objReadValues.IsvalidMinSOCReadValue != 12)
                                    {
                                        tCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (objReadValues.IsvalidMinSOCReadValue == 12)
                                {
                                    tCount = 0;
                                    while (tCount < 2) // Register 23
                                    {
                                        await SendWriteOrReadStartCommandToBMS();

                                        await Task.Delay(100);

                                        //var setMinSocReg23 = new byte[] { 0xdd, 0x5a, 0x23, 0x02, 0x04, 0xc4, 0xff, 0x13, 0x77 }; // 12.2
                                        var setMinSocReg23 = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP_REL, 12.2);
                                        await SendCommandToBMS(setMinSocReg23);

                                        await Task.Delay(100);

                                        await SendWriteOrReadEndCommandToBMS();

                                        await ShowBusyindicatior(true);

                                        await Task.Delay(100);

                                        await MinSocRead(RegisterEnum.REG_PUVP_REL);

                                        await ShowBusyindicatior(true);

                                        if (objReadValues.IsvalidMinSOCReadValue != 12.2)
                                        {
                                            tCount++;
                                        }
                                        else
                                        {
                                            IsValid = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (IsValid && objReadValues.MinSOCReadValue == NewMinSOCValue)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnMaxchargeAmps_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                int NewMaxChargeAmpsValue = (int)rangesliderMaxChargeAmps.Value;
                isProccesBegin = true;

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(150);

                var WriteToReg28 = objProtocol.CreateMv10Byte(RegisterEnum.REG_CHGOC, Convert.ToInt32(rangesliderMaxChargeAmps.Value));
                await SendCommandToBMS(WriteToReg28);

                await Task.Delay(150);

                await SendWriteOrReadEndCommandToBMS();

                await ShowBusyindicatior(true);

                await MaxChargeAmpsRead();

                await Task.Delay(200);

                if (objReadValues.MaxChargeAmpsReadValue == NewMaxChargeAmpsValue)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnMaxdischargeAmps_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool isValid = false;
                await ShowBusyindicatior(true);

                int NewMaxDisChargeAmpsValue = (int)rangesliderMaxDisChargeAmps.Value;
                isProccesBegin = true;

                int tCount = 0;

                while (tCount < 2) // Register 29
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(150);

                    int rangesilder = Convert.ToInt32(rangesliderMaxDisChargeAmps.Value);// * -1;

                    uint iDisChargeAmp = 65536 - ((uint)rangesilder * 100);

                    var WriteToReg29 = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_DSGOC, "02", iDisChargeAmp);
                    await SendCommandToBMS(WriteToReg29);

                    await Task.Delay(150);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(200);

                    await ShowBusyindicatior(true);

                    await MaxDischargeAmpsRead(RegisterEnum.REG_DSGOC);

                    await Task.Delay(100);

                    if (objReadValues.MaxDischargeAmpsReadValue != NewMaxDisChargeAmpsValue)
                    {
                        tCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (objReadValues.MaxDischargeAmpsReadValue == NewMaxDisChargeAmpsValue)
                {
                    tCount = 0;
                    while (tCount < 2) // Register 3f
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(150);

                        var WriteToReg3f = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_DSGOC_DELAYS, "02", 2592);
                        await SendCommandToBMS(WriteToReg3f);

                        await Task.Delay(150);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(200);

                        await ShowBusyindicatior(true);

                        await MaxDischargeAmpsRead(RegisterEnum.REG_DSGOC_DELAYS);

                        await Task.Delay(100);

                        if (objReadValues.MaxDischargeDelayReadValue != "10-32")
                        {
                            tCount++;
                        }
                        else
                        {
                            isValid = true;
                            break;
                        }
                    }
                }

                if (isValid)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnFullCapacity_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool IsValid = false;
                int tCount = 0;

                await ShowBusyindicatior(true);

                int NewFullCapacityValue = (int)rangesliderFullCapacity.Value;

                isProccesBegin = true;

                while (tCount < 2) // Register 10
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    var WriteToReg10 = objProtocol.CreateMv10Byte(RegisterEnum.REG_DESIGN_CAP, Convert.ToDouble(rangesliderFullCapacity.Value));
                    await SendCommandToBMS(WriteToReg10);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();

                    await ShowBusyindicatior(true);

                    await Task.Delay(100);

                    await FullCapacityRead(RegisterEnum.REG_DESIGN_CAP);

                    await ShowBusyindicatior(true);

                    if (objReadValues.IsvalidFullCapacityValue != NewFullCapacityValue)
                    {
                        tCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (objReadValues.IsvalidFullCapacityValue == NewFullCapacityValue)
                {
                    tCount = 0;
                    while (tCount < 2) // Register 11
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        double cyclecapinput = rangesliderFullCapacity.Value / 2;

                        var WriteToReg11 = objProtocol.CreateMv10Byte(RegisterEnum.REG_CYCLE_CAP, cyclecapinput);
                        await SendCommandToBMS(WriteToReg11);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await FullCapacityRead(RegisterEnum.REG_CYCLE_CAP);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidFullCapacityValue != (NewFullCapacityValue / 2))
                        {
                            tCount++;
                        }
                        else
                        {
                            IsValid = true;
                            break;
                        }
                    }
                }


                if (IsValid && objReadValues.FullCapacityValue == NewFullCapacityValue)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnCalibrateCapacity_Clicked(object sender, EventArgs e)
        {
            try
            {
                CalibrationUpdateInProgress = true;
                int NewFullCapacityValue = (int)rangesliderFullCapacity.Value;

                if (!ProceedForCalibrateCapacity)
                {
                    if (Is24VBattery)
                    {
                        await ShowDisplayPopup("ComfirmationPopUpResetCapacity", "Reset Capacity should only be done on a fully charged battery. It will also set your Max SOC to 100% and LVC to 21.6v.");
                    }
                    else
                    {
                        await ShowDisplayPopup("ComfirmationPopUpResetCapacity", "Reset Capacity should only be done on a fully charged battery. It will also set your Max SOC to 100% and LVC to 10.8v.");
                    }

                    ProceedForBanchmarkTest = true;
                }

                if (ProceedForCalibrateCapacity)
                {
                    isProccesBegin = true;

                    await ShowBusyindicatior(true);

                    await CalibrateCapacityStart();

                    if (!objDeviceInformation.WriteReadOddValueCapacity)
                    {
                        #region Write & Read Command For Capacity
                        await WriteAndReadSingleFullVoltage();
                        await Task.Delay(500);

                        if (objCapacity.isValidSingleFull)
                        {
                            await WriteAndReadSingleCutOffVoltage();
                            await Task.Delay(500);

                            if (objCapacity.isValidSingleCutOff)
                            {
                                await WriteAndRead100CapacityVoltage();
                                await Task.Delay(500);

                                if (objCapacity.isValid100Capacity)
                                {
                                    await WriteAndRead80CapacityVoltage();
                                    await Task.Delay(500);

                                    if (objCapacity.isValid80Capacity)
                                    {
                                        await WriteAndRead60CapacityVoltage();
                                        await Task.Delay(500);

                                        if (objCapacity.isValid60Capacity)
                                        {
                                            await WriteAndRead40CapacityVoltage();
                                            await Task.Delay(500);

                                            if (objCapacity.isValid40Capacity)
                                            {
                                                await WriteAndRead20CapacityVoltage();
                                                await Task.Delay(500);

                                                if (objCapacity.isValid20Capacity)
                                                {
                                                    if ((objCapacity.SingleFullCapacityValue == 3.500) && (objCapacity.SingleCutOffCapacityValue == 2.700) && (objCapacity.Capacity100Value == 3.400) && (objCapacity.Capacity80Value == 3.329) && (objCapacity.Capacity60Value == 3.322) && (objCapacity.Capacity40Value == 3.291) && (objCapacity.Capacity20Value == 3.266))
                                                    {
                                                        NewFullCapacityValue = objDeviceInformation.FullCapacityValue;

                                                        await SetCapacity(NewFullCapacityValue);
                                                    }
                                                    else
                                                    {
                                                        objCapacity.isValidSingleFull = false;
                                                        objCapacity.isValidSingleCutOff = false;
                                                        objCapacity.isValid100Capacity = false;
                                                        objCapacity.isValid80Capacity = false;
                                                        objCapacity.isValid60Capacity = false;
                                                        objCapacity.isValid40Capacity = false;
                                                        objCapacity.isValid20Capacity = false;
                                                        isProccesBegin = true;

                                                        await ShowBusyindicatior(false);
                                                        await ShowDisplayPopup("Error", "Unable to update, Please re-try");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if ((objCapacity.SingleFullCapacityValue != 3.500) || (objCapacity.Capacity100Value != 3.400) || (objCapacity.Capacity80Value != 3.329) || (objCapacity.Capacity60Value != 3.322) || (objCapacity.Capacity40Value != 3.291) || (objCapacity.Capacity20Value != 3.266) || (objCapacity.SingleCutOffCapacityValue != 2.700))
                        {
                            objCapacity.isValidSingleFull = objCapacity.isValidSingleCutOff = objCapacity.isValid100Capacity = objCapacity.isValid80Capacity = objCapacity.isValid60Capacity = objCapacity.isValid40Capacity = objCapacity.isValid20Capacity = false;

                            await ShowBusyindicatior(false);
                            await ShowDisplayPopup("Error", "Unable to update, Please re-try");
                        }
                        #endregion
                    }
                    else
                    {
                        #region Write & Read Command For Capacity
                        await WriteAndReadSingleFullVoltage();
                        await Task.Delay(600);

                        if (objCapacity.isValidSingleFull)
                        {
                            await WriteAndReadSingleCutOffVoltage();
                            await Task.Delay(600);

                            if (objCapacity.isValidSingleCutOff)
                            {
                                await WriteAndRead100CapacityVoltage();
                                await Task.Delay(600);

                                if (objCapacity.isValid100Capacity)
                                {
                                    await WriteAndRead90CapacityVoltage();
                                    await Task.Delay(600);

                                    if (objCapacity.isValid90Capacity)
                                    {
                                        await WriteAndRead80CapacityVoltage();
                                        await Task.Delay(600);

                                        if (objCapacity.isValid80Capacity)
                                        {
                                            await WriteAndRead70CapacityVoltage();
                                            await Task.Delay(600);

                                            if (objCapacity.isValid70Capacity)
                                            {
                                                await WriteAndRead60CapacityVoltage();
                                                await Task.Delay(600);

                                                if (objCapacity.isValid60Capacity)
                                                {
                                                    await WriteAndRead50CapacityVoltage();
                                                    await Task.Delay(600);

                                                    if (objCapacity.isValid50Capacity)
                                                    {
                                                        await WriteAndRead40CapacityVoltage();
                                                        await Task.Delay(600);

                                                        if (objCapacity.isValid40Capacity)
                                                        {
                                                            await WriteAndRead30CapacityVoltage();
                                                            await Task.Delay(600);

                                                            if (objCapacity.isValid30Capacity)
                                                            {
                                                                await WriteAndRead20CapacityVoltage();
                                                                await Task.Delay(600);

                                                                if (objCapacity.isValid20Capacity)
                                                                {
                                                                    await WriteAndRead10CapacityVoltage();
                                                                    await Task.Delay(600);

                                                                    if (objCapacity.isValid10Capacity)
                                                                    {
                                                                        if ((objCapacity.SingleFullCapacityValue == 3.500) && (objCapacity.SingleCutOffCapacityValue == 2.700) && (objCapacity.Capacity100Value == 3.400) && (objCapacity.Capacity90Value == 3.330) && (objCapacity.Capacity80Value == 3.329) && (objCapacity.Capacity70Value == 3.328) && (objCapacity.Capacity60Value == 3.322) && (objCapacity.Capacity50Value == 3.294) && (objCapacity.Capacity40Value == 3.291) && (objCapacity.Capacity30Value == 3.289) && (objCapacity.Capacity20Value == 3.266) && (objCapacity.Capacity10Value == 3.215))
                                                                        {
                                                                            NewFullCapacityValue = objDeviceInformation.FullCapacityValue;

                                                                            if (NewFullCapacityValue > 0)
                                                                            {
                                                                                await SetCapacity(NewFullCapacityValue);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            objCapacity.isValidSingleFull = false;
                                                                            objCapacity.isValidSingleCutOff = false;
                                                                            objCapacity.isValid100Capacity = false;
                                                                            objCapacity.isValid90Capacity = false;
                                                                            objCapacity.isValid80Capacity = false;
                                                                            objCapacity.isValid70Capacity = false;
                                                                            objCapacity.isValid60Capacity = false;
                                                                            objCapacity.isValid50Capacity = false;
                                                                            objCapacity.isValid40Capacity = false;
                                                                            objCapacity.isValid30Capacity = false;
                                                                            objCapacity.isValid20Capacity = false;
                                                                            objCapacity.isValid10Capacity = false;

                                                                            isProccesBegin = true;

                                                                            await ShowBusyindicatior(false);
                                                                            await ShowDisplayPopup("Error", "Unable to update, Please re-try");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if ((objCapacity.SingleFullCapacityValue != 3.500) || (objCapacity.SingleCutOffCapacityValue != 2.700) || (objCapacity.Capacity100Value != 3.400) || (objCapacity.Capacity90Value != 3.330) || (objCapacity.Capacity80Value != 3.329) || (objCapacity.Capacity70Value != 3.328) || (objCapacity.Capacity60Value != 3.322) || (objCapacity.Capacity50Value != 3.294) || (objCapacity.Capacity40Value != 3.291) || (objCapacity.Capacity30Value != 3.289) || (objCapacity.Capacity20Value != 3.266) || (objCapacity.Capacity10Value != 3.215))
                        {
                            objCapacity.isValidSingleFull = objCapacity.isValidSingleCutOff = objCapacity.isValid100Capacity = objCapacity.isValid90Capacity = objCapacity.isValid80Capacity = objCapacity.isValid70Capacity = objCapacity.isValid60Capacity = objCapacity.isValid50Capacity = objCapacity.isValid40Capacity = objCapacity.isValid30Capacity = objCapacity.isValid20Capacity = objCapacity.isValid10Capacity = false;

                            await ShowBusyindicatior(false);
                            await ShowDisplayPopup("Error", "Unable to update, Please re-try");
                        }
                        #endregion
                    }
                    ProceedForCalibrateCapacity = false;
                    ProceedForBanchmarkTest = false;
                }

                isProccesBegin = false;
                CalibrationUpdateInProgress = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnSaveChargingDischarging_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                isProccesBegin = true;

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                if (CheckBoxChargingOnOff.IsChecked && CheckBoxDisChargingOnOff.IsChecked)
                {
                    // var ChargingOn = new byte[] { 0xdd, 0x5a, 0xe1, 0x02, 0x00, 0x00, 0xff, 0x1d, 0x77 };  //charging chalu karava mate nu
                    var ChargingOn = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_CTRL_MOSFET, "02", 0);
                    await SendCommandToBMS(ChargingOn);
                }
                else if (!CheckBoxChargingOnOff.IsChecked && !CheckBoxDisChargingOnOff.IsChecked)
                {
                    // var DischargingOff = new byte[] { 0xdd, 0x5a, 0xe1, 0x02, 0x00, 0x03, 0xff, 0x1a, 0x77 };  // discharging bandh karava mate nu
                    var DischargingOff = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_CTRL_MOSFET, "02", 3);
                    await SendCommandToBMS(DischargingOff);
                }
                else if (!CheckBoxChargingOnOff.IsChecked && CheckBoxDisChargingOnOff.IsChecked)
                {
                    // var ChargingOff = new byte[] { 0xdd, 0x5a, 0xe1, 0x02, 0x00, 0x01, 0xff, 0x1c, 0x77 };  // charging bandh karava mate nu
                    var ChargingOff = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_CTRL_MOSFET, "02", 1);
                    await SendCommandToBMS(ChargingOff);
                }
                else if (CheckBoxChargingOnOff.IsChecked && !CheckBoxDisChargingOnOff.IsChecked)
                {
                    // var DischargingOn = new byte[] { 0xdd, 0x5a, 0xe1, 0x02, 0x00, 0x02, 0xff, 0x1b, 0x77 };  // discharging chalu karava mate nu
                    var DischargingOn = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_CTRL_MOSFET, "02", 2);
                    await SendCommandToBMS(DischargingOn);
                }

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(100);

                await ReadBMSDetails03Command();

                await Task.Delay(1000);

                isProccesBegin = false;

                await ShowBusyindicatior(false);

                await ShowDisplayPopup("Success", "");

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
        private async void btnLowTempSlider_Clicked(object sender, EventArgs e)
        {
            try
            {
                isProccesBegin = true;

                await ShowBusyindicatior(true);

                await SendCommandForLowTempAndHeatingMode(false);

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnMaxBatteryTemp_Clicked(object sender, EventArgs e)
        {
            try
            {
                int tCount = 0;
                bool IsValid = false;
                isProccesBegin = true;
                await ShowBusyindicatior(true);
                int NewMaxBatteryTempValue = (int)rangesliderMaxBatteryTemp.Value;

                if (Convert.ToString(rangesliderMaxBatteryTemp.Value) == "113" || Convert.ToString(rangesliderMaxBatteryTemp.Value) == "45")
                {
                    while (tCount < 2) // Register 18
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        // var setMaxBatteryTempReg18 = new byte[] { 0xdd, 0x5a, 0x18, 0x02, 0x0c, 0x6e, 0xff, 0x6c, 0x77 }; // 45
                        var setMaxBatteryTempReg18 = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGOT, 45);
                        await SendCommandToBMS(setMaxBatteryTempReg18);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MaxBatteryTempRead(RegisterEnum.REG_CHGOT);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidChargeMaxBatteryTempValue != 45)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidChargeMaxBatteryTempValue == 45)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 19
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            // var setMaxBatteryTempReg19 = new byte[] { 0xdd, 0x5a, 0x19, 0x02, 0x0c, 0x0a, 0xff, 0xcf, 0x77 }; // 35
                            var setMaxBatteryTempReg19 = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGOT_REL, 35);
                            await SendCommandToBMS(setMaxBatteryTempReg19);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxBatteryTempRead(RegisterEnum.REG_CHGOT_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidChargeMaxBatteryTempValue != 35)
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }

                    if (objReadValues.IsvalidChargeMaxBatteryTempValue == 35)
                    {
                        while (tCount < 2) // Register 1c
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            // var setMaxBatteryTempReg1c = new byte[] { 0xdd, 0x5a, 0x1c, 0x02, 0x0c, 0x6e, 0xff, 0x68, 0x77 }; // 45
                            var setMaxBatteryTempReg1c = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGOT, 45);
                            await SendCommandToBMS(setMaxBatteryTempReg1c);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxBatteryTempRead(RegisterEnum.REG_DSGOT);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidDischargeMaxBatteryTempValue != 45)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidDischargeMaxBatteryTempValue == 45)
                        {
                            tCount = 0;
                            while (tCount < 2) // Register 1d
                            {
                                await SendWriteOrReadStartCommandToBMS();

                                await Task.Delay(100);

                                // var setMaxBatteryTempReg1d = new byte[] { 0xdd, 0x5a, 0x1d, 0x02, 0x0c, 0x0a, 0xff, 0xcb, 0x77 }; // 35
                                var setMaxBatteryTempReg1d = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGOT_REL, 35);
                                await SendCommandToBMS(setMaxBatteryTempReg1d);

                                await Task.Delay(100);

                                await SendWriteOrReadEndCommandToBMS();

                                await ShowBusyindicatior(true);

                                await Task.Delay(100);

                                await MaxBatteryTempRead(RegisterEnum.REG_DSGOT_REL);

                                await ShowBusyindicatior(true);

                                if (objReadValues.IsvalidDischargeMaxBatteryTempValue != 35)
                                {
                                    tCount++;
                                }
                                else
                                {
                                    IsValid = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (Convert.ToString(rangesliderMaxBatteryTemp.Value) == "122" || Convert.ToString(rangesliderMaxBatteryTemp.Value) == "50")
                {
                    while (tCount < 2) // Register 18
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        // var setMaxBatteryTempReg18 = new byte[] { 0xdd, 0x5a, 0x18, 0x02, 0x0c, 0xa0, 0xff, 0x3a, 0x77 }; // 50
                        var setMaxBatteryTempReg18 = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGOT, 50);
                        await SendCommandToBMS(setMaxBatteryTempReg18);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MaxBatteryTempRead(RegisterEnum.REG_CHGOT);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidChargeMaxBatteryTempValue != 50)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidChargeMaxBatteryTempValue == 50)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 19
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            // var setMaxBatteryTempReg19 = new byte[] { 0xdd, 0x5a, 0x19, 0x02, 0x0c, 0x3c, 0xff, 0x9d, 0x77 }; // 40
                            var setMaxBatteryTempReg19 = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGOT_REL, 40);
                            await SendCommandToBMS(setMaxBatteryTempReg19);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxBatteryTempRead(RegisterEnum.REG_CHGOT_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidChargeMaxBatteryTempValue != 40)
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }

                    if (objReadValues.IsvalidChargeMaxBatteryTempValue == 40)
                    {
                        while (tCount < 2) // Register 1c
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            // var setMaxBatteryTempReg1c = new byte[] { 0xdd, 0x5a, 0x1c, 0x02, 0x0c, 0xa0, 0xff, 0x36, 0x77 }; // 50
                            var setMaxBatteryTempReg1c = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGOT, 50);
                            await SendCommandToBMS(setMaxBatteryTempReg1c);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxBatteryTempRead(RegisterEnum.REG_DSGOT);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidDischargeMaxBatteryTempValue != 50)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidDischargeMaxBatteryTempValue == 50)
                        {
                            tCount = 0;
                            while (tCount < 2) // Register 1d
                            {
                                await SendWriteOrReadStartCommandToBMS();

                                await Task.Delay(100);

                                // var setMaxBatteryTempReg1d = new byte[] { 0xdd, 0x5a, 0x1d, 0x02, 0x0c, 0x3c, 0xff, 0x99, 0x77 }; // 40
                                var setMaxBatteryTempReg1d = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGOT_REL, 40);
                                await SendCommandToBMS(setMaxBatteryTempReg1d);

                                await Task.Delay(100);

                                await SendWriteOrReadEndCommandToBMS();

                                await ShowBusyindicatior(true);

                                await Task.Delay(100);

                                await MaxBatteryTempRead(RegisterEnum.REG_DSGOT_REL);

                                await ShowBusyindicatior(true);

                                if (objReadValues.IsvalidDischargeMaxBatteryTempValue != 40)
                                {
                                    tCount++;
                                }
                                else
                                {
                                    IsValid = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (Convert.ToString(rangesliderMaxBatteryTemp.Value) == "131" || Convert.ToString(rangesliderMaxBatteryTemp.Value) == "55")
                {
                    while (tCount < 2) // Register 18
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        // var setMaxBatteryTempReg18 = new byte[] { 0xdd, 0x5a, 0x18, 0x02, 0x0c, 0xd2, 0xff, 0x08, 0x77 }; // 55
                        var setMaxBatteryTempReg18 = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGOT, 55);
                        await SendCommandToBMS(setMaxBatteryTempReg18);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MaxBatteryTempRead(RegisterEnum.REG_CHGOT);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidChargeMaxBatteryTempValue != 55)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidChargeMaxBatteryTempValue == 55)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 19
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            // var setMaxBatteryTempReg19 = new byte[] { 0xdd, 0x5a, 0x19, 0x02, 0x0c, 0x6e, 0xff, 0x6b, 0x77 }; // 45
                            var setMaxBatteryTempReg19 = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGOT_REL, 45);
                            await SendCommandToBMS(setMaxBatteryTempReg19);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxBatteryTempRead(RegisterEnum.REG_CHGOT_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidChargeMaxBatteryTempValue != 45)
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }

                    if (objReadValues.IsvalidChargeMaxBatteryTempValue == 45)
                    {
                        while (tCount < 2) // Register 1c
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            // var setMaxBatteryTempReg1c = new byte[] { 0xdd, 0x5a, 0x1c, 0x02, 0x0c, 0xd2, 0xff, 0x04, 0x77 }; // 55
                            var setMaxBatteryTempReg1c = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGOT, 55);
                            await SendCommandToBMS(setMaxBatteryTempReg1c);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxBatteryTempRead(RegisterEnum.REG_DSGOT);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidDischargeMaxBatteryTempValue != 55)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidDischargeMaxBatteryTempValue == 55)
                        {
                            tCount = 0;
                            while (tCount < 2) // Register 1d
                            {
                                await SendWriteOrReadStartCommandToBMS();

                                await Task.Delay(100);

                                // var setMaxBatteryTempReg1d = new byte[] { 0xdd, 0x5a, 0x1d, 0x02, 0x0c, 0x6e, 0xff, 0x67, 0x77 }; // 45
                                var setMaxBatteryTempReg1d = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGOT_REL, 45);
                                await SendCommandToBMS(setMaxBatteryTempReg1d);

                                await Task.Delay(100);

                                await SendWriteOrReadEndCommandToBMS();

                                await ShowBusyindicatior(true);

                                await Task.Delay(100);

                                await MaxBatteryTempRead(RegisterEnum.REG_DSGOT_REL);

                                await ShowBusyindicatior(true);

                                if (objReadValues.IsvalidDischargeMaxBatteryTempValue != 45)
                                {
                                    tCount++;
                                }
                                else
                                {
                                    IsValid = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (Convert.ToString(rangesliderMaxBatteryTemp.Value) == "140" || Convert.ToString(rangesliderMaxBatteryTemp.Value) == "60")
                {
                    while (tCount < 2) // Register 18
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        // var setMaxBatteryTempReg18 = new byte[] { 0xdd, 0x5a, 0x18, 0x02, 0x0d, 0x04, 0xff, 0xd5, 0x77 }; // 60
                        var setMaxBatteryTempReg18 = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGOT, 60);
                        await SendCommandToBMS(setMaxBatteryTempReg18);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await MaxBatteryTempRead(RegisterEnum.REG_CHGOT);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidChargeMaxBatteryTempValue != 60)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidChargeMaxBatteryTempValue == 60)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 19
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            // var setMaxBatteryTempReg19 = new byte[] { 0xdd, 0x5a, 0x19, 0x02, 0x0c, 0xa0, 0xff, 0x39, 0x77 }; // 50
                            var setMaxBatteryTempReg19 = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGOT_REL, 50);
                            await SendCommandToBMS(setMaxBatteryTempReg19);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxBatteryTempRead(RegisterEnum.REG_CHGOT_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidChargeMaxBatteryTempValue != 50)
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }

                    if (objReadValues.IsvalidChargeMaxBatteryTempValue == 50)
                    {
                        while (tCount < 2) // Register 1c
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            // var setMaxBatteryTempReg1c = new byte[] { 0xdd, 0x5a, 0x1c, 0x02, 0x0d, 0x04, 0xff, 0xd1, 0x77 }; // 60
                            var setMaxBatteryTempReg1c = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGOT, 60);
                            await SendCommandToBMS(setMaxBatteryTempReg1c);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await MaxBatteryTempRead(RegisterEnum.REG_DSGOT);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidDischargeMaxBatteryTempValue != 60)
                            {
                                tCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (objReadValues.IsvalidDischargeMaxBatteryTempValue == 60)
                        {
                            tCount = 0;
                            while (tCount < 2) // Register 1d
                            {
                                await SendWriteOrReadStartCommandToBMS();

                                await Task.Delay(100);

                                // var setMaxBatteryTempReg1d = new byte[] { 0xdd, 0x5a, 0x1d, 0x02, 0x0c, 0xa0, 0xff, 0x35, 0x77 }; // 50
                                var setMaxBatteryTempReg1d = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGOT_REL, 50);
                                await SendCommandToBMS(setMaxBatteryTempReg1d);

                                await Task.Delay(100);

                                await SendWriteOrReadEndCommandToBMS();

                                await ShowBusyindicatior(true);

                                await Task.Delay(100);

                                await MaxBatteryTempRead(RegisterEnum.REG_DSGOT_REL);

                                await ShowBusyindicatior(true);

                                if (objReadValues.IsvalidDischargeMaxBatteryTempValue != 50)
                                {
                                    tCount++;
                                }
                                else
                                {
                                    IsValid = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (IsValid && objReadValues.chargeMaxBatteryTempValue == NewMaxBatteryTempValue && objReadValues.dischargeMaxBatteryTempValue == NewMaxBatteryTempValue)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnCalibrateTemp_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                bool IsValid = false;
                isProccesBegin = true;
                int TemperatuteValue = 0;
                var keyboardService = App.Services.GetService<IKeyboardService>();
                keyboardService?.HideKeyboard();
                if (!string.IsNullOrWhiteSpace(txtTemperatureCalibration.Text))
                {
                    TemperatuteValue = Convert.ToInt32(txtTemperatureCalibration.Text);

                    if (CheckValidNumer(Convert.ToString(TemperatuteValue)))
                    {
                        // d0
                        await SendWriteOrReadStartCommandToBMS();

                        var setTempratureD0 = objProtocol.GetTempratureBytes("d0", Convert.ToInt32(TemperatuteValue));
                        await SendCommandToBMS(setTempratureD0);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(500);

                        // d1
                        await SendWriteOrReadStartCommandToBMS();

                        var setTempratureD1 = objProtocol.GetTempratureBytes("d1", Convert.ToInt32(TemperatuteValue));
                        await SendCommandToBMS(setTempratureD1);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(500);

                        // d2
                        await SendWriteOrReadStartCommandToBMS();

                        var setTempratureD2 = objProtocol.GetTempratureBytes("d2", Convert.ToInt32(TemperatuteValue));
                        await SendCommandToBMS(setTempratureD2);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(500);
                        txtTemperatureCalibration.Text = string.Empty;

                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("Success", "Calibrate Successfully");
                    }
                    else
                    {
                        await ShowBusyindicatior(false);

                        await ShowDisplayPopup("Error", "Please enter valid input. Range should be between 1 °C to 38 °C");
                    }
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Error", "Please enter valid input.");
                }
                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async void btnEstablishConnectionUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                isProccesBegin = true;

                if (rdEstablishConnectionFive.IsChecked == true)
                {
                    Preferences.Set("EstablishConnectionData", "5");
                    EstablishConnectionDatAttemps = 5;
                }
                else if (rdEstablishConnectionTen.IsChecked == true)
                {
                    Preferences.Set("EstablishConnectionData", "10");
                    EstablishConnectionDatAttemps = 10;
                }
                else if (rdEstablishConnectionThirty.IsChecked == true)
                {
                    Preferences.Set("EstablishConnectionData", "30");
                    EstablishConnectionDatAttemps = 30;
                }
                else if (rdEstablishConnectionSixty.IsChecked == true)
                {
                    Preferences.Set("EstablishConnectionData", "60");
                    EstablishConnectionDatAttemps = 60;
                }

                await Task.Delay(500);

                await btnEstablishConnectionRead();

                isProccesBegin = false;

                string Tooldata = "EstablishConnection" + '^' + EstablishConnectionDatAttemps;
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ToolTabUpdate, Tooldata);

                await ShowBusyindicatior(false);
                await ShowDisplayPopup("Success", "");
            }
            catch (Exception ex)
            {
                await ShowBusyindicatior(false);
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnPullRequestUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                isProccesBegin = true;

                if (rdPulllRequestOne.IsChecked == true)
                {
                    Preferences.Set("PollRequestData", "1000");
                    PullingRequestTime = 1000;
                }
                else if (rdPulllRequestTwo.IsChecked == true)
                {
                    Preferences.Set("PollRequestData", "2000");
                    PullingRequestTime = 2000;
                }
                else if (rdPulllRequestThree.IsChecked == true)
                {
                    Preferences.Set("PollRequestData", "3000");
                    PullingRequestTime = 3000;
                }

                await Task.Delay(500);

                await btnPullRequestRead();

                isProccesBegin = false;

                await ShowBusyindicatior(false);

                await ShowDisplayPopup("Success", "");
            }
            catch (Exception ex)
            {
                await ShowBusyindicatior(false);
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnTemperatureUnitUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                isProccesBegin = true;
                await ShowBusyindicatior(true);

                if (rdCelsius.IsChecked == true)
                {
                    Preferences.Set("CelsiusorFahrenheit", "Celsius");
                }
                else if (rdFahrenheit.IsChecked == true)
                {
                    Preferences.Set("CelsiusorFahrenheit", "Fahrenheit");
                    IsFahrenheit = true;
                }

                await Task.Delay(500);

                if (rdLowTempcharging.IsChecked)
                    await SetLowTempHeatingMode(Convert.ToInt32(rdLowTempcharging.ClassId));
                if (rdStandbyHeating.IsChecked)
                    await SetLowTempHeatingMode(Convert.ToInt32(rdStandbyHeating.ClassId));
                if (rdDisabledORLowTempProtectionOnly.IsChecked)
                    await SetLowTempHeatingMode(Convert.ToInt32(rdDisabledORLowTempProtectionOnly.ClassId));

                await btnTemperatureUnitRead();

                await ChargingUnderTempRead();

                await MaxBatteryTempRead();

                if (sCelsiusorFahrenheit == "Celsius" || sCelsiusorFahrenheit == "Fahrenheit")
                {
                    string Tooldata = "TemperatureUnit" + '^' + sCelsiusorFahrenheit;
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ToolTabUpdate, Tooldata);

                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                await ShowBusyindicatior(false);
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnLowTempDischargingUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                int tCount = 0;
                bool IsValid = false;
                isProccesBegin = true;

                int TempratureValue = 0;

                if (CheckBoxLowTempDischarg.IsChecked)
                {
                    // Discharge Under Temp
                    while (tCount < 2) // Register 1e
                    {
                        TempratureValue = -20;

                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        // var setLowTempDischargeReg1e = new byte[] { 0xdd, 0x5a, 0x1e, 0x02, 0x09, 0xe4, 0xfe, 0xf3, 0x77 }; //-20
                        var setLowTempDischargeReg1e = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGUT, -20);
                        await SendCommandToBMS(setLowTempDischargeReg1e);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await LowTempDischargeRead(RegisterEnum.REG_DSGUT);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsValidLowTempDischargeValue != TempratureValue)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsValidLowTempDischargeValue == TempratureValue)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 1f
                        {
                            TempratureValue = -15;

                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            // var setLowTempDischargeReg1f = new byte[] { 0xdd, 0x5a, 0x1f, 0x02, 0x0a, 0x16, 0xff, 0xbf, 0x77 }; // -15
                            var setLowTempDischargeReg1f = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGUT_REL, -15);
                            await SendCommandToBMS(setLowTempDischargeReg1f);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await LowTempDischargeRead(RegisterEnum.REG_DSGUT_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsValidLowTempDischargeValue != TempratureValue)
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }
                }
                else if (!CheckBoxLowTempDischarg.IsChecked)
                {
                    // Discharge Under Temp
                    while (tCount < 2) // Register 1e
                    {
                        TempratureValue = -1;

                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        // var setLowTempDischargeReg1e = new byte[] { 0xdd, 0x5a, 0x1e, 0x02, 0x0a, 0xa2, 0xff, 0x34, 0x77 }; //-1
                        var setLowTempDischargeReg1e = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGUT, -1);
                        await SendCommandToBMS(setLowTempDischargeReg1e);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await LowTempDischargeRead(RegisterEnum.REG_DSGUT);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsValidLowTempDischargeValue != TempratureValue)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsValidLowTempDischargeValue == TempratureValue)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 1f
                        {
                            TempratureValue = 2;

                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            // var setLowTempDischargeReg1f = new byte[] { 0xdd, 0x5a, 0x1f, 0x02, 0x0a, 0xc0, 0xff, 0x15, 0x77 }; //2
                            var setLowTempDischargeReg1f = objProtocol.CreateTempratureByte(RegisterEnum.REG_DSGUT_REL, 2);
                            await SendCommandToBMS(setLowTempDischargeReg1f);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await LowTempDischargeRead(RegisterEnum.REG_DSGUT_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsValidLowTempDischargeValue != TempratureValue)
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }
                }

                if (IsValid)
                {
                    await ShowBusyindicatior(false);

                    Preferences.Set("LowTempDischarge", Convert.ToString(CheckBoxLowTempDischarg.IsChecked));

                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");

                    CheckBoxLowTempDischarg.IsChecked = !CheckBoxLowTempDischarg.IsChecked;
                }

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnFaultRelease_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                int tCount = 0;
                bool IsValid = false;
                isProccesBegin = true;

                int FaultReleaseValue = 0;

                if (rdAutomatic.IsChecked == true)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("WarningConfirmation", "Warning, automatic fault release may cause damage to your battery \r\nif the underlying problem is not resolved, only use if absolutely necessary for device compatibility.");
                }
                else if (rdManual.IsChecked == true)
                {
                    while (tCount < 2) // Register 2d
                    {
                        FaultReleaseValue = 1;

                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        // var Load check = new byte[] { 0xdd, 0x5a, 0x2d, 0x02, 0x00, 0x0E, 0xff, 0xC3, 0x77 };
                        var Loadcheck = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_FUNC_CONFIG, "02", 14);
                        await SendCommandToBMS(Loadcheck);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(1000);

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await FaultReleaseRead();

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsValidFaultReleaseValue != FaultReleaseValue)
                        {
                            tCount++;
                        }
                        else
                        {
                            IsValid = true;
                            break;
                        }
                    }

                    if (IsValid)
                    {
                        await ShowBusyindicatior(false);

                        await ShowDisplayPopup("Success", "");
                    }
                    else
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("NotSuccess", "");
                    }
                }

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void FaultRelease_Update()
        {
            try
            {
                await ShowBusyindicatior(true);

                int tCount = 0;
                bool IsValid = false;
                isProccesBegin = true;

                int FaultReleaseValue = 0;

                if (rdAutomatic.IsChecked == true)
                {
                    if (ProceedForFaultReleaseUpdate)
                    {
                        while (tCount < 2) // Register 2d
                        {
                            FaultReleaseValue = 0;

                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(200);

                            // var FaultRelease = new byte[] { 0xdd, 0x5a, 0x2d, 0x02, 0x00, 0x0C, 0xff, 0xC5, 0x77 };
                            var FaultRelease = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_FUNC_CONFIG, "02", 12);
                            await SendCommandToBMS(FaultRelease);

                            await Task.Delay(200);

                            await SendWriteOrReadEndCommandToBMS();

                            await Task.Delay(200);

                            await Task.Delay(100);

                            await FaultReleaseRead();

                            await ShowBusyindicatior(false);

                            if (objReadValues.IsValidFaultReleaseValue != FaultReleaseValue)
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }

                        ProceedForFaultReleaseUpdate = false;
                        ProceedForBanchmarkTest = false;
                        ProceedForCalibrateCapacity = false;

                        if (IsValid)
                        {
                            await ShowBusyindicatior(false);

                            await ShowDisplayPopup("Success", "");
                        }
                        else
                        {
                            await ShowBusyindicatior(false);
                            await ShowDisplayPopup("NotSuccess", "");
                        }
                    }
                    else
                    {
                        objRadioButtonViewModel = new RadioButtonViewModel();
                        objRadioButtonViewModel.FaultReleaseData = new bool[2];

                        objRadioButtonViewModel.FaultReleaseData[0] = true;
                        objRadioButtonViewModel.FaultReleaseData[1] = false;

                        rdManual.BindingContext = objRadioButtonViewModel;
                        rdManual.IsChecked = objRadioButtonViewModel.FaultReleaseData[0];

                        rdAutomatic.BindingContext = objRadioButtonViewModel;
                        rdAutomatic.IsChecked = objRadioButtonViewModel.FaultReleaseData[1];
                    }
                }

                await ShowBusyindicatior(false);
                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        public async Task SetCapacity(int Capacity)
        {
            try
            {
                if (rangesliderFullCapacity.Value > 0)
                {
                    Capacity = Convert.ToInt32(rangesliderFullCapacity.Value);
                }

                int tCount = 0;
                bool IsValid = false;
                isProccesBegin = true;
                await ShowBusyindicatior(true);
                if (Capacity > 0)
                {
                    isProccesBegin = true;

                    while (tCount < 2) // Register 10
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        var WriteToReg10 = objProtocol.CreateMv10Byte(RegisterEnum.REG_DESIGN_CAP, Capacity);
                        await SendCommandToBMS(WriteToReg10);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(100);

                        await FullCapacityRead(RegisterEnum.REG_DESIGN_CAP);

                        if (objReadValues.IsvalidFullCapacityValue != Capacity)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidFullCapacityValue == Capacity)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 11
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            double cyclecapinput = Capacity / 2;

                            var WriteToReg11 = objProtocol.CreateMv10Byte(RegisterEnum.REG_CYCLE_CAP, cyclecapinput);
                            await SendCommandToBMS(WriteToReg11);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await Task.Delay(350);

                            await FullCapacityRead(RegisterEnum.REG_CYCLE_CAP);

                            if (objReadValues.IsvalidFullCapacityValue != cyclecapinput)
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }

                    if (IsValid && objReadValues.FullCapacityValue == Capacity)
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("Success", "");
                    }
                    else
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("NotSuccess", "");
                    }

                }
                isProccesBegin = false;
            }
            catch { }
        }
        private async void UpdateSOCModeImage(string SOCMode)
        {
            try
            {
                if (SOCMode == "Service")
                {
                    stackBatteryPercentage.IsVisible = false;
                    stackSOCModeMain.IsVisible = true;
                    imgMainTabAnotaion.Source = "servicemodeicon.png";
                    lblSOCMode.Text = "Service Mode";
                }
                else if (SOCMode == "Storage")
                {
                    stackBatteryPercentage.IsVisible = false;
                    stackSOCModeMain.IsVisible = true;
                    imgMainTabAnotaion.Source = "storagemodeicon.png";
                    lblSOCMode.Text = "Storage Mode";
                }
                else
                {
                    stackBatteryPercentage.IsVisible = true;
                    stackSOCModeMain.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private static bool CheckValidNumer(string number)
        {
            bool a = false;
            if (number.All(Char.IsDigit))
            {
                int number2 = Convert.ToInt32(number);

                if (number2 >= 1 && number2 <= 38)
                {
                    a = true;
                }
            }
            return a;
        }
        private async Task SendCommandForLowTempAndHeatingMode(bool SendHeatingModeCommand)
        {
            try
            {
                byte[] SendCommandE4 = new byte[0];
                int tCount = 0;
                bool IsValid = false;
                int NewLowTempValue = 0;
                string HeatingMode = string.Empty;

                if (SendHeatingModeCommand)
                {
                    NewLowTempValue = (int)NF_rangesliderLowTemp.Value;

                    if (rdLowTempcharging.IsChecked == true)
                    {
                        HeatingMode = "01";
                    }
                    else if (rdStandbyHeating.IsChecked == true)
                    {
                        HeatingMode = "02";
                    }
                    else if (rdDisabledORLowTempProtectionOnly.IsChecked == true)
                    {
                        HeatingMode = "00";
                    }
                }
                else
                {
                    NewLowTempValue = (int)rangesliderLowTemp.Value;
                }

                if (IsFahrenheit && NewLowTempValue < 35)
                {
                    NewLowTempValue = 35;
                }
                else if (!IsFahrenheit && NewLowTempValue < 1)
                {
                    NewLowTempValue = 1;
                }

                if (Convert.ToString(NewLowTempValue) == "35" || Convert.ToString(NewLowTempValue) == "1")
                {
                    while (tCount < 2) // Register 1a
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        var setLowTempReg1a = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT, 1); // 1°C ,35°F 
                        await SendCommandToBMS(setLowTempReg1a);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await ChargingUnderTempRead(RegisterEnum.REG_CHGUT);

                        await ShowBusyindicatior(true);

                        if (objReadValues.IsvalidLowTempValue != 1)
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (objReadValues.IsvalidLowTempValue == 1)
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 1b
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            var setLowTempReg1b = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT_REL, 4); // 4°C ,40°F
                            await SendCommandToBMS(setLowTempReg1b);

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await ChargingUnderTempRead(RegisterEnum.REG_CHGUT_REL);

                            await ShowBusyindicatior(true);

                            if (objReadValues.IsvalidLowTempValue != 4)
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }

                    if (SendHeatingModeCommand)
                    {
                        SendCommandE4 = objProtocol.PrepareBytesForNewFirmwareStartAndStopTemp(1, 4, HeatingMode); // 35 , 40
                    }
                }
                else if (Convert.ToString(NewLowTempValue) == "40" || Convert.ToString(NewLowTempValue) == "4")
                {
                    while (tCount < 2) // Register 1a
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        if (HeatingMode == "02")
                        {
                            NewLowTempValue = 1;
                            if (IsFahrenheit)
                            {
                                NewLowTempValue = 35;
                            }
                            var setLowTempReg1a = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT, 1); // 1°C ,35°F
                            await SendCommandToBMS(setLowTempReg1a);
                        }
                        else
                        {
                            var setLowTempReg1a = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT, 4); // 4°C ,40°F
                            await SendCommandToBMS(setLowTempReg1a);
                        }

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await ChargingUnderTempRead(RegisterEnum.REG_CHGUT);

                        await ShowBusyindicatior(true);

                        if ((HeatingMode == "02" && objReadValues.IsvalidLowTempValue != 1) || (HeatingMode != "02" && objReadValues.IsvalidLowTempValue != 4))
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if ((HeatingMode == "02" && objReadValues.IsvalidLowTempValue == 1) || (HeatingMode != "02" && objReadValues.IsvalidLowTempValue == 4))
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 1b
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            if (HeatingMode == "02")
                            {
                                var setLowTempReg1b = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT_REL, 4); // 4°C ,40°F
                                await SendCommandToBMS(setLowTempReg1b);
                            }
                            else
                            {
                                var setLowTempReg1b = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT_REL, 7); // 7°C ,45°F
                                await SendCommandToBMS(setLowTempReg1b);
                            }

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await ChargingUnderTempRead(RegisterEnum.REG_CHGUT_REL);

                            await ShowBusyindicatior(true);

                            if ((HeatingMode == "02" && objReadValues.IsvalidLowTempValue != 4) || (HeatingMode != "02" && objReadValues.IsvalidLowTempValue != 7))
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }

                    if (SendHeatingModeCommand)
                    {
                        SendCommandE4 = objProtocol.PrepareBytesForNewFirmwareStartAndStopTemp(4, 7, HeatingMode); // 40 , 45
                    }
                }
                else if (Convert.ToString(NewLowTempValue) == "45" || Convert.ToString(NewLowTempValue) == "7")
                {
                    while (tCount < 2) // Register 1a
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);
                        if (HeatingMode == "02")
                        {
                            NewLowTempValue = 1;
                            if (IsFahrenheit)
                            {
                                NewLowTempValue = 35;
                            }
                            var setLowTempReg1a = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT, 1); // 1°C ,35°F
                            await SendCommandToBMS(setLowTempReg1a);
                        }
                        else
                        {
                            var setLowTempReg1a = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT, 7); // 7°C ,45°F
                            await SendCommandToBMS(setLowTempReg1a);
                        }

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await ChargingUnderTempRead(RegisterEnum.REG_CHGUT);

                        await ShowBusyindicatior(true);

                        if ((HeatingMode == "02" && objReadValues.IsvalidLowTempValue != 1) || (HeatingMode != "02" && objReadValues.IsvalidLowTempValue != 7))
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if ((HeatingMode == "02" && objReadValues.IsvalidLowTempValue == 1) || (HeatingMode != "02" && objReadValues.IsvalidLowTempValue == 7))
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 1b
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            if (HeatingMode == "02")
                            {
                                var setLowTempReg1b = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT_REL, 4); // 4°C ,40°F
                                await SendCommandToBMS(setLowTempReg1b);
                            }
                            else
                            {
                                var setLowTempReg1b = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT_REL, 10); // 10°C ,50°F
                                await SendCommandToBMS(setLowTempReg1b);
                            }

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await ChargingUnderTempRead(RegisterEnum.REG_CHGUT_REL);

                            await ShowBusyindicatior(true);

                            if ((HeatingMode == "02" && objReadValues.IsvalidLowTempValue != 4) || (HeatingMode != "02" && objReadValues.IsvalidLowTempValue != 10))
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }

                    if (SendHeatingModeCommand)
                    {
                        SendCommandE4 = objProtocol.PrepareBytesForNewFirmwareStartAndStopTemp(7, 10, HeatingMode); //45 , 50                        
                    }
                }
                else if (Convert.ToString(NewLowTempValue) == "50" || Convert.ToString(NewLowTempValue) == "10")
                {
                    while (tCount < 2) // Register 1a
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        if (HeatingMode == "02")
                        {
                            NewLowTempValue = 1;
                            if (IsFahrenheit)
                            {
                                NewLowTempValue = 35;
                            }
                            var setLowTempReg1a = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT, 1); // 1°C ,35°F
                            await SendCommandToBMS(setLowTempReg1a);
                        }
                        else
                        {
                            var setLowTempReg1a = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT, 10); // 10°C ,50°F
                            await SendCommandToBMS(setLowTempReg1a);
                        }

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(100);

                        await ChargingUnderTempRead(RegisterEnum.REG_CHGUT);

                        await ShowBusyindicatior(true);

                        if ((HeatingMode == "02" && objReadValues.IsvalidLowTempValue != 1) || (HeatingMode != "02" && objReadValues.IsvalidLowTempValue != 10))
                        {
                            tCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if ((HeatingMode == "02" && objReadValues.IsvalidLowTempValue == 1) || (HeatingMode != "02" && objReadValues.IsvalidLowTempValue == 10))
                    {
                        tCount = 0;
                        while (tCount < 2) // Register 1b
                        {
                            await SendWriteOrReadStartCommandToBMS();

                            await Task.Delay(100);

                            if (HeatingMode == "02")
                            {
                                var setLowTempReg1b = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT_REL, 4); // 4°C ,40°F
                                await SendCommandToBMS(setLowTempReg1b);
                            }
                            else
                            {
                                var setLowTempReg1b = objProtocol.CreateTempratureByte(RegisterEnum.REG_CHGUT_REL, 13); // 13°C ,55°F
                                await SendCommandToBMS(setLowTempReg1b);
                            }

                            await Task.Delay(100);

                            await SendWriteOrReadEndCommandToBMS();

                            await ShowBusyindicatior(true);

                            await Task.Delay(100);

                            await ChargingUnderTempRead(RegisterEnum.REG_CHGUT_REL);

                            await ShowBusyindicatior(true);

                            if ((HeatingMode == "02" && objReadValues.IsvalidLowTempValue != 4) || (HeatingMode != "02" && objReadValues.IsvalidLowTempValue != 13))
                            {
                                tCount++;
                            }
                            else
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }

                    if (SendHeatingModeCommand)
                    {
                        SendCommandE4 = objProtocol.PrepareBytesForNewFirmwareStartAndStopTemp(10, 13, HeatingMode);  //50 , 55                        
                    }
                }

                if (SendHeatingModeCommand)
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    await SendCommandToBMS(SendCommandE4);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(1000);

                    await ShowBusyindicatior(true);

                    await ReadSOCAndHeatingMode();

                    await ShowBusyindicatior(true);

                    await Task.Delay(100);
                }

                if (IsValid && objReadValues.LowTempValue == NewLowTempValue && (!SendHeatingModeCommand || objReadValues.HeatingModeValue == Convert.ToInt32(HeatingMode)))
                {
                    if (HeatingMode == "00")
                    {
                        grdHeaterTest.IsVisible = false;
                    }
                    else
                    {
                        grdHeaterTest.IsVisible = true;
                    }

                    await ChargingUnderTempRead(RegisterEnum.REG_CHGUT);

                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async void btnEnablePin_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                lblMessageResetPassword.IsVisible = false;
                lblMessageResetPassword.Text = "";
                if ((bool)checkEnablePin.IsChecked)
                {
                    regionResetPin.IsVisible = true;
                    isSystemisSettingDefaultPin = false;
                    isAlreadyIntialLoadDecidedtheDefaultPassword = false;

                    grdEnablePinrowmain.Height = 160;
                }
                else if ((bool)!checkEnablePin.IsChecked)
                {
                    bPasswordRest = false;
                    regionResetPin.IsVisible = false;

                    grdEnablePinrowmain.Height = 90;

                    txtEnterNewPassword.Text = "";
                    txtEnterConfirmPassword.Text = "";
                    if (!isAlreadyIntialLoadDecidedtheDefaultPassword)
                    {
                        isSystemisSettingDefaultPin = true;
                        await SetDefaultPin();
                        int iTry = 0;
                        while (iTry < 5)
                        {
                            await WaitForResponsePassword();
                            if (bPasswordRest == true) { break; }
                            iTry++;
                        }

                        if (bPasswordRest)
                        {
                            string sMacAddress = lblMacAddressID.Text + "_Password";
                            string strPassword = Preferences.Get(sMacAddress, string.Empty);
                            if (!string.IsNullOrWhiteSpace(strPassword))
                            {
                                Preferences.Clear(sMacAddress);
                            }

                            await ShowBusyindicatior(false);
                            await ShowDisplayPopup("Password", "Pin code was removed.");
                        }
                        else
                        {
                            await ShowBusyindicatior(false);
                            await ShowDisplayPopup("Error", "Pin code was not removed!");
                        }
                    }
                }
                else
                {
                    regionResetPin.IsVisible = false;
                }
                await ShowBusyindicatior(false);
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnOverAmpTimeOut_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool isValid = false;
                await ShowBusyindicatior(true);

                int NewOverAmpTimeOutValue = (int)rangesliderOverAmpTimeOut.Value;
                isProccesBegin = true;

                int tCount = 0;

                while (tCount < 2) // Register 3f
                {
                    await SetOverTempTimeOut(Convert.ToInt32(rangesliderOverAmpTimeOut.Value));

                    await Task.Delay(200);

                    await ShowBusyindicatior(true);

                    await MaxDischargeAmpsRead(RegisterEnum.REG_DSGOC_DELAYS);

                    await Task.Delay(100);

                    if (objReadValues.OverAmpTimeOutReadValue != NewOverAmpTimeOutValue)
                    {
                        tCount++;
                    }
                    else
                    {
                        isValid = true;
                        break;
                    }
                }

                if (isValid)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task SetOverTempTimeOut(int ampTimeOutValue)
        {
            try
            {
                string ampTimeOut = Convert.ToInt32(ampTimeOutValue).ToString("X") + "20";

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(150);

                var WriteToReg29 = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_DSGOC_DELAYS, "02", Convert.ToUInt32(ampTimeOut, 16));
                await SendCommandToBMS(WriteToReg29);

                await Task.Delay(150);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
            }
        }

        #region New Firmware
        public async void btnNF_UpdateMinMaxSOC_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                int NewSOCMaxValue = (int)NF_rangesliderMaxSOC.Value;
                int NewSOCMinValue = (int)NF_rangesliderMinSOC.Value;

                await MinMaxSOCUpdate(NewSOCMaxValue, NewSOCMinValue);

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        public async Task MinMaxSOCUpdate(int NewSOCMaxValue, int NewSOCMinValue)
        {
            try
            {
                int tCount = 0;
                bool IsValid = false;
                isProccesBegin = true;

                int OldSOCMaxReadValue = objReadValues.SocMaxValue;
                int OldSOCMinReadValue = objReadValues.SocMinValue;

                for (int i = 0; i < 2; i++)
                {
                    while (tCount < 2)
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        var setRegE4 = objProtocol.PrepareBytesForNewFirmwareMaxMinSOC(NewSOCMaxValue, NewSOCMinValue, Convert.ToString(i).PadLeft(2, '0')); //E4
                        await SendCommandToBMS(setRegE4);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        await ShowBusyindicatior(true);

                        await Task.Delay(1000);

                        await ReadSOCAndHeatingMode();

                        await ShowBusyindicatior(true);

                        if ((objReadValues.SocMaxValue != NewSOCMaxValue) && (objReadValues.SocMinValue != NewSOCMinValue))
                        {
                            IsValid = false;
                            tCount++;
                        }
                        else
                        {
                            IsValid = true;
                            break;
                        }
                    }
                }

                if (!IsH2SettingUpdating)
                {
                    if (IsValid && (objReadValues.SocMaxValue == NewSOCMaxValue) && (objReadValues.SocMinValue == NewSOCMinValue))
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("Success", "Setting updated, please re-load the tools tab to see latest values.");
                    }
                    else
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("NotSuccess", "");
                    }
                }
                else
                {
                    await ShowBusyindicatior(false);
                    if (IsValid && (objReadValues.SocMaxValue == NewSOCMaxValue) && (objReadValues.SocMinValue == NewSOCMinValue))
                    {
                        IsH2SettingUpdating = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowBusyindicatior(false);
            }
        }
        private async void NF_btnHeatingModeSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                if ((objbleDevice.Amps < decimal.Zero || objbleDevice.Amps > decimal.Zero || !objbleDevice.ChargingMOSFET || !objbleDevice.DischargingMOSFET) && rdDisabledORLowTempProtectionOnly.IsChecked)
                {
                    CurrentPopup = string.Empty;
                    await ShowDisplayPopup("Error", "Warning disabled heating mode should only be used in limited cases such as for testing. Some BMS functions will be disabled and integrations with Victron may not work properly.\nBattery must be in standby and discharging and charging must be enabled before switching heating modes.");
                }
                else
                {
                    isProccesBegin = true;

                    await SendCommandForLowTempAndHeatingMode(true);

                    await ShowBusyindicatior(false);

                    isProccesBegin = false;
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnTestHeatingPads_Clicked(object sender, EventArgs e)
        {
            try
            {
                int HeatingTimer = 15;
                switch (rangesliderHeatingPadTimer.Value)
                {
                    case 1:
                        HeatingTimer = 15;
                        break;
                    case 2:
                        HeatingTimer = 60;
                        break;
                    case 3:
                        HeatingTimer = 120;
                        break;
                    case 4:
                        HeatingTimer = 240;
                        break;
                }
                string stringTime = HeatingTimer.ToString("X");
                string strHeatingPadValue = "18" + "82" + stringTime.PadLeft(2, '0');

                await ShowBusyindicatior(true);

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // var setRegE4 = new byte[] { 0xdd, 0x5a, 0xe4, 0x03, 0x18, 0x82, 0x0f, 0xfe, 0x70, 0x77 }; //E4 // 15 Sec
                // var setRegE4 = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_HEAT_TEST, "03", 1606159);
                var setRegE4 = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_HEAT_TEST, "03", Convert.ToUInt32(strHeatingPadValue, 16));
                await SendCommandToBMS(setRegE4);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await ShowBusyindicatior(false);

                await ShowDisplayPopup("Success", "Heating pads will run for " + HeatingTimer + " seconds");
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void RadioButtonGroupViewHeatingModes_Clicked(object sender, EventArgs e)
        {
            try
            {
                InputKit.Shared.Controls.RadioButton objRadioButton = (InputKit.Shared.Controls.RadioButton)sender;
                await SetLowTempHeatingMode(Convert.ToInt32(objRadioButton.ClassId));

                string strData = string.Empty;
                if (objRadioButton.ClassId == "0")
                {
                    strData = "DisabledHeatingRadio_" + objRadioButton.IsChecked;
                }
                else if (objRadioButton.ClassId == "1")
                {
                    strData = "LowTempHeatingRadio_" + objRadioButton.IsChecked;
                }
                else if (objRadioButton.ClassId == "2")
                {
                    strData = "StandbyHeatingRadio_" + objRadioButton.IsChecked;
                }
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.RadioButtonValueChange, strData);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task SetLowTempHeatingMode(int HeatingMode)
        {
            try
            {
                lblLowTempslider.Text = "Activation Temperature :";
                if (HeatingMode == 1 || HeatingMode == 01)
                {
                    if (IsFahrenheit)
                    {
                        NF_rangesliderLowTemp.Minimum = 35;
                        NF_rangesliderLowTemp.Maximum = 45;
                        NF_rangesliderLowTemp.Interval = NF_rangesliderLowTemp.StepSize = 5;
                    }
                    else
                    {
                        NF_rangesliderLowTemp.Minimum = 1;
                        NF_rangesliderLowTemp.Maximum = 7;
                        NF_rangesliderLowTemp.Interval = NF_rangesliderLowTemp.StepSize = 3;
                    }

                    if (TempsliderLowTempValue == 10 || TempsliderLowTempValue == 50)
                    {
                        if (IsFahrenheit)
                        {
                            NF_rangesliderLowTemp.Value = 45;
                        }
                        else
                        {
                            NF_rangesliderLowTemp.Value = 7;
                        }
                    }
                    else
                    {
                        NF_rangesliderLowTemp.Value = TempsliderLowTempValue;
                    }
                }
                else if (HeatingMode == 2 || HeatingMode == 02)
                {
                    if (IsFahrenheit)
                    {
                        NF_rangesliderLowTemp.Minimum = 40;
                        NF_rangesliderLowTemp.Maximum = 50;
                        NF_rangesliderLowTemp.Interval = NF_rangesliderLowTemp.StepSize = 5;
                    }
                    else
                    {
                        NF_rangesliderLowTemp.Minimum = 4;
                        NF_rangesliderLowTemp.Maximum = 10;
                        NF_rangesliderLowTemp.Interval = NF_rangesliderLowTemp.StepSize = 3;
                    }

                    if (TempsliderLowTempValue == 1 || TempsliderLowTempValue == 35)
                    {
                        if (IsFahrenheit)
                        {
                            NF_rangesliderLowTemp.Value = 40;
                        }
                        else
                        {
                            NF_rangesliderLowTemp.Value = 4;
                        }
                    }
                    else
                    {
                        NF_rangesliderLowTemp.Value = TempsliderLowTempValue;
                    }
                }
                else if (HeatingMode == 0 || HeatingMode == 00)
                {
                    if (IsFahrenheit)
                    {
                        NF_rangesliderLowTemp.Minimum = 35;
                        NF_rangesliderLowTemp.Maximum = 45;
                        NF_rangesliderLowTemp.Interval = NF_rangesliderLowTemp.StepSize = 5;
                    }
                    else
                    {
                        NF_rangesliderLowTemp.Minimum = 1;
                        NF_rangesliderLowTemp.Maximum = 7;
                        NF_rangesliderLowTemp.Interval = NF_rangesliderLowTemp.StepSize = 3;
                    }

                    if (TempsliderLowTempValue == 10 || TempsliderLowTempValue == 50)
                    {
                        if (IsFahrenheit)
                        {
                            NF_rangesliderLowTemp.Value = 45;
                        }
                        else
                        {
                            NF_rangesliderLowTemp.Value = 7;
                        }
                    }
                    else
                    {
                        NF_rangesliderLowTemp.Value = TempsliderLowTempValue;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion New Firmware

        #region H2 Board Specific Settings

        private async void btnActiveBalancer_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                uint iValue = 0;
                if ((bool)checkBalancerOnOff.IsChecked)
                {
                    iValue = 1;
                }
                else if ((bool)!checkBalancerOnOff.IsChecked)
                {
                    iValue = 0;
                }

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(150);

                var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_BAL_STATUS, "03", iValue);
                await SendCommandToBMS(bytes);

                await Task.Delay(150);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(500);

                if (objReadValues.IsActiveBalancerOn)
                {
                    stackBalancerSettings.IsVisible = true;
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ActiveBalancerToolTab, true);
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "Please set active balancer settings.");
                }
                else
                {
                    stackBalancerSettings.IsVisible = false;
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ActiveBalancerToolTab, false);
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "Active balancer disabled!");
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnSetBalanceParameterSetting_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                int[] iActivationVoltage = new int[4];

                if (rangesliderActivationVoltage.Value == 1)
                {
                    iActivationVoltage[0] = Convert.ToInt32(3.325 * 1000);
                    iActivationVoltage[1] = Convert.ToInt32(3.285 * 1000);
                }
                else if (rangesliderActivationVoltage.Value == 2)
                {
                    iActivationVoltage[0] = Convert.ToInt32(3.35 * 1000);
                    iActivationVoltage[1] = Convert.ToInt32(3.30 * 1000);
                }
                else if (rangesliderActivationVoltage.Value == 3)
                {
                    iActivationVoltage[0] = Convert.ToInt32(3.375 * 1000);
                    iActivationVoltage[1] = Convert.ToInt32(3.315 * 1000);
                }
                else if (rangesliderActivationVoltage.Value == 4)
                {
                    iActivationVoltage[0] = Convert.ToInt32(3.400 * 1000);
                    iActivationVoltage[1] = Convert.ToInt32(3.330 * 1000);
                }
                else if (rangesliderActivationVoltage.Value == 5)
                {
                    iActivationVoltage[0] = Convert.ToInt32(3.425 * 1000);
                    iActivationVoltage[1] = Convert.ToInt32(3.345 * 1000);
                }


                if (Convert.ToInt32(rangesliderDeviationLimit.Value) == 10)
                {
                    iActivationVoltage[2] = 10;
                    iActivationVoltage[3] = 5;
                }
                else if (Convert.ToInt32(rangesliderDeviationLimit.Value) == 20)
                {
                    iActivationVoltage[2] = 20;
                    iActivationVoltage[3] = 10;
                }
                else if (Convert.ToInt32(rangesliderDeviationLimit.Value) == 30)
                {
                    iActivationVoltage[2] = 30;
                    iActivationVoltage[3] = 20;
                }
                else if (Convert.ToInt32(rangesliderDeviationLimit.Value) == 40)
                {
                    iActivationVoltage[2] = 40;
                    iActivationVoltage[3] = 30;
                }
                else if (Convert.ToInt32(rangesliderDeviationLimit.Value) == 50)
                {
                    iActivationVoltage[2] = 50;
                    iActivationVoltage[3] = 40;
                }

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // var bytes2 = new byte[] { 0xcc, 0xd9, 0x0a, 0x01, 0xf4, 0x00, 0x64, 0x0d, 0xde, 0x0d, 0x16, 0xfd, 0x8f, 0xcf };
                var bytes2 = objProtocol.CreateBalanceParameterSetting(iActivationVoltage[0], iActivationVoltage[1], iActivationVoltage[2], iActivationVoltage[3]);
                await SendCommandToBMS(bytes2);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                if (objReadValues.IsActivationVoltage)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnHighSOCMonitor_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (lblFullyChargedDate.Text == "N/A" && (bool)checkSOCOnOff.IsChecked)
                {
                    await ShowDisplayPopup("Error", "Battery must be charged to full voltage and SOC before Regulate SOC can be enabled.");
                }
                else if ((bool)checkSOCOnOff.IsChecked && objReadValues.DaySinceFullCharge > 30)
                {
                    await ShowDisplayPopup("Error", "Your battery has not been fully charged for " + objReadValues.DaySinceFullCharge + " days, Please charge your battery and try again.");
                }
                else
                {
                    await ShowBusyindicatior(true);
                    uint iValue = 0;
                    if ((bool)checkSOCOnOff.IsChecked)
                    {
                        iValue = 1;
                    }
                    else if ((bool)!checkSOCOnOff.IsChecked)
                    {
                        iValue = 0;
                    }

                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_T_NTC_4, "03", iValue);
                    await SendCommandToBMS(bytes);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(1000);

                    await ShowBusyindicatior(false);

                    if (objReadValues.IsHighSOCMonitorOn)
                    {
                        stackSOCParameter.IsVisible = true;
                        await ShowDisplayPopup("Success", "Please set high soc limit.");
                    }
                    else if (!objReadValues.IsHighSOCMonitorOn)
                    {
                        stackSOCParameter.IsVisible = false;

                        await ShowDisplayPopup("Success", "Regulate high soc disabled.");
                    }
                    else
                    {
                        await ShowDisplayPopup("NotSuccess", "");
                    }
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnSetSOCParameter_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                IsH2SettingUpdating = true;

                int[] iSOCParameter = new int[2];

                if (rangesliderTimeInterval.Value > 0)
                {
                    iSOCParameter[0] = (int)rangesliderTimeInterval.Value;
                }

                if (rangesliderHighSOCLimit.Value > 0)
                {
                    iSOCParameter[1] = (int)rangesliderHighSOCLimit.Value;
                }

                if (iSOCParameter[0] > 0 && iSOCParameter[1] > 0)
                {
                    string Value = iSOCParameter[0].ToString("X") + iSOCParameter[1].ToString("X") + Convert.ToInt32("33").ToString("X");
                    uint iValue = Convert.ToUInt32(Value, 16);

                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_T_NTC_3, "05", iValue);
                    await SendCommandToBMS(bytes);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(500);

                    await ShowBusyindicatior(false);

                    await OnToolTabLoad();

                    await Task.Delay(100);

                    await ReadSOCAndHeatingMode();

                    if (objReadValues.IsSOCParamSet)
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("Success", "");
                    }
                    else
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("NotSuccess", "");
                    }
                }

                IsH2SettingUpdating = false;
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnChargePulse_Clicked(object sender, EventArgs e)
        {
            try
            {
                uint iValue = (uint)rangesliderChargePulse.Value;

                if ((objReadValues.CurrentSOC < 10 || Convert.ToInt32(NF_rangesliderMinSOC.Value) < 10) && iValue != 0)
                {
                    //await ShowDisplayPopup("Error", "Pulse Charge can not be enabled if BMS SOC or Min SOC is below 10%.");
                    await ShowDisplayPopup("Error", "Pulse Charge can not be enabled if state of charge is below 10%.");
                }
                else
                {
                    IsH2SettingUpdating = true;
                    await ShowBusyindicatior(true);

                    if (iValue != 0)
                    {
                        int NewSOCMaxValue = (int)NF_rangesliderMaxSOC.Value;
                        await MinMaxSOCUpdate(NewSOCMaxValue, 10);

                        await Task.Delay(500);
                    }

                    await ChargePulseUpdate(iValue);

                    await Task.Delay(500);

                    await ShowBusyindicatior(false);

                    if (iValue != 0)
                    {
                        await ReadSOCAndHeatingMode();

                        await Task.Delay(100);

                        await OnToolTabLoad();
                    }

                    if (!objReadValues.IsChargePulseOn && iValue == 0)
                    {
                        await ShowDisplayPopup("Success", "");
                    }
                    else if (objReadValues.IsChargePulseTimeSet)
                    {
                        if (lblSOCLowerLimit.Text.Replace("%", "") == "0")
                        {
                            lblSOCLowerLimit.Text = Convert.ToString(10) + "%";
                        }

                        await ShowDisplayPopup("Success", "");
                    }
                    else
                    {
                        await ShowDisplayPopup("NotSuccess", "");
                    }
                    IsH2SettingUpdating = false;
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        public async Task ChargePulseUpdate(uint iValue = 0)
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(150);

                if (iValue > 0)
                {
                    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_PULSE, "03", 1);
                    await SendCommandToBMS(bytes);

                    await Task.Delay(150);

                    var bytes2 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CHARGE_PULSE, "03", iValue);
                    await SendCommandToBMS(bytes2);
                }
                else
                {
                    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_PULSE, "03", 0);
                    await SendCommandToBMS(bytes);

                    rangesliderChargePulse.Value = 0;
                }

                await Task.Delay(150);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(500);

                await ShowBusyindicatior(false);
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnReSyncTime_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                TimeSyncronized = false;
                await WriteRTCTimeInterval();

                await Task.Delay(150);

                await ShowBusyindicatior(false);
            }
            catch (Exception ex)
            {
            }
        }

        private async void btnEnableBluetoothRestart_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                uint value = 0;
                if (CheckBoxEnableBluetoothRestart.IsChecked)
                {
                    value = 1;
                }

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var DeviceBindB2 = objProtocol.CreateH2CustomCommand(RegisterEnum.BLE_RESTART, "03", value);
                await SendCommandToBMS(DeviceBindB2);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(100);

                await ReadEnableBluetoothAfterRestart();

                await Task.Delay(500);

                if (objReadValues.IsEnableBLERestart)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "Auto start bluetooth is enabled.");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "Auto start bluetooth is disabled.");
                }
            }
            catch (Exception ex)
            {
            }
        }


        #endregion H2 Board Specific Settings

        #endregion ToolTab Update       

        #region Read Tool and Calibration tab Values
        private async Task ReadHomeTab()
        {
            try
            {
                await ShowBusyindicatior(true);

                #region CommonRead Commands

                await ReadBMSCodeWith0ACommand();

                await btnTemperatureUnitRead();

                await Task.Delay(100);

                if (UsedNewFirmware)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        await ReadSOCAndHeatingMode();
                        if (objReadValues.HeatingStartTempValue > decimal.Zero) { break; }
                    }
                    NF_StackHomeIcon.IsVisible = true;
                }
                else
                {
                    NF_StackHomeIcon.IsVisible = false;
                }

                await Task.Delay(200);

                for (int i = 0; i < 3; i++)
                {
                    await ChargingUnderTempRead(RegisterEnum.REG_CHGUT);

                    await Task.Delay(200);

                    if (objReadValues.LowTempValue > decimal.Zero) { break; }
                }

                await btnPullRequestRead();

                await btnEstablishConnectionRead();

                await Task.Delay(100);

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                await LowTempDischargeRead(RegisterEnum.REG_DSGUT);

                await Task.Delay(100);

                await MaxDischargeAmpsRead(RegisterEnum.REG_DSGOC_DELAYS);

                await Task.Delay(100);

                //Read Max Soc Reg
                // var getRegister26Value = new byte[] { 0xdd, 0xa5, 0x26, 0x00, 0xff, 0xda, 0x77 };
                var getRegister26Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CUVP);
                await SendCommandToBMS(getRegister26Value);

                await Task.Delay(100);

                //Read Min Soc Reg
                // var getRegister24Value = new byte[] { 0xdd, 0xa5, 0x24, 0x00, 0xff, 0xdc, 0x77 };
                var getRegister24Value = objProtocol.CreateReadCommand(RegisterEnum.REG_COVP);
                await SendCommandToBMS(getRegister24Value);

                await Task.Delay(100);

                //Read CycleCount Reg
                // var getRegister17Value = new byte[] { 0xdd, 0xa5, 0x17, 0x00, 0xff, 0xe9, 0x77 };
                var getRegister17Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CYCLE_CNT);
                await SendCommandToBMS(getRegister17Value);

                await Task.Delay(100);

                //Read ProtectionStatusErrorCount Reg
                // var getRegisterAAValue = new byte[] { 0xdd, 0xa5, 0xaa, 0x00, 0xff, 0x56, 0x77 };
                var getRegisterAAValue = objProtocol.CreateReadCommand(RegisterEnum.REG_ERROR_LOGS);
                await SendCommandToBMS(getRegisterAAValue);

                await Task.Delay(100);

                await GetSerialNumberFromBMS();

                await Task.Delay(100);

                //FaultReleaseRead
                // var getFaultReleaseValue = new byte[] { 0xdd, 0xa5, 0x2d, 0x00, 0xff, 0xd3, 0x77 };
                var getFaultReleaseValue = objProtocol.CreateReadCommand(RegisterEnum.REG_FUNC_CONFIG);
                await SendCommandToBMS(getFaultReleaseValue);

                await Task.Delay(100);

                //MaxBatteryTempRead
                // var getRegister18Value = new byte[] { 0xdd, 0xa5, 0x18, 0x00, 0xff, 0xe8, 0x77 };
                var getRegister18Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CHGOT);
                await SendCommandToBMS(getRegister18Value);

                await Task.Delay(100);

                //MaxChargeAmpsRead
                // var getRegister28Value = new byte[] { 0xdd, 0xa5, 0x28, 0x00, 0xff, 0xd8, 0x77 };
                var getRegister28Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CHGOC);
                await SendCommandToBMS(getRegister28Value);

                await Task.Delay(100);

                //MaxDischargeAmpsRead
                // var getRegister29Value = new byte[] { 0xdd, 0xa5, 0x29, 0x00, 0xff, 0xd7, 0x77 };
                var getRegister29Value = objProtocol.CreateReadCommand(RegisterEnum.REG_DSGOC);
                await SendCommandToBMS(getRegister29Value);

                await Task.Delay(100);

                //FullCapacityRead
                // var getRegister10Value = new byte[] { 0xdd, 0xa5, 0x10, 0x00, 0xff, 0xf0, 0x77 };
                var getRegister10Value = objProtocol.CreateReadCommand(RegisterEnum.REG_DESIGN_CAP);
                await SendCommandToBMS(getRegister10Value);

                await Task.Delay(100);

                #endregion CommonRead Commands

                #region H2 Read Commands

                if (objReadValues._IsSFKH2Device)
                {
                    await ReadDaySinceFullCharge("date");

                    await Task.Delay(50);

                    await ReadDaySinceFullCharge();

                    await Task.Delay(100);

                    await ReadRTCTimeInterval();

                    await Task.Delay(100);

                    await ReadEnableBluetoothAfterRestart();

                    await Task.Delay(100);

                    await ReadNicIdCommand();

                    await Task.Delay(100);

                    await ReadBindUnbindStatus();

                    await Task.Delay(100);

                    await ReadBMSResetCount();

                    await Task.Delay(100);

                    await ReadFirmwareVersion();

                    await Task.Delay(100);

                    for (int i = 0; i < 3; i++)
                    {
                        await ReadH2ParameterValues();

                        await Task.Delay(200);

                        if (objReadValues.IsChargePulseOn || objReadValues.IsHighSOCMonitorOn) { break; }
                    }

                    await ReadRegulateSOCActiveStatus();
                }

                #endregion H2 Read Commands

                if (objDeviceInformation.DefaultOverAmpTimeOut > 0)
                {
                    await SetOverTempTimeOut(objDeviceInformation.DefaultOverAmpTimeOut);
                }

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                brWarningOne.IsVisible = true;

                await ShowBusyindicatior(false);
            }
            catch (Exception ex)
            {

            }
        }
        private async Task LoadToolTabData()
        {
            try
            {
                if (!BechmarkTestRunning)
                {
                    await ShowAbsoluteLayout(true, "", true);

                    await Task.Delay(1000);

                    await btnEstablishConnectionRead();

                    await btnPullRequestRead();

                    await ShowNickNameText();

                    await btnTemperatureUnitRead();

                    await LowTempDischargeValueRead();

                    await LowTempDischargeRead(RegisterEnum.REG_DSGUT);

                    await MaxDischargeAmpsRead(RegisterEnum.REG_DSGOC_DELAYS);

                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    if (!isProccesBegin)
                    {
                        //MaxBatteryTempRead
                        // var getRegister18Value = new byte[] { 0xdd, 0xa5, 0x18, 0x00, 0xff, 0xe8, 0x77 };
                        var getRegister18Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CHGOT);
                        await SendCommandToBMS(getRegister18Value);

                        await Task.Delay(100);
                    }

                    if (!isProccesBegin)
                    {
                        //MinSocRead
                        // var getRegister26Value = new byte[] { 0xdd, 0xa5, 0x26, 0x00, 0xff, 0xda, 0x77 };
                        var getRegister26Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CUVP);
                        await SendCommandToBMS(getRegister26Value);

                        await Task.Delay(100);
                    }

                    if (!isProccesBegin)
                    {
                        //MaxSOCRead
                        // var getRegister24Value = new byte[] { 0xdd, 0xa5, 0x24, 0x00, 0xff, 0xdc, 0x77 };
                        var getRegister24Value = objProtocol.CreateReadCommand(RegisterEnum.REG_COVP);
                        await SendCommandToBMS(getRegister24Value);

                        await Task.Delay(100);
                    }

                    if (!isProccesBegin)
                    {
                        //MaxChargeAmpsRead
                        // var getRegister28Value = new byte[] { 0xdd, 0xa5, 0x28, 0x00, 0xff, 0xd8, 0x77 };
                        var getRegister28Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CHGOC);
                        await SendCommandToBMS(getRegister28Value);

                        await Task.Delay(100);
                    }
                    if (!isProccesBegin)
                    {

                        //MaxDischargeAmpsRead
                        // var getRegister29Value = new byte[] { 0xdd, 0xa5, 0x29, 0x00, 0xff, 0xd7, 0x77 };
                        var getRegister29Value = objProtocol.CreateReadCommand(RegisterEnum.REG_DSGOC);
                        await SendCommandToBMS(getRegister29Value);

                        await Task.Delay(100);
                    }

                    if (!isProccesBegin)
                    {
                        //FullCapacityRead
                        // var getRegister10Value = new byte[] { 0xdd, 0xa5, 0x10, 0x00, 0xff, 0xf0, 0x77 };
                        var getRegister10Value = objProtocol.CreateReadCommand(RegisterEnum.REG_DESIGN_CAP);
                        await SendCommandToBMS(getRegister10Value);

                        await Task.Delay(100);
                    }

                    if (!isProccesBegin)
                    {
                        //FaultReleaseRead
                        // var getFaultReleaseValue = new byte[] { 0xdd, 0xa5, 0x2d, 0x00, 0xff, 0xd3, 0x77 };
                        var getFaultReleaseValue = objProtocol.CreateReadCommand(RegisterEnum.REG_FUNC_CONFIG);
                        await SendCommandToBMS(getFaultReleaseValue);

                        await Task.Delay(100);
                    }

                    if ((!isProccesBegin) && UsedNewFirmware)
                    {
                        //BMS Read Soc Protection and Heating Mode
                        // var HeatingMode = new byte[] { 0xdd, 0xa5, 0x09, 0x01, 0x01, 0xff, 0xf5, 0x77 };
                        var HeatingMode = objProtocol.CreateCustomReadCommand(RegisterEnum.REG_HEAT_SOC_LIMITS, "01", 1);
                        await SendCommandToBMS(HeatingMode);
                    }

                    await Task.Delay(100);

                    if (LowTemperatureHeatingSupport && !isProccesBegin)
                    {
                        //ChargingUnderTempRead
                        // var getRegister1bValue = new byte[] { 0xdd, 0xa5, 0x1b, 0x00, 0xff, 0xe5, 0x77 };
                        var getRegister1bValue = objProtocol.CreateReadCommand(RegisterEnum.REG_CHGUT);
                        await SendCommandToBMS(getRegister1bValue);
                    }

                    if (objReadValues.MaxSOCReadValue == 0)
                    {
                        await SetDefaultMaxSocRead();
                    }

                    if (!UsedNewFirmware)
                    {
                        //SOC Setting
                        if (objReadValues._IsSFKH2Device)
                        {
                            grdMaximumSOC.IsVisible = false;
                        }
                        else
                        {
                            grdMaximumSOC.IsVisible = true;
                        }
                        NewFirmwareSocSetting.IsVisible = false;

                        //Heating Settings 
                        HeatingSettings.IsVisible = false;
                    }
                    else
                    {
                        //SOC Setting
                        grdMaximumSOC.IsVisible = false;
                        NewFirmwareSocSetting.IsVisible = true;

                        //Heating Settings 
                        HeatingSettings.IsVisible = true;
                        LowTempStackLayout.IsVisible = false;
                    }


                    await ShowBusyindicatior(false);

                    await ShowAbsoluteLayout(false, "", true);

                    await Task.Delay(1000);

                    if (nReadCounter >= EstablishConnectionDatAttemps)
                    {
                        string NickName = Preferences.Get(lblMacAddressID.Text.ToString(), string.Empty);
                        if (string.IsNullOrWhiteSpace(NickName))
                        {
                            NickName = _connectedDevice.DeviceName.Trim();
                        }
                        await ShowDisplayPopup("Error", "Unable to connect : " + NickName);
                        MainPage Page = new MainPage(false);
                        await Navigation.PushAsync(Page);
                    }

                    if (objReadValues._IsSFKH2Device)
                    {
                        await ReadRTCTimeInterval();

                        await ReadH2ParameterValues();

                        await ReadDebugModeStatus();

                        await ReadEnableBluetoothAfterRestart();
                    }
                }
                else
                {
                    await ShowAbsoluteLayout(true, "The Tool tab is disabled while a benchmark test is running.", false);
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task MaxSOCRead(string RegisterNumber = "24")
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var byteValue = objProtocol.CreateReadCommand(RegisterNumber);
                await SendCommandToBMS(byteValue);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task MinSocRead(string RegisterNumber = "26")
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var byteValue = objProtocol.CreateReadCommand(RegisterNumber);
                await SendCommandToBMS(byteValue);

                await Task.Delay(200);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task MaxChargeAmpsRead()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var getRegister28Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CHGOC);
                await SendCommandToBMS(getRegister28Value);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task MaxDischargeAmpsRead(string RegisterNumber = RegisterEnum.REG_DSGOC)
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // var getRegister29Value = new byte[] { 0xcc, 0xcb, 0x29, 0x00, 0xff, 0xd7, 0xcf };
                var getRegister29Value = objProtocol.CreateReadCommand(RegisterNumber);
                await SendCommandToBMS(getRegister29Value);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task FullCapacityRead(string RegisterNumber = "10")
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(200);

                var byteValue = objProtocol.CreateReadCommand(RegisterNumber);
                await SendCommandToBMS(byteValue);

                await Task.Delay(200);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task ChargingUnderTempRead(string RegisterNumber = "1a")
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var byteValue = objProtocol.CreateReadCommand(RegisterNumber);
                await SendCommandToBMS(byteValue);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task MaxBatteryTempRead(string RegisterNumber = "18")
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var byteValue = objProtocol.CreateReadCommand(RegisterNumber);
                await SendCommandToBMS(byteValue);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task btnEstablishConnectionRead()
        {
            try
            {
                var _Temp = Preferences.Get("EstablishConnectionData", string.Empty);
                int.TryParse(_Temp, out EstablishConnectionDatAttemps);

                if (EstablishConnectionDatAttemps == 0)
                {
                    Preferences.Set("EstablishConnectionData", "10");
                    EstablishConnectionDatAttemps = 10;
                }

                objRadioButtonViewModel = new RadioButtonViewModel();
                objRadioButtonViewModel.ConnectionEstablishData = new bool[4];

                if (EstablishConnectionDatAttemps == 5)
                {
                    objRadioButtonViewModel.ConnectionEstablishData[0] = true;
                    objRadioButtonViewModel.ConnectionEstablishData[1] = false;
                    objRadioButtonViewModel.ConnectionEstablishData[2] = false;
                    objRadioButtonViewModel.ConnectionEstablishData[3] = false;
                }
                else if (EstablishConnectionDatAttemps == 10)
                {
                    objRadioButtonViewModel.ConnectionEstablishData[0] = false;
                    objRadioButtonViewModel.ConnectionEstablishData[1] = true;
                    objRadioButtonViewModel.ConnectionEstablishData[2] = false;
                    objRadioButtonViewModel.ConnectionEstablishData[3] = false;
                }
                else if (EstablishConnectionDatAttemps == 30)
                {
                    objRadioButtonViewModel.ConnectionEstablishData[0] = false;
                    objRadioButtonViewModel.ConnectionEstablishData[1] = false;
                    objRadioButtonViewModel.ConnectionEstablishData[2] = true;
                    objRadioButtonViewModel.ConnectionEstablishData[3] = false;
                }
                else if (EstablishConnectionDatAttemps == 60)
                {
                    objRadioButtonViewModel.ConnectionEstablishData[0] = false;
                    objRadioButtonViewModel.ConnectionEstablishData[1] = false;
                    objRadioButtonViewModel.ConnectionEstablishData[2] = false;
                    objRadioButtonViewModel.ConnectionEstablishData[3] = true;
                }

                rdEstablishConnectionFive.BindingContext = objRadioButtonViewModel;
                rdEstablishConnectionFive.IsChecked = objRadioButtonViewModel.ConnectionEstablishData[0];

                rdEstablishConnectionTen.BindingContext = objRadioButtonViewModel;
                rdEstablishConnectionTen.IsChecked = objRadioButtonViewModel.ConnectionEstablishData[1];

                rdEstablishConnectionThirty.BindingContext = objRadioButtonViewModel;
                rdEstablishConnectionThirty.IsChecked = objRadioButtonViewModel.ConnectionEstablishData[2];

                rdEstablishConnectionSixty.BindingContext = objRadioButtonViewModel;
                rdEstablishConnectionSixty.IsChecked = objRadioButtonViewModel.ConnectionEstablishData[3];

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task btnPullRequestRead()
        {
            try
            {
                var _Temp = Preferences.Get("PollRequestData", string.Empty);
                int.TryParse(_Temp, out PullingRequestTime);

                if (PullingRequestTime == 0)
                {
                    Preferences.Set("PollRequestData", "3000");
                    PullingRequestTime = 3000;
                }

                objRadioButtonViewModel = new RadioButtonViewModel();
                objRadioButtonViewModel.PullRequestData = new bool[3];

                if (PullingRequestTime == 1000)
                {
                    objRadioButtonViewModel.PullRequestData[0] = true;
                    objRadioButtonViewModel.PullRequestData[1] = false;
                    objRadioButtonViewModel.PullRequestData[2] = false;
                }
                else if (PullingRequestTime == 2000)
                {
                    objRadioButtonViewModel.PullRequestData[0] = false;
                    objRadioButtonViewModel.PullRequestData[1] = true;
                    objRadioButtonViewModel.PullRequestData[2] = false;
                }
                else if (PullingRequestTime == 3000)
                {
                    objRadioButtonViewModel.PullRequestData[0] = false;
                    objRadioButtonViewModel.PullRequestData[1] = false;
                    objRadioButtonViewModel.PullRequestData[2] = true;
                }

                rdPulllRequestOne.BindingContext = objRadioButtonViewModel;
                rdPulllRequestOne.IsChecked = objRadioButtonViewModel.PullRequestData[0];

                rdPulllRequestTwo.BindingContext = objRadioButtonViewModel;
                rdPulllRequestTwo.IsChecked = objRadioButtonViewModel.PullRequestData[1];

                rdPulllRequestThree.BindingContext = objRadioButtonViewModel;
                rdPulllRequestThree.IsChecked = objRadioButtonViewModel.PullRequestData[2];

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task btnTemperatureUnitRead()
        {
            try
            {
                var _Temp = Preferences.Get("CelsiusorFahrenheit", string.Empty);

                sCelsiusorFahrenheit = Convert.ToString(_Temp);

                await Task.Delay(1000);

                if (sCelsiusorFahrenheit == "" || sCelsiusorFahrenheit == null)
                {
                    Preferences.Set("CelsiusorFahrenheit", "Fahrenheit");

                    _Temp = Preferences.Get("CelsiusorFahrenheit", string.Empty);
                    sCelsiusorFahrenheit = Convert.ToString(_Temp);
                }

                objRadioButtonViewModel = new RadioButtonViewModel();
                objRadioButtonViewModel.TemperatureUnit = new bool[2];

                await Task.Delay(1000);

                if (sCelsiusorFahrenheit == "Celsius")
                {
                    objRadioButtonViewModel.TemperatureUnit[0] = true;
                    objRadioButtonViewModel.TemperatureUnit[1] = false;

                    IsFahrenheit = false;
                }
                else if (sCelsiusorFahrenheit == "Fahrenheit")
                {
                    objRadioButtonViewModel.TemperatureUnit[0] = false;
                    objRadioButtonViewModel.TemperatureUnit[1] = true;

                    IsFahrenheit = true;
                }

                rdCelsius.BindingContext = objRadioButtonViewModel;
                rdCelsius.IsChecked = objRadioButtonViewModel.TemperatureUnit[0];

                rdFahrenheit.BindingContext = objRadioButtonViewModel;
                rdFahrenheit.IsChecked = objRadioButtonViewModel.TemperatureUnit[1];

                if (IsFahrenheit)
                {
                    lblBMSTemprature.Text = "BMS Temp.\n(°F)";
                    lblCaseTemprature.Text = "Case Temp. 1\n(°F)";
                    lblCaseTemprature2.Text = "Case Temp. 2\n(°F)";
                }
                else
                {
                    lblBMSTemprature.Text = "BMS Temp.\n(°C)";
                    lblCaseTemprature.Text = "Case Temp. 1\n(°C)";
                    lblCaseTemprature2.Text = "Case Temp. 2\n(°C)";
                }

                await CelsiusorFahrenheitGauges();

            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task LowTempDischargeRead(string RegisterNumber = "1c")
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var byteValue = objProtocol.CreateReadCommand(RegisterNumber);
                await SendCommandToBMS(byteValue);

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task FaultReleaseRead()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // var getFaultReleaseValue = new byte[] { 0xdd, 0xa5, 0x2d, 0x00, 0xff, 0xd3, 0x77 };
                var getFaultReleaseValue = objProtocol.CreateReadCommand(RegisterEnum.REG_FUNC_CONFIG);
                await SendCommandToBMS(getFaultReleaseValue);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task LowTempDischargeValueRead()
        {
            try
            {
                bool CheckboxValue = Convert.ToBoolean(Preferences.Get("LowTempDischarge", string.Empty));

                CheckBoxLowTempDischarg.IsChecked = CheckboxValue;
            }
            catch (Exception ex)
            {
            }
        }
        private async Task CycleCountRead()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // var getRegister17Value = new byte[] { 0xdd, 0xa5, 0x17, 0x00, 0xff, 0xe9, 0x77 };
                var getRegister17Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CYCLE_CNT);
                await SendCommandToBMS(getRegister17Value);

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task ProtectionStatusErrorCountRead()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // var getRegisterAAValue = new byte[] { 0xdd, 0xa5, 0xaa, 0x00, 0xff, 0x56, 0x77 };
                var getRegisterAAValue = objProtocol.CreateReadCommand(RegisterEnum.REG_ERROR_LOGS);
                await SendCommandToBMS(getRegisterAAValue);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(1000);

                if (objReadValues._IsSFKH2Device)
                {
                    await H2BmsLogRead();
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task H2BmsLogRead(uint value = 0)
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // CC D7 03 00 FF FD CF
                var bytes2 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_T_NTC_7, "03", value);
                await SendCommandToBMS(bytes2);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
            }
        }
        private async Task SetDefaultMaxSocRead()
        {
            try
            {
                objRadioButtonViewModel = new RadioButtonViewModel();
                objRadioButtonViewModel.MaximumSocData = new bool[3];

                objRadioButtonViewModel.MaximumSocData[0] = false;
                objRadioButtonViewModel.MaximumSocData[1] = false;
                objRadioButtonViewModel.MaximumSocData[2] = true;
                objReadValues.MaxSOCReadValue = 100;
                objReadValues.IsvalidMaxSOCReadValue = 3.65;

                if (objReadValues.MaxSOCReadValue > 0)
                {
                    rbMaximumSOCService.BindingContext = objRadioButtonViewModel;
                    rbMaximumSOCService.IsChecked = objRadioButtonViewModel.MaximumSocData[0];

                    rbMaximumSOCStorage.BindingContext = objRadioButtonViewModel;
                    rbMaximumSOCStorage.IsChecked = objRadioButtonViewModel.MaximumSocData[1];

                    rbMaximumSOCNormal.BindingContext = objRadioButtonViewModel;
                    rbMaximumSOCNormal.IsChecked = objRadioButtonViewModel.MaximumSocData[2];
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task CelsiusorFahrenheitGauges()
        {
            try
            {
                if (IsFahrenheit)
                {
                    rangeSliderTempBMS.Minimum = 20;
                    rangeSliderTempBMS.Maximum = 140;

                    rangeSliderTempCase.Minimum = 20;
                    rangeSliderTempCase.Maximum = 140;

                    ObservableCollection<GaugeGradientStop> gaugeGradientStops = new ObservableCollection<GaugeGradientStop>();

                    GaugeGradientStop gaugeGradientStop1 = new GaugeGradientStop();
                    gaugeGradientStop1.Value = 20;
                    gaugeGradientStop1.Color = Colors.DarkBlue;
                    gaugeGradientStops.Add(gaugeGradientStop1);

                    GaugeGradientStop gaugeGradientStop2 = new GaugeGradientStop();
                    gaugeGradientStop2.Value = 34;
                    gaugeGradientStop2.Color = Colors.LightBlue;
                    gaugeGradientStops.Add(gaugeGradientStop2);

                    GaugeGradientStop gaugeGradientStop3 = new GaugeGradientStop();
                    gaugeGradientStop3.Value = 50;
                    gaugeGradientStop3.Color = Colors.SeaGreen;
                    gaugeGradientStops.Add(gaugeGradientStop3);

                    GaugeGradientStop gaugeGradientStop4 = new GaugeGradientStop();
                    gaugeGradientStop4.Value = 60;
                    gaugeGradientStop4.Color = Colors.Green;
                    gaugeGradientStops.Add(gaugeGradientStop4);

                    GaugeGradientStop gaugeGradientStop5 = new GaugeGradientStop();
                    gaugeGradientStop5.Value = 90;
                    gaugeGradientStop5.Color = Colors.Orange;
                    gaugeGradientStops.Add(gaugeGradientStop5);

                    GaugeGradientStop gaugeGradientStop6 = new GaugeGradientStop();
                    gaugeGradientStop6.Value = 100;
                    gaugeGradientStop6.Color = Colors.DarkOrange;
                    gaugeGradientStops.Add(gaugeGradientStop6);

                    GaugeGradientStop gaugeGradientStop7 = new GaugeGradientStop();
                    gaugeGradientStop7.Value = 120;
                    gaugeGradientStop7.Color = Colors.Red;
                    gaugeGradientStops.Add(gaugeGradientStop7);

                    grdColorChangesBMS.GradientStops = gaugeGradientStops;
                    grdColorChangesCase.GradientStops = gaugeGradientStops;

                    NF_rangesliderLowTemp.BindingContext = rangesliderLowTemp.BindingContext = objDeviceInformation;
                    NF_rangesliderLowTemp.Minimum = rangesliderLowTemp.Minimum = objDeviceInformation.CustomCollectionLowTemp[0].Value;
                    NF_rangesliderLowTemp.Maximum = rangesliderLowTemp.Maximum = objDeviceInformation.CustomCollectionLowTemp[objDeviceInformation.CustomCollectionLowTemp.Count - 1].Value;
                    NF_rangesliderLowTemp.Interval = rangesliderLowTemp.Interval = NF_rangesliderLowTemp.StepSize = rangesliderLowTemp.StepSize = 5;

                    lblBmsTempScale.Maximum = 120;
                    lblCaseTempScale.Maximum = 120;

                    rangesliderMaxBatteryTemp.BindingContext = objDeviceInformation;
                    rangesliderMaxBatteryTemp.Minimum = objDeviceInformation.CustomCollectionMaxBatteryTemp[0].Value;
                    rangesliderMaxBatteryTemp.Maximum = objDeviceInformation.CustomCollectionMaxBatteryTemp[objDeviceInformation.CustomCollectionMaxBatteryTemp.Count - 1].Value;
                    rangesliderMaxBatteryTemp.Interval = rangesliderMaxBatteryTemp.StepSize = 9;
                }
                else
                {
                    rangeSliderTempBMS.Minimum = 0;
                    rangeSliderTempBMS.Maximum = 60;

                    rangeSliderTempCase.Minimum = 0;
                    rangeSliderTempCase.Maximum = 60;

                    ObservableCollection<GaugeGradientStop> gaugeGradientStops = new ObservableCollection<GaugeGradientStop>();

                    GaugeGradientStop gaugeGradientStop1 = new GaugeGradientStop();
                    gaugeGradientStop1.Value = 0;
                    gaugeGradientStop1.Color = Colors.DarkBlue;
                    gaugeGradientStops.Add(gaugeGradientStop1);

                    GaugeGradientStop gaugeGradientStop2 = new GaugeGradientStop();
                    gaugeGradientStop2.Value = 5;
                    gaugeGradientStop2.Color = Colors.LightBlue;
                    gaugeGradientStops.Add(gaugeGradientStop2);

                    GaugeGradientStop gaugeGradientStop3 = new GaugeGradientStop();
                    gaugeGradientStop3.Value = 10;
                    gaugeGradientStop3.Color = Colors.SeaGreen;
                    gaugeGradientStops.Add(gaugeGradientStop3);

                    GaugeGradientStop gaugeGradientStop4 = new GaugeGradientStop();
                    gaugeGradientStop4.Value = 15;
                    gaugeGradientStop4.Color = Colors.Green;
                    gaugeGradientStops.Add(gaugeGradientStop4);

                    GaugeGradientStop gaugeGradientStop5 = new GaugeGradientStop();
                    gaugeGradientStop5.Value = 32;
                    gaugeGradientStop5.Color = Colors.Orange;
                    gaugeGradientStops.Add(gaugeGradientStop5);

                    GaugeGradientStop gaugeGradientStop6 = new GaugeGradientStop();
                    gaugeGradientStop6.Value = 37;
                    gaugeGradientStop6.Color = Colors.DarkOrange;
                    gaugeGradientStops.Add(gaugeGradientStop6);

                    GaugeGradientStop gaugeGradientStop7 = new GaugeGradientStop();
                    gaugeGradientStop7.Value = 48;
                    gaugeGradientStop7.Color = Colors.Red;
                    gaugeGradientStops.Add(gaugeGradientStop7);

                    grdColorChangesBMS.GradientStops = gaugeGradientStops;
                    grdColorChangesCase.GradientStops = gaugeGradientStops;

                    NF_rangesliderLowTemp.BindingContext = rangesliderLowTemp.BindingContext = objDeviceInformation;
                    NF_rangesliderLowTemp.Minimum = rangesliderLowTemp.Minimum = objDeviceInformation.CustomCollectionLowTempCelsius[0].Value;
                    NF_rangesliderLowTemp.Maximum = rangesliderLowTemp.Maximum = objDeviceInformation.CustomCollectionLowTempCelsius[objDeviceInformation.CustomCollectionLowTempCelsius.Count - 1].Value;
                    NF_rangesliderLowTemp.Interval = rangesliderLowTemp.Interval = NF_rangesliderLowTemp.StepSize = rangesliderLowTemp.StepSize = 3;

                    lblBmsTempScale.Maximum = 60;
                    lblCaseTempScale.Maximum = 60;

                    rangesliderMaxBatteryTemp.BindingContext = objDeviceInformation;
                    rangesliderMaxBatteryTemp.Minimum = objDeviceInformation.CustomCollectionMaxBatteryTempCelsius[0].Value;
                    rangesliderMaxBatteryTemp.Maximum = objDeviceInformation.CustomCollectionMaxBatteryTempCelsius[objDeviceInformation.CustomCollectionMaxBatteryTempCelsius.Count - 1].Value;
                    rangesliderMaxBatteryTemp.Interval = rangesliderMaxBatteryTemp.StepSize = 5;
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task GetSerialNumberFromBMS()
        {
            try
            {
                int barcodeCount = 0;
                while (barcodeCount < 3 && string.IsNullOrWhiteSpace(BarcodeValue))
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    // var getRegisterA2Value = new byte[] { 0xdd, 0xa5, 0xa2, 0x00, 0xff, 0x5e, 0x77 };
                    var getRegisterA2Value = objProtocol.CreateReadCommand(RegisterEnum.REG_BARCODE);
                    await SendCommandToBMS(getRegisterA2Value);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(300);
                    barcodeCount++;
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task ReadSOCAndHeatingMode()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // var bytes2 = new byte[] { 0xdd, 0xa5, 0x09, 0x01, 0x01, 0xff, 0xf5, 0x77 }; //BMS Read Soc Protection and Heating Mode
                // var bytes2 = new byte[] { 0xCC, 0xCB, 0x09, 0x01, 0x01, 0xff, 0xf5, 0xCF }; //BMS Read Soc Protection and Heating Mode
                var bytes2 = objProtocol.CreateCustomReadCommand(RegisterEnum.REG_HEAT_SOC_LIMITS, "01", 1);
                await SendCommandToBMS(bytes2);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task ReadH2ParameterValues()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // CC E8 03 01 FF FC CF
                var bytes2 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_E8, "03", 01);
                await SendCommandToBMS(bytes2);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task ReadDebugModeStatus()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // CC E8 03 01 FF FC CF
                var bytes2 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_DEBUG_READ, "03", 00);
                await SendCommandToBMS(bytes2);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task ReadRegulateSOCActiveStatus()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // CC E3 03 01 FF FC CF
                var bytes2 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_E3, "03", 01);
                await SendCommandToBMS(bytes2);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async Task ReadDaySinceFullCharge(string command = "days")
        {
            try
            {
                if (command == "days")
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    // CC F1 03 01 FF FC CF
                    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_SINCE_FULL, "03", 1);
                    await SendCommandToBMS(bytes);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();
                }
                else if (command == "date")
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    // CC F1 03 01 FF FC CF
                    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.DAY_SINCE_DATE, "03", 1);
                    await SendCommandToBMS(bytes);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteCalibrationAccessed()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // CC C6 03 01 FF FC CF
                var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.WRITE_CALIBRATION_ACCESS, "03", 1);
                await SendCommandToBMS(bytes);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
            }
        }
        private async Task ReadCalibrationAccessed()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // CC C6 03 01 FF FC CF
                var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.READ_CALIBRATION_ACCESS, "03", 1);
                await SendCommandToBMS(bytes);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
            }
        }
        private async Task ReadBatteryType()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // CC F9 03 00 FF FC CF
                var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.BATT_TYPE, "03", 0);
                await SendCommandToBMS(bytes);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task ReadActiveBalancer()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // CC C1 03 00 FF FC CF
                var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_V_CELL_18, "03", 1);
                await SendCommandToBMS(bytes);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task ReadBindUnbindStatus(string register = RegisterEnum.BIND_DEVICE)
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // CC C1 03 00 FF FC CF
                var bytes = objProtocol.CreateH2CustomCommand(register, "03", 0);
                await SendCommandToBMS(bytes);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task ReadEnableBluetoothAfterRestart()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // CC C1 03 00 FF FC CF
                var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.BLE_RESTART_READ, "03", 0);
                await SendCommandToBMS(bytes);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
            }
        }
        private async Task ReadBMSResetCount()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_V_CELL_02, "03", 0);
                await SendCommandToBMS(bytes);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
            }
        }
        private async Task ReadFirmwareVersion()
        {
            try
            {
                int firmwareCount = 0;
                while (firmwareCount < 3 && string.IsNullOrWhiteSpace(objReadValues.H2FirmwareVersion))
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_FIRMWARE, "03", 0);
                    await SendCommandToBMS(bytes);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();
                    firmwareCount++;
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion Read Tool and Calibration tab Values

        #region Read Reset Capacity Registers
        private async Task CalibrateCapacityStart()
        {
            try
            {
                // Cell Under Voltage
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var setCUV = objProtocol.CreateMvByte(RegisterEnum.REG_CUVP, 2.8);
                await SendCommandToBMS(setCUV);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(500);

                // Pack Under Voltage
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                byte[] setPUV;
                if (Is24VBattery)
                {
                    setPUV = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP, 21.6);
                    await SendCommandToBMS(setPUV);
                }
                else
                {
                    setPUV = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP, 10.8);
                    await SendCommandToBMS(setPUV);
                }

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(500);

                // Pack Under Voltage Release
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                byte[] setPUV_REL;
                if (Is24VBattery)
                {
                    setPUV_REL = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP_REL, 22.4);
                    await SendCommandToBMS(setPUV_REL);
                }
                else
                {
                    setPUV_REL = objProtocol.CreateMv10Byte(RegisterEnum.REG_PUVP_REL, 11.2);
                    await SendCommandToBMS(setPUV_REL);
                }

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(500);
            }
            catch (Exception ex)
            {
            }
        }
        private async Task WriteAndReadSingleFullVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValidSingleFull)
                {
                    tempCount--;

                    await SendWriteOrReadStartCommandToBMS();

                    // var setSingleFull = new byte[] { 0xdd, 0x5a, 0x12, 0x02, 0x0d, 0xac, 0xff, 0x33, 0x77 }; 3500                    
                    var setSingleFull = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_100, 3.500);
                    await SendCommandToBMS(setSingleFull);

                    await Task.Delay(100);

                    // var getRegister12Value = new byte[] { 0xdd, 0xa5, 0x12, 0x00, 0xff, 0xee, 0x77 };
                    var getRegister12Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_100);
                    await SendCommandToBMS(getRegister12Value);

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndReadSingleCutOffVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValidSingleCutOff)
                {
                    tempCount--;

                    // var setSingleCutOff = new byte[] { 0xdd, 0x5a, 0x13, 0x02, 0x0a, 0x8c, 0xff, 0x55, 0x77 }; 2700
                    var setSingleCutOff = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_0, 2.7);
                    await SendCommandToBMS(setSingleCutOff);

                    await Task.Delay(100);

                    // var getRegister13Value = new byte[] { 0xdd, 0xa5, 0x13, 0x00, 0xff, 0xed, 0x77 };
                    var getRegister13Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_0);
                    await SendCommandToBMS(getRegister13Value);

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndRead100CapacityVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValid100Capacity)
                {
                    tempCount--;

                    // var set100 = new byte[] { 0xdd, 0x5a, 0x47, 0x02, 0x0d, 0x48, 0xff, 0x62, 0x77 }; //3.400                    
                    var set100 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP100, 3.4);
                    await SendCommandToBMS(set100);

                    await Task.Delay(100);

                    //var getRegister47Value = new byte[] { 0xdd, 0xa5, 0x47, 0x00, 0xff, 0xb9, 0x77 };
                    var getRegister47Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP100);
                    await SendCommandToBMS(getRegister47Value);

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndRead90CapacityVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValid90Capacity)
                {
                    tempCount--;

                    // var set90 = new byte[] { 0xdd, 0x5a, 0x42, 0x02, 0x0d, 0x02, 0xff, 0xad, 0x77 }; //3.330
                    var set90 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_90, 3.330);
                    set90 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_90, 3.330);
                    await SendCommandToBMS(set90);

                    await Task.Delay(100);

                    // var getRegister42Value = new byte[] { 0xdd, 0xa5, 0x42, 0x00, 0xff, 0xbe, 0x77 };
                    var getRegister42Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_90);
                    await SendCommandToBMS(getRegister42Value);

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndRead80CapacityVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValid80Capacity)
                {
                    tempCount--;

                    // var set80 = new byte[] { 0xdd, 0x5a, 0x32, 0x02, 0x0d, 0x01, 0xff, 0xbe, 0x77 };  //3.329
                    var set80 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_80, 3.329);
                    set80 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_90, 3.329);
                    await SendCommandToBMS(set80);

                    await Task.Delay(100);

                    // var getRegister32Value = new byte[] { 0xdd, 0xa5, 0x32, 0x00, 0xff, 0xce, 0x77 };
                    var getRegister32Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_80);
                    await SendCommandToBMS(getRegister32Value);

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndRead70CapacityVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValid70Capacity)
                {
                    tempCount--;

                    // var set70 = new byte[] { 0xdd, 0x5a, 0x43, 0x02, 0x0d, 0x00, 0xff, 0xae, 0x77 }; //3.328
                    var set70 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_70, 3.328);
                    await SendCommandToBMS(set70);

                    await Task.Delay(100);

                    var getRegister43Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_70);
                    await SendCommandToBMS(getRegister43Value);

                    await Task.Delay(100);
                }

            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndRead60CapacityVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValid60Capacity)
                {
                    tempCount--;

                    // var set60 = new byte[] { 0xdd, 0x5a, 0x33, 0x02, 0x0c, 0xfa, 0xfe, 0xc5, 0x77 };  //3.322
                    var set60 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_60, 3.322);
                    await SendCommandToBMS(set60);

                    await Task.Delay(100);

                    //var getRegister33Value = new byte[] { 0xdd, 0xa5, 0x33, 0x00, 0xff, 0xcd, 0x77 };
                    var getRegister33Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_60);
                    await SendCommandToBMS(getRegister33Value);

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndRead50CapacityVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValid50Capacity)
                {
                    tempCount--;

                    // var set50 = new byte[] { 0xdd, 0x5a, 0x44, 0x02, 0x0c, 0xde, 0xfe, 0xd0, 0x77 }; //3.294
                    var set50 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_50, 3.294);
                    await SendCommandToBMS(set50);

                    await Task.Delay(100);

                    //var getRegister44Value = new byte[] { 0xdd, 0xa5, 0x44, 0x00, 0xff, 0xbc, 0x77 };
                    var getRegister44Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_50);
                    await SendCommandToBMS(getRegister44Value);

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndRead40CapacityVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValid40Capacity)
                {
                    tempCount--;

                    // var set40 = new byte[] { 0xdd, 0x5a, 0x34, 0x02, 0x0c, 0xdb, 0xfe, 0xe3, 0x77 }; // 3.291
                    var set40 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_40, 3.291);
                    await SendCommandToBMS(set40);

                    await Task.Delay(100);

                    //var getRegister13Value = new byte[] { 0xdd, 0xa5, 0x34, 0x00, 0xff, 0xcc, 0x77 };
                    var getRegister13Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_40);
                    await SendCommandToBMS(getRegister13Value);

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndRead30CapacityVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValid30Capacity)
                {
                    tempCount--;

                    // var set30 = new byte[] { 0xdd, 0x5a, 0x45, 0x02, 0x0c, 0xd9, 0xfe, 0xd4, 0x77 }; //3.289
                    var set30 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_30, 3.289);
                    await SendCommandToBMS(set30);

                    await Task.Delay(100);

                    //var getRegister45Value = new byte[] { 0xdd, 0xa5, 0x45, 0x00, 0xff, 0xbb, 0x77 };
                    var getRegister45Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_30);
                    await SendCommandToBMS(getRegister45Value);

                    await Task.Delay(100);
                }

            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndRead20CapacityVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValid20Capacity)
                {
                    tempCount--;

                    // var set20 = new byte[] { 0xdd, 0x5a, 0x35, 0x02, 0x0c, 0xc2, 0xfe, 0xfb, 0x77 }; //3.266
                    var set20 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_20, 3.266);
                    await SendCommandToBMS(set20);

                    await Task.Delay(100);

                    //var getRegister35Value = new byte[] { 0xdd, 0xa5, 0x35, 0x00, 0xff, 0xcb, 0x77 };
                    var getRegister35Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_20);
                    await SendCommandToBMS(getRegister35Value);

                    await Task.Delay(100);
                }

            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async Task WriteAndRead10CapacityVoltage()
        {
            try
            {
                int tempCount = 3;

                while (tempCount > 0 && tempCount <= 3 && !objCapacity.isValid10Capacity)
                {
                    tempCount--;

                    // var set10 = new byte[] { 0xdd, 0x5a, 0x46, 0x02, 0x0c, 0x8f, 0xff, 0x1d, 0x77 }; //3.215
                    var set10 = objProtocol.CreateMvByte(RegisterEnum.REG_CAP_10, 3.215);
                    await SendCommandToBMS(set10);

                    await Task.Delay(100);

                    // var getRegister46Value = new byte[] { 0xdd, 0xa5, 0x46, 0x00, 0xff, 0xba, 0x77 };
                    var getRegister46Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CAP_10);
                    await SendCommandToBMS(getRegister46Value);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }

        #endregion Read Reset Capacity Registers     

        #region Benchmark Tab Data
        private async Task WriteLogs(bool CreateNewFile = false)
        {
            try
            {
                objFileViewModelList = new List<FileViewModel>();
                List<string> fileList = new List<string>();
                if (CreateNewFile)
                {
                    System.Globalization.CultureInfo us = new System.Globalization.CultureInfo("en-US");
                    string path = FileSystem.AppDataDirectory;
                    logFilePath = System.IO.Path.Combine(path, "SFKBMSDATA/");

                    if (!File.Exists(logFilePath))
                    {
                        Directory.CreateDirectory(logFilePath);

                        if (Directory.Exists(logFilePath))
                        {
                            foreach (var file in Directory.GetFiles(logFilePath))
                            {
                                FileViewModel FileModel = new FileViewModel();
                                FileInfo sFileInfo = new FileInfo(file.ToString());
                                TimeSpan TS = DateTime.Now.Subtract(sFileInfo.LastWriteTime);
                                FileModel.FileFullPath = sFileInfo.FullName;
                                FileModel.FileName = sFileInfo.Name;
                                FileModel.TotalSeccounds = TS.TotalSeconds;
                                objFileViewModelList.Add(FileModel);
                            }

                            if (objFileViewModelList != null)
                            {
                                if (objFileViewModelList.Count >= 5)
                                {
                                    double TimeSec = 0.0;
                                    foreach (var item in objFileViewModelList)
                                    {
                                        if (item.TotalSeccounds > TimeSec)
                                        {
                                            TimeSec = item.TotalSeccounds;
                                        }
                                    }

                                    foreach (var item in objFileViewModelList)
                                    {
                                        if (item.TotalSeccounds == TimeSec)
                                        {
                                            File.Delete(item.FileFullPath);
                                        }
                                    }
                                }
                            }
                        }

                        string TestName = Convert.ToString(ChargeDischargeComboBox.SelectedValue);
                        ModelDeviceName = Regex.Replace(ModelDeviceName, "[^0-9a-zA-Z:,]+", ".");
                        string Date = Regex.Replace(DateTime.Now.ToString("yyyy-MM-dd"), "[^0-9a-zA-Z:,]+", ".");
                        string Time = Regex.Replace(DateTime.Now.ToString("HH-mm-ss"), "[^0-9a-zA-Z:,]+", ".");

                        logFilePath = System.IO.Path.Combine(logFilePath, ModelDeviceName + "_" + Date + "_" + Time + "_" + Convert.ToString(TestName.Substring(0, 1)) + ".txt");

                        if (!File.Exists(logFilePath))
                        {
                            using (StreamWriter writer = new StreamWriter(logFilePath, true))
                            {
                                if (Is24VBattery)
                                {
                                    writer.WriteLine("Time Interval(sec),Voltage, Cell 1 Volt, Cell 2 Volt, Cell 3 Volt, Cell 4 Volt,Cell 5 Volt, Cell 6 Volt, Cell 7 Volt, Cell 8 Volt,Watts, Amps,Temperature 1(BMS Temp),Temperature 2(Case Temp),Time,Unit Type,Error Type");

                                }
                                else
                                {
                                    writer.WriteLine("Time Interval(sec),Voltage, Cell 1 Volt, Cell 2 Volt, Cell 3 Volt, Cell 4 Volt,Watts, Amps,Temperature 1(BMS Temp),Temperature 2(Case Temp),Time,Unit Type,Error Type");
                                }

                                writer.Close();

                                await ShowDisplayPopup("Information", "Test has started, please turn on load.");
                            }
                        }
                    }
                    CreateNewFile = false;
                }
                else
                {
                    if (BechmarkTestRunning)
                    {
                        using (StreamWriter writer = new StreamWriter(logFilePath, true))
                        {
                            writer.WriteLine(strTextData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BechmarkTestRunning = false;
                if (ex.Message.StartsWith("access to the path") && ex.Message.Contains("denied."))
                {
                    await ShowDisplayPopup("Error", ex.Message + "\n\nPlease Allow all Permission for the storage.");
                }
                else
                {
                    await ShowDisplayPopup("Error", "Error occurred when generating log file.\n\nError: " + ex.Message);
                }
            }
        }
        private async void btnBeginTest_Clicked(object sender, EventArgs e)
        {
            try
            {
                string TestName = Convert.ToString(ChargeDischargeComboBox.SelectedValue);

                if (!BechmarkTestRunning)
                {
                    if (TestName == "Charge Test")
                    {
                        //Charging Test
                        BechmarkTestRunning = true;
                        IntervalTime = 0;
                        BenchmarkTestCount = 0;
                        StartTimer();
                        await WriteLogs(true);

                        btnBeginTest.IsEnabled = false;
                        btnAbortTest.BackgroundColor = Colors.Black;
                        btnAbortTest.TextColor = Colors.White;

                        CargeDisChargeBenchmarktest();
                    }
                    else if (TestName == "Discharge Test")
                    {
                        //DisCharging Test

                        if (!ProceedForBanchmarkTest)
                        {
                            if (UsedNewFirmware)
                            {
                                if (Is24VBattery)
                                {
                                    await ShowDisplayPopup("ComfirmationPopUp", "Benchmark requires a fully charged battery where each cell is 3.4v or higher, the BMS must have the LVC set to 20.0 volts And also Max state of charge must be 100% and Min must be 0%. \n\nIt will take approximately 5 - 7 hours to complete and the app will need to remain running. A tablet device is recommended that is connected to a power source. You will need a load between 400 - 800 watt to complete the benchmark. \n\nOnce you have verified the prerequisites, click continue, and then turn on the load to begin recording your results. \n\nThe test will end if watts exceed 1200 and will need to be restarted.");
                                }
                                else
                                {
                                    await ShowDisplayPopup("ComfirmationPopUp", "Benchmark requires a fully charged battery where each cell is 3.4v or higher, the BMS must have the LVC set to 10.0 volts And also Max state of charge must be 100% and Min must be 0%. \n\nIt will take approximately 5 - 7 hours to complete and the app will need to remain running. A tablet device is recommended that is connected to a power source. You will need a load between 400 - 800 watt to complete the benchmark. \n\nOnce you have verified the prerequisites, click continue, and then turn on the load to begin recording your results. \n\nThe test will end if watts exceed 1200 and will need to be restarted.");
                                }
                            }
                            else
                            {
                                if (Is24VBattery)
                                {
                                    await ShowDisplayPopup("ComfirmationPopUp", "Benchmark requires a fully charged battery where each cell is 3.4v or higher, the BMS must have the LVC set to 20.0 volts. \n\nIt will take approximately 5 - 7 hours to complete and the app will need to remain running. A tablet device is recommended that is connected to a power source. You will need a load between 400 - 800 watt to complete the benchmark. \n\nOnce you have verified the prerequisites, click continue, and then turn on the load to begin recording your results. \n\nThe test will end if watts exceed 1200 and will need to be restarted.");
                                }
                                else
                                {
                                    await ShowDisplayPopup("ComfirmationPopUp", "Benchmark requires a fully charged battery where each cell is 3.4v or higher, the BMS must have the LVC set to 10.0 volts. \n\nIt will take approximately 5 - 7 hours to complete and the app will need to remain running. A tablet device is recommended that is connected to a power source. You will need a load between 400 - 800 watt to complete the benchmark. \n\nOnce you have verified the prerequisites, click continue, and then turn on the load to begin recording your results. \n\nThe test will end if watts exceed 1200 and will need to be restarted.");
                                }
                            }
                        }

                        if (ProceedForBanchmarkTest)
                        {
                            if (objReadValues.MinSOCReadValue == 10)
                            {
                                if ((objReadValues.SocMaxValue == 100 && objReadValues.SocMinValue == 0) || !UsedNewFirmware)
                                {
                                    BechmarkTestRunning = true;
                                    IntervalTime = 0;
                                    BenchmarkTestCount = 0;
                                    StartTimer();
                                    await WriteLogs(true);


                                    btnBeginTest.IsEnabled = false;
                                    btnAbortTest.BackgroundColor = Colors.Black;
                                    btnAbortTest.TextColor = Colors.White;

                                    CargeDisChargeBenchmarktest();
                                }
                                else
                                {
                                    await ShowDisplayPopup("Error", "Please make sure Max state of charge must be 100% and Min must be 0%");
                                }
                            }
                            else
                            {
                                if (Is24VBattery)
                                {
                                    await ShowDisplayPopup("Error", "Please make sure the Low Voltage Cut-off is set to 20.0v");
                                }
                                else
                                {
                                    await ShowDisplayPopup("Error", "Please make sure the Low Voltage Cut-off is set to 10.0v");
                                }
                            }
                            ProceedForBanchmarkTest = false;
                        }
                    }
                }
                else
                {
                    await ShowDisplayPopup("Information", "Test is already in progress.");
                }
            }
            catch { }
        }
        private async void btnAbortTest_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (BechmarkTestRunning)
                {

                    BechmarkTestRunning = false;
                    logFilePath = string.Empty;
                    BenchmarkTestCount = 0;
                    objbleDevice.Total_AmpHours = 0;
                    objbleDevice.Total_WattHours = 0;
                    btnAbortTest.BackgroundColor = Color.FromHex("#e3e3e3");
                    btnAbortTest.TextColor = Colors.Gray;
                    btnBeginTest.IsEnabled = true;

                    await ShowAbsoluteLayout(false);

                    await ShowDisplayPopup("Information", "Test has been aborted.");

                    await ShowAllFiles();
                }
            }
            catch { }
        }
        private async void btnUploadResult_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                var menuItem = sender as ImageButton;
                string FileName = menuItem.CommandParameter as string;



                System.Globalization.CultureInfo us = new System.Globalization.CultureInfo("en-US");
                string path = FileSystem.AppDataDirectory;
                path = System.IO.Path.Combine(path, "SFKBMSDATA/");
                string FullFilePath = System.IO.Path.Combine(path, FileName); ;

                if (logFilePath != FullFilePath)
                {
                    byte[] data = File.ReadAllBytes(FullFilePath);
                    MemoryStream mstream = new MemoryStream(data);

                    if (mstream != null)
                    {
                        MultipartFormDataContent objMultipartFormDataContent = new MultipartFormDataContent();
                        objMultipartFormDataContent.Add(new StreamContent(mstream), "\"file\"", FileName);

                        string sResult = string.Empty;
                        try
                        {
                            HttpClient client = new HttpClient();
                            string SiteURL = "https://www.sunfunkits.com/app/UploadBenchmarkCSVFile";
                            client.DefaultRequestHeaders.Add("Mac_id", Convert.ToString(lblMacAddressID.Text));

                            string IpAddress = Convert.ToString(Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault());

                            client.DefaultRequestHeaders.Add("Device_IpAddress", IpAddress);
                            var httpResponseMessage = await client.PostAsync(SiteURL, objMultipartFormDataContent);

                            sResult = await httpResponseMessage.Content.ReadAsStringAsync();

                            sResult = sResult.Remove(0, (sResult.IndexOf('>') + 1));
                        }
                        catch (HttpRequestException ex)
                        {
                            await ShowBusyindicatior(false);
                            await ShowDisplayPopup("Error", "Unable to connect to the server. Please check your internet connection.");
                            var i = (ImageButton)sender;
                            string CurrentImageSource = i.Source.ToString();
                            if (CurrentImageSource.Contains("upload.png") == true)
                            {
                                i.Source = "reupload.png";
                            }
                            else
                            {
                                i.Source = "reupload.png";
                            }
                        }
                        catch (Exception ex)
                        {
                            await ShowBusyindicatior(false);
                            await ShowDisplayPopup("Error", "An unexpected error occurred. Please try again later or Contact sunfunkits sales.");
                            var i = (ImageButton)sender;
                            string CurrentImageSource = i.Source.ToString();
                            if (CurrentImageSource.Contains("upload.png") == true)
                            {
                                i.Source = "reupload.png";
                            }
                            else
                            {
                                i.Source = "reupload.png";
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(sResult))
                        {
                            if (sResult.Contains("SUCCESS"))
                            {
                                var Result = sResult.Split('|');
                                string URL = Result[1];
                                Preferences.Set(FileName, URL);
                                await ShowAllFiles();
                                await ShowBusyindicatior(false);
                                await ShowDisplayPopup("Information", "Test submitted successfully.");
                            }
                            else
                            {
                                await ShowBusyindicatior(false);
                                if (sResult.Contains("EXERROR"))
                                {
                                    await ShowDisplayPopup("Error", "An error occurred, test results were not uploaded");

                                    var i = (ImageButton)sender;
                                    string CurrentImageSource = i.Source.ToString();
                                    if (CurrentImageSource.Contains("upload.png") == true)
                                    {
                                        i.Source = "reupload.png";
                                    }
                                    else
                                    {
                                        i.Source = "reupload.png";
                                    }
                                }
                                else
                                {
                                    await ShowDisplayPopup("Error", sResult.ToString());
                                }

                            }
                        }
                    }
                }
                else
                {
                    await ShowDisplayPopup("Information", "You can only upload this file after Benchmark Test is Completed.");
                }
            }
            catch (Exception ex)
            {
                await ShowBusyindicatior(false);
                await ShowDisplayPopup("Error", ex.Message.ToString());
            }
        }
        private async void btnViewResult_Clicked(object sender, EventArgs e)
        {
            try
            {
                var menuItem = sender as ImageButton;
                string URL = menuItem.CommandParameter as string;

                if (!string.IsNullOrWhiteSpace(URL))
                {
                    await Launcher.OpenAsync(new Uri(URL));
                }
            }
            catch (Exception ex)
            { }
        }
        private async void btnChangeName_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(lblMacAddressID.Text))
                {
                    var keyboardService = App.Services.GetService<IKeyboardService>();
                    keyboardService?.HideKeyboard();
                    Preferences.Set(lblMacAddressID.Text, txtNickName.Text);
                    await ShowDisplayPopup("Information", "Nick Name Set Successfully.");
                    await ShowNickNameText();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnDeleteFile_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                var menuItem = sender as ImageButton;
                string FileName = menuItem.CommandParameter as string;

                System.Globalization.CultureInfo us = new System.Globalization.CultureInfo("en-US");
                string path = FileSystem.AppDataDirectory;
                path = System.IO.Path.Combine(path, "SFKBMSDATA/");
                string FullFilePath = System.IO.Path.Combine(path, FileName);

                if (logFilePath != FullFilePath)
                {

                    System.IO.File.Delete(FullFilePath);

                    await ShowAllFiles();

                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Information", "File Deleted Successfully.");
                }
                else
                {
                    await ShowDisplayPopup("Information", "You can only delete this file after Benchmark Test is Completed.");
                }
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", ex.Message);
            }
        }
        private async Task ShowNickNameText()
        {
            try
            {
                var temp = Preferences.Get(lblMacAddressID.Text.ToString(), string.Empty);
                if (!string.IsNullOrWhiteSpace(temp))
                {
                    txtNickName.Text = temp.ToString();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void StartTimer()
        {
            try
            {
                int hours = 0;
                int min = 0;
                int sec = 0;

                decimal pmin = 0;
                decimal psec = 0;

                while (BechmarkTestRunning)
                {
                    sec++;
                    psec = psec + 0.2M;

                    if (sec >= 60)
                    {
                        min++;
                        pmin = pmin + 0.2M;

                        sec = 0;
                        psec = 0;
                        if (min >= 60)
                        {
                            hours++;
                            min = 0;
                            pmin = 0;

                            if (hours > 12)
                            {
                                //pointerHour.Value = 0;
                            }
                        }
                    }
                    string time = Convert.ToString(hours).PadLeft(2, '0') + ":" + Convert.ToString(min).PadLeft(2, '0') + ":" + Convert.ToString(sec).PadLeft(2, '0');
                    lblTimer.Text = time;
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void CargeDisChargeBenchmarktest()
        {
            try
            {
                DateTime dPreviousDateTime = DateTime.Now;
                DateTime dDateTime = DateTime.Now;
                TimeSpan diffResult = new TimeSpan();
                bool bproceed = false;
                string UnitType = string.Empty;
                if (IsFahrenheit)
                {
                    UnitType = "F";
                }
                else
                {
                    UnitType = "C";
                }
                while (BechmarkTestRunning)
                {
                    dDateTime = DateTime.Now;
                    int Interval = PullingRequestTime / 1000;

                    diffResult = dDateTime.Subtract(dPreviousDateTime);

                    if (Convert.ToInt32(diffResult.Seconds) >= Interval)
                    {
                        if ((objbleDevice.Amps >= 1 || objbleDevice.Amps <= -1) && objbleDevice.Watts >= 0)
                        {
                            if (objbleDevice.Watts < 1200)
                            {
                                objbleDevice.Amps = (objbleDevice.Amps * objDeviceInformation.MultiplicationConstant);
                                objbleDevice.Watts = (objbleDevice.Watts * objDeviceInformation.MultiplicationConstant);

                                objbleDevice.Amp_Hours = objbleDevice.Amps / 1200;
                                objbleDevice.Watt_Hours = objbleDevice.Watts / 1200;

                                //if (Interval == 3)
                                //{
                                //    objbleDevice.Amp_Hours = objbleDevice.Amps / 1200;
                                //    objbleDevice.Watt_Hours = objbleDevice.Watts / 1200;
                                //}
                                //else if (Interval == 5)
                                //{
                                //    objbleDevice.Amp_Hours = objbleDevice.Amps / 720;
                                //    objbleDevice.Watt_Hours = objbleDevice.Watts / 720;
                                //}
                                //else if (Interval == 8)
                                //{
                                //    objbleDevice.Amp_Hours = objbleDevice.Amps / 450;
                                //    objbleDevice.Watt_Hours = objbleDevice.Watts / 450;
                                //}
                                if (objbleDevice.Amps <= -1)
                                {
                                    objbleDevice.Amp_Hours = objbleDevice.Amp_Hours * -1;
                                }

                                if (objbleDevice.Watts <= -1)
                                {
                                    objbleDevice.Watt_Hours = objbleDevice.Watt_Hours * -1;
                                }

                                //Total-Amp-Hours
                                objbleDevice.Total_AmpHours += objbleDevice.Amp_Hours;
                                objbleDevice.Total_AmpHours = Math.Round(objbleDevice.Total_AmpHours, 5);

                                //Total-Watt-Hours
                                objbleDevice.Total_WattHours += objbleDevice.Watt_Hours;
                                objbleDevice.Total_WattHours = Math.Round(objbleDevice.Total_WattHours, 5);

                                objbleDevice.Amp_Hours = Math.Round(objbleDevice.Amp_Hours, 5);
                                objbleDevice.Watt_Hours = Math.Round(objbleDevice.Watt_Hours, 5);

                                strTextData = IntervalTime + "," + objbleDevice.Voltes + "," + objbleDevice.Cell1volt + "," + objbleDevice.Cell2volt + "," + objbleDevice.Cell3volt + "," + objbleDevice.Cell4volt + "," + objbleDevice.Cell5volt + "," + objbleDevice.Cell6volt + "," + objbleDevice.Cell7volt + "," + objbleDevice.Cell8volt + "," + objbleDevice.Watts + "," + objbleDevice.Amps + "," + objbleDevice.BMSTempreture + "," + objbleDevice.CaseTempreture + "," + Convert.ToString(dDateTime.ToString("MM/dd/yyyy hh:mm:ss tt").Replace('-', '/')) + "," + UnitType + ",N";
                                await WriteLogs();
                                IntervalTime += Interval;
                                BenchmarkTestCount = 0;
                            }
                            else
                            {
                                BechmarkTestRunning = false;
                                btnBeginTest.IsEnabled = true;
                                logFilePath = string.Empty;
                                await ShowAbsoluteLayout(false);

                                await ShowAllFiles();

                                await ShowDisplayPopup("Information", "Test has Ended.\nwatts are too high, decrease battery load and try again keeping watts between 200-800 with a maximum of 1200.");
                            }
                        }
                        else
                        {
                            if (BenchmarkTestCount <= 20)
                            {
                                strTextData = IntervalTime + "," + objbleDevice.Voltes + "," + objbleDevice.Cell1volt + "," + objbleDevice.Cell2volt + "," + objbleDevice.Cell3volt + "," + objbleDevice.Cell4volt + "," + objbleDevice.Cell5volt + "," + objbleDevice.Cell6volt + "," + objbleDevice.Cell7volt + "," + objbleDevice.Cell8volt + "," + objbleDevice.Watts + "," + objbleDevice.Amps + "," + objbleDevice.BMSTempreture + "," + objbleDevice.CaseTempreture + "," + Convert.ToString(dDateTime.ToString("MM/dd/yyyy hh:mm:ss tt").Replace('-', '/')) + "," + UnitType + ",R";
                                await WriteLogs();
                                IntervalTime += Interval;
                                BenchmarkTestCount++;
                            }
                            else
                            {
                                BechmarkTestRunning = false;
                                btnBeginTest.IsEnabled = true;
                                logFilePath = string.Empty;
                                await ShowAbsoluteLayout(false);

                                await ShowAllFiles();

                                await ShowDisplayPopup("Information", "Test has been completed, no amps or watts detected.");
                            }
                        }
                    }
                    else
                    {
                        if (bproceed)
                        {
                            strTextData = IntervalTime + "," + objbleDevice.Voltes + "," + objbleDevice.Cell1volt + "," + objbleDevice.Cell2volt + "," + objbleDevice.Cell3volt + "," + objbleDevice.Cell4volt + "," + objbleDevice.Cell5volt + "," + objbleDevice.Cell6volt + "," + objbleDevice.Cell7volt + "," + objbleDevice.Cell8volt + "," + objbleDevice.Watts + "," + objbleDevice.Amps + "," + objbleDevice.BMSTempreture + "," + objbleDevice.CaseTempreture + "," + Convert.ToString(dDateTime.ToString("MM/dd/yyyy hh:mm:ss tt").Replace('-', '/')) + "," + UnitType + ",T";
                            await WriteLogs();
                            IntervalTime += Interval;
                            BenchmarkTestCount++;
                        }
                        bproceed = true;
                    }

                    dPreviousDateTime = dDateTime;
                    decimal Voltes = Math.Round(objbleDevice.Voltes, 2);
                    if (Is24VBattery)
                    {
                        if (Voltes > 27M)
                        {
                            lblBenchmarkVolts.TextColor = Colors.Green;
                        }
                        else if (Voltes <= 27 && Voltes > 26M)
                        {
                            lblBenchmarkVolts.TextColor = Colors.Yellow;
                        }
                        else if (Voltes <= 26 && Voltes > 25M)
                        {
                            lblBenchmarkVolts.TextColor = Colors.Orange;
                        }
                        else
                        {
                            lblBenchmarkVolts.TextColor = Colors.Red;
                        }
                    }
                    else
                    {
                        if (Voltes > 13M)
                        {
                            lblBenchmarkVolts.TextColor = Colors.Green;
                        }
                        else if (Voltes <= 13 && Voltes > 12M)
                        {
                            lblBenchmarkVolts.TextColor = Colors.Yellow;
                        }
                        else if (Voltes <= 12 && Voltes > 11M)
                        {
                            lblBenchmarkVolts.TextColor = Colors.Orange;
                        }
                        else
                        {
                            lblBenchmarkVolts.TextColor = Colors.Red;
                        }
                    }

                    lblBenchmarkVolts.Text = Convert.ToString(Math.Round(objbleDevice.Voltes, 2));
                    lblBenchmarkAmps.Text = Convert.ToString(Math.Round(objbleDevice.Amps, 2));

                    if (Math.Round(objbleDevice.Amps, 2) > 70)
                    {
                        lblBenchmarkAmps.TextColor = Colors.Red;
                    }
                    else
                    {
                        lblBenchmarkAmps.TextColor = Colors.Black;
                    }

                    if (Math.Round(objbleDevice.Watts, 2) > 800)
                    {
                        lblBenchmarkWatts.TextColor = Colors.Red;
                    }
                    else
                    {
                        lblBenchmarkWatts.TextColor = Colors.Black;
                    }


                    lblBenchmarkWatts.Text = Convert.ToString(Math.Round(objbleDevice.Watts, 2));
                    lblBenchmarkAmpHours.Text = Convert.ToString(Math.Round(objbleDevice.Total_AmpHours, 2));
                    lblBenchmarkWattHours.Text = Convert.ToString(Math.Round(objbleDevice.Total_WattHours, 2));

                    await Task.Delay(PullingRequestTime);

                    if (!BechmarkTestRunning)
                    {
                        objbleDevice.Total_AmpHours = 0;
                        objbleDevice.Total_WattHours = 0;
                        BenchmarkTestCount = 0;
                        btnAbortTest.BackgroundColor = Color.FromHex("#e3e3e3");
                        btnAbortTest.TextColor = Colors.Gray;
                        btnBeginTest.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task ShowAllFiles()
        {
            try
            {
                lstBenchMarkFilesList.ItemsSource = null;
                objFileViewModelList = new List<FileViewModel>();
                System.Globalization.CultureInfo us = new System.Globalization.CultureInfo("en-US");
                string path = FileSystem.AppDataDirectory;
                string FilePath = System.IO.Path.Combine(path, "SFKBMSDATA/");

                if (!File.Exists(FilePath))
                {
                    if (Directory.Exists(FilePath))
                    {
                        foreach (var file in Directory.GetFiles(FilePath))
                        {
                            FileViewModel FileModel = new FileViewModel();
                            FileInfo sFileInfo = new FileInfo(file.ToString());

                            sFileInfo.MoveTo(FilePath + sFileInfo.Name);
                        }

                        int TestID = 1;
                        foreach (var file in Directory.GetFiles(FilePath))
                        {
                            FileViewModel FileModel = new FileViewModel();
                            FileInfo sFileInfo = new FileInfo(file.ToString());
                            TimeSpan TS = DateTime.Now.Subtract(sFileInfo.LastWriteTime);
                            FileModel.FileFullPath = sFileInfo.FullName;
                            FileModel.FileName = sFileInfo.Name;
                            FileModel.TotalSeccounds = TS.TotalSeconds;

                            string URL = Preferences.Get(FileModel.FileName, string.Empty);
                            if (!string.IsNullOrWhiteSpace(URL))
                            {
                                FileModel.URL = URL;
                                FileModel.UploadResultIsVisibled = false;
                                FileModel.ViewResultIsVisibled = true;
                            }
                            else
                            {
                                FileModel.UploadResultIsVisibled = true;
                                FileModel.ViewResultIsVisibled = false;
                            }

                            objFileViewModelList.Add(FileModel);
                        }
                        if (objFileViewModelList != null && objFileViewModelList.Count > 0)
                        {
                            objFileViewModelList = objFileViewModelList.OrderBy(m => m.TotalSeccounds).ToList();

                            foreach (var item in objFileViewModelList)
                            {
                                string[] Name = item.FileName.Split('_');
                                Name[3] = Name[3].Replace(".txt", "");
                                if (!string.IsNullOrWhiteSpace(Name[3]))
                                {
                                    if (Name[3] == "S")
                                    {
                                        Name[3] = "Standby";
                                        item.TestTypeColor = Colors.Black;
                                    }
                                    else if (Name[3] == "C")
                                    {
                                        Name[3] = "Charging";
                                        item.TestTypeColor = Colors.Green;
                                    }
                                    else if (Name[3] == "D")
                                    {
                                        Name[3] = "Discharging";
                                        item.TestTypeColor = Colors.Red;
                                    }
                                    else if (Name[3] == "H")
                                    {
                                        Name[3] = "High";
                                        item.TestTypeColor = Colors.Black;
                                    }
                                    else
                                    {
                                        item.TestTypeColor = Colors.Black;
                                    }
                                }

                                item.DisplayFileName = Name[0].Replace('.', ' ');
                                item.DateTime = "\n" + Name[1].Replace('.', '/') + " " + Name[2].Replace('.', ':');
                                item.TestType = "\n" + Name[3];
                                item.TestID = TestID;
                                TestID++;
                            }

                            lstBenchMarkFilesList.ItemsSource = objFileViewModelList.ToArray();

                            #region Benchmark Tab

                            if (Device.Idiom == TargetIdiom.Phone)
                            {
                                stackBenchMarkFilesList.HeightRequest = lstBenchMarkFilesList.HeightRequest = ((objFileViewModelList.Count + 1) * 85);
                            }
                            else if (Device.Idiom == TargetIdiom.Tablet)
                            {
                                stackBenchMarkFilesList.HeightRequest = lstBenchMarkFilesList.HeightRequest = ((objFileViewModelList.Count + 1) * 85);
                            }

                            #endregion Benchmark Tab

                            lstBenchMarkFilesList.IsVisible = true;
                            lstBenchMarkFilesList.IsEnabled = true;
                            lblNoFileFound.IsVisible = false;
                        }
                        else
                        {
                            lstBenchMarkFilesList.IsVisible = false;
                            lstBenchMarkFilesList.IsEnabled = false;
                            lblNoFileFound.IsVisible = true;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        private async void imgbtnBenchmarkRefresh_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowAllFiles();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion Benchmark Tab Data

        #region BusyIndecator And Reconnect Functionality
        private async Task ReEstablishConnection(bool ProccedforReconnect)
        {
            try
            {
                if (ProccedforReconnect)
                {
                    RunMainPageWhileLoop = false;
                    isProccesBegin = true;
                    ChecksumFails = 0;

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        IsBusyIndicatorForReconnecing.IsVisible = true;
                        stackBusy.IsVisible = true;

                        if (_connectedDevice != null)
                        {
                            IsBusyIndicatorForReconnecingLable.Text = "Attempting to reconnect: '" + _connectedDevice.DeviceName.Trim() + "'";
                        }
                        else
                        {
                            IsBusyIndicatorForReconnecingLable.Text = "Attempting to reconnect";
                        }
                    });

                    if (IsReconnectFailed)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            IsBusyIndicatorForReconnecing.IsVisible = false;
                            stackBusy.IsVisible = false;
                        });

                        await ShowDisplayPopup("Information", "Unable to connect battery.");

                        await Task.Delay(2000);

                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await Navigation.PushAsync(new MainPage(false));
                        });
                    }
                }
                else
                {
                    RunMainPageWhileLoop = true;
                    isProccesBegin = false;
                    nReadCounter = 0;
                    IsReconnectFailed = false;

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        IsBusyIndicatorForReconnecingLable.Text = "Re-connection successful";
                        IsBusyIndicatorForReconnecing.IsVisible = false;
                        stackBusy.IsVisible = false;
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private async Task StopAllServices()
        {
            try
            {
            }
            catch (Exception ex) { }
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
                        stackbIsBusyIndicator.IsVisible = sfbIsBusyIndicator.IsVisible = objBusyIndicator.IsVisible;
                        stackbIsBusyIndicator.BackgroundColor = Colors.Transparent;
                        stackbIsBusyIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;
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
                        lblBusyIndicator.Text = Command;
                        lblBusyIndicator.TextColor = Color.FromHex("#f0c000");
                        stackbIsBusyIndicator.IsVisible = sfbIsBusyIndicator.IsVisible = objBusyIndicator.IsVisible;
                        stackbIsBusyIndicator.BackgroundColor = Colors.Transparent;
                        stackbIsBusyIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;
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
        private async Task ShowAbsoluteLayout(bool bShowIndicator, string message = "", bool isToolTab = false)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    objBusyIndicator = new BusyIndicatorViewModel();

                    objBusyIndicator.IsBusy = objBusyIndicator.IsVisible = bShowIndicator;
                    objBusyIndicator.Message = message;

                    IsBusyIndicator.HorizontalOptions = IsBusyIndicator.VerticalOptions = LayoutOptions.FillAndExpand;
                    sfbIsBusyIndicator.HorizontalOptions = sfbIsBusyIndicator.VerticalOptions = LayoutOptions.FillAndExpand;

                    lblBusyIndicator.TextColor = Color.FromHex("#000");

                    if (isToolTab)
                    {
                        IsBusyIndicator.Margin = new Thickness(0, 0, 0, 0);

                        IsBusyIndicator.BindingContext = objBusyIndicator;
                        IsBusyIndicator.IsEnabled = objBusyIndicator.IsBusy;
                        lblBusyIndicator.Text = "Reading BMS settings, please wait...";
                        stackbIsBusyIndicator.IsVisible = sfbIsBusyIndicator.IsVisible = objBusyIndicator.IsVisible;
                        sfbIsBusyIndicator.BackgroundColor = Colors.Transparent;
                        stackbIsBusyIndicator.BackgroundColor = Colors.White;
                        stackbIsBusyIndicator.VerticalOptions = LayoutOptions.Start;
                        stackbIsBusyIndicator.HeightRequest = DeviceHeight - SKFTabView.TabBarHeight; ;
                        stackbIsBusyIndicator.WidthRequest = DeviceWidth;
                        sfbIsBusyIndicator.HeightRequest = 260;
                        sfbIsBusyIndicator.WidthRequest = DeviceWidth;
                    }
                    else
                    {
                        IsBusyIndicator.Margin = new Thickness(0, 30, 0, 0);

                        IsBusyIndicatorForReconnecing.IsVisible = bShowIndicator;
                        IsBusyIndicatorForReconnecingLable.Text = message;
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
                stkDisplayMessagePopUpBusy.IsVisible = true;
                lblText.TextColor = Color.FromHex("#2D2D2D");
                stackCancelAndYesButton.IsVisible = false;
                btnOK.IsVisible = true;
                btnOK.BackgroundColor = Color.FromHex("#39B54A");
                btnOK.StrokeThickness = 0;
                lblText.HorizontalTextAlignment = TextAlignment.Center;
                lblOK.TextColor = Color.FromHex("#FFF");
                imgOK.Source = "yes.png";
                FaultReleasePopup = false;
                stackProceedAndCaneclButton.IsVisible = false;
                lblYes.Text = "Proceed";
                lblCancel.Text = "Cancel";
                PopUpStatus = Status;
                btnCancle.IsVisible = false;

                if (!Message.StartsWith("Unable to connect battery"))
                {
                    string data = Status + '^' + Message;
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ShowDisplayPopup, data);
                }

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
                    stackCancelAndYesButton.IsVisible = true;
                    btnOK.IsVisible = false;
                    lblYes.Text = "Proceed";
                }
                else if (Status == "ComfirmationPopUpBatteryType")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    lblText.HorizontalTextAlignment = TextAlignment.Start;
                    stackCancelAndYesButton.IsVisible = true;
                    btnOK.IsVisible = false;
                    lblYes.Text = "Proceed";
                    CurrentPopup = Status;
                }
                else if (Status == "RestartESP32")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    lblText.HorizontalTextAlignment = TextAlignment.Start;
                    stackCancelAndYesButton.IsVisible = true;
                    btnOK.IsVisible = false;
                    lblYes.Text = "Proceed";
                    lblCancel.Text = "Cancel";
                    CurrentPopup = Status;
                }
                else if (Status == "LoadDefaultValue")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    lblText.HorizontalTextAlignment = TextAlignment.Start;
                    stackCancelAndYesButton.IsVisible = true;
                    btnOK.IsVisible = false;
                    lblYes.Text = "Proceed";
                    CurrentPopup = Status;
                }
                else if (Status == "ComfirmationPopUpDisableHeating")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    lblText.HorizontalTextAlignment = TextAlignment.Start;
                    stackCancelAndYesButton.IsVisible = true;
                    btnOK.IsVisible = false;
                    lblYes.Text = "Proceed";
                    CurrentPopup = Status;
                }
                else if (Status == "ComfirmationPopUpResetCapacity")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    stackCancelAndYesButton.IsVisible = true;
                    btnOK.IsVisible = false;
                    lblYes.Text = "Proceed";
                }
                else if (Status == "ComfirmationPopUpRTCSync")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    stackCancelAndYesButton.IsVisible = true;
                    btnOK.IsVisible = false;
                    lblYes.Text = "Proceed";
                    lblCancel.Text = "Cancel";
                    CurrentPopup = Status;
                }
                else if (Status == "Alert")
                {
                    imgDisplay.Source = "disconnectbattery.png";
                    stackCancelAndYesButton.IsVisible = true;
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
                    stackCancelAndYesButton.IsVisible = true;
                    lblText.Text = Message;
                    lblYes.Text = "Proceed";
                    btnOK.IsVisible = false;
                    FaultReleasePopup = true;
                }
                else if (Status == "Firwware Warning")
                {
                    imgDisplay.Source = "information.png";
                    lblText.Text = Message;
                    stackProceedAndCaneclButton.IsVisible = true;
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
                    stackCancelAndYesButton.IsVisible = false;
                    btnOK.IsVisible = false;
                    btnCancle.IsVisible = true;
                    CurrentPopup = Status;
                }
                else if (Status == "ComfirmationPopUpChargePulse")
                {
                    imgDisplay.Source = "comfirmationpopup.png";
                    lblText.Text = Message;
                    lblText.HorizontalTextAlignment = TextAlignment.Start;
                    stackCancelAndYesButton.IsVisible = true;
                    btnOK.IsVisible = false;
                    lblYes.Text = "Proceed";
                    CurrentPopup = Status;
                }

                await ShowDisplayPopupDesign(DeviceWidth, DeviceHeight);
            }
            catch (Exception ex)
            {

            }
        }
        private async void DisplayPopupProceed_Clicked(object sender, EventArgs e)
        {
            stkDisplayMessagePopUp.IsVisible = false;
            stkDisplayMessagePopUpBusy.IsVisible = false;


            if (ProceedForFirmwareUpgradeForH2)
            {
                ProceedForFirmwareUpgradeForH2 = false;
                btnUpgradeH2FirmwareByBluetooth();
            }

            string data = "PopupProceed_Clicked";
            await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DisplayPopup_Clicked, data);
        }
        private async void DisplayPopupCancel_Clicked(object sender, EventArgs e)
        {
            if (CurrentPopup == "BmsMismatch")
            {
                MainPage Page = new MainPage(false);
                await Navigation.PushAsync(Page);
            }
            else
            {
                stkDisplayMessagePopUp.IsVisible = false;
                stkDisplayMessagePopUpBusy.IsVisible = false;
                ProceedForFirmwareUpgradeForH2 = false;
            }

            string data = "PopupCancel_Clicked";
            await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DisplayPopup_Clicked, data);
        }
        private async void DisplayPopup_Clicked(object sender, EventArgs e)
        {
            stkDisplayMessagePopUp.IsVisible = false;
            stkDisplayMessagePopUpBusy.IsVisible = false;

            string data = "Popup_Clicked";
            await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DisplayPopup_Clicked, data);

            if (bFirmwareUptodate)
            {
                bFirmwareUptodate = false;
                return;
            }

            if (CurrentPopup == "ComfirmationPopUpRTCSync")
            {
                TimeSyncronized = true;
                return;
            }

            if (FaultReleasePopup)
            {
                ProceedForFaultReleaseUpdate = false;
                FaultRelease_Update();
            }

            if (IsScantabSelect)
            {
                SKFTabView.SelectedIndex = 1;
                IsScantabSelect = false;
            }

            if (RemoteSessionDisplayMessageOK)
            {
                await ShowBusyindicatior(true, "Please wait..");
                MainPage Page = new MainPage(false);
                await Navigation.PushAsync(Page);
            }

            if (H2FirmwareUpgradeCompleted)
            {
                try
                {
                    RunMainPageWhileLoop = false;
                    BechmarkTestRunning = false;
                    IsScantabSelect = false;
                    BreakWhileLoop = true;
                    await StopAllServices();

                    await ShowBusyindicatior(true);
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DiscounnectDevice, _connectedDevice.MacAddress);
                    H2FirmwareUpgradeCompleted = false;
                }
                catch (Exception ex) { H2FirmwareUpgradeCompleted = false; }
                MainPage Page = new MainPage(false);
                await Navigation.PushAsync(Page);
            }
            CurrentPopup = string.Empty;
        }
        private async void DisplayPopupYes_Clicked(object sender, EventArgs e)
        {
            stkDisplayMessagePopUp.IsVisible = false;
            stkDisplayMessagePopUpBusy.IsVisible = false;
            await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DisplayPopup_Clicked, "DisplayPopupYes_Clicked");

            if (bFirmwareUptodate)
            {
                bFirmwareUptodate = false;
                if (FirmwareType == "SFK_H2")
                {
                    btnClickToProceedForH2_Clicked(null, null);
                }
                //if (FirmwareType == "Nuvoton_100SBN")
                //{
                //    btnUpgrade_Clicked(null, null);
                //}
                return;
            }

            if (CurrentPopup == "LoadDefaultValue")
            {
                bRestoreDefaultValue = true;
                btnLoadDefaultValues_Clicked(null, null);
            }

            if (CurrentPopup == "RestartESP32")
            {
                await RestartESP32();
            }

            if (CurrentPopup == "ComfirmationPopUpChargePulse")
            {
                int NewSOCMaxValue = (int)NF_rangesliderMaxSOC.Value;
                int NewSOCMinValue = (int)NF_rangesliderMinSOC.Value;
                bProceedMaxMinSOC = true;

                await ChargePulseUpdate();

                await Task.Delay(100);

                await MinMaxSOCUpdate(NewSOCMaxValue, NewSOCMinValue);
            }

            if (CurrentPopup == "ComfirmationPopUpRTCSync")
            {
                countRTCUpdateTime = 0;
                await WriteRTCTimeInterval();
            }

            if (!ProceedForHeatingMode && !IsScantabSelect && CurrentPopup == "ComfirmationPopUpDisableHeating")
            {
                CurrentPopup = string.Empty;
                ProceedForHeatingMode = true;
                NF_btnHeatingModeSave_Clicked(null, null);
            }

            if (!ProceedForCalibrateCapacity && !IsScantabSelect && ProceedForBanchmarkTest)
            {
                ProceedForCalibrateCapacity = true;
                btnCalibrateCapacity_Clicked(null, null);
            }

            if (!ProceedForBanchmarkTest && !IsScantabSelect && IsBenchmarktabSelect)
            {
                ProceedForBanchmarkTest = true;
                btnBeginTest_Clicked(null, null);
            }

            if (!ProceedForFaultReleaseUpdate && FaultReleasePopup)
            {
                ProceedForFaultReleaseUpdate = true;
                FaultRelease_Update();
            }

            if (IsScantabSelect)
            {
                RunMainPageWhileLoop = false;
                BechmarkTestRunning = false;
                IsScantabSelect = false;
                BreakWhileLoop = true;

                await StopAllServices();

                await ShowBusyindicatior(true);

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DiscounnectDevice, _connectedDevice.MacAddress);

                ResponceCount = 0;
                while (ResponceCount < 10)
                {
                    await Task.Delay(1000);
                    if (!IsDeviceConnectionStatus)
                    {
                        break;
                    }
                    ResponceCount++;
                }

                RemoteSessionDetails._remote.OnReceive += (from, type, json) => { _ = Task.Run(async () => { await DeviceDetailReceive(from, type, json); }); };

                MainPage Page = new MainPage(false);
                await Navigation.PushAsync(Page);
            }
        }
        private async void btnCancelReconect_Clicked(object sender, EventArgs e)
        {
            BreakWhileLoop = true;
            try
            {
                await StopAllServices();

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DiscounnectDevice, _connectedDevice.MacAddress);

                ResponceCount = 0;
                while (ResponceCount < 10)
                {
                    await Task.Delay(1000);
                    if (!IsDeviceConnectionStatus)
                    {
                        break;
                    }
                    ResponceCount++;
                }
            }
            catch { }
            RemoteSessionDetails._remote.OnReceive -= (from, type, json) => { _ = Task.Run(async () => { await DeviceDetailReceive(from, type, json); }); };

            MainPage Page = new MainPage(false);
            await Navigation.PushAsync(Page);
        }

        #endregion BusyIndecator And Reconnect Functionality

        #region CalibrationTab Update

        #region Voltage Calibration
        private async Task VoltageCalibrationTab()
        {
            try
            {
                txtTemperatureCalibration.Text = "0";
                txtChargeCurrentSCValue.Text = "0";
                txtDisChargeCurrentSCValue.Text = "0";
                lblErrorMassage.IsVisible = false;
                txtCalibrationPassword.PINValue = "";
                DateTime TodayDate = DateTime.Now;

                int Day = TodayDate.Day;
                int Month = TodayDate.Month;
                int Year = Convert.ToInt16(Convert.ToString(TodayDate.Year).Substring(2));

                string CalibrationPassword = Convert.ToString(Day + Month).PadLeft(2, '0') + Convert.ToString(Month + Month).PadLeft(2, '0') + Convert.ToString(Year + Month).PadLeft(2, '0');

                if (!string.IsNullOrWhiteSpace(sCalibrationPassword))
                {
                    if (sCalibrationPassword == CalibrationPassword)
                    {
                        PasswordConfirmBoxSL.IsVisible = false;
                        stackMainCalibration.IsVisible = true;

                        await ReadCalibrationTab();
                    }
                    else
                    {
                        PasswordConfirmBoxSL.IsVisible = true;
                        stackMainCalibration.IsVisible = false;
                    }
                }
                else
                {
                    PasswordConfirmBoxSL.IsVisible = true;
                    stackMainCalibration.IsVisible = false;
                }

                await ShowDisplayPopupDesign(DeviceWidth, DeviceHeight);
            }
            catch (Exception ex)
            {

            }
        }
        private async Task ReadCalibrationTab()
        {
            try
            {
                await GetSerialNumberFromBMS();

                await Task.Delay(100);

                if (objReadValues._IsSFKH2Device)
                {
                    await WriteCalibrationAccessed();

                    await Task.Delay(200);

                    await ReadCalibrationAccessed();

                    await Task.Delay(100);

                    if (CurrentBattType != 1)
                    {
                        stackActiveBalancerCutOffMain.IsVisible = true;
                    }

                    await ReadH2ParameterValues();

                    await Task.Delay(100);

                    await ReadBMSResetCount();

                    await Task.Delay(100);

                    await ReadDaySinceFullCharge("date");

                    await Task.Delay(100);

                    await ReadDaySinceFullCharge();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnVoltagePasswordSubmit_Tapped(object sender, EventArgs e)
        {
            try
            {
                DateTime TodayDate = DateTime.Now;

                int Day = TodayDate.Day;
                int Month = TodayDate.Month;
                int Year = Convert.ToInt16(Convert.ToString(TodayDate.Year).Substring(2));

                string CalibrationPassword = Convert.ToString(Day + Month).PadLeft(2, '0') + Convert.ToString(Month + Month).PadLeft(2, '0') + Convert.ToString(Year + Month).PadLeft(2, '0');

                string EnteredPassword = txtCalibrationPassword.PINValue;

                EnteredPassword.PadLeft(2, '0');

                if (EnteredPassword == CalibrationPassword)
                {
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ComfirmPassword, "Success|" + Convert.ToString(EnteredPassword));

                    lblErrorMassage.Text = "Access Granted";
                    lblErrorMassage.IsVisible = true;
                    lblErrorMassage.TextColor = Colors.Green;

                    sCalibrationPassword = EnteredPassword;
                    PasswordConfirmBoxSL.IsVisible = false;
                    stackMainCalibration.IsVisible = true;

                    await VoltageCalibrationTab();

                    await Task.Delay(3000);
                    lblErrorMassage.IsVisible = false;
                }
                else
                {
                    lblmsgheight.Height = 50;
                    lblErrorMassage.IsVisible = true;
                    await Task.Delay(3000);
                    lblErrorMassage.IsVisible = false;
                    lblmsgheight.Height = new GridLength(1, GridUnitType.Star);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async void btnVoltagePasswordQuite_Tapped(object sender, EventArgs e)
        {
            try
            {
                SKFTabView.SelectedIndex = 1;
                PasswordConfirmBoxSL.IsVisible = false;
                stackPasswordBusy.IsVisible = false;
                stackMainCalibration.IsVisible = true;

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.ComfirmPassword, "Cancel");
            }
            catch (Exception ex)
            {

            }
        }
        private async void VoltageCalibrationUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                int tCount = 0;
                bool IsValid = false;
                isProccesBegin = true;
                bool bproceed = true;
                string RegisterValue = string.Empty;
                byte RegisterValueinInt32 = 0;
                decimal NewVoltageCalibrationValue = Convert.ToDecimal(txtVoltageCalibrationValue.Text);

                if (NewVoltageCalibrationValue < 2500 || NewVoltageCalibrationValue > 3650)
                {
                    if (NewVoltageCalibrationValue == 0 || string.IsNullOrWhiteSpace(strVoltageCalibrationValues[0]))
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("Error", "Please select a cell from the above for the voltage calibration.");
                    }
                    else
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("Error", "Please enter valid input. Range should be between 2500mv to 3650 mv.");
                    }
                    bproceed = false;
                }
                else if (string.IsNullOrWhiteSpace(strVoltageCalibrationValues[0]))
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Error", "Please select a cell from the above for the voltage calibration 11.");
                }

                if (bproceed)
                {
                    int CellNumber = Convert.ToInt16(strVoltageCalibrationValues[0].Replace("Cell ", ""));
                    NewVoltageCalibrationValue = NewVoltageCalibrationValue / 1000;
                    List<decimal> OldVoltageCalibrationValue = objReadValues.VoltageCalibrationValue;

                    switch (CellNumber)
                    {
                        case 1:
                            RegisterValue = RegisterEnum.REG_CAL_V_CELL_01;
                            break;
                        case 2:
                            RegisterValue = RegisterEnum.REG_CAL_V_CELL_02;
                            break;
                        case 3:
                            RegisterValue = RegisterEnum.REG_CAL_V_CELL_03;
                            break;
                        case 4:
                            RegisterValue = RegisterEnum.REG_CAL_V_CELL_04;
                            break;
                        case 5:
                            RegisterValue = RegisterEnum.REG_CAL_V_CELL_05;
                            break;
                        case 6:
                            RegisterValue = RegisterEnum.REG_CAL_V_CELL_06;
                            break;
                        case 7:
                            RegisterValue = RegisterEnum.REG_CAL_V_CELL_07;
                            break;
                        case 8:
                            RegisterValue = RegisterEnum.REG_CAL_V_CELL_08;
                            break;
                    }

                    uint iRegisterValue = uint.Parse(RegisterValue, System.Globalization.NumberStyles.AllowHexSpecifier);
                    RegisterValueinInt32 = BitConverter.GetBytes(iRegisterValue)[0];

                    while (tCount < 2)
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(150);

                        var WriteToReg = objProtocol.PrepareBytesForVoltageCalibration(RegisterValueinInt32, RegisterValue, NewVoltageCalibrationValue);
                        await SendCommandToBMS(WriteToReg);

                        await Task.Delay(150);

                        await ReadCellCount04Command();

                        await Task.Delay(1500);

                        await ShowBusyindicatior(true);

                        if (objReadValues.VoltageCalibrationValue[CellNumber - 1] != Math.Round(NewVoltageCalibrationValue, 2))
                        {
                            tCount++;
                        }
                        else
                        {
                            IsValid = true;
                            break;
                        }
                    }

                    if (IsValid && (objReadValues.VoltageCalibrationValue[CellNumber - 1] == Math.Round(NewVoltageCalibrationValue, 2)))
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("Success", "");
                    }
                    else
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("NotSuccess", "");
                    }
                }

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                //New Added
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void frmSelectCell_Tapped(object sender, EventArgs e)
        {
            try
            {
                foreach (var childElement in frmLastSelectedCell.Children)
                {
                    StackLayout stack1 = (StackLayout)childElement;
                    foreach (var inner1childElement in stack1.Children)
                    {
                        StackLayout stack2 = (StackLayout)inner1childElement;
                        foreach (var inner2childElement in stack2.Children)
                        {
                            if (inner2childElement is Label label)
                            {
                                label.TextColor = Colors.White;
                                label.FontAttributes = FontAttributes.None;
                            }
                            else if (inner2childElement is Image image)
                            {
                                image.Source = "batteryiconcalibration.png";
                            }
                        }
                    }
                }

                if (frmLastSelectedCell != null)
                {
                    frmLastSelectedCell.BackgroundColor = Color.FromHex("#6E6E6E");
                }
                Frame viewCell = (Frame)sender;
                if (viewCell != null)
                {
                    viewCell.BackgroundColor = Color.FromHex("#F0C000");
                    frmLastSelectedCell = viewCell;
                }

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.VoltageCalibration, viewCell);

                foreach (var childElement in frmLastSelectedCell.Children)
                {
                    StackLayout stack1 = (StackLayout)childElement;
                    foreach (var inner1childElement in stack1.Children)
                    {
                        StackLayout stack2 = (StackLayout)inner1childElement;
                        foreach (var inner2childElement in stack2.Children)
                        {
                            if (inner2childElement is Label label)
                            {
                                label.TextColor = Colors.Black;
                                label.FontAttributes = FontAttributes.Bold;
                                if (label.ClassId.ToLower().Trim().StartsWith("cellname"))
                                {
                                    strVoltageCalibrationValues[0] = label.Text;
                                }
                                else if (label.ClassId.ToLower().Trim().StartsWith("cellvoltage"))
                                {
                                    strVoltageCalibrationValues[1] = label.Text;
                                    txtVoltageCalibrationValue.Text = label.Text;
                                }
                            }
                            else if (inner2childElement is Image image)
                            {
                                image.Source = "batteryiconcalibrationselected.png";
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {

            }
        }
        private async void lblVoltageCalibrationEdit_Tapped(object sender, EventArgs e)
        {
            try
            {
                lbl10mV.BackgroundColor = Colors.White;
                lbl100mV.BackgroundColor = Colors.White;
                lbl1000mV.BackgroundColor = Colors.White;

                lbl10mV.TextColor = Colors.Black;
                lbl100mV.TextColor = Colors.Black;
                lbl1000mV.TextColor = Colors.Black;

                lbl10mV.ClassId = "false";
                lbl100mV.ClassId = "false";
                lbl1000mV.ClassId = "false";

                Label label = (Label)sender;
                if (label.Text == "10")
                {
                    lbl10mV.BackgroundColor = Color.FromHex("#F7931E");
                    lbl10mV.TextColor = Colors.White;
                    lbl10mV.ClassId = "true";
                }
                else if (label.Text == "100")
                {
                    lbl100mV.BackgroundColor = Color.FromHex("#F7931E");
                    lbl100mV.TextColor = Colors.White;
                    lbl100mV.ClassId = "true";
                }
                else if (label.Text == "1000")
                {
                    lbl1000mV.BackgroundColor = Color.FromHex("#F7931E");
                    lbl1000mV.TextColor = Colors.White;
                    lbl1000mV.ClassId = "true";
                }

            }
            catch (Exception ex)
            {

            }
        }
        private async void imgBtnAddmV_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (lbl10mV.ClassId == "true")
                {
                    txtVoltageCalibrationValue.Text = Convert.ToString(Convert.ToInt32(txtVoltageCalibrationValue.Text) + 10);
                }
                else if (lbl100mV.ClassId == "true")
                {
                    txtVoltageCalibrationValue.Text = Convert.ToString(Convert.ToInt32(txtVoltageCalibrationValue.Text) + 100);
                }
                else if (lbl1000mV.ClassId == "true")
                {
                    txtVoltageCalibrationValue.Text = Convert.ToString(Convert.ToInt32(txtVoltageCalibrationValue.Text) + 1000);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async void imgBtnReducemV_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(txtVoltageCalibrationValue.Text) > 0)
                {
                    if (lbl10mV.ClassId == "true")
                    {
                        txtVoltageCalibrationValue.Text = Convert.ToString(Convert.ToInt32(txtVoltageCalibrationValue.Text) - 10);
                    }
                    else if (lbl100mV.ClassId == "true")
                    {
                        txtVoltageCalibrationValue.Text = Convert.ToString(Convert.ToInt32(txtVoltageCalibrationValue.Text) - 100);
                    }
                    else if (lbl1000mV.ClassId == "true")
                    {
                        txtVoltageCalibrationValue.Text = Convert.ToString(Convert.ToInt32(txtVoltageCalibrationValue.Text) - 1000);
                    }

                    if (Convert.ToInt32(txtVoltageCalibrationValue.Text) < 0)
                    {
                        txtVoltageCalibrationValue.Text = Convert.ToString(0);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void SetDesignforVoltageCalibration()
        {
            try
            {
                if (DeviceWidth > DeviceHeight) // landscaper
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        await LandscapDesignForMobile(DeviceWidth, DeviceHeight);
                    }
                    else if (Device.Idiom == TargetIdiom.Tablet)
                    {
                        await LandscapDesignForTab(DeviceWidth, DeviceHeight);
                    }
                }
                else //portrait
                {
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        await PotraitDesignForMobile(DeviceWidth, DeviceHeight);
                    }
                    else if (Device.Idiom == TargetIdiom.Tablet)
                    {
                        await PotraitDesignForTab(DeviceWidth, DeviceHeight);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion Voltage Calibration

        #region Temp Calibration
        private async void btnTempCalibrate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                string TempType = strTemperatureCalibrationValues[0];

                isProccesBegin = true;
                decimal TemperatuteValue = 0;
                if (!string.IsNullOrWhiteSpace(strTemperatureCalibrationValues[0]))
                {
                    if (!string.IsNullOrWhiteSpace(txtTempCalibrationValue.Text))
                    {
                        TemperatuteValue = Convert.ToDecimal(txtTempCalibrationValue.Text);

                        if (CheckValidNumer(Convert.ToString(TemperatuteValue)))
                        {
                            if (TempType == "BMS T.")
                            {
                                // d0
                                await SendWriteOrReadStartCommandToBMS();

                                var setTempratureD0 = objProtocol.GetTempratureBytes("d0", Convert.ToInt32(TemperatuteValue));
                                await SendCommandToBMS(setTempratureD0);

                                await SendWriteOrReadEndCommandToBMS();

                                await Task.Delay(500);
                            }
                            else if (TempType == "Case T. 1")
                            {
                                // d1
                                await SendWriteOrReadStartCommandToBMS();

                                var setTempratureD1 = objProtocol.GetTempratureBytes("d1", Convert.ToInt32(TemperatuteValue));
                                await SendCommandToBMS(setTempratureD1);

                                await SendWriteOrReadEndCommandToBMS();

                                await Task.Delay(500);
                            }
                            else if (TempType == "Case T. 2")
                            {
                                // d2
                                await SendWriteOrReadStartCommandToBMS();

                                var setTempratureD2 = objProtocol.GetTempratureBytes("d2", Convert.ToInt32(TemperatuteValue));
                                await SendCommandToBMS(setTempratureD2);

                                await SendWriteOrReadEndCommandToBMS();

                                await Task.Delay(500);
                            }

                            txtTempCalibrationValue.Text = "0";

                            await ShowBusyindicatior(false);
                            await ShowDisplayPopup("Success", "Calibrate Successfully");
                        }
                        else
                        {
                            await ShowBusyindicatior(false);
                            if (TemperatuteValue == 0)
                            {
                                await ShowDisplayPopup("Error", "Please select a Temperature Type from the above to the update value.");
                            }
                            else
                            {
                                await ShowDisplayPopup("Error", "Please enter valid input. Range should be between 1 °C to 38 °C");
                            }
                        }
                    }
                    else
                    {
                        await ShowBusyindicatior(false);
                        await ShowDisplayPopup("Error", "Please enter valid input.");
                    }
                }
                else
                {
                    await ShowDisplayPopup("Error", "Please select a Temperature Type from the above to the update value.");
                }
                await ShowBusyindicatior(false);
                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async void frmSelectTempCalibration_Tapped(object sender, EventArgs e)
        {
            try
            {
                foreach (var childElement in frmLastSelectedTemp.Children)
                {
                    StackLayout stack1 = (StackLayout)childElement;
                    foreach (var inner1childElement in stack1.Children)
                    {
                        StackLayout stack2 = (StackLayout)inner1childElement;
                        foreach (var inner2childElement in stack2.Children)
                        {
                            if (inner2childElement is Label label)
                            {
                                label.TextColor = Colors.White;
                                label.FontAttributes = FontAttributes.None;
                            }
                            else if (inner2childElement is Image image)
                            {
                                if (Convert.ToString(image.Source).ToLower().Replace("file: ", "").Trim().StartsWith("bms_t"))
                                {
                                    image.Source = "bmst.png";
                                }
                                else if (Convert.ToString(image.Source).ToLower().Replace("file: ", "").Trim().StartsWith("case_t"))
                                {
                                    image.Source = "caset.png";
                                }
                            }
                        }
                    }
                }

                if (frmLastSelectedTemp != null)
                {
                    frmLastSelectedTemp.BackgroundColor = Color.FromHex("#6E6E6E");
                }
                Frame viewCell = (Frame)sender;
                if (viewCell != null)
                {
                    viewCell.BackgroundColor = Color.FromHex("#F0C000");
                    frmLastSelectedTemp = viewCell;
                }

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.TemperatureCalibraion, viewCell);

                foreach (var childElement in frmLastSelectedTemp.Children)
                {
                    StackLayout stack1 = (StackLayout)childElement;
                    foreach (var inner1childElement in stack1.Children)
                    {
                        StackLayout stack2 = (StackLayout)inner1childElement;
                        foreach (var inner2childElement in stack2.Children)
                        {
                            if (inner2childElement is Label label)
                            {
                                label.TextColor = Colors.Black;
                                label.FontAttributes = FontAttributes.Bold;
                                if (label.ClassId.ToLower().Trim().StartsWith("tempname"))
                                {
                                    strTemperatureCalibrationValues[0] = label.Text;
                                }
                                else if (label.ClassId.ToLower().Trim().StartsWith("tempvalue"))
                                {
                                    strTemperatureCalibrationValues[1] = label.Text;
                                    txtTempCalibrationValue.Text = Convert.ToString(Math.Round(Convert.ToDecimal(label.Text), 00));
                                }
                            }
                            else if (inner2childElement is Image image)
                            {
                                if (Convert.ToString(image.Source).ToLower().Replace("file: ", "").Trim().StartsWith("bms_t"))
                                {
                                    image.Source = "bmstselected.png";
                                }
                                else if (Convert.ToString(image.Source).ToLower().Replace("file: ", "").Trim().StartsWith("case_t"))
                                {
                                    image.Source = "casetselected.png";
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
        private async void imgBtnReduceTCmV_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToDecimal(txtTempCalibrationValue.Text) > decimal.Zero)
                {
                    txtTempCalibrationValue.Text = Convert.ToString(Convert.ToDecimal(txtTempCalibrationValue.Text) - 1);
                }

                if (Convert.ToInt32(txtTempCalibrationValue.Text) < 0)
                {
                    txtTempCalibrationValue.Text = Convert.ToString(0);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async void imgBtnAddTCmV_Clicked(object sender, EventArgs e)
        {
            try
            {
                txtTempCalibrationValue.Text = Convert.ToString(Convert.ToDecimal(txtTempCalibrationValue.Text) + 1);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion Temp Calibration

        #region Shunt Calibration                
        private async void btnIdleCurrentSC_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                isProccesBegin = true;
                int IdleCurrentValue = 0;
                if (!string.IsNullOrWhiteSpace(lblidleCurrentValue.Text))
                {
                    IdleCurrentValue = Convert.ToInt32(lblidleCurrentValue.Text);

                    // ad
                    await SendWriteOrReadStartCommandToBMS();

                    var setCurrenteD0 = objProtocol.GetIdleCurrentRegisterBytes(Convert.ToInt32(IdleCurrentValue));
                    await SendCommandToBMS(setCurrenteD0);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(500);

                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "Idle Current Calibrate Successfully");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Error", "Please enter valid input.");
                }
                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async void btnChargeCurrentSC_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                isProccesBegin = true;
                int ChargeCurrentValue = 0;
                if (!string.IsNullOrWhiteSpace(txtChargeCurrentSCValue.Text))
                {
                    ChargeCurrentValue = Convert.ToInt32(txtChargeCurrentSCValue.Text);

                    // ae
                    await SendWriteOrReadStartCommandToBMS();

                    var setCurrenteD0 = objProtocol.GetChargeCurrentRegisterBytes(Convert.ToInt32(ChargeCurrentValue));
                    await SendCommandToBMS(setCurrenteD0);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(500);

                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "Charge Current Calibrate Successfully");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Error", "Please enter valid input.");
                }
                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async void btnDisChargeCurrentSC_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                isProccesBegin = true;
                int DisChargeCurrentValue = 0;
                if (!string.IsNullOrWhiteSpace(txtDisChargeCurrentSCValue.Text))
                {
                    DisChargeCurrentValue = Convert.ToInt32(txtDisChargeCurrentSCValue.Text);

                    // af
                    await SendWriteOrReadStartCommandToBMS();

                    var setCurrenteD0 = objProtocol.GetDisChargeCurrentRegisterBytes(Convert.ToInt32(DisChargeCurrentValue));
                    await SendCommandToBMS(setCurrenteD0);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(500);

                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "DisCharge Current Calibrate Successfully");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Error", "Please enter valid input.");
                }
                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nWriteCounter < EstablishConnectionDatAttemps)
                {
                    nWriteCounter++;
                }
            }
        }
        private async void lblChargeCurrentSCEdit_Tapped(object sender, EventArgs e)
        {
            try
            {
                lblChargeCurrentSC10mA.BackgroundColor = Colors.White;
                lblChargeCurrentSC100mA.BackgroundColor = Colors.White;
                lblChargeCurrentSC1000mA.BackgroundColor = Colors.White;

                lblChargeCurrentSC10mA.TextColor = Colors.Black;
                lblChargeCurrentSC100mA.TextColor = Colors.Black;
                lblChargeCurrentSC1000mA.TextColor = Colors.Black;

                lblChargeCurrentSC10mA.ClassId = "false";
                lblChargeCurrentSC100mA.ClassId = "false";
                lblChargeCurrentSC1000mA.ClassId = "false";

                Label label = (Label)sender;
                if (label.Text == "10")
                {
                    lblChargeCurrentSC10mA.BackgroundColor = Color.FromHex("#F7931E");
                    lblChargeCurrentSC10mA.TextColor = Colors.White;
                    lblChargeCurrentSC10mA.ClassId = "true";
                }
                else if (label.Text == "100")
                {
                    lblChargeCurrentSC100mA.BackgroundColor = Color.FromHex("#F7931E");
                    lblChargeCurrentSC100mA.TextColor = Colors.White;
                    lblChargeCurrentSC100mA.ClassId = "true";
                }
                else if (label.Text == "1000")
                {
                    lblChargeCurrentSC1000mA.BackgroundColor = Color.FromHex("#F7931E");
                    lblChargeCurrentSC1000mA.TextColor = Colors.White;
                    lblChargeCurrentSC1000mA.ClassId = "true";
                }

            }
            catch (Exception ex)
            {

            }
        }
        private async void lblDisChargeCurrentSCEdit_Tapped(object sender, EventArgs e)
        {
            try
            {
                lblDisChargeCurrentSC10mA.BackgroundColor = Colors.White;
                lblDisChargeCurrentSC100mA.BackgroundColor = Colors.White;
                lblDisChargeCurrentSC1000mA.BackgroundColor = Colors.White;

                lblDisChargeCurrentSC10mA.TextColor = Colors.Black;
                lblDisChargeCurrentSC100mA.TextColor = Colors.Black;
                lblDisChargeCurrentSC1000mA.TextColor = Colors.Black;

                lblDisChargeCurrentSC10mA.ClassId = "false";
                lblDisChargeCurrentSC100mA.ClassId = "false";
                lblDisChargeCurrentSC1000mA.ClassId = "false";

                Label label = (Label)sender;
                if (label.Text == "10")
                {
                    lblDisChargeCurrentSC10mA.BackgroundColor = Color.FromHex("#F7931E");
                    lblDisChargeCurrentSC10mA.TextColor = Colors.White;
                    lblDisChargeCurrentSC10mA.ClassId = "true";
                }
                else if (label.Text == "100")
                {
                    lblDisChargeCurrentSC100mA.BackgroundColor = Color.FromHex("#F7931E");
                    lblDisChargeCurrentSC100mA.TextColor = Colors.White;
                    lblDisChargeCurrentSC100mA.ClassId = "true";
                }
                else if (label.Text == "1000")
                {
                    lblDisChargeCurrentSC1000mA.BackgroundColor = Color.FromHex("#F7931E");
                    lblDisChargeCurrentSC1000mA.TextColor = Colors.White;
                    lblDisChargeCurrentSC1000mA.ClassId = "true";
                }

            }
            catch (Exception ex)
            {

            }
        }
        private async void imgBtnChargeCurrentSCReducemV_Clicked(object sender, EventArgs e)
        {
            if (Convert.ToInt16(txtChargeCurrentSCValue.Text) > 0)
            {
                if (lblChargeCurrentSC10mA.ClassId == "true")
                {
                    txtChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtChargeCurrentSCValue.Text) - 10);
                }
                else if (lblChargeCurrentSC100mA.ClassId == "true")
                {
                    txtChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtChargeCurrentSCValue.Text) - 100);
                }
                else if (lblChargeCurrentSC1000mA.ClassId == "true")
                {
                    txtChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtChargeCurrentSCValue.Text) - 1000);
                }

                if (Convert.ToInt32(txtChargeCurrentSCValue.Text) < 0)
                {
                    txtChargeCurrentSCValue.Text = Convert.ToString(0);
                }
            }
        }
        private async void imgBtnChargeCurrentSCAddmV_Clicked(object sender, EventArgs e)
        {
            if (lblChargeCurrentSC10mA.ClassId == "true")
            {
                txtChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtChargeCurrentSCValue.Text) + 10);
            }
            else if (lblChargeCurrentSC100mA.ClassId == "true")
            {
                txtChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtChargeCurrentSCValue.Text) + 100);
            }
            else if (lblChargeCurrentSC1000mA.ClassId == "true")
            {
                txtChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtChargeCurrentSCValue.Text) + 1000);
            }
        }
        private async void imgBtnDisChargeCurrentSCReducemV_Clicked(object sender, EventArgs e)
        {
            if (Convert.ToInt16(txtDisChargeCurrentSCValue.Text) > 0)
            {
                if (lblDisChargeCurrentSC10mA.ClassId == "true")
                {
                    txtDisChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtDisChargeCurrentSCValue.Text) - 10);
                }
                else if (lblDisChargeCurrentSC100mA.ClassId == "true")
                {
                    txtDisChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtDisChargeCurrentSCValue.Text) - 100);
                }
                else if (lblDisChargeCurrentSC1000mA.ClassId == "true")
                {
                    txtDisChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtDisChargeCurrentSCValue.Text) - 1000);
                }

                if (Convert.ToInt32(txtDisChargeCurrentSCValue.Text) < 0)
                {
                    txtDisChargeCurrentSCValue.Text = Convert.ToString(0);
                }
            }
        }
        private async void imgBtnDisChargeCurrentSCAddmV_Clicked(object sender, EventArgs e)
        {
            if (lblDisChargeCurrentSC10mA.ClassId == "true")
            {
                txtDisChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtDisChargeCurrentSCValue.Text) + 10);
            }
            else if (lblDisChargeCurrentSC100mA.ClassId == "true")
            {
                txtDisChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtDisChargeCurrentSCValue.Text) + 100);
            }
            else if (lblDisChargeCurrentSC1000mA.ClassId == "true")
            {
                txtDisChargeCurrentSCValue.Text = Convert.ToString(Convert.ToInt32(txtDisChargeCurrentSCValue.Text) + 1000);
            }
        }
        private void txtShuntCalibration_Focused(object sender, FocusEventArgs e)
        {
            try
            {
                this.TranslationY = SKFTabView.TabBarHeight;
            }
            catch (Exception ex)
            {
            }
        }
        private void txtShuntCalibration_Unfocused(object sender, FocusEventArgs e)
        {
            try
            {
                this.TranslationY = 0;
            }
            catch (Exception ex)
            {
            }
        }

        #endregion Shunt Calibration
        private async void btnSerialNumber_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                var keyboardService = App.Services.GetService<IKeyboardService>();
                keyboardService?.HideKeyboard();
                string NewValue = txtSerialNumber.Text;
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var setA2Value = objProtocol.PrepareSerialNumber(txtSerialNumber.Text);
                await SendCommandToBMS(setA2Value);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(500);

                // Read Barcode
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // var getRegisterA2Value = new byte[] { 0xdd, 0xa5, 0xa2, 0x00, 0xff, 0x5e, 0x77 };
                var getRegisterA2Value = objProtocol.CreateReadCommand(RegisterEnum.REG_BARCODE);
                await SendCommandToBMS(getRegisterA2Value);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(500);

                if (BarcodeValue == NewValue)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }

            }
            catch (Exception ex)
            {

            }
        }
        private async void btnResetCycleCount_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);
                await SendWriteOrReadStartCommandToBMS();

                bResetCycleCount = true;

                await Task.Delay(100);

                // var ResetCycleCount = new byte[] { 0xDD, 0x5A, 0x17, 0x02, 0x00, 0x00, 0xFF, 0xE7, 0x77 };
                var ResetCycleCount = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_CYCLE_CNT, "02", 0);
                await SendCommandToBMS(ResetCycleCount);

                await Task.Delay(100);

                await GetResetCycleCountFromBMS();

                await Task.Delay(2000);

                if (bResetCycleCount)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnResetLogFaults_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // var ResetLogFaults = new byte[] { 0xDD, 0x5A, 0x01, 0x02, 0x28, 0x28, 0xFF, 0xAD, 0x77 };
                var ResetLogFaults = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_EXIT_FACTORY, "02", 10280);
                await SendCommandToBMS(ResetLogFaults);

                if (objReadValues._IsSFKH2Device)
                {
                    await Task.Delay(500);

                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    var ResetLogFaultsV4 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_V_CELL_19, "03", 0);
                    await SendCommandToBMS(ResetLogFaultsV4);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();
                }

                await Task.Delay(2000);

                if (bResetLogFaults)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnRestartESP32_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!bProccedRestartESP)
                {
                    await ShowDisplayPopup("RestartESP32", "This will require you to reconnect to the BMS and re-enable Bluetooth on the battery.");
                }
                else if (bProccedRestartESP)
                {
                    await RestartESP32();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task GetResetCycleCountFromBMS()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                //var getRegister17Value = new byte[] { 0xdd, 0xa5, 0x17, 0x00, 0xff, 0xe9, 0x77 };
                var getRegister17Value = objProtocol.CreateReadCommand(RegisterEnum.REG_CYCLE_CNT);
                await SendCommandToBMS(getRegister17Value);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private void txtActiveBalancerCutOff_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out int value))
            {
                if (value < 2700 || value > 3500)
                {
                    lblActiveBalancerCutOffError.IsVisible = true;
                }
                else
                {
                    lblActiveBalancerCutOffError.IsVisible = false;
                }
            }
            else
            {
                txtActiveBalancerCutOff.Text = ""; // or keep the old value
            }
        }
        private async void btnActiveBalancerCutOff_Clicked(object sender, EventArgs e)
        {
            try
            {
                int.TryParse(txtActiveBalancerCutOff.Text, out int value);
                if (value >= 2700 && value <= 3500)
                {
                    string Value = value.ToString("X").PadLeft(4, '0');
                    uint iValue = Convert.ToUInt32(Value, 16);

                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    // CC F7 04  00 00 FF FD CF                     
                    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CUT_OFF, "04", iValue);
                    await SendCommandToBMS(bytes);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();
                }
                else
                {
                    lblActiveBalancerCutOffError.IsVisible = true;
                }

                if (objReadValues.IsBalancerCutOff)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnCalibrateSOC_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool IsValid = false;
                int tCount = 0;

                await ShowBusyindicatior(true);

                if (objReadValues.FullCapacityValue <= 0)
                {
                    await FullCapacityRead(RegisterEnum.REG_DESIGN_CAP);
                }

                await Task.Delay(150);

                double socValue = Math.Round(objReadValues.FullCapacityValue * rangeCalibrateSOC.Value / 100.0, 2);

                while (tCount < 2) // Register E0
                {
                    isProccesBegin = true;

                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    var WriteToReg10 = objProtocol.CreateMv10Byte(RegisterEnum.REG_CAP_REMAINING, Convert.ToDouble(socValue));
                    await SendCommandToBMS(WriteToReg10);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();

                    await ShowBusyindicatior(true);

                    await Task.Delay(500);

                    await ShowBusyindicatior(true);

                    if (!objReadValues.bCalibrateSOCSuccess)
                    {
                        tCount++;
                    }
                    else
                    {
                        IsValid = true;
                        break;
                    }
                }

                if (objReadValues.bCalibrateSOCSuccess)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }

                isProccesBegin = false;
            }
            catch (Exception ex)
            {
                if (nReadCounter < EstablishConnectionDatAttemps)
                {
                    nReadCounter++;
                }
            }
        }
        private async void btnBatteryType_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                uint iValue = 0;
                if ((bool)rdTypeOne.IsChecked)
                {
                    iValue = 1;
                }
                else if ((bool)rdTypeTwo.IsChecked)
                {
                    iValue = 2;
                }

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(150);

                var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.BATT_TYPE, "03", iValue);
                await SendCommandToBMS(bytes);

                await Task.Delay(150);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(150);

                await ReadH2ParameterValues();

                await Task.Delay(500);

                if (objReadValues.BatteryType == 1 || objReadValues.BatteryType == 2)
                {
                    bProccedBatteryType = false;

                    if (objReadValues.BatteryType == 2)
                    {
                        await EnableActiveBalancer();
                    }

                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    bProccedBatteryType = false;
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task EnableActiveBalancer()
        {
            try
            {
                bool isValid = false;
                int count = 0;
                while (count < 2)
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(150);

                    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_BAL_STATUS, "03", 1);
                    await SendCommandToBMS(bytes);

                    await Task.Delay(150);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(500);

                    if (objReadValues.IsActiveBalancerOn)
                    {
                        isValid = true;
                        count = 0;
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }

                if (isValid)
                {
                    isValid = false;
                    while (count < 2)
                    {
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(100);

                        // var bytes2 = new byte[] { 0xcc, 0xd9, 0x0a, 0x01, 0xf4, 0x00, 0x64, 0x0d, 0xde, 0x0d, 0x16, 0xfd, 0x8f, 0xcf };
                        var bytes2 = objProtocol.CreateBalanceParameterSetting(Convert.ToInt32(3.35 * 1000), Convert.ToInt32(3.30 * 1000), 30, 20);
                        await SendCommandToBMS(bytes2);

                        await Task.Delay(100);

                        await SendWriteOrReadEndCommandToBMS();

                        if (objReadValues.IsActivationVoltage)
                        {
                            isValid = true;
                            count = 0;
                            break;
                        }
                        else
                        {
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task RestartESP32()
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(150);

                var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.ESP_RESTART, "03", 1);
                await SendCommandToBMS(bytes);

                await Task.Delay(150);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(300);

                RunMainPageWhileLoop = false;
                BechmarkTestRunning = false;
                IsScantabSelect = false;
                BreakWhileLoop = true;

                await StopAllServices();

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.DiscounnectDevice, _connectedDevice.MacAddress);

                MainPage Page = new MainPage(false);
                await Navigation.PushAsync(Page);
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnVariableInterval_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                string Value = Convert.ToString(rangeAppConnectedInterval.Value).PadLeft(2, '0') + Convert.ToString(rangeAppDisconnectedInterval.Value).PadLeft(2, '0');
                uint iValue = Convert.ToUInt32(Value, 16);

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var bytes2 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_T_NTC_1, "04", iValue);
                await SendCommandToBMS(bytes2);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                if (IsTimeInterval)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnResetSOCLogFaults_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Task.Delay(500);

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var ResetLogFaultsC2 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_V_CELL_19, "03", 1);
                await SendCommandToBMS(ResetLogFaultsC2);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(2000);

                if (bBmsUsageLogClear)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnResetTemperatureLogFaults_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Task.Delay(500);

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var ResetLogFaultsC2 = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_V_CELL_19, "03", 2);
                await SendCommandToBMS(ResetLogFaultsC2);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(2000);

                if (bBmsUsageLogClear)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnBindUnbindBattery_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (CheckBoxBindUnbind.IsChecked)
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    var DeviceBindB2 = objProtocol.CreateH2CustomCommand(RegisterEnum.BIND_DEVICE, "03", 1);
                    await SendCommandToBMS(DeviceBindB2);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();
                }
                else if (!CheckBoxBindUnbind.IsChecked)
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    var DeviceBindB2 = objProtocol.CreateH2CustomCommand(RegisterEnum.BIND_DEVICE, "03", 2);
                    await SendCommandToBMS(DeviceBindB2);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();
                }

                await Task.Delay(100);

                await ReadBindUnbindStatus();

                await Task.Delay(500);

                if (objReadValues.IsDeviceBind)
                {
                    await ShowDisplayPopup("Success", "Device bind successfully.");
                }
                else
                {
                    await ShowDisplayPopup("Success", "Device unbind successfully.");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnEnableDebugMode_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                if (!CheckBoxEnableDebugMode.IsChecked)
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    var DeviceBindB2 = objProtocol.CreateH2CustomCommand(RegisterEnum.DEBUG_MODE, "03", 00);
                    await SendCommandToBMS(DeviceBindB2);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();
                }
                else if (CheckBoxEnableDebugMode.IsChecked)
                {
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(100);

                    var DeviceBindB2 = objProtocol.CreateH2CustomCommand(RegisterEnum.DEBUG_MODE, "03", 01);
                    await SendCommandToBMS(DeviceBindB2);

                    await Task.Delay(100);

                    await SendWriteOrReadEndCommandToBMS();
                }

                await Task.Delay(100);

                await ReadDebugModeStatus();

                await Task.Delay(500);

                if (objReadValues.IsDebugModeOn)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "Debug enabled successfully.");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "Debug disabled successfully.");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnUpdateDaySinceFullCharge_Clicked(object sender, EventArgs e)
        {
            try
            {
                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                var DaySinceCharge = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CAL_V_CELL_20, "03", Convert.ToUInt32(txtDaySinceFullCharge.Text));
                await SendCommandToBMS(DaySinceCharge);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(100);

                await ReadDaySinceFullCharge();

                await Task.Delay(500);

                if (objReadValues.DaySinceFullCharge == Convert.ToInt32(txtDaySinceFullCharge.Text))
                {
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnManufatureDate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                DateTime temp = txtManufactureDate.Date;
                string NewDate = temp.ToString("yyyy-MM-dd");

                uint year = (uint)((temp.Year - 2000) * 512);
                uint month = (uint)((temp.Month) * 32);
                uint day = (uint)temp.Day;

                uint iManufatureDate = year + month + day;

                await SendWriteOrReadStartCommandToBMS();

                await Task.Delay(100);

                // DD 5A 15 02 34 6C FF 49 77
                var WriteToReg29 = objProtocol.CreateCustomWriteCommand(RegisterEnum.REG_MFG_DATE, "02", iManufatureDate);
                await SendCommandToBMS(WriteToReg29);

                await Task.Delay(100);

                await SendWriteOrReadEndCommandToBMS();

                await Task.Delay(500);

                if (bManufatureDate)
                {
                    txtManufactureDate.Date = Convert.ToDateTime(lblProductionDate.Text);
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    txtManufactureDate.Date = Convert.ToDateTime(lblProductionDate.Text);
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void imgbtnRefreshBmsTime_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ReadRTCTimeInterval();
            }
            catch (Exception ex)
            {
            }
        }

        #region Restore Default Value

        int iReadRegister = 0;
        string strDefaultValues = string.Empty;
        private async void btnLoadDefaultValues_Clicked(object sender, EventArgs e)
        {
            try
            {
                string strDeviceName = Convert.ToString(DefaultValueDeviceComboBox.SelectedValue);
                if (string.IsNullOrWhiteSpace(strDeviceName) || strDeviceName == "Select")
                {
                    await ShowDisplayPopup("Error", "Please select device.");
                }
                else
                {
                    if (!bRestoreDefaultValue)
                    {
                        await ShowDisplayPopup("LoadDefaultValue", "This will change your all setting to the default values, are you sure you want to proceed?");
                    }
                    else
                    {
                        await ShowBusyindicatior(true);

                        strDefaultValues = await GetDefaultValueData(strDeviceName);

                        if (!string.IsNullOrWhiteSpace(strDefaultValues))
                        {
                            await ShowBusyindicatior(false);
                            if (strDefaultValues.StartsWith("DefaultData|"))
                            {
                                await RestoreDefaultValuesAsync();
                                await ShowBusyindicatior(false);
                            }
                            else
                            {
                                await ShowDisplayPopup("Error", "Something went wrong!");
                            }
                        }
                        else
                        {
                            await ShowDisplayPopup("Error", "Please Try Again.");
                            await ShowBusyindicatior(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stackProgressBarMain.IsVisible = stkDisplayMessagePopUpBusy.IsVisible = false;
                await ShowBusyindicatior(false);
            }
        }
        public async Task RestoreDefaultValuesAsync()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ProgressBarPointer.Progress = 0;
                    lblProgressBar.Text = $"{0}%";
                });

                bool ProceedForLoadDefaultSettings = true;
                isProccesBegin = true;
                CalibrationUpdateInProgress = true;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    stackProgressBarMain.IsVisible = stkDisplayMessagePopUpBusy.IsVisible = ProceedForLoadDefaultSettings;
                    lblProgressBarText.Text = "Loading Default Settings...";
                });

                await ShowBusyindicatior((!ProceedForLoadDefaultSettings));

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
                            bool bProceed = true;
                            int iTry = 0;
                            var item = strValueArray[i];
                            string register = string.Empty;

                            if (item.Contains("[!BARCODE!]"))
                            {
                                bProceed = false;
                            }
                            else
                            {
                                register = Convert.ToString(item.Trim().Split(' ')[2]);
                                iReadRegister = Convert.ToInt32(register, 16);
                                if (DeviceDefaultValues.H2Registers.Any(prefix => register.ToLower().StartsWith(prefix.ToLower())) && !objReadValues._IsSFKH2Device)
                                {
                                    bProceed = false;
                                }
                            }

                            if (bProceed)
                            {
                                if (objReadValues._IsSFKH2Device && bProceed)
                                {
                                    item = item.Trim().ToLower().Replace("dd 5a", "cc cd").ToLower();

                                    string[] parts = item.Split(' ');
                                    parts[parts.Length - 1] = "cf";

                                    item = string.Join(" ", parts);
                                }

                                byte[] byteArray = item.Trim().Split(' ').Select(hex => Convert.ToByte(hex, 16)).ToArray();
                                if (bProceed)
                                {
                                    while (iTry < 3)
                                    {
                                        await SendCommandToBMS(byteArray);
                                        WaitForResponse();

                                        if (bIsSendNExt) { break; }
                                        iTry++;
                                    }
                                }
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
                            await ReadCalibrationTab();

                            ProceedForLoadDefaultSettings = false;
                            // await Plugin.BLE.CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(_connectedDevice);
                            stackProgressBarMain.IsVisible = stkDisplayMessagePopUpBusy.IsVisible = ProceedForLoadDefaultSettings;
                            lblProgressBarText.Text = "Upgrading firmware...";
                            await ShowDisplayPopup("Success", "Default value has been loaded successfully.");
                        }
                        else
                        {
                            ProceedForLoadDefaultSettings = false;
                            // await Plugin.BLE.CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(_connectedDevice);
                            stackProgressBarMain.IsVisible = stkDisplayMessagePopUpBusy.IsVisible = ProceedForLoadDefaultSettings;
                            lblProgressBarText.Text = "Upgrading firmware...";
                            await ShowDisplayPopup("Error", "Unable to load default settings.");
                        }
                    });
                }

                isProccesBegin = false;
                CalibrationUpdateInProgress = false;
                bRestoreDefaultValue = false;

                await ShowBusyindicatior(false);
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", "Please try again.");
                stackProgressBarMain.IsVisible = stkDisplayMessagePopUpBusy.IsVisible = false;
                lblProgressBarText.Text = "Upgrading firmware...";
                bRestoreDefaultValue = false;
            }
        }
        public async Task<string> GetDefaultValueData(string strDeviceName = "")
        {
            string saAllLines = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                string SiteURL = "https://www.sunfunkits.com/app/GetDefaultValueDataForCalibration";

                client.DefaultRequestHeaders.Add("DEVICE_MODEL", strDeviceName);
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
        private async Task WaitForResponse()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
                if (bIsSendNExt == true)
                    break;
            }
        }

        #endregion

        #endregion CalibrationTab Update        

        #region Networking Tab Data

        private async void frmSelectWirelessMode_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                imgBle.Source = "bleicon.png";
                imgZigbee.Source = "zigbeeicon.png";
                imgThred.Source = "thredicon.png";

                lblBluetooth.TextColor = lblZigbee.TextColor = lblThred.TextColor = Color.FromHex("#BBBBBB");

                stackBleMain.IsVisible = false;
                stackZigbeeMain.IsVisible = false;
                stackThredMain.IsVisible = false;

                Frame frame = (Frame)sender;
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.SelectWirelessMode, frame.ClassId);

                if (frame.ClassId == "Ble")
                {
                    imgBle.Source = "bleactiveicon.png";
                    stackBleMain.IsVisible = true;
                    lblBluetooth.TextColor = Color.FromHex("#0082FC");
                }
                else if (frame.ClassId == "Zigbee")
                {
                    imgZigbee.Source = "zigbeeactiveicon.png";
                    stackZigbeeMain.IsVisible = true;
                    lblZigbee.TextColor = Color.FromHex("#DC2624");
                }
                else if (frame.ClassId == "Thred")
                {
                    imgThred.Source = "thredactiveicon.png";
                    stackThredMain.IsVisible = true;
                    lblThred.TextColor = Color.FromHex("#F7931E");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void rangesliderBleGain_LabelCreated(object sender, SliderLabelCreatedEventArgs e)
        {
            try
            {
                var item = objDeviceInformation.CustomCollectionBleGain.FirstOrDefault(x => x.Value == Convert.ToDouble(e.Text));
                if (item != null)
                {
                    e.Text = item.Label;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnBleGain_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ShowBusyindicatior(true);

                uint iValue = 1;
                //if (iValue > 0)
                //{
                //    IsBleGain = true;
                //    await SendWriteOrReadStartCommandToBMS();

                //    await Task.Delay(200);

                //    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_WIRLESS_MODE, "03", iValue);
                //    await SendCommandToBMS(bytes);

                //    await Task.Delay(200);

                //    await SendWriteOrReadEndCommandToBMS();

                //    await Task.Delay(1000);
                //}

                iValue = 0;
                if (rangesliderBleGain.Value == 0)
                {
                    iValue = 5;
                }
                else if (rangesliderBleGain.Value == 3)
                {
                    iValue = 6;
                }
                else if (rangesliderBleGain.Value == 6)
                {
                    iValue = 7;
                }
                else if (rangesliderBleGain.Value == 9)
                {
                    iValue = 8;
                }

                if (iValue > 0)
                {
                    IsBleGain = true;
                    await SendWriteOrReadStartCommandToBMS();

                    await Task.Delay(200);

                    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_BLE_GAIN, "03", iValue);
                    await SendCommandToBMS(bytes);

                    await Task.Delay(200);

                    await SendWriteOrReadEndCommandToBMS();

                    await Task.Delay(1000);
                }

                IsBleGain = false;

                if (objReadValues.IsBleGain)
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("Success", "");
                }
                else
                {
                    await ShowBusyindicatior(false);
                    await ShowDisplayPopup("NotSuccess", "");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnZigbeeUpdate_Clicked(object sender, EventArgs e)
        {

            try
            {
                if (!string.IsNullOrWhiteSpace(txtPanId.Text) && !string.IsNullOrWhiteSpace(txtChannelMask.Text) && !string.IsNullOrWhiteSpace(txtNetworkKey.Text))
                {
                    uint iValue = 3;
                    //if (iValue > 0)
                    //{
                    //    IsBleGain = true;
                    //    await SendWriteOrReadStartCommandToBMS();

                    //    await Task.Delay(200);

                    //    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_WIRLESS_MODE, "03", iValue);
                    //    await SendCommandToBMS(bytes);

                    //    await Task.Delay(200);

                    //    await SendWriteOrReadEndCommandToBMS();

                    //    await Task.Delay(1000);
                    //}

                    // PAN ID
                    iValue = Convert.ToUInt32(txtPanId.Text);
                    if (iValue > 0)
                    {
                        IsBleGain = true;
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(200);

                        var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_PANID_ZIGBEE, "03", iValue);
                        await SendCommandToBMS(bytes);

                        await Task.Delay(200);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(1000);
                    }

                    // Channel
                    iValue = Convert.ToUInt32(txtChannelMask.Text);
                    if (iValue > 0)
                    {
                        IsBleGain = true;
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(200);

                        var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CHANNEL_ZIGBEE, "03", iValue);
                        await SendCommandToBMS(bytes);

                        await Task.Delay(200);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(1000);
                    }

                    iValue = Convert.ToUInt32(txtNetworkKey.Text);
                    if (iValue > 0)
                    {
                        IsBleGain = true;
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(200);

                        var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_NETWORK_ZIGBEE, "18", iValue);
                        await SendCommandToBMS(bytes);

                        await Task.Delay(200);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(1000);
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnThredUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtPanIdThred.Text) && !string.IsNullOrWhiteSpace(txtChannelThred.Text) && !string.IsNullOrWhiteSpace(txtNetworkKeyThred.Text))
                {
                    uint iValue = 2;
                    //if (iValue > 0)
                    //{
                    //    IsBleGain = true;
                    //    await SendWriteOrReadStartCommandToBMS();

                    //    await Task.Delay(200);

                    //    var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_WIRLESS_MODE, "03", iValue);
                    //    await SendCommandToBMS(bytes);

                    //    await Task.Delay(200);

                    //    await SendWriteOrReadEndCommandToBMS();

                    //    await Task.Delay(1000);
                    //}

                    // PAN ID
                    iValue = Convert.ToUInt32(txtPanIdThred.Text);
                    if (iValue > 0)
                    {
                        IsBleGain = true;
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(200);

                        var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_PANID_THREAD, "03", iValue);
                        await SendCommandToBMS(bytes);

                        await Task.Delay(200);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(1000);
                    }

                    // Channel
                    iValue = Convert.ToUInt32(txtChannelThred.Text);
                    if (iValue > 0)
                    {
                        IsBleGain = true;
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(200);

                        var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_CHANNEL_THREAD, "03", iValue);
                        await SendCommandToBMS(bytes);

                        await Task.Delay(200);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(1000);
                    }

                    iValue = Convert.ToUInt32(txtNetworkKeyThred.Text);
                    if (iValue > 0)
                    {
                        IsBleGain = true;
                        await SendWriteOrReadStartCommandToBMS();

                        await Task.Delay(200);

                        var bytes = objProtocol.CreateH2CustomCommand(RegisterEnum.REG_NETWORK_THREAD, "18", iValue);
                        await SendCommandToBMS(bytes);

                        await Task.Delay(200);

                        await SendWriteOrReadEndCommandToBMS();

                        await Task.Delay(1000);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion Networking Tab Data

        #region Check And Proceed For Firmware Update   

        private bool _isInternalChange = false;
        private async void btnCheckForUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.FirmWareCheckForUpdate, null);

                _isInternalChange = true;
                rbSFK_H2.IsChecked = false;
                rbNuvoton_100SBN.IsChecked = false;

                if (objReadValues._IsSFKH2Device)
                {
                    rbSFK_H2.IsChecked = true;
                    FirmwareType = "SFK_H2";
                }
                //else if (!objReadValues._IsSFKH2Device && _IsCommunicateWithUSB)
                //{
                //    rbNuvoton_100SBN.IsChecked = true;
                //    FirmwareType = "Nuvoton_100SBN";
                //}
                GetFirmwareInfo(FirmwareType);
                _isInternalChange = false;

            }
            catch (Exception)
            {
                _isInternalChange = false;
            }
        }
        private void FirmwareType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (_isInternalChange)
                {
                    return;
                }
                InputKit.Shared.Controls.RadioButton radioButton = (InputKit.Shared.Controls.RadioButton)sender;
                if (radioButton.IsChecked)
                {
                    FirmwareType = radioButton.ClassId;
                    FirmwareVersion = "";
                    FirmwareVersionComboBox.SelectedItem = null;
                    btnProceedForUpdate.IsVisible = false;
                    stackfirmwareReleseNotes.IsVisible = false;
                    LableReleaseNotes.Text = null;

                    GetFirmwareInfo(FirmwareType);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async void FirmwareVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (FirmwareVersionComboBox.SelectedItem is FirmwareInfo selectedFirmware)
                {
                    await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.FirmWareVersionSelectedIndex, selectedFirmware);

                    if (selectedFirmware.FirmwareVersion == "Select Firmware Version")
                    {
                        LableReleaseNotes.Text = null;
                        btnProceedForUpdate.IsVisible = false;
                        stackfirmwareReleseNotes.IsVisible = false;
                        stackDisplayUpdateAvailable.IsVisible = false;
                        lblTextforfirmwareUpdateAvailable.Text = "";
                        return;
                    }

                    FirmwareVersion = selectedFirmware.FirmwareVersion;
                    LableReleaseNotes.Text = selectedFirmware.ReleaseNotes;
                    lblReleaseNoteHeader.Text = "Release Notes - " + selectedFirmware.UploadedOn;
                    btnProceedForUpdate.IsVisible = true;
                    stackfirmwareReleseNotes.IsVisible = true;
                    stackDisplayUpdateAvailable.IsVisible = false;
                    lblTextforfirmwareUpdateAvailable.Text = "";
                }
            }
            catch (Exception ex)
            {
            }
        }
        public async void GetFirmwareInfo(string FirmwareType)
        {
            try
            {
                await ShowBusyindicatior(true, "Checking Firmware Update..");

                HttpClient client = new HttpClient();
                string url = "https://www.sunfunkits.com/app/CheckForFirmwareUpdate";

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Firmware_Type", FirmwareType);
                client.DefaultRequestHeaders.Add("Mac_id", Convert.ToString(lblMacAddressID.Text));
                client.DefaultRequestHeaders.Add("Bms_id", Convert.ToString(BarcodeValue));

                HttpResponseMessage response = await client.GetAsync(url);

                await Task.Delay(1000);

                if (response.IsSuccessStatusCode)
                {
                    string httpResponseMessage = await response.Content.ReadAsStringAsync();
                    string[] data = httpResponseMessage.Split("|^|");

                    firmwareList.Clear();
                    if (data.Length > 1)
                    {
                        if (data[1] == "UPDATE")
                        {
                            for (int i = 2; i < data.Length; i += 3)
                            {
                                if (i + 2 < data.Length)
                                {
                                    firmwareList.Add(new FirmwareInfo
                                    {
                                        FirmwareVersion = data[i],
                                        ReleaseNotes = data[i + 1],
                                        UploadedOn = data[i + 2]
                                    });
                                }
                            }

                            if (firmwareList.Count > 0)
                            {
                                // Add first default option
                                firmwareList.Insert(0, new FirmwareInfo
                                {
                                    FirmwareVersion = "Select Firmware Version",
                                    ReleaseNotes = ""
                                });

                                if (firmwareList.Count > 1)
                                {
                                    firmwareList[1].DisplayFirmwareVersion = firmwareList[1].FirmwareVersion + " (Latest)";
                                    lblFirmwareUpdateMessage.IsVisible = true;
                                }

                                FirmwareVersionComboBox.ItemsSource = firmwareList;
                                FirmwareVersionComboBox.DisplayMemberPath = "DisplayTextFirmwareVersion";
                                FirmwareVersionComboBox.SelectedItem = firmwareList[0];
                                LableReleaseNotes.Text = firmwareList[0].ReleaseNotes;

                                stackFirmwareDetails.IsVisible = true;
                                btnCheckForUpdate.IsVisible = false;
                                lblTextforfirmwareUpdateAvailable.Text = "";
                                stackDisplayUpdateAvailable.IsVisible = false;
                            }
                            else
                            {
                                lblTextforfirmwareUpdateAvailable.Text = "No firmware available";
                                stackDisplayUpdateAvailable.IsVisible = true;
                                stackFirmwareDetails.IsVisible = false;
                            }
                        }
                        else if (data[1] == "NODATA")
                        {
                            lblTextforfirmwareUpdateAvailable.Text = "No firmware available";
                            stackDisplayUpdateAvailable.IsVisible = true;
                            stackFirmwareDetails.IsVisible = false;
                        }
                        else if (data[1] == "DATANOTFOUND")
                        {
                            lblTextforfirmwareUpdateAvailable.Text = "Unable to determine battery type.";
                            stackDisplayUpdateAvailable.IsVisible = true;
                            stackFirmwareDetails.IsVisible = false;
                        }
                        else if (data[1] == "ERROR")
                        {
                            lblTextforfirmwareUpdateAvailable.Text = "Something went wrong";
                            stackDisplayUpdateAvailable.IsVisible = true;
                            stackFirmwareDetails.IsVisible = false;
                        }
                        else if (data[1] == "UNABLETOCHECK")
                        {
                            lblTextforfirmwareUpdateAvailable.Text = "Firmware upgrade is currently unavailable at this time. Please contact sales for further assistance.";
                            stackDisplayUpdateAvailable.IsVisible = true;
                            stackFirmwareDetails.IsVisible = false;
                        }
                        else
                        {
                            lblTextforfirmwareUpdateAvailable.Text = "Contact sunfunkits sales.";
                            lblTextforfirmwareUpdateAvailable.TextColor = Colors.Red;
                        }
                    }
                    await ShowBusyindicatior(false);
                }
            }
            catch (HttpRequestException ex)
            {
                await ShowBusyindicatior(false);
                lblTextforfirmwareUpdateAvailable.Text = "Unable to connect to the server. Please check your internet connection.";
                stackDisplayUpdateAvailable.IsVisible = true;
                stackFirmwareDetails.IsVisible = false;
            }
            catch (Exception ex)
            {
                await ShowBusyindicatior(false);
                lblTextforfirmwareUpdateAvailable.Text = "An unexpected error occurred. Please try again later or Contact sunfunkits sales.";
                stackDisplayUpdateAvailable.IsVisible = true;
                stackFirmwareDetails.IsVisible = false;
            }
        }
        private async void btnProceedForUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.FirmWareProceedForUpdate, null);

                if (FirmwareType == "SFK_H2")
                {
                    if (Convert.ToInt32(objReadValues.H2FirmwareVersion.Replace(".", "")) < 1050)
                    {
                        await ShowDisplayPopup("Information", "Bluetooth firmware update is only available if you have version 1.0.50 installed. Please use the USB-C port to update the firmware.");
                        return;
                    }
                }

                await ShowBusyindicatior(true, "Please Wait..");

                HttpClientHandler handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true };
                HttpClient client = new HttpClient(handler);
                string SiteURL = "https://www.sunfunkits.com/app/ProcessForFirmwareUpdate";

                client.DefaultRequestHeaders.Add("Device_Name", Convert.ToString(lblAboutModelName.Text));
                client.DefaultRequestHeaders.Add("Selected_Firmware_version", Convert.ToString(FirmwareVersion));
                client.DefaultRequestHeaders.Add("Firmware_Type", Convert.ToString(FirmwareType));
                if (FirmwareType == "SFK_H2")
                {
                    client.DefaultRequestHeaders.Add("Client_Firmware_version", Convert.ToString(objReadValues.H2FirmwareVersion));
                }
                //else if (FirmwareType == "Nuvoton_100SBN")
                //{
                //    client.DefaultRequestHeaders.Add("DOM", Convert.ToString(lblProductionDate.Text));
                //    client.DefaultRequestHeaders.Add("Client_Firmware_version", Convert.ToString(Firmware_Version));
                //}

                var httpResponseMessage = await client.GetStringAsync(SiteURL);

                await Task.Delay(1000);

                string[] responseData = httpResponseMessage.Split("|^|");

                if (responseData.Length > 1)
                {
                    string response = responseData[1];


                    lblTextforfirmwareUpdateAvailable.Text = "";

                    if (response.Contains("UPDATE"))
                    {
                        if (FirmwareType == "SFK_H2")
                        {
                            strH2FirmwareData = responseData[2];
                            btnClickToProceedForH2_Clicked(null, null);
                        }
                        //if (FirmwareType == "Nuvoton_100SBN")
                        //{
                        //    if (!string.IsNullOrWhiteSpace(responseData[2]) && responseData[2].StartsWith(":"))
                        //    {
                        //        strHexData = responseData[2];
                        //    }
                        //}
                        stackDisplayUpdateAvailable.IsVisible = false;
                        lblTextforfirmwareUpdateAvailable.Text = "";
                    }
                    else if (response == "UPTODATE")
                    {
                        if (FirmwareType == "SFK_H2")
                        {
                            strH2FirmwareData = responseData[2];
                        }
                        //if (FirmwareType == "Nuvoton_100SBN")
                        //{
                        //    if (!string.IsNullOrWhiteSpace(responseData[2]) && responseData[2].StartsWith(":"))
                        //    {
                        //        strHexData = responseData[2];
                        //    }
                        //}
                        btnClickToFirmWareUptoDate_Clicked(null, null);
                        stackDisplayUpdateAvailable.IsVisible = false;
                        lblTextforfirmwareUpdateAvailable.Text = "";
                    }
                    else if (response == "ERROR")
                    {
                        stackDisplayUpdateAvailable.IsVisible = true;
                        lblTextforfirmwareUpdateAvailable.Text = "Something went wrong";
                        lblTextforfirmwareUpdateAvailable.TextColor = Colors.Red;
                    }
                    else if (response == "EXERROR")
                    {
                        stackDisplayUpdateAvailable.IsVisible = true;
                        lblTextforfirmwareUpdateAvailable.Text = "An error occurred in checking battery information on server.";
                        lblTextforfirmwareUpdateAvailable.TextColor = Colors.Red;
                    }
                    else if (response == "UNABLETOCHECK")
                    {
                        stackDisplayUpdateAvailable.IsVisible = true;
                        lblTextforfirmwareUpdateAvailable.Text = "Firmware upgrade is currently unavailable at this time. Please contact sales for further assistance.";
                        lblTextforfirmwareUpdateAvailable.TextColor = Colors.Red;
                    }
                    else
                    {
                        stackDisplayUpdateAvailable.IsVisible = true;
                        lblTextforfirmwareUpdateAvailable.Text = "Contact sunfunkits sales.";
                        lblTextforfirmwareUpdateAvailable.TextColor = Colors.Red;
                    }

                }
                await ShowBusyindicatior(false);
            }
            catch (HttpRequestException ex)
            {
                await ShowBusyindicatior(false);
                lblTextforfirmwareUpdateAvailable.Text = "Unable to connect to the server. Please check your internet connection.";
                lblTextforfirmwareUpdateAvailable.TextColor = Colors.Red;
            }
            catch (Exception ex)
            {
                await ShowBusyindicatior(false);
                lblTextforfirmwareUpdateAvailable.Text = "An unexpected error occurred. Please try again later or Contact sunfunkits sales.";
                lblTextforfirmwareUpdateAvailable.TextColor = Colors.Red;
            }
        }
        private async void btnClickToFirmWareUptoDate_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!bFirmwareUptodate)
                {
                    await ShowDisplayPopup("WarningConfirmation", "Firmware (" + FirmwareVersion + ") already installed. Would you like to reinstall it?");
                    bFirmwareUptodate = true;
                }
            }
            catch (Exception ex)
            {
            }
        }

        #region SFK_H2 New Firmware Update By Bluetooth     

        private async void btnClickToProceedForH2_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!ProceedForFirmwareUpgradeForH2)
                {
                    await ShowDisplayPopup("Firwware Warning", "Warning! Do not close the app while the update is happening, if updating via Bluetooth disconnect the USB-C and or RS-485 connections. \nAfter the update is complete you may need to press the Bluetooth icon on the battery to enable Bluetooth access again.");
                    ProceedForFirmwareUpgradeForH2 = true;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void btnUpgradeH2FirmwareByBluetooth()
        {
            try
            {
                RunMainPageWhileLoop = false;
                BreakWhileLoop = true;
                stackProgressBarMain.IsVisible = true;
                stackBusyFirmwareUpgrade.IsVisible = true;

                await SendCommandToBMS(Encoding.UTF8.GetBytes("START"), false);

                int chunkSize = 500 - 3; int offset = 0; int lastPercent = -1;
                byte[] fileData = Convert.FromBase64String(strH2FirmwareData);

                while (offset < fileData.Length)
                {
                    if (bFirmwareUpdated)
                    {
                        break;
                    }

                    int size = Math.Min(chunkSize, fileData.Length - offset);

                    byte[] chunk = new byte[size];
                    Array.Copy(fileData, offset, chunk, 0, size);

                    await SendCommandToBMS(chunk, false);

                    offset += size;

                    int percent = (int)((offset * 100.0) / fileData.Length);

                    if (percent != lastPercent && percent < 100)
                    {
                        lastPercent = percent;
                        SetProgress(percent);
                    }
                }

                await SendCommandToBMS(Encoding.UTF8.GetBytes("END"), false);

                SetProgress(100);

                stackProgressBarMain.IsVisible = false;
                stackBusyFirmwareUpgrade.IsVisible = false;
                H2FirmwareUpgradeCompleted = true;
                await ShowDisplayPopup("Success", "Firmware update. The app will now go back to the scan screen,\nyou may need to enable Bluetooth on the battery again before it will appear on the device list.");
            }
            catch (Exception ex)
            {
                await ShowDisplayPopup("Error", "Failed to upgrade. Please try again or contact sales.");
            }
        }
        public async void SetProgress(int per = 0)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ProgressBarPointer.Progress = per;
                    lblProgressBar.Text = $"{per} %";
                });
            }
            catch (Exception ex)
            {
            }
        }
        #endregion SFK_H2 New Firmware Update By Bluetooth

        #endregion Check And Proceed For Firmware Update

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

                        if (currentPage is DeviceDetails)
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

        private async void CheckBoxChange_CheckChanged(object sender, EventArgs e)
        {
            try
            {
                string strData = string.Empty;
                InputKit.Shared.Controls.CheckBox checkBox = (InputKit.Shared.Controls.CheckBox)sender;
                strData = checkBox.ClassId + "_" + checkBox.IsChecked;

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.CheckBoxValueChange, strData);
            }
            catch (Exception ex)
            {
            }
        }

        private async void SfSliderValueChange_ValueChangeEnd(object sender, EventArgs e)
        {
            try
            {
                string strData = string.Empty;
                Syncfusion.Maui.Sliders.SfSlider sfSlider = (Syncfusion.Maui.Sliders.SfSlider)sender;
                strData = sfSlider.ClassId + "_" + sfSlider.Value;

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.SfSliderValueChange, strData);
            }
            catch (Exception ex)
            {
            }
        }

        private async void RadioButtonChange_Clicked(object sender, EventArgs e)
        {
            try
            {
                string strData = string.Empty;
                InputKit.Shared.Controls.RadioButton radiobutton = (InputKit.Shared.Controls.RadioButton)sender;
                strData = radiobutton.ClassId + "_" + radiobutton.IsChecked;

                await RemoteSessionDetails.SendDataToRemoteSession(RemoteSessionSendCommandTypes.RadioButtonValueChange, strData);
            }
            catch (Exception ex)
            {
            }
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

        #endregion Remote Session                
    }
}