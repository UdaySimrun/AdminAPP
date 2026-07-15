using Android.OS;
using Java.Interop;
using Android.Content;
using Android.Widget;

namespace SFKBle_Admin
{
    public sealed class UsbSerialPortInfo : Java.Lang.Object, IParcelable
    {
        static readonly IParcelableCreator creator = new ParcelableCreator();

        [ExportField("CREATOR")]
        public static IParcelableCreator GetCreator()
        {
            return creator;
        }

        public UsbSerialPortInfo()
        {
        }

        public UsbSerialPortInfo(UsbSerialPort port)
        {
            var device = port.Driver.Device;
            VendorId = device.VendorId;
            DeviceId = device.DeviceId;
            PortNumber = port.PortNumber;
        }

        private UsbSerialPortInfo(Parcel parcel)
        {
            VendorId = parcel.ReadInt();
            DeviceId = parcel.ReadInt();
            PortNumber = parcel.ReadInt();
        }

        public int VendorId { get; set; }

        public int DeviceId { get; set; }

        public int PortNumber { get; set; }

        #region IParcelable implementation

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            dest.WriteInt(VendorId);
            dest.WriteInt(DeviceId);
            dest.WriteInt(PortNumber);
        }

        #endregion

        #region ParcelableCreator implementation

        public sealed class ParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            #region IParcelableCreator implementation

            public Java.Lang.Object CreateFromParcel(Parcel parcel)
            {
                return new UsbSerialPortInfo(parcel);
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new UsbSerialPortInfo[size];
            }

            #endregion
        }

        #endregion


        #region UsbSerialPortAdapter implementation
        public class UsbSerialPortAdapter : ArrayAdapter<UsbSerialPort>
        {
            public UsbSerialPortAdapter(Context context)
                : base(context, global::Android.Resource.Layout.SimpleExpandableListItem2)
            {
            }
        }

        #endregion
    }
}