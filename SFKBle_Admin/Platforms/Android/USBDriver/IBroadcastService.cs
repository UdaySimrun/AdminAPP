using Android.Hardware.Usb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SFKBle_Admin.UsbSerialPortInfo;

namespace SFKBle_Admin
{
    public interface IBroadcastService
    {
        void ExecuteUSBDeviceProcess();

        Task<IList<IUsbSerialDriver>> GetDriver();

        UsbSerialPortAdapter GetAdapter();

        UsbManager GetUsbManager();

        Tuple<int, int, int> GetUsbSerialPortInfo(UsbSerialPort selectedPort);

        Task<bool> RequestUsbPermissionAsync(UsbDevice _UsbDevice);
        Task<List<string>> GetUSBStatus();
        Task<bool> GetSingleUSBStatus();
    }
}