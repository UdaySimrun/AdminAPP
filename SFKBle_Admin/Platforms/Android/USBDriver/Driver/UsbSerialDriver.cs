using System.Collections.Generic;
using Android.Hardware.Usb;

namespace SFKBle_Admin
{
    public interface IUsbSerialDriver
    {
        UsbDevice Device { get; }

        UsbDevice GetDevice();

        List<UsbSerialPort> Ports { get; }
        List<UsbSerialPort> GetPorts();
    }
    public class UsbSerialDriver : IUsbSerialDriver
    {
        protected UsbDevice mDevice;
        protected UsbSerialPort mPort;
        public UsbDevice Device => GetDevice();

        public List<UsbSerialPort> Ports => GetPorts();

        public UsbDevice GetDevice()
        {
            return mDevice;
        }

        public List<UsbSerialPort> GetPorts()
        {
            return new List<UsbSerialPort> { mPort };
        }
    }
}