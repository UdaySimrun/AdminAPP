using System;

namespace SFKBle_Admin
{
    public class SerialDataReceivedArgs : EventArgs
    {
        public SerialDataReceivedArgs(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; private set; }
    }
}