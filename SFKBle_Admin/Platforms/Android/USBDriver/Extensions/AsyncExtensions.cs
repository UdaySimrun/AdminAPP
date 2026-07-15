using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Hardware.Usb;

namespace SFKBle_Admin
{
    public static partial class AsyncExtensions
    {
        public static Task<IList<IUsbSerialDriver>> FindAllDriversAsync(this UsbSerialProber prober, UsbManager manager)
        {
            var tcs = new TaskCompletionSource<IList<IUsbSerialDriver>>();

            Task.Run(() =>
            {
                tcs.TrySetResult(prober.FindAllDrivers(manager));
            });
            return tcs.Task;
        }
    }
}