using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace SFKBle_Admin.SFK_Protocol
{
    public class DeviceDefaultValues
    {
        public static readonly string[] BMSDeviceList = { "sfk", "300", "3000", "275", "2750", "27500", "2000", "1500", "1450", "1001", "340elr" };

        public static readonly string[] BMS8Cells = { "1450", "sfk145hpx", "sfk8s150", "sfk8s100", "1001" };
        
        public static readonly string[] H2Registers = { "D8", "D9", "F9", "D4", "F3", "A1" };

        private static readonly Dictionary<string, Func<DeviceInformation>> DeviceFactory = new(StringComparer.OrdinalIgnoreCase)
        {
            // 8 Cells
            { "sfk8s100", () => new SFK8S100() }, { "1001", () => new SFK8S100() },
            { "sfk145hpx", () => new SFK145HPX() }, { "1450", () => new SFK145HPX() },
            { "sfk8s150", () => new SFK8S150() },
            
            // 4 Cells

            { "sfkkit", () => new SFKKIT() },

            { "sfk260hp", () => new SFK260HP() },
            { "sfk300s",  () => new SFK300S()  }, { "3000", () => new SFK300S()  },
            { "sfk275hp", () => new SFK275HP() },

            { "sfk100",   () => new SFK100()   },
            { "sfk150v2", () => new SFK150V2() }, { "1500", () => new SFK150V2() },
            { "sfk200v2", () => new SFK200V2() }, { "2000", () => new SFK200V2() },
            { "sfk200",   () => new SFK200()   },
            { "sfk260", () => new SFK260() },

            { "sfk300hp", () => new SFK300HP() }, { "300", () => new SFK300HP() },
            { "sfk275se", () => new SFK275SE() }, { "27500", () => new SFK275SE() },

            { "sfk260ex", () => new SFK260EX() },
            { "sfk275ex", () => new SFK275EX() }, { "275", () => new SFK275EX() }, { "2750", () => new SFK275EX() },
            { "sfk315ex", () => new SFK315EX() },

            { "340elr", () => new SFK340ELR() },

            { "sfkv6r", () => new SFKV6R() },
            { "sfkv6e", () => new SFKV6E() },
        };

        public async static Task<DeviceInformation> GetDeviceData(string DeviceName = "")
        {
            try
            {
                string normalizedName = Regex.Replace(Convert.ToString(DeviceName).ToLower().Trim(), "[^0-9a-zA-Z:,]+", "");
                foreach (var key in DeviceFactory.Keys)
                {
                    if (normalizedName.StartsWith(key.ToLower()))
                    {
                        DeviceName = key; break;
                    }
                }
                if (DeviceFactory.TryGetValue(DeviceName, out var objDeviceInformation))
                {
                    return objDeviceInformation();
                }
            }
            catch (Exception ex) { }
            return new DeviceInformation();
        }
        public async static Task<IProtocol> GetProtocolDetails(string Protocol = "")
        {
            switch (Protocol)
            {
                case "SFKV1":
                    return new SFKV1Protocol();
                case "SFKV2":
                    return new SFKV2Protocol();
                case "SFKV3":
                    return new SFKV3Protocol();
                case "SFKV4":
                    return new SFKV4Protocol();
            }
            return new SFKV1Protocol();
        }
    }

    #region Device Information

    public class SFK275EX : DeviceInformation
    {
        public SFK275EX()
        {
            ModelName = "SFK-275EX";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = true;

            NewFirmwareUpdate = true;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 40;

            MaxChargeAmpsEnd = 160;

            MaxChargeAmpsInterval = 40;

            MaxDisChargeAmpsStart = 50;

            MaxDisChargeAmpsEnd = 200;

            MaxDisChargeAmpsInterval = 50;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            FullCapacityValue = 275;

            MultiplicationConstant = 1.000M;

            OverAmpTimeOut = true;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = true;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
                new RangeSliderItem { Label = "-50", Value = 50 },
                new RangeSliderItem { Label = "-100", Value = 100 },
                new RangeSliderItem { Label = "-150", Value = 150 },
                new RangeSliderItem { Label = "-200", Value = 200 }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "40", Value = 40 },
            new RangeSliderItem { Label = "80", Value = 80 },
            new RangeSliderItem { Label = "120", Value = 120 },
            new RangeSliderItem { Label = "160", Value = 160  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK300HP : DeviceInformation
    {
        public SFK300HP()
        {
            ModelName = "SFK-300HP";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = true;

            NewFirmwareUpdate = true;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 40;

            MaxChargeAmpsEnd = 160;

            MaxChargeAmpsInterval = 40;

            MaxDisChargeAmpsStart = 50;

            MaxDisChargeAmpsEnd = 200;

            MaxDisChargeAmpsInterval = 50;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            FullCapacityValue = 300;

            MultiplicationConstant = 1.000M;

            OverAmpTimeOut = true;

            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = true;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-150", Value = 150 },
            new RangeSliderItem { Label = "-200", Value = 200  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "40", Value = 40 },
            new RangeSliderItem { Label = "80", Value = 80 },
            new RangeSliderItem { Label = "120", Value = 120 },
            new RangeSliderItem { Label = "160", Value = 160 }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320 }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK275HP : DeviceInformation
    {
        public SFK275HP()
        {
            ModelName = "SFK-275HP";

            LowTemperatureHeatingSupport = false;

            Benchmark = true;

            ShowWarranty = true;

            NewFirmwareUpdate = false;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 10;

            MaxChargeAmpsEnd = 110;

            MaxChargeAmpsInterval = 20;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 150;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            FullCapacityValue = 275;

            MultiplicationConstant = 1.0075M;

            OverAmpTimeOut = false;

            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = true;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-125", Value = 125 },
            new RangeSliderItem { Label = "-150", Value = 150  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10", Value = 10 },
            new RangeSliderItem { Label = "30", Value = 30 },
            new RangeSliderItem { Label = "50", Value = 50 },
            new RangeSliderItem { Label = "70", Value = 70 },
            new RangeSliderItem { Label = "90", Value = 90 },
            new RangeSliderItem { Label = "110", Value = 110  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13 }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK260HP : DeviceInformation
    {
        public SFK260HP()
        {
            ModelName = "SFK-260HP";

            LowTemperatureHeatingSupport = false;

            Benchmark = true;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = false;

            MaxChargeAmpsStart = 10;

            MaxChargeAmpsEnd = 110;

            MaxChargeAmpsInterval = 20;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 150;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            HasCaseTemp2 = false;

            FullCapacityValue = 260;

            WriteReadOddValueCapacity = false;

            MultiplicationConstant = 1.0075M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-125", Value = 125 },
            new RangeSliderItem { Label = "-150", Value = 150  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10", Value = 10 },
            new RangeSliderItem { Label = "30", Value = 30 },
            new RangeSliderItem { Label = "50", Value = 50 },
            new RangeSliderItem { Label = "70", Value = 70 },
            new RangeSliderItem { Label = "90", Value = 90 },
            new RangeSliderItem { Label = "110", Value = 110  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFKKIT : DeviceInformation
    {
        public SFKKIT()
        {
            ModelName = "SFK-150A BMS";

            LowTemperatureHeatingSupport = false;

            Benchmark = false;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = false;

            MaxChargeAmpsStart = 10;

            MaxChargeAmpsEnd = 110;

            MaxChargeAmpsInterval = 20;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 150;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = false;

            ShowFullCapacityPart = true;

            HasCaseTemp2 = false;

            FullCapacityValue = 150;

            WriteReadOddValueCapacity = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-125", Value = 125 },
            new RangeSliderItem { Label = "-150", Value = 150 }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10", Value = 10 },
            new RangeSliderItem { Label = "30", Value = 30 },
            new RangeSliderItem { Label = "50", Value = 50 },
            new RangeSliderItem { Label = "70", Value = 70 },
            new RangeSliderItem { Label = "90", Value = 90 },
            new RangeSliderItem { Label = "110", Value = 110 }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK275SE : DeviceInformation
    {
        public SFK275SE()
        {
            ModelName = "SFK-275SE";

            LowTemperatureHeatingSupport = false;

            Benchmark = true;

            ShowWarranty = true;

            NewFirmwareUpdate = true;

            HeatingPadWarning = false;

            MaxChargeAmpsStart = 10;

            MaxChargeAmpsEnd = 110;

            MaxChargeAmpsInterval = 20;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 150;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            FullCapacityValue = 275;

            MultiplicationConstant = 1.0075M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = true;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-125", Value = 125 },
            new RangeSliderItem { Label = "-150", Value = 150  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10", Value = 10 },
            new RangeSliderItem { Label = "30", Value = 30 },
            new RangeSliderItem { Label = "50", Value = 50 },
            new RangeSliderItem { Label = "70", Value = 70 },
            new RangeSliderItem { Label = "90", Value = 90 },
            new RangeSliderItem { Label = "110", Value = 110  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK300S : DeviceInformation
    {
        public SFK300S()
        {
            ModelName = "SFK-300S";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = true;

            NewFirmwareUpdate = true;

            HeatingPadWarning = false;

            MaxChargeAmpsStart = 10;

            MaxChargeAmpsEnd = 110;

            MaxChargeAmpsInterval = 20;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 150;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            FullCapacityValue = 300;

            MultiplicationConstant = 1.000M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-125", Value = 125 },
            new RangeSliderItem { Label = "-150", Value = 150 }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10", Value = 10 },
            new RangeSliderItem { Label = "30", Value = 30 },
            new RangeSliderItem { Label = "50", Value = 50 },
            new RangeSliderItem { Label = "70", Value = 70 },
            new RangeSliderItem { Label = "90", Value = 90 },
            new RangeSliderItem { Label = "110", Value = 110 }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK150V2 : DeviceInformation
    {
        public SFK150V2()
        {
            ModelName = "SFK-150A BMS V2";

            LowTemperatureHeatingSupport = false;

            Benchmark = true;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = false;

            MaxChargeAmpsStart = 10;

            MaxChargeAmpsEnd = 110;

            MaxChargeAmpsInterval = 20;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 150;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = false;

            ShowFullCapacityPart = true;

            FullCapacityValue = 300;

            MultiplicationConstant = 1.0075M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-125", Value = 125 },
            new RangeSliderItem { Label = "-150", Value = 150  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10", Value = 10 },
            new RangeSliderItem { Label = "30", Value = 30 },
            new RangeSliderItem { Label = "50", Value = 50 },
            new RangeSliderItem { Label = "70", Value = 70 },
            new RangeSliderItem { Label = "90", Value = 90 },
            new RangeSliderItem { Label = "110", Value = 110  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK100 : DeviceInformation
    {
        public SFK100()
        {
            ModelName = "SFK-100A BMS";

            LowTemperatureHeatingSupport = false;

            Benchmark = false;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = false;

            MaxChargeAmpsStart = 5;

            MaxChargeAmpsEnd = 45;

            MaxChargeAmpsInterval = 10;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 100;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = false;

            ShowFullCapacityPart = true;

            FullCapacityValue = 300;

            WriteReadOddValueCapacity = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "5", Value = 5 },
            new RangeSliderItem { Label = "15", Value = 15 },
            new RangeSliderItem { Label = "25", Value = 25 },
            new RangeSliderItem { Label = "35", Value = 35 },
            new RangeSliderItem { Label = "45", Value = 45  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK200V2 : DeviceInformation
    {
        public SFK200V2()
        {
            ModelName = "SFK-200 V2";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 40;

            MaxChargeAmpsEnd = 160;

            MaxChargeAmpsInterval = 40;

            MaxDisChargeAmpsStart = 50;

            MaxDisChargeAmpsEnd = 200;

            MaxDisChargeAmpsInterval = 50;

            Is24VBattery = false;

            ShowFullCapacityPart = true;

            FullCapacityValue = 300;

            MultiplicationConstant = 1.000M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-150", Value = 150 },
            new RangeSliderItem { Label = "-200", Value = 200 }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "40", Value = 40 },
            new RangeSliderItem { Label = "80", Value = 80 },
            new RangeSliderItem { Label = "120", Value = 120 },
            new RangeSliderItem { Label = "160", Value = 160 }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320 }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK200 : DeviceInformation
    {
        public SFK200()
        {
            ModelName = "SFK-200A BMS";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = false;

            MaxChargeAmpsStart = 5;

            MaxChargeAmpsEnd = 45;

            MaxChargeAmpsInterval = 10;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 100;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = false;

            ShowFullCapacityPart = true;

            FullCapacityValue = 300;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "5", Value = 5 },
            new RangeSliderItem { Label = "15", Value = 15 },
            new RangeSliderItem { Label = "25", Value = 25 },
            new RangeSliderItem { Label = "35", Value = 35 },
            new RangeSliderItem { Label = "45", Value = 45  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320 }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK260EX : DeviceInformation
    {
        public SFK260EX()
        {
            ModelName = "SFK-260EX";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = true;

            NewFirmwareUpdate = false;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 40;

            MaxChargeAmpsEnd = 160;

            MaxChargeAmpsInterval = 40;

            MaxDisChargeAmpsStart = 50;

            MaxDisChargeAmpsEnd = 200;

            MaxDisChargeAmpsInterval = 50;

            Is24VBattery = false;

            ShowFullCapacityPart = true;

            FullCapacityValue = 260;

            MultiplicationConstant = 1.000M;

            OverAmpTimeOut = true;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-150", Value = 150 },
            new RangeSliderItem { Label = "-200", Value = 200  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "40", Value = 40 },
            new RangeSliderItem { Label = "80", Value = 80 },
            new RangeSliderItem { Label = "120", Value = 120 },
            new RangeSliderItem { Label = "160", Value = 160  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320 }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK260 : DeviceInformation
    {
        public SFK260()
        {
            ModelName = "SFK-260";

            LowTemperatureHeatingSupport = false;

            Benchmark = false;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = false;

            MaxChargeAmpsStart = 5;

            MaxChargeAmpsEnd = 75;

            MaxChargeAmpsInterval = 10;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 125;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            HasCaseTemp2 = false;

            FullCapacityValue = 260;

            WriteReadOddValueCapacity = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-125", Value = 125  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "5", Value = 5 },
            new RangeSliderItem { Label = "15", Value = 15 },
            new RangeSliderItem { Label = "25", Value = 25 },
            new RangeSliderItem { Label = "35", Value = 35 },
            new RangeSliderItem { Label = "45", Value = 45 },
            new RangeSliderItem { Label = "55", Value = 55 },
            new RangeSliderItem { Label = "65", Value = 65 },
            new RangeSliderItem { Label = "75", Value = 75  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13 }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK315EX : DeviceInformation
    {
        public SFK315EX()
        {
            ModelName = "SFK315EX";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = true;

            NewFirmwareUpdate = false;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 40;

            MaxChargeAmpsEnd = 160;

            MaxChargeAmpsInterval = 40;

            MaxDisChargeAmpsStart = 50;

            MaxDisChargeAmpsEnd = 200;

            MaxDisChargeAmpsInterval = 50;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            FullCapacityValue = 315;

            MultiplicationConstant = 1.000M;

            OverAmpTimeOut = true;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = true;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-150", Value = 150 },
            new RangeSliderItem { Label = "-200", Value = 200  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "40", Value = 40 },
            new RangeSliderItem { Label = "80", Value = 80 },
            new RangeSliderItem { Label = "120", Value = 120 },
            new RangeSliderItem { Label = "160", Value = 160  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK340ELR : DeviceInformation
    {
        public SFK340ELR()
        {
            ModelName = "340ELR";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = true;

            NewFirmwareUpdate = false;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 30;

            MaxChargeAmpsEnd = 120;

            MaxChargeAmpsInterval = 30;

            MaxDisChargeAmpsStart = 40;

            MaxDisChargeAmpsEnd = 160;

            MaxDisChargeAmpsInterval = 40;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            FullCapacityValue = 340;

            MultiplicationConstant = 1.000M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 5;

            RestoreDefaultValue = true;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-40", Value = 40 },
            new RangeSliderItem { Label = "-80", Value = 80 },
            new RangeSliderItem { Label = "-120", Value = 120 },
            new RangeSliderItem { Label = "-160", Value = 160 }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "30", Value = 30 },
            new RangeSliderItem { Label = "60", Value = 60 },
            new RangeSliderItem { Label = "90", Value = 90  },
            new RangeSliderItem { Label = "120", Value = 120 }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  },
            new RangeSliderItem { Label = "340", Value = 340  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFKV6R : DeviceInformation
    {
        public SFKV6R()
        {
            ModelName = "SFKV6R";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 5;

            MaxChargeAmpsEnd = 75;

            MaxChargeAmpsInterval = 10;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 125;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            FullCapacityValue = 320;

            MultiplicationConstant = 1.000M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-125", Value = 125  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "5", Value = 5 },
            new RangeSliderItem { Label = "15", Value = 15 },
            new RangeSliderItem { Label = "25", Value = 25 },
            new RangeSliderItem { Label = "35", Value = 35 },
            new RangeSliderItem { Label = "45", Value = 45 },
            new RangeSliderItem { Label = "55", Value = 55 },
            new RangeSliderItem { Label = "65", Value = 65 },
            new RangeSliderItem { Label = "75", Value = 75  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFKV6E : DeviceInformation
    {
        public SFKV6E()
        {
            ModelName = "SFKV6E";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 40;

            MaxChargeAmpsEnd = 160;

            MaxChargeAmpsInterval = 40;

            MaxDisChargeAmpsStart = 50;

            MaxDisChargeAmpsEnd = 200;

            MaxDisChargeAmpsInterval = 50;

            Is24VBattery = false;

            ShowFullCapacityPart = false;

            FullCapacityValue = 320;

            MultiplicationConstant = 1.000M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-150", Value = 150 },
            new RangeSliderItem { Label = "-200", Value = 200  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "40", Value = 40 },
            new RangeSliderItem { Label = "80", Value = 80 },
            new RangeSliderItem { Label = "120", Value = 120 },
            new RangeSliderItem { Label = "160", Value = 160  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "240", Value = 240 },
            new RangeSliderItem { Label = "260", Value = 260 },
            new RangeSliderItem { Label = "280", Value = 280 },
            new RangeSliderItem { Label = "300", Value = 300 },
            new RangeSliderItem { Label = "320", Value = 320  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10.0v", Value = 10 },
            new RangeSliderItem { Label = "10.8v", Value = 11 },
            new RangeSliderItem { Label = "11.6v", Value = 12 },
            new RangeSliderItem { Label = "12v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    // 8 Cell bms
    public class SFK8S100 : DeviceInformation
    {
        public SFK8S100()
        {
            ModelName = "SFK-8S100";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 15;

            MaxChargeAmpsEnd = 75;

            MaxChargeAmpsInterval = 15;

            MaxDisChargeAmpsStart = 20;

            MaxDisChargeAmpsEnd = 100;

            MaxDisChargeAmpsInterval = 20;

            CapacityInterval = 15;

            Is24VBattery = true;

            ShowFullCapacityPart = true;

            FullCapacityValue = 260;

            MultiplicationConstant = 1.0075M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-20", Value = 20 },
            new RangeSliderItem { Label = "-40", Value = 40 },
            new RangeSliderItem { Label = "-60", Value = 60 },
            new RangeSliderItem { Label = "-80", Value = 80 },
            new RangeSliderItem { Label = "-100", Value = 100  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "15", Value = 15 },
            new RangeSliderItem { Label = "30", Value = 30 },
            new RangeSliderItem { Label = "45", Value = 45 },
            new RangeSliderItem { Label = "60", Value = 60 },
            new RangeSliderItem { Label = "75", Value = 75  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "100", Value = 100 },
            new RangeSliderItem { Label = "115", Value = 115 },
            new RangeSliderItem { Label = "130", Value = 130 },
            new RangeSliderItem { Label = "145", Value = 145 },
            new RangeSliderItem { Label = "160", Value = 160  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "20.0v", Value = 10 },
            new RangeSliderItem { Label = "21.6v", Value = 11 },
            new RangeSliderItem { Label = "23.2v", Value = 12 },
            new RangeSliderItem { Label = "24v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK8S150 : DeviceInformation
    {
        public SFK8S150()
        {
            ModelName = "SFK-8S150";

            LowTemperatureHeatingSupport = false;

            Benchmark = true;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 10;

            MaxChargeAmpsEnd = 110;

            MaxChargeAmpsInterval = 20;

            MaxDisChargeAmpsStart = 25;

            MaxDisChargeAmpsEnd = 150;

            MaxDisChargeAmpsInterval = 25;

            Is24VBattery = true;

            ShowFullCapacityPart = true;

            CapacityInterval = 15;

            FullCapacityValue = 260;

            MultiplicationConstant = 1.0075M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-25", Value = 25 },
            new RangeSliderItem { Label = "-50", Value = 50 },
            new RangeSliderItem { Label = "-75", Value = 75 },
            new RangeSliderItem { Label = "-100", Value = 100 },
            new RangeSliderItem { Label = "-125", Value = 125 },
            new RangeSliderItem { Label = "-150", Value = 150  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "10", Value = 10 },
            new RangeSliderItem { Label = "30", Value = 30 },
            new RangeSliderItem { Label = "50", Value = 50 },
            new RangeSliderItem { Label = "70", Value = 70 },
            new RangeSliderItem { Label = "90", Value = 90 },
            new RangeSliderItem { Label = "110", Value = 110  }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "100", Value = 100 },
            new RangeSliderItem { Label = "115", Value = 115 },
            new RangeSliderItem { Label = "130", Value = 130 },
            new RangeSliderItem { Label = "145", Value = 145 },
            new RangeSliderItem { Label = "160", Value = 160  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "20.0v", Value = 10 },
            new RangeSliderItem { Label = "21.6v", Value = 11 },
            new RangeSliderItem { Label = "23.2v", Value = 12 },
            new RangeSliderItem { Label = "24v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class SFK145HPX : DeviceInformation
    {
        public SFK145HPX()
        {
            ModelName = "SFK-145HPX";

            LowTemperatureHeatingSupport = true;

            Benchmark = true;

            ShowWarranty = false;

            NewFirmwareUpdate = false;

            HeatingPadWarning = true;

            MaxChargeAmpsStart = 15;

            MaxChargeAmpsEnd = 75;

            MaxChargeAmpsInterval = 15;

            MaxDisChargeAmpsStart = 20;

            MaxDisChargeAmpsEnd = 100;

            MaxDisChargeAmpsInterval = 20;

            Is24VBattery = true;

            ShowFullCapacityPart = false;

            CapacityInterval = 15;

            FullCapacityValue = 145;

            MultiplicationConstant = 1.0075M;

            OverAmpTimeOut = false;
            
            DefaultOverAmpTimeOut = 0;

            RestoreDefaultValue = false;

            CustomCollectionMaxDisChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "-20", Value = 20 },
            new RangeSliderItem { Label = "-40", Value = 40 },
            new RangeSliderItem { Label = "-60", Value = 60 },
            new RangeSliderItem { Label = "-80", Value = 80 },
            new RangeSliderItem { Label = "-100", Value = 100  }
            };

            CustomCollectionMaxChargeAmps = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "15", Value = 15 },
            new RangeSliderItem { Label = "30", Value = 30 },
            new RangeSliderItem { Label = "45", Value = 45 },
            new RangeSliderItem { Label = "60", Value = 60 },
            new RangeSliderItem { Label = "75", Value = 75 }
            };

            CustomCollectionFullCapacity = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "100", Value = 100 },
            new RangeSliderItem { Label = "115", Value = 115 },
            new RangeSliderItem { Label = "130", Value = 130 },
            new RangeSliderItem { Label = "145", Value = 145 },
            new RangeSliderItem { Label = "160", Value = 160  }
            };

            CustomCollectionLowVoltageCutOff = new ObservableCollection<RangeSliderItem>()
            {
            new RangeSliderItem { Label = "20.0v", Value = 10 },
            new RangeSliderItem { Label = "21.6v", Value = 11 },
            new RangeSliderItem { Label = "23.2v", Value = 12 },
            new RangeSliderItem { Label = "24v", Value = 13  }
            };

            // Assign Common Values
            CommonValues _ObjCommonValues = new CommonValues();
            CustomCollectionMaxSoc = _ObjCommonValues.CustomCollectionMaxSoc;
            NF_CustomCollectionMinSoc = _ObjCommonValues.NF_CustomCollectionMinSoc;
            CustomCollectionLowTemp = _ObjCommonValues.CustomCollectionLowTemp;
            CustomCollectionLowTempCelsius = _ObjCommonValues.CustomCollectionLowTempCelsius;
            CustomCollectionMaxBatteryTemp = _ObjCommonValues.CustomCollectionMaxBatteryTemp;
            CustomCollectionMaxBatteryTempCelsius = _ObjCommonValues.CustomCollectionMaxBatteryTempCelsius;
            CustomCollectionAmpsDraw = _ObjCommonValues.CustomCollectionAmpsDraw;
            CustomCollectionVoltsDraw = _ObjCommonValues.CustomCollectionVoltsDraw;
            CustomCollectionMaxSpeed = _ObjCommonValues.CustomCollectionMaxSpeed;
            CustomCollectionMaxPower = _ObjCommonValues.CustomCollectionMaxPower;
            CustomCollectionVoltageDiviation = _ObjCommonValues.CustomCollectionVoltageDiviation;
            CustomCollectionBalanceTemperatureLimitCelsius = _ObjCommonValues.CustomCollectionBalanceTemperatureLimitCelsius;
            CustomCollectionBalanceTemperatureLimit = _ObjCommonValues.CustomCollectionBalanceTemperatureLimit;
            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionBalanceVoltageLimit = _ObjCommonValues.CustomCollectionBalanceVoltageLimit;

            //H2 Board specific values
            ActivationVoltageInterval = _ObjCommonValues.ActivationVoltageInterval;
            DeviationLimitInterval = _ObjCommonValues.DeviationLimitInterval;
            SOCTimeInterval = _ObjCommonValues.SOCTimeInterval;
            HighSOCLimitInterval = _ObjCommonValues.HighSOCLimitInterval;
            ChargePulseInterval = _ObjCommonValues.ChargePulseInterval;
            BleGainInterval = _ObjCommonValues.BleGainInterval;

            CustomCollectionActivationVoltage = _ObjCommonValues.CustomCollectionActivationVoltage;
            CustomCollectionDeviationLimit = _ObjCommonValues.CustomCollectionDeviationLimit;
            CustomCollectionTimeInterval = _ObjCommonValues.CustomCollectionTimeInterval;
            CustomCollectionHighSOCLimit = _ObjCommonValues.CustomCollectionHighSOCLimit;
            CustomCollectionChargePulse = _ObjCommonValues.CustomCollectionChargePulse;
            CustomCollectionNetworkMode = _ObjCommonValues.CustomCollectionNetworkMode;
            CustomCollectionBleGain = _ObjCommonValues.CustomCollectionBleGain;
            CustomCollectionMultiHeatingTemp = _ObjCommonValues.CustomCollectionMultiHeatingTemp;
            CustomCollectionMultiHeatingTempCelsius = _ObjCommonValues.CustomCollectionMultiHeatingTempCelsius;
            CustomCollectionCalibrateSOC = _ObjCommonValues.CustomCollectionCalibrateSOC;
            CustomCollectionHeatingPadTimer = _ObjCommonValues.CustomCollectionHeatingPadTimer;
        }
    }

    public class CommonValues : DeviceInformation
    {
        public CommonValues()
        {
            ActivationVoltageInterval = 1;

            DeviationLimitInterval = 10;

            SOCTimeInterval = 12;

            HighSOCLimitInterval = 5;

            ChargePulseInterval = 1;

            BleGainInterval = 3;

            CustomCollectionMaxSoc = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "80%", Value = 80 },
                new RangeSliderItem { Label = "85%", Value = 85 },
                new RangeSliderItem { Label = "90%", Value = 90 },
                new RangeSliderItem { Label = "95%", Value = 95 },
                new RangeSliderItem { Label = "100%", Value = 100 }
            };

            NF_CustomCollectionMinSoc = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "0%", Value = 0 },
                new RangeSliderItem { Label = "5%", Value = 5 },
                new RangeSliderItem { Label = "10%", Value = 10 },
                new RangeSliderItem { Label = "15%", Value = 15 },
                new RangeSliderItem { Label = "20%", Value = 20 }
            };

            CustomCollectionLowTemp = new ObservableCollection<RangeSliderItem>()
            {
                new RangeSliderItem { Label = "35°F", Value = 35 },
                new RangeSliderItem { Label = "40°F", Value = 40 },
                new RangeSliderItem { Label = "45°F", Value = 45 },
                new RangeSliderItem { Label = "50°F", Value = 50 }
            };

            CustomCollectionLowTempCelsius = new ObservableCollection<RangeSliderItem>()
            {
                new RangeSliderItem { Label = "1°C", Value = 1 },
                new RangeSliderItem { Label = "4°C", Value = 4 },
                new RangeSliderItem { Label = "7°C", Value = 7 },
                new RangeSliderItem { Label = "10°C", Value = 10 }
            };

            CustomCollectionMaxBatteryTemp = new ObservableCollection<RangeSliderItem>()
            {
                new RangeSliderItem { Label = "113°F", Value = 113 },
                new RangeSliderItem { Label = "122°F", Value = 122 },
                new RangeSliderItem { Label = "131°F", Value = 131 },
                new RangeSliderItem { Label = "140°F", Value = 140 }
            };

            CustomCollectionMaxBatteryTempCelsius = new ObservableCollection<RangeSliderItem>()
            {
                new RangeSliderItem { Label = "45°C", Value = 45 },
                new RangeSliderItem { Label = "50°C", Value = 50 },
                new RangeSliderItem { Label = "55°C", Value = 55 },
                new RangeSliderItem { Label = "60°C", Value = 60 }
            };

            CustomCollectionAmpsDraw = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "3", Value = 3 },
                new RangeSliderItem { Label = "6", Value = 6 },
                new RangeSliderItem { Label = "9", Value = 9 },
                new RangeSliderItem { Label = "12", Value = 12 },
                new RangeSliderItem { Label = "15", Value = 15 }
            };

            CustomCollectionVoltsDraw = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "0.05", Value = 0.05 },
                new RangeSliderItem { Label = "0.1", Value = 0.1 },
                new RangeSliderItem { Label = "0.15", Value = 0.15 },
                new RangeSliderItem { Label = "0.20", Value = 0.20 },
                new RangeSliderItem { Label = "0.25", Value = 0.25 },
                new RangeSliderItem { Label = "0.30", Value = 0.30 }
            };

            CustomCollectionMaxSpeed = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "10", Value = 10 },
                new RangeSliderItem { Label = "20", Value = 20 },
                new RangeSliderItem { Label = "30", Value = 30 },
                new RangeSliderItem { Label = "40", Value = 40 },
                new RangeSliderItem { Label = "50", Value = 50 },
                new RangeSliderItem { Label = "60", Value = 60 }
            };

            CustomCollectionMaxPower = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "3", Value = 3 },
                new RangeSliderItem { Label = "5", Value = 5 },
                new RangeSliderItem { Label = "7", Value = 7 },
                new RangeSliderItem { Label = "9", Value = 9 },
                new RangeSliderItem { Label = "11", Value = 11 },
                new RangeSliderItem { Label = "14", Value = 13 }
            };

            CustomCollectionVoltageDiviation = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "100mV", Value = 100 },
                new RangeSliderItem { Label = "200mV", Value = 200 },
                new RangeSliderItem { Label = "300mV", Value = 300 },
                new RangeSliderItem { Label = "400mV", Value = 400 },
                new RangeSliderItem { Label = "500mV", Value = 500 }
            };

            CustomCollectionBalanceTemperatureLimitCelsius = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "40 °C", Value = 40 },
                new RangeSliderItem { Label = "45 °C", Value = 45 },
                new RangeSliderItem { Label = "50 °C", Value = 50 }
            };

            CustomCollectionBalanceTemperatureLimit = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "104 °F", Value = 104 },
                new RangeSliderItem { Label = "113 °F", Value = 113 },
                new RangeSliderItem { Label = "122 °F", Value = 122 }
            };

            CustomCollectionActivationVoltage = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "3.38", Value = 3.38 },
                new RangeSliderItem { Label = "3.42", Value = 3.42 },
                new RangeSliderItem { Label = "3.46", Value = 3.46 },
                new RangeSliderItem { Label = "3.50", Value = 3.50 }
            };

            CustomCollectionBalanceVoltageLimit = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "3.325", Value = 3.32 },
                new RangeSliderItem { Label = "3.350", Value = 3.35 },
                new RangeSliderItem { Label = "3.375", Value = 3.38 }
            };

            CustomCollectionActivationVoltage = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "3.325v", Value = 1 },
                new RangeSliderItem { Label = "3.35v", Value = 2 },
                new RangeSliderItem { Label = "3.375v", Value = 3 },
                new RangeSliderItem { Label = "3.400v", Value = 4 },
                new RangeSliderItem { Label = "3.425v", Value = 5 }
            };

            CustomCollectionDeviationLimit = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "10mv", Value = 10 },
                new RangeSliderItem { Label = "20mv", Value = 20 },
                new RangeSliderItem { Label = "30mv", Value = 30 },
                new RangeSliderItem { Label = "40mv", Value = 40 },
                new RangeSliderItem { Label = "50mv", Value = 50 },
            };

            CustomCollectionTimeInterval = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "12H", Value = 12 },
                new RangeSliderItem { Label = "24H", Value = 24 },
                new RangeSliderItem { Label = "36H", Value = 36 },
                new RangeSliderItem { Label = "48H", Value = 48 }
            };

            CustomCollectionHighSOCLimit = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "80%", Value = 80 },
                new RangeSliderItem { Label = "85%", Value = 85 },
                new RangeSliderItem { Label = "90%", Value = 90 },
                new RangeSliderItem { Label = "95%", Value = 95 }
            };

            CustomCollectionChargePulse = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "Disable", Value = 0 },
                new RangeSliderItem { Label = "1H", Value = 1 },
                new RangeSliderItem { Label = "2H", Value = 2 },
                new RangeSliderItem { Label = "3H", Value = 3 },
                new RangeSliderItem { Label = "4H", Value = 4 }
            };

            CustomCollectionNetworkMode = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "Zigbee", Value = 1 },
                new RangeSliderItem { Label = "Thred", Value = 2 },
                new RangeSliderItem { Label = "Ble", Value = 3 },
            };

            CustomCollectionBleGain = new ObservableCollection<RangeSliderItem>
            {
                new RangeSliderItem { Label = "0 db", Value = 0 },
                new RangeSliderItem { Label = "3 db", Value = 3 },
                new RangeSliderItem { Label = "6 db", Value = 6 },
                new RangeSliderItem { Label = "9 db", Value = 9 },
            };

            CustomCollectionMultiHeatingTemp = new ObservableCollection<RangeSliderItem>()
            {
                new RangeSliderItem { Label = "113°F", Value = 113 },
                new RangeSliderItem { Label = "122°F", Value = 122 },
                new RangeSliderItem { Label = "131°F", Value = 131 },
                new RangeSliderItem { Label = "140°F", Value = 140 }
            };

            CustomCollectionMultiHeatingTempCelsius = new ObservableCollection<RangeSliderItem>()
            {
                new RangeSliderItem { Label = "45°C", Value = 45 },
                new RangeSliderItem { Label = "50°C", Value = 50 },
                new RangeSliderItem { Label = "55°C", Value = 55 },
                new RangeSliderItem { Label = "60°C", Value = 60 }
            };

            CustomCollectionCalibrateSOC = new ObservableCollection<RangeSliderItem>()
            {
                new RangeSliderItem { Label = "0%", Value = 0 },
                new RangeSliderItem { Label = "20%", Value = 20 },
                new RangeSliderItem { Label = "40%", Value = 40 },
                new RangeSliderItem { Label = "60%", Value = 60 },
                new RangeSliderItem { Label = "80%", Value = 80 },
                new RangeSliderItem { Label = "100%", Value = 100 },
            };

            CustomCollectionHeatingPadTimer = new ObservableCollection<RangeSliderItem>()
            {
                new RangeSliderItem { Label = "15", Value = 1 },
                new RangeSliderItem { Label = "60", Value = 2 },
                new RangeSliderItem { Label = "120", Value = 3 },
                new RangeSliderItem { Label = "240", Value = 4 },
            };

        }
    }

    #endregion Device Information   
}