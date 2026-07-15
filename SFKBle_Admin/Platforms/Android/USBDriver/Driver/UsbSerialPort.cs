using Android.Hardware.Usb;

namespace SFKBle_Admin
{
    public abstract class UsbSerialPort
    {
        /** 5 data bits. */
        public const int DATABITS_5 = 5;

        /** 6 data bits. */
        public const int DATABITS_6 = 6;

        /** 7 data bits. */
        public const int DATABITS_7 = 7;

        /** 8 data bits. */
        public const int DATABITS_8 = 8;

        /** No flow control. */
        public const int FLOWCONTROL_NONE = 0;

        /** RTS/CTS input flow control. */
        public const int FLOWCONTROL_RTSCTS_IN = 1;

        /** RTS/CTS output flow control. */
        public const int FLOWCONTROL_RTSCTS_OUT = 2;

        /** XON/XOFF input flow control. */
        public const int FLOWCONTROL_XONXOFF_IN = 4;

        /** XON/XOFF output flow control. */
        public const int FLOWCONTROL_XONXOFF_OUT = 8;

        /** No parity. */
        public const int PARITY_NONE = 0;

        /** Odd parity. */
        public const int PARITY_ODD = 1;

        /** Even parity. */
        public const int PARITY_EVEN = 2;

        /** Mark parity. */
        public const int PARITY_MARK = 3;

        /** Space parity. */
        public const int PARITY_SPACE = 4;

        /** 1 stop bit. */
        public const int STOPBITS_1 = 1;

        /** 1.5 stop bits. */
        public const int STOPBITS_1_5 = 3;

        /** 2 stop bits. */
        public const int STOPBITS_2 = 2;

        public IUsbSerialDriver Driver => GetDriver();

        public abstract IUsbSerialDriver GetDriver();

        public int PortNumber => GetPortNumber();
        public abstract int GetPortNumber();

        public abstract string GetSerial();

        public abstract void Open(UsbDeviceConnection connection);

        public abstract void Close();

        public abstract int Read(byte[] dest, int timeoutMillis);

        public abstract int Write(byte[] src, int timeoutMillis);

        public abstract void SetParameters(
                int baudRate, int dataBits, StopBits stopBits, Parity parity);
        public abstract bool GetCD();
        public abstract bool GetCTS();

        public abstract bool GetDSR();

        public abstract bool GetDTR();

        public abstract void SetDTR(bool value);

        public abstract bool GetRI();

        public abstract bool GetRTS();

        public abstract void SetRTS(bool value);

        public abstract bool PurgeHwBuffers(bool flushRX, bool flushTX);
    }
}