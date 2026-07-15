using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SFKBle_Admin
{
    public class SFKViewModel
    {
        public ObservableCollection<VirualBattery>? virualBatteryList { get; set; }
        public ObservableCollection<MinimumAndMaximumSOC>? MaximumSOCList { get; set; }
        public ObservableCollection<MinimumAndMaximumSOC>? MinimumSOCList { get; set; }
    }
    public class CurrentError
    {
        public string? ErrorType { get; set; }
        public bool? IsError { get; set; }
        public string? ImageScource { get; set; }

    }
    public class VirualBattery
    {
        public string? Name { get; set; }
    }
    public class MinimumAndMaximumSOC
    {
        public string? Value { get; set; }
    }
    public class RadioButtonViewModel
    {
        public bool[]? PullRequestData { get; set; }
        public bool[]? ConnectionEstablishData { get; set; }
        public bool[]? MaximumSocData { get; set; }
        public bool[]? TemperatureUnit { get; set; }
        public bool[]? BatterySetupType { get; set; }
        public bool[]? FaultReleaseData { get; set; }
    }
    public class BatterySetup
    {
        public string? Name { get; set; }
        public string? ImagePath { get; set; }
    }
    public class DisplayDevice
    {
        public string? ImageSrcSignalStrength { get; set; }
        public string? DeviceName { get; set; }
        public string? MacAddress { get; set; }
        public string? DeviceDetail { get; set; }
        public string? ImageName { get; set; }
        public bool IsVisible { get; set; }
        public int PortNumber { get; set; }
        public bool Selected { get; set; }
        public bool IsRestore { get; set; }
        public bool IsChecked { get; set; }
        public bool BarcodeReadSucces { get; set; }
        public bool IsDeviceAttached { get; set; }
    }
    public class BusyIndicatorViewModel
    {
        public bool IsBusy { get; set; }
        public bool IsVisible { get; set; }
        public string? Message { get; set; }
    }
    public class BusyIndicatorReconnectionViewModel
    {
        public bool IsBusy { get; set; }
        public bool IsVisible { get; set; }
        public string? Message { get; set; }
    }
    public class MultiDeviceViewModel
    {
        public List<BLEDevice>? BlEDeviceList { get; set; }
        public decimal TotalVoltes { get; set; }
        public decimal TotalAmps { get; set; }
        public decimal TotalWatts { get; set; }
        public double TotalSOC { get; set; }
        public decimal[]? arrTotalVoltes { get; set; }
        public decimal[]? arrTotalAmps { get; set; }
        public decimal[]? arrTotalWatts { get; set; }
        public double[]? arrTotalSOC { get; set; }
        public double[]? arrFirmwareVersion { get; set; }
        public decimal[]? arrCaseTemp1 { get; set; }
        public decimal[]? arrCaseTemp2 { get; set; }
        public decimal AvgVoltes { get; set; }
        public decimal AvgAmps { get; set; }

    }
    public class BLEDevice
    {
        public string? DeviceName { get; set; }
        public string? DeviceMacAddress { get; set; }
        public string? SOC { get; set; }
        public string? RemainingAH { get; set; }
        public decimal Voltes { get; set; }
        public decimal Amps { get; set; }
        public decimal Amp_Hours { get; set; }
        public decimal Total_AmpHours { get; set; }
        public decimal Watts { get; set; }
        public decimal Watt_Hours { get; set; }
        public decimal Total_WattHours { get; set; }
        public decimal BMSTempreture { get; set; }
        public decimal CaseTempreture { get; set; }
        public decimal CaseTempreture2 { get; set; }
        public decimal Cell1volt { get; set; }
        public decimal Cell2volt { get; set; }
        public decimal Cell3volt { get; set; }
        public decimal Cell4volt { get; set; }
        public decimal Cell5volt { get; set; }
        public decimal Cell6volt { get; set; }
        public decimal Cell7volt { get; set; }
        public decimal Cell8volt { get; set; }
        public bool IsChargingOn { get; set; }
        public bool IsDischargingOn { get; set; }
        public bool ProceedForChargingOn { get; set; }
        public bool ProceedForDischargingOn { get; set; }
        public int MaxSocValue { get; set; }
        public string? BarcodeValue { get; set; }
        public double FirmwareVeriosn { get; set; }

        //New Firmware
        public decimal HeatingStartTempValue { get; set; }
        public decimal HeatingStopTempValue { get; set; }
        public int HeatingModeValue { get; set; }
        public string? HeatingMode { get; set; }
        public int SocProtectionBitValue { get; set; }
        public int SocMaxValue { get; set; }
        public int SocMinValue { get; set; }
        public string SOCHeatingModeHex { get; set; }
        public int LowTempValue { get; set; }
        public double HeatingTempValue { get; set; }
        public double IsvalidLowTempValue { get; set; }
        public bool ChargingMOSFET { get; set; }
        public bool DischargingMOSFET { get; set; }
        public int OverAmpTimeOutReadValue { get; set; }
        public double IsValidLowTempDischargeValue { get; set; }
        public bool bValidLowTempDischarge { get; set; }
        public int BatteryType { get; set; }
        public bool IsChargePulseOn { get; set; }
        public bool IsHighSOCMonitorOn { get; set; }
    }
    public class FileViewModel
    {
        public int TestID { get; set; }
        public string? FileFullPath { get; set; }
        public string? FileName { get; set; }
        public string? DisplayFileName { get; set; }
        public string? TestType { get; set; }
        public Color TestTypeColor { get; set; }
        public string? DateTime { get; set; }
        public double TotalSeccounds { get; set; }
        public string? URL { get; set; }
        public bool UploadResultIsVisibled { get; set; }
        public bool ViewResultIsVisibled { get; set; }
    }
    public class Capacity_ViewModel
    {
        //Validation ReadValues
        public bool isValidSingleFull { get; set; }
        public bool isValidSingleCutOff { get; set; }
        public bool isValid100Capacity { get; set; }
        public bool isValid90Capacity { get; set; }
        public bool isValid80Capacity { get; set; }
        public bool isValid70Capacity { get; set; }
        public bool isValid60Capacity { get; set; }
        public bool isValid50Capacity { get; set; }
        public bool isValid40Capacity { get; set; }
        public bool isValid30Capacity { get; set; }
        public bool isValid20Capacity { get; set; }
        public bool isValid10Capacity { get; set; }

        //Store Read Values
        public double SingleFullCapacityValue { get; set; }
        public double SingleCutOffCapacityValue { get; set; }
        public double Capacity100Value { get; set; }
        public double Capacity90Value { get; set; }
        public double Capacity80Value { get; set; }
        public double Capacity70Value { get; set; }
        public double Capacity60Value { get; set; }
        public double Capacity50Value { get; set; }
        public double Capacity40Value { get; set; }
        public double Capacity30Value { get; set; }
        public double Capacity20Value { get; set; }
        public double Capacity10Value { get; set; }
    }
    public class VoltageCalibration : INotifyPropertyChanged
    {
        public Color BackgroundColor { get; set; }
        public Microsoft.Maui.Graphics.Color TextColor { get; set; }
        public string? BatteryImage { get; set; }
        public string? CellName { get; set; }
        private decimal _cellvoltage;
        public decimal CellVoltage
        {
            get => _cellvoltage;
            set
            {
                if (_cellvoltage != value)
                {
                    _cellvoltage = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool isSelected { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class TemperatureCalibration : INotifyPropertyChanged
    {
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public string? TempratureImage { get; set; }
        public string? TempName { get; set; }
        private decimal _tempValue;
        public decimal TempValue
        {
            get => _tempValue;
            set
            {
                if (_tempValue != value)
                {
                    _tempValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public bool isSelected { get; set; }
    }


    public class ValidReadValues
    {
        //Validation ReadValues
        public double IsvalidMaxSOCReadValue { get; set; }
        public double IsvalidMinSOCReadValue { get; set; }
        public double IsvalidFullCapacityValue { get; set; }
        public double IsvalidCalibrateTempValue { get; set; }
        public double IsvalidLowTempValue { get; set; }
        public double IsValidLowTempDischargeValue { get; set; }
        public double IsValidFaultReleaseValue { get; set; }
        public double IsvalidChargeMaxBatteryTempValue { get; set; }
        public double IsvalidDischargeMaxBatteryTempValue { get; set; }
        public double IsvalidCalibrateSOC { get; set; }
        public bool bCalibrateSOCSuccess { get; set; }

        //Store Read Values
        public int MaxChargeAmpsReadValue { get; set; }
        public int MaxDischargeAmpsReadValue { get; set; }
        public int OverAmpTimeOutReadValue { get; set; }
        public string MaxDischargeDelayReadValue { get; set; }
        public int MaxSOCReadValue { get; set; }
        public int MinSOCReadValue { get; set; }
        public int FullCapacityValue { get; set; }
        public int CalibrateTempReadValue { get; set; }
        public int chargeMaxBatteryTempValue { get; set; }
        public int dischargeMaxBatteryTempValue { get; set; }
        public int LowTempValue { get; set; }
        public int LowTempDischargeValue { get; set; }
        public int CycleCountValue { get; set; }
        public int FaultReleaseValue { get; set; }
        public List<decimal>? VoltageCalibrationValue { get; set; }
        public int CalibrateSOCValue { get; set; }
        public int CurrentSOC { get; set; }

        //H2 Board
        public bool IsHighSOCMonitorOn { get; set; } = false;
        public bool IsActiveBalancerOn { get; set; } = false;
        public bool IsActivationVoltage { get; set; } = false;
        public bool IsSOCParamSet { get; set; } = false;
        public bool IsChargePulseOn { get; set; } = false;
        public bool IsChargePulseTimeSet { get; set; } = false;

        public bool IsBleGain { get; set; } = false;
        public bool IsBalancerCutOff { get; set; } = false;
        public int BatteryType { get; set; }
        public bool IsDeviceBind { get; set; }
        public bool IsDebugModeOn { get; set; }
        public int DaySinceFullCharge { get; set; }
        public bool IsEnableBLERestart { get; set; }

        //New Firmware
        public decimal HeatingStartTempValue { get; set; }
        public decimal HeatingStopTempValue { get; set; }
        public int HeatingModeValue { get; set; }
        public int SocProtectionBitValue { get; set; }
        public int SocMaxValue { get; set; }
        public int SocMinValue { get; set; }
        public bool _IsSFKH2Device { get; set; } = false;
        public string H2FirmwareVersion { get; set; }
    }

    public class FuelIndicator
    {
        public int ID { get; set; }
        public bool ShowArrow { get; set; }
        public bool IsVertical { get; set; }
        public bool IsHorizontal { get; set; }
        public Color IndicatorColor { get; set; }
        public double IndicatorHeight { get; set; }
        public double IndicatorWidth { get; set; }
        public Thickness IndicatorMargin { get; set; }
    }

    public class RangeSliderItem
    {
        public string Label { get; set; }
        public double Value { get; set; }
    }

    public class DeviceInformation
    {
        public string? ModelName { get; set; }
        public bool LowTemperatureHeatingSupport { get; set; }
        public bool Benchmark { get; set; }
        public bool ShowWarranty { get; set; }
        public bool NewFirmwareUpdate { get; set; }
        public bool HeatingPadWarning { get; set; }
        public int MaxDisChargeAmpsStart { get; set; }
        public int MaxDisChargeAmpsEnd { get; set; }
        public int MaxDisChargeAmpsInterval { get; set; }
        public int MaxChargeAmpsStart { get; set; }
        public int MaxChargeAmpsEnd { get; set; }
        public int MaxChargeAmpsInterval { get; set; }
        public int CapacityInterval = 20;
        public bool Is24VBattery { get; set; }
        public bool ShowFullCapacityPart { get; set; }
        public int FullCapacityValue { get; set; }
        public bool HasCaseTemp2 = true; // { get; set; }
        public double ActivationVoltageInterval { get; set; }
        public int DeviationLimitInterval { get; set; }
        public int SOCTimeInterval { get; set; }
        public int HighSOCLimitInterval { get; set; }
        public int ChargePulseInterval { get; set; }
        public int BleGainInterval { get; set; }

        public bool WriteReadOddValueCapacity = true;
        public decimal MultiplicationConstant { get; set; } = 1.000M;
        public bool OverAmpTimeOut { get; set; } = false;
        public int DefaultOverAmpTimeOut { get; set; } = 0;
        public int CalibrateSOCInterval = 20;
        public bool RestoreDefaultValue { get; set; } = false;

        public ObservableCollection<RangeSliderItem> CustomCollectionMaxChargeAmps { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionMaxDisChargeAmps { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionFullCapacity { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionLowVoltageCutOff { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionMaxSoc { get; set; }
        public ObservableCollection<RangeSliderItem> NF_CustomCollectionMinSoc { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionActivationVoltage { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionDeviationLimit { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionTimeInterval { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionHighSOCLimit { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionChargePulse { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionMaxBatteryTemp { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionMaxBatteryTempCelsius { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionLowTemp { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionLowTempCelsius { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionPullRequestSecond { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionAmpsDraw { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionVoltsDraw { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionMaxSpeed { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionMaxPower { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionVoltageDiviation { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionBalanceTemperatureLimitCelsius { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionBalanceTemperatureLimit { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionBalanceVoltageLimit { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionNetworkMode { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionBleGain { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionMultiHeatingTemp { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionMultiHeatingTempCelsius { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionCalibrateSOC { get; set; }
        public ObservableCollection<RangeSliderItem> CustomCollectionHeatingPadTimer { get; set; }
    }
    public class FirmwareInfo
    {
        public string FirmwareVersion { get; set; }
        public string ReleaseNotes { get; set; }
        public string UploadedOn { get; set; }
        public string DisplayFirmwareVersion { get; set; }

        public string DisplayTextFirmwareVersion
        {
            get
            {
                return string.IsNullOrEmpty(DisplayFirmwareVersion)
                    ? FirmwareVersion
                    : DisplayFirmwareVersion;
            }
        }
    }
}

