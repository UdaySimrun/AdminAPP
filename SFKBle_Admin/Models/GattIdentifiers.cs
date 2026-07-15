using System;
using System.Collections.Generic;
using System.Text;

namespace SFKBle_Admin
{
    public class GattIdentifiers
    {
        public static Guid UartGattServiceId = Guid.Parse("0000FF00-0000-1000-8000-00805F9B34FB");
        public static Guid UartGattChracteristicBMSName = Guid.Parse("00002A50-0000-1000-8000-00805F9B34FB");
        public static Guid UartGattCharacteristicReceiveId = Guid.Parse("0000FF01-0000-1000-8000-00805F9B34FB");
        public static Guid UartGattCharacteristicSendId = Guid.Parse("0000FF02-0000-1000-8000-00805F9B34FB");
        public static Guid SpecialNotificationDescriptorId = Guid.Parse("0000FF01-0000-1000-8000-00805F9B34FB");
    }
}
