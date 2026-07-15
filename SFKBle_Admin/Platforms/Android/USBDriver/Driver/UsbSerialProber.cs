using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Lang.Reflect;

namespace SFKBle_Admin
{
    public class UsbSerialProber
    {
        private ProbeTable mProbeTable;

        public UsbSerialProber(ProbeTable probeTable)
        {
            mProbeTable = probeTable;
        }

        public static UsbSerialProber GetDefaultProber()
        {
            return new UsbSerialProber(GetDefaultProbeTable());
        }

        public static ProbeTable DefaultProbeTable => GetDefaultProbeTable();

        public static ProbeTable GetDefaultProbeTable()
        {
            ProbeTable probeTable = new ProbeTable();
            probeTable.AddDriver(typeof(FtdiSerialDriver));
            return probeTable;
        }

        public List<IUsbSerialDriver> FindAllDrivers(UsbManager usbManager)
        {
            List<IUsbSerialDriver> result = new List<IUsbSerialDriver>();

            foreach (UsbDevice usbDevice in usbManager.DeviceList.Values)
            {
                IUsbSerialDriver driver = ProbeDevice(usbDevice);
                if (driver != null)
                {
                    result.Add(driver);
                }
            }
            return result;
        }

        public IUsbSerialDriver ProbeDevice(UsbDevice usbDevice)
        {
            int vendorId = usbDevice.VendorId;
            int productId = usbDevice.ProductId;

            var driverClass = mProbeTable.FindDriver(vendorId, productId);
            if (driverClass != null)
            {
                IUsbSerialDriver driver;
                try
                {
                    driver = (IUsbSerialDriver)Activator.CreateInstance(driverClass, new System.Object[] { usbDevice });
                }
                catch (NoSuchMethodException e)
                {
                    throw new RuntimeException(e);
                }
                catch (IllegalArgumentException e)
                {
                    throw new RuntimeException(e);
                }
                catch (InstantiationException e)
                {
                    throw new RuntimeException(e);
                }
                catch (IllegalAccessException e)
                {
                    throw new RuntimeException(e);
                }
                catch (InvocationTargetException e)
                {
                    throw new RuntimeException(e);
                }
                return driver;
            }
            return null;
        }

    }
}