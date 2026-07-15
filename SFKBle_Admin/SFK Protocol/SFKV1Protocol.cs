
namespace SFKBle_Admin.SFK_Protocol
{
    public class SFKV1Protocol : IProtocol
    {
        public byte[] CreateMvByte(string Register, double UserDecValue)
        {
            byte[] arrByte = new byte[9];
            try
            {
                int[] arrayint = new int[10];
                var UserValue = UserDecValue;

                int intValue = Convert.ToInt32(UserValue * 1000);

                string strHexValue = intValue.ToString("X").PadLeft(4, '0');

                string str1 = strHexValue.Substring(0, 2);
                string str2 = strHexValue.Substring(2, 2);

                arrayint[0] = int.Parse(Register, System.Globalization.NumberStyles.HexNumber);
                arrayint[1] = int.Parse("02", System.Globalization.NumberStyles.HexNumber);
                arrayint[2] = int.Parse(str1, System.Globalization.NumberStyles.HexNumber);
                arrayint[3] = int.Parse(str2, System.Globalization.NumberStyles.HexNumber);

                int tempSum = arrayint[0] + arrayint[1] + arrayint[2] + arrayint[3];

                string strCheckSumHex = tempSum.ToString("X").PadLeft(4, '0');

                string strGetChecksum1 = strCheckSumHex.Substring(0, 2);
                string strGetChecksum2 = strCheckSumHex.Substring(2, 2);

                string CheckSum1 = (255 - int.Parse(strGetChecksum1, System.Globalization.NumberStyles.HexNumber)).ToString("X");
                string CheckSum2 = (256 - int.Parse(strGetChecksum2, System.Globalization.NumberStyles.HexNumber)).ToString("X");

                uint num1 = uint.Parse(CheckSum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrayint[4] = BitConverter.GetBytes(num1)[0];

                uint num2 = uint.Parse(CheckSum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrayint[4] = BitConverter.GetBytes(num1)[0];

                arrByte[0] = 0xdd;
                arrByte[1] = 0x5a;
                arrByte[2] = Convert.ToByte(Register, 16);
                arrByte[3] = Convert.ToByte("02", 16);

                uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(str2, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[5] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(CheckSum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(CheckSum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[7] = BitConverter.GetBytes(num)[0];

                arrByte[8] = 0x77;
            }
            catch (Exception ex) { }
            return arrByte;
        }
        public byte[] CreateMv10Byte(string Register, double UserDecValue)
        {
            byte[] arrByte = new byte[9];
            try
            {
                int[] arrayint = new int[10];
                var UserValue = UserDecValue;

                int intValue = Convert.ToInt32(UserValue * 100);

                string strHexValue = intValue.ToString("X").PadLeft(4, '0');

                string str1 = strHexValue.Substring(0, 2);
                string str2 = strHexValue.Substring(2, 2);

                arrayint[0] = int.Parse(Register, System.Globalization.NumberStyles.HexNumber);
                arrayint[1] = int.Parse("02", System.Globalization.NumberStyles.HexNumber);
                arrayint[2] = int.Parse(str1, System.Globalization.NumberStyles.HexNumber);
                arrayint[3] = int.Parse(str2, System.Globalization.NumberStyles.HexNumber);

                int tempSum = arrayint[0] + arrayint[1] + arrayint[2] + arrayint[3];

                string strCheckSumHex = tempSum.ToString("X").PadLeft(4, '0');

                string strGetChecksum1 = strCheckSumHex.Substring(0, 2);
                string strGetChecksum2 = strCheckSumHex.Substring(2, 2);

                string CheckSum1 = (255 - int.Parse(strGetChecksum1, System.Globalization.NumberStyles.HexNumber)).ToString("X");
                string CheckSum2 = (256 - int.Parse(strGetChecksum2, System.Globalization.NumberStyles.HexNumber)).ToString("X");

                uint num1 = uint.Parse(CheckSum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrayint[4] = BitConverter.GetBytes(num1)[0];

                uint num2 = uint.Parse(CheckSum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrayint[4] = BitConverter.GetBytes(num1)[0];

                arrByte[0] = 0xdd;
                arrByte[1] = 0x5a;
                arrByte[2] = Convert.ToByte(Register, 16);
                arrByte[3] = Convert.ToByte("02", 16);

                uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(str2, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[5] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(CheckSum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(CheckSum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[7] = BitConverter.GetBytes(num)[0];

                arrByte[8] = 0x77;
            }
            catch (Exception ex) { }
            return arrByte;
        }
        public byte[] CreateTempratureByte(string Register, double UserDecValue)
        {
            byte[] arrByte = new byte[9];
            try
            {
                int[] arrayint = new int[10]; // 10.8
                var UserValue = Convert.ToInt32(UserDecValue);

                int intValue = (UserValue * 10) + 2731;

                string strHexValue = intValue.ToString("X").PadLeft(4, '0');

                //double doubleValue = Convert.ToDouble(((UserValue + 273.15) * 10));

                //string strHexValue = Convert.ToInt32(doubleValue).ToString("X").PadLeft(4, '0');

                string str1 = strHexValue.Substring(0, 2);
                string str2 = strHexValue.Substring(2, 2);

                arrayint[0] = int.Parse(Register, System.Globalization.NumberStyles.HexNumber);
                arrayint[1] = int.Parse("02", System.Globalization.NumberStyles.HexNumber);
                arrayint[2] = int.Parse(str1, System.Globalization.NumberStyles.HexNumber);
                arrayint[3] = int.Parse(str2, System.Globalization.NumberStyles.HexNumber);

                int tempSum = arrayint[0] + arrayint[1] + arrayint[2] + arrayint[3];

                string strCheckSumHex = tempSum.ToString("X").PadLeft(4, '0');

                string strGetChecksum1 = strCheckSumHex.Substring(0, 2);
                string strGetChecksum2 = strCheckSumHex.Substring(2, 2);

                string CheckSum1 = (255 - int.Parse(strGetChecksum1, System.Globalization.NumberStyles.HexNumber)).ToString("X");
                string CheckSum2 = (256 - int.Parse(strGetChecksum2, System.Globalization.NumberStyles.HexNumber)).ToString("X");

                arrByte[0] = 0xdd;
                arrByte[1] = 0x5a;
                arrByte[2] = Convert.ToByte(Register, 16);
                arrByte[3] = Convert.ToByte("02", 16);

                uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(str2, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[5] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(CheckSum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(CheckSum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[7] = BitConverter.GetBytes(num)[0];

                arrByte[8] = 0x77;
            }
            catch (Exception ex) { }
            return arrByte;
        }
        public byte[] CreateReadCommand(string Register)
        {
            byte[] arrByte = new byte[7];
            try
            {
                int iRegister = int.Parse(Register, System.Globalization.NumberStyles.HexNumber);

                int tempSum = iRegister + 0;

                string strCheckSumHex = tempSum.ToString("X").PadLeft(4, '0');

                string strGetChecksum1 = strCheckSumHex.Substring(0, 2);
                string strGetChecksum2 = strCheckSumHex.Substring(2, 2);

                string CheckSum1 = (255 - int.Parse(strGetChecksum1, System.Globalization.NumberStyles.HexNumber)).ToString("X");
                string CheckSum2 = (256 - int.Parse(strGetChecksum2, System.Globalization.NumberStyles.HexNumber)).ToString("X");

                arrByte[0] = 0xdd;
                arrByte[1] = 0xa5;
                arrByte[2] = Convert.ToByte(Register, 16);
                arrByte[3] = Convert.ToByte("0", 16);

                uint num = uint.Parse(CheckSum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(CheckSum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                arrByte[5] = BitConverter.GetBytes(num)[0];

                arrByte[6] = 0x77;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return arrByte;
        }
        public byte[] CreateCustomReadCommand(string Register, string Length, uint strValues = 0)
        {
            byte[] arrByte;
            try
            {
                int iLength = Convert.ToInt32(Length);
                List<byte> lstByte = new List<byte>();
                lstByte.Add(0xdd);
                lstByte.Add(0xa5);
                lstByte.Add(BitConverter.GetBytes(Convert.ToUInt32(Register, 16))[0]);
                lstByte.Add(BitConverter.GetBytes(Convert.ToUInt32(Length, 16))[0]);

                var HexValue = strValues.ToString("X").PadLeft((iLength * 2), '0');
                byte[] bytes = Enumerable.Range(0, HexValue.Length / 2).Select(i => Convert.ToByte(HexValue.Substring(i * 2, 2), 16)).ToArray();

                uint tempSum = 0;
                for (int i = 0; i < iLength; i++)
                {
                    lstByte.Add(BitConverter.GetBytes(Convert.ToInt32(bytes[i]))[0]);
                    tempSum += Convert.ToUInt32(bytes[i]);
                }
                tempSum += Convert.ToUInt32(Register, 16);
                tempSum += Convert.ToUInt32(iLength);
                string strCheckSumHex = tempSum.ToString("X").PadLeft(4, '0');

                string strGetChecksum1 = strCheckSumHex.Substring(0, 2);
                string strGetChecksum2 = strCheckSumHex.Substring(2, 2);

                string CheckSum1 = (255 - int.Parse(strGetChecksum1, System.Globalization.NumberStyles.HexNumber)).ToString("X");
                string CheckSum2 = (256 - int.Parse(strGetChecksum2, System.Globalization.NumberStyles.HexNumber)).ToString("X");

                uint num = uint.Parse(CheckSum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                lstByte.Add(BitConverter.GetBytes(num)[0]);

                num = uint.Parse(CheckSum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                lstByte.Add(BitConverter.GetBytes(num)[0]);

                lstByte.Add(0x77);

                arrByte = lstByte.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return arrByte;
        }
        public byte[] CreateCustomWriteCommand(string Register, string Length, uint strValues = 0)
        {
            byte[] arrByte;
            try
            {
                int iLength = Convert.ToInt32(Length);
                List<byte> lstByte = new List<byte>();
                lstByte.Add(0xdd);
                lstByte.Add(0x5a);
                lstByte.Add(BitConverter.GetBytes(Convert.ToUInt32(Register, 16))[0]);
                lstByte.Add(BitConverter.GetBytes(Convert.ToUInt32(Length, 16))[0]);

                var HexValue = strValues.ToString("X").PadLeft((iLength * 2), '0');
                byte[] bytes = Enumerable.Range(0, HexValue.Length / 2).Select(i => Convert.ToByte(HexValue.Substring(i * 2, 2), 16)).ToArray();

                uint tempSum = 0;
                for (int i = 0; i < bytes.Count(); i++)
                {
                    lstByte.Add(BitConverter.GetBytes(Convert.ToInt32(bytes[i]))[0]);
                    tempSum += Convert.ToUInt32(bytes[i]);
                }
                tempSum += Convert.ToUInt32(Register, 16);
                tempSum += Convert.ToUInt32(iLength);
                string strCheckSumHex = tempSum.ToString("X").PadLeft(4, '0');

                string strGetChecksum1 = strCheckSumHex.Substring(0, 2);
                string strGetChecksum2 = strCheckSumHex.Substring(2, 2);

                string CheckSum1 = (255 - int.Parse(strGetChecksum1, System.Globalization.NumberStyles.HexNumber)).ToString("X");
                string CheckSum2 = (256 - int.Parse(strGetChecksum2, System.Globalization.NumberStyles.HexNumber)).ToString("X");

                uint num = uint.Parse(CheckSum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                lstByte.Add(BitConverter.GetBytes(num)[0]);

                num = uint.Parse(CheckSum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                lstByte.Add(BitConverter.GetBytes(num)[0]);

                lstByte.Add(0x77);

                arrByte = lstByte.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return arrByte;
        }
        public byte[] PrepareCustomerPassword(string sPassword)
        {
            byte[] byteArrayofReg06 = new byte[14];
            try
            {

                byteArrayofReg06[0] = 0xdd;
                byteArrayofReg06[1] = 0x5a;
                byteArrayofReg06[2] = 0x06;
                byteArrayofReg06[3] = 0x07;
                byteArrayofReg06[4] = 0x06;

                int j = 5; int passsum = 0;
                for (int i = 0; i < sPassword.Length; i++)
                {
                    passsum = passsum + Convert.ToInt32(Convert.ToString(sPassword[i]));
                    string str1 = Convert.ToInt32(Convert.ToString(sPassword[i])).ToString("X");
                    uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);

                    byteArrayofReg06[j] = BitConverter.GetBytes(num)[0];
                    j++;

                }
                passsum = passsum + 19;
                string shx = passsum.ToString("X");

                shx = shx.PadLeft(4, '0');

                string shx1fnl = shx.Substring(0, 2);
                string shx2fnl = shx.Substring(2, 2);

                int intdx1fnl = Convert.ToInt32(shx1fnl, 16);
                int intdx2fnl = Convert.ToInt32(shx2fnl, 16);

                intdx1fnl = 255 - intdx1fnl;

                intdx2fnl = 256 - intdx2fnl;
                string checksum1 = intdx1fnl.ToString("X");
                string checksum2 = intdx2fnl.ToString("X");

                uint num1 = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg06[11] = BitConverter.GetBytes(num1)[0];

                num1 = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg06[12] = BitConverter.GetBytes(num1)[0];

                byteArrayofReg06[13] = 0x77;
            }
            catch
            {
            }
            return byteArrayofReg06;
        }
        public byte[] PrepareCustomerPassword_Reset(string soldPassword, string sPassword)
        {
            byte[] byteArrayofReg07 = new byte[20];
            try
            {
                //start bit
                byteArrayofReg07[0] = 0xdd;
                byteArrayofReg07[1] = 0x5a;
                byteArrayofReg07[2] = 0x07;
                byteArrayofReg07[3] = 0x0d;
                byteArrayofReg07[4] = 0x0c;

                int j = 5; int passsum = 0;

                for (int i = 0; i < soldPassword.Length; i++)
                {
                    passsum = passsum + Convert.ToInt32(Convert.ToString(soldPassword[i]));
                    string str1 = Convert.ToInt32(Convert.ToString(soldPassword[i])).ToString("X");
                    uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);

                    byteArrayofReg07[j] = BitConverter.GetBytes(num)[0];
                    j++;

                }

                for (int i = 0; i < sPassword.Length; i++)
                {
                    passsum = passsum + Convert.ToInt32(Convert.ToString(sPassword[i]));
                    string str1 = Convert.ToInt32(Convert.ToString(sPassword[i])).ToString("X");
                    uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);

                    byteArrayofReg07[j] = BitConverter.GetBytes(num)[0];
                    j++;

                }

                passsum = passsum + 32;
                string shx = passsum.ToString("X");

                shx = shx.PadLeft(4, '0');

                string shx1fnl = shx.Substring(0, 2);
                string shx2fnl = shx.Substring(2, 2);

                int intdx1fnl = Convert.ToInt32(shx1fnl, 16);
                int intdx2fnl = Convert.ToInt32(shx2fnl, 16);

                intdx1fnl = 255 - intdx1fnl;

                intdx2fnl = 256 - intdx2fnl;
                string checksum1 = intdx1fnl.ToString("X");
                string checksum2 = intdx2fnl.ToString("X");

                uint num1 = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg07[17] = BitConverter.GetBytes(num1)[0];

                num1 = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg07[18] = BitConverter.GetBytes(num1)[0];

                byteArrayofReg07[19] = 0x77;
            }
            catch
            {
            }
            return byteArrayofReg07;
        }
        public byte[] PrepareSerialNumber(string SerialNumber)
        {
            byte[] byteArrayofRegA2 = new byte[SerialNumber.Length + 8];
            try
            {
                //start bit
                byteArrayofRegA2[0] = 0xdd;
                byteArrayofRegA2[1] = 0x5a;
                byteArrayofRegA2[2] = 0xa2;

                string l1 = (SerialNumber.Length + 1).ToString("X");
                uint lenghth1 = uint.Parse(l1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegA2[3] = BitConverter.GetBytes(lenghth1)[0];

                string l2 = (SerialNumber.Length).ToString("X");
                uint lenghth2 = uint.Parse(l2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegA2[4] = BitConverter.GetBytes(lenghth2)[0];

                int j = 5; int SnSum = 0;
                for (int i = 0; i < SerialNumber.Length; i++)
                {
                    SnSum += (int)SerialNumber[i];
                    int asciiValue = (int)SerialNumber[i];
                    uint num = uint.Parse(asciiValue.ToString("X"), System.Globalization.NumberStyles.AllowHexSpecifier);
                    byteArrayofRegA2[j] = BitConverter.GetBytes(num)[0];
                    j++;
                }

                int ndeci1 = Convert.ToInt32("a2", 16);
                int ndeci2 = Convert.ToInt32(SerialNumber.Length + 1);
                int ndeci3 = Convert.ToInt32(SerialNumber.Length);

                int TotalDeci = ndeci1 + ndeci2 + ndeci3 + SnSum;
                int Decimal = 65536 - TotalDeci;
                string shx = Decimal.ToString("X");

                shx = shx.PadLeft(4, '0');

                string checksum1 = shx.Substring(0, 2);
                string checksum2 = shx.Substring(2, 2);

                uint num1 = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegA2[j] = BitConverter.GetBytes(num1)[0];

                num1 = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegA2[j + 1] = BitConverter.GetBytes(num1)[0];

                byteArrayofRegA2[j + 2] = 0x77;
            }
            catch (Exception ex)
            {

            }
            return byteArrayofRegA2;
        }
        public byte[] PrepareDefaultPin09()
        {
            byte[] byteArrayofRegA2 = new byte[14];
            try
            {
                //start bit
                byteArrayofRegA2[0] = 0xdd;
                byteArrayofRegA2[1] = 0x5a;
                byteArrayofRegA2[2] = 0x09;
                byteArrayofRegA2[3] = 0x07;
                byteArrayofRegA2[4] = 0x06;
                byteArrayofRegA2[5] = 0x4a;
                byteArrayofRegA2[6] = 0x31;
                byteArrayofRegA2[7] = 0x42;
                byteArrayofRegA2[8] = 0x32;
                byteArrayofRegA2[9] = 0x44;
                byteArrayofRegA2[10] = 0x34;
                byteArrayofRegA2[11] = 0xfe;
                byteArrayofRegA2[12] = 0x83;
                byteArrayofRegA2[13] = 0x77;
            }
            catch (Exception ex)
            {
            }
            return byteArrayofRegA2;
        }
        public byte[] PrepareHeatSocLimit()
        {
            byte[] byteArrayofRegA2 = new byte[8];
            try
            {
                //start bit
                byteArrayofRegA2[0] = 0xdd;
                byteArrayofRegA2[1] = 0xa5;
                byteArrayofRegA2[2] = 0x09;
                byteArrayofRegA2[3] = 0x01;
                byteArrayofRegA2[4] = 0x01;
                byteArrayofRegA2[5] = 0xff;
                byteArrayofRegA2[6] = 0xf5;
                byteArrayofRegA2[7] = 0x77;
            }
            catch (Exception ex)
            {
            }
            return byteArrayofRegA2;
        }
        public byte[] StartandEndCommand(string Command = "start")
        {
            byte[] arrByte = new byte[9];
            try
            {
                if (Command == "start")
                {
                    arrByte[0] = 0xdd;
                    arrByte[1] = 0x5a;
                    arrByte[2] = 0x00;
                    arrByte[3] = 0x02;
                    arrByte[4] = 0x56;
                    arrByte[5] = 0x78;
                    arrByte[6] = 0xff;
                    arrByte[7] = 0x30;
                    arrByte[8] = 0x77;
                }
                else if (Command == "end")
                {
                    arrByte[0] = 0xdd;
                    arrByte[1] = 0x5a;
                    arrByte[2] = 0x01;
                    arrByte[3] = 0x02;
                    arrByte[4] = 0x00;
                    arrByte[5] = 0x00;
                    arrByte[6] = 0xff;
                    arrByte[7] = 0xfd;
                    arrByte[8] = 0x77;
                }
            }
            catch (Exception ex)
            {
            }
            return arrByte;
        }

        #region Prepare Register Bytes And Conversion
        public int makeInt(byte b3, byte b2, byte b1, byte b0)
        {
            return (((b3) << 24) | ((b2 & 0xff) << 16) | ((b1 & 0xff) << 8) | ((b0 & 0xff)));
        }
        public byte[] PrepareRegister10Bytes(int nChargeValue)
        {
            byte[] byteArrayofReg10 = new byte[9];
            try
            {

                nChargeValue = nChargeValue * 100;
                string sHex = nChargeValue.ToString("X");

                sHex = sHex.PadLeft(4, '0');

                string str1 = sHex.Substring(0, 2);
                string str2 = sHex.Substring(2, 2);

                if (sHex.Length == 8)
                {
                    str1 = sHex.Substring(4, 2);
                    str2 = sHex.Substring(6, 2);
                }

                int ndeci1 = Convert.ToInt32("10", 16); // hex to decimal
                int ndeci2 = Convert.ToInt32("02", 16); // hex to decimal
                int ndeci3 = Convert.ToInt32(str1, 16); // hex to decimal
                int ndeci4 = Convert.ToInt32(str2, 16); // hex to decimal

                int TotalDeci = ndeci1 + ndeci2 + ndeci3 + ndeci4;

                string shx = TotalDeci.ToString("X");

                shx = shx.PadLeft(4, '0');

                string shx1fnl = shx.Substring(0, 2);
                string shx2fnl = shx.Substring(2, 2);

                int intdx1fnl = Convert.ToInt32(shx1fnl, 16);
                int intdx2fnl = Convert.ToInt32(shx2fnl, 16);

                intdx1fnl = 255 - intdx1fnl;

                intdx2fnl = 256 - intdx2fnl;
                string checksum1 = intdx1fnl.ToString("X");
                string checksum2 = intdx2fnl.ToString("X");

                byteArrayofReg10[0] = 0xdd;
                byteArrayofReg10[1] = 0x5a;
                byteArrayofReg10[2] = 0x10;
                byteArrayofReg10[3] = 0x02;

                uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);

                byteArrayofReg10[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(str2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg10[5] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg10[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg10[7] = BitConverter.GetBytes(num)[0];

                byteArrayofReg10[8] = 0x77;
            }
            catch
            {
            }
            return byteArrayofReg10;
        }
        public byte[] PrepareBytesForVoltageCalibration(byte RegisterValueinInt32, string RegisterValue, decimal VoltageValue)
        {
            byte[] byteArrayofReg10 = new byte[9];
            try
            {
                int[] arrayint = new int[10];
                var UserValue = Convert.ToDecimal(VoltageValue);

                int intValue = Convert.ToInt32(UserValue * 1000);

                string strHexValue = intValue.ToString("X").PadLeft(4, '0');

                string str1 = strHexValue.Substring(0, 2);
                string str2 = strHexValue.Substring(2, 2);

                arrayint[0] = int.Parse(RegisterValue, System.Globalization.NumberStyles.HexNumber);
                arrayint[1] = int.Parse("02", System.Globalization.NumberStyles.HexNumber);
                arrayint[2] = int.Parse(str1, System.Globalization.NumberStyles.HexNumber);
                arrayint[3] = int.Parse(str2, System.Globalization.NumberStyles.HexNumber);

                int tempSum = arrayint[0] + arrayint[1] + arrayint[2] + arrayint[3];

                string strCheckSumHex = tempSum.ToString("X").PadLeft(4, '0');

                string strGetChecksum1 = strCheckSumHex.Substring(0, 2);
                string strGetChecksum2 = strCheckSumHex.Substring(2, 2);

                string CheckSum1 = (255 - int.Parse(strGetChecksum1, System.Globalization.NumberStyles.HexNumber)).ToString("X");
                string CheckSum2 = (256 - int.Parse(strGetChecksum2, System.Globalization.NumberStyles.HexNumber)).ToString("X");

                byteArrayofReg10[0] = 0xdd;
                byteArrayofReg10[1] = 0x5a;
                byteArrayofReg10[2] = RegisterValueinInt32;
                byteArrayofReg10[3] = 0x02;

                uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);

                byteArrayofReg10[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(str2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg10[5] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(CheckSum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg10[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(CheckSum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg10[7] = BitConverter.GetBytes(num)[0];

                byteArrayofReg10[8] = 0x77;

            }
            catch
            {
            }
            return byteArrayofReg10;
        }
        public byte[] PrepareRegister11Bytes(double nChargeValue)
        {
            byte[] byteArrayofReg11 = new byte[9];
            try
            {

                nChargeValue = nChargeValue * 100;
                string sHex = Convert.ToInt32(nChargeValue).ToString("X");

                sHex = sHex.PadLeft(4, '0');

                string str1 = sHex.Substring(0, 2);
                string str2 = sHex.Substring(2, 2);

                if (sHex.Length == 8)
                {
                    str1 = sHex.Substring(4, 2);
                    str2 = sHex.Substring(6, 2);
                }

                int ndeci1 = Convert.ToInt32("11", 16); // hex to decimal
                int ndeci2 = Convert.ToInt32("02", 16); // hex to decimal
                int ndeci3 = Convert.ToInt32(str1, 16); // hex to decimal
                int ndeci4 = Convert.ToInt32(str2, 16); // hex to decimal

                int TotalDeci = ndeci1 + ndeci2 + ndeci3 + ndeci4;

                string shx = TotalDeci.ToString("X");

                shx = shx.PadLeft(4, '0');

                string shx1fnl = shx.Substring(0, 2);
                string shx2fnl = shx.Substring(2, 2);

                int intdx1fnl = Convert.ToInt32(shx1fnl, 16);
                int intdx2fnl = Convert.ToInt32(shx2fnl, 16);

                intdx1fnl = 255 - intdx1fnl;

                intdx2fnl = 256 - intdx2fnl;
                string checksum1 = intdx1fnl.ToString("X");
                string checksum2 = intdx2fnl.ToString("X");

                byteArrayofReg11[0] = 0xdd;
                byteArrayofReg11[1] = 0x5a;
                byteArrayofReg11[2] = 0x11;
                byteArrayofReg11[3] = 0x02;

                uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);

                byteArrayofReg11[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(str2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg11[5] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg11[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg11[7] = BitConverter.GetBytes(num)[0];

                byteArrayofReg11[8] = 0x77;
            }
            catch
            {
            }
            return byteArrayofReg11;
        }
        public void IntToHex(int number, ref string str1, ref string str2)
        {
            try
            {
                number = (number * 10) + 2731;

                string hex = number.ToString("X");

                hex = hex.PadLeft(4, '0');

                str1 = hex.Substring(0, 2);
                str2 = hex.Substring(2, 2);
            }
            catch { }
        }
        public byte[] GetTempratureBytes(string byteRegister, int nTemprature)
        {
            byte[] byteArrayofTemprature = new byte[9];

            try
            {
                string TempSplit1 = string.Empty;
                string TempSplit2 = string.Empty;
                string checksum1 = string.Empty;
                string checksum2 = string.Empty;

                IntToHex(nTemprature, ref TempSplit1, ref TempSplit2);
                ConvertHexToDex(byteRegister.ToString(), "02", TempSplit1, TempSplit2, ref checksum1, ref checksum2);


                byteArrayofTemprature[0] = 0xdd;
                byteArrayofTemprature[1] = 0x5a;
                byteArrayofTemprature[2] = BitConverter.GetBytes(Convert.ToInt32(byteRegister, 16))[0];  //byteRegister.ToString("X");
                byteArrayofTemprature[3] = 0x02;

                uint num = uint.Parse(TempSplit1, System.Globalization.NumberStyles.AllowHexSpecifier);

                byteArrayofTemprature[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(TempSplit2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofTemprature[5] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofTemprature[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofTemprature[7] = BitConverter.GetBytes(num)[0];

                byteArrayofTemprature[8] = 0x77;
            }
            catch (Exception ex)
            { }
            return byteArrayofTemprature;
        }
        public byte[] GetIdleCurrentRegisterBytes(int Current)
        {
            byte[] byteArrayofReg28 = new byte[9];
            try
            {
                if (Current < 0)
                {
                    Current = Math.Abs(Current);
                }

                string sHex = (Current / 10).ToString("X");

                sHex = sHex.PadLeft(4, '0');

                string str1 = sHex.Substring(0, 2);
                string str2 = sHex.Substring(2, 2);

                int ndeci1 = Convert.ToInt32("ad", 16); // hex to decimal
                int ndeci2 = Convert.ToInt32("02", 16); // hex to decimal
                int ndeci3 = Convert.ToInt32(str1, 16); // hex to decimal
                int ndeci4 = Convert.ToInt32(str2, 16); // hex to decimal

                int TotalDeci = ndeci1 + ndeci2 + ndeci3 + ndeci4;

                string shx = TotalDeci.ToString("X");

                shx = shx.PadLeft(4, '0');

                string shx1fnl = shx.Substring(0, 2);
                string shx2fnl = shx.Substring(2, 2);

                int intdx1fnl = Convert.ToInt32(shx1fnl, 16);
                int intdx2fnl = Convert.ToInt32(shx2fnl, 16);

                intdx1fnl = 255 - intdx1fnl;

                intdx2fnl = 256 - intdx2fnl;
                string checksum1 = intdx1fnl.ToString("X");
                string checksum2 = intdx2fnl.ToString("X");

                byteArrayofReg28[0] = 0xdd;
                byteArrayofReg28[1] = 0x5a;
                byteArrayofReg28[2] = 0xad;
                byteArrayofReg28[3] = 0x02;

                uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);

                byteArrayofReg28[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(str2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg28[5] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg28[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg28[7] = BitConverter.GetBytes(num)[0];

                byteArrayofReg28[8] = 0x77;
            }
            catch
            {
            }
            return byteArrayofReg28;
        }
        public byte[] GetChargeCurrentRegisterBytes(int Current)
        {
            byte[] byteArrayofReg28 = new byte[9];
            try
            {
                string sHex = (Current / 10).ToString("X");

                sHex = sHex.PadLeft(4, '0');

                string str1 = sHex.Substring(0, 2);
                string str2 = sHex.Substring(2, 2);

                int ndeci1 = Convert.ToInt32("ae", 16); // hex to decimal
                int ndeci2 = Convert.ToInt32("02", 16); // hex to decimal
                int ndeci3 = Convert.ToInt32(str1, 16); // hex to decimal
                int ndeci4 = Convert.ToInt32(str2, 16); // hex to decimal

                int TotalDeci = ndeci1 + ndeci2 + ndeci3 + ndeci4;

                string shx = TotalDeci.ToString("X");

                shx = shx.PadLeft(4, '0');

                string shx1fnl = shx.Substring(0, 2);
                string shx2fnl = shx.Substring(2, 2);

                int intdx1fnl = Convert.ToInt32(shx1fnl, 16);
                int intdx2fnl = Convert.ToInt32(shx2fnl, 16);

                intdx1fnl = 255 - intdx1fnl;

                intdx2fnl = 256 - intdx2fnl;
                string checksum1 = intdx1fnl.ToString("X");
                string checksum2 = intdx2fnl.ToString("X");

                byteArrayofReg28[0] = 0xdd;
                byteArrayofReg28[1] = 0x5a;
                byteArrayofReg28[2] = 0xae;
                byteArrayofReg28[3] = 0x02;

                uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);

                byteArrayofReg28[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(str2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg28[5] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg28[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg28[7] = BitConverter.GetBytes(num)[0];

                byteArrayofReg28[8] = 0x77;
            }
            catch
            {
            }
            return byteArrayofReg28;
        }
        public byte[] GetDisChargeCurrentRegisterBytes(int Current)
        {
            byte[] byteArrayofReg28 = new byte[9];
            try
            {
                string sHex = (Current / 10).ToString("X");

                sHex = sHex.PadLeft(4, '0');

                string str1 = sHex.Substring(0, 2);
                string str2 = sHex.Substring(2, 2);

                int ndeci1 = Convert.ToInt32("af", 16); // hex to decimal
                int ndeci2 = Convert.ToInt32("02", 16); // hex to decimal
                int ndeci3 = Convert.ToInt32(str1, 16); // hex to decimal
                int ndeci4 = Convert.ToInt32(str2, 16); // hex to decimal

                int TotalDeci = ndeci1 + ndeci2 + ndeci3 + ndeci4;

                string shx = TotalDeci.ToString("X");

                shx = shx.PadLeft(4, '0');

                string shx1fnl = shx.Substring(0, 2);
                string shx2fnl = shx.Substring(2, 2);

                int intdx1fnl = Convert.ToInt32(shx1fnl, 16);
                int intdx2fnl = Convert.ToInt32(shx2fnl, 16);

                intdx1fnl = 255 - intdx1fnl;

                intdx2fnl = 256 - intdx2fnl;
                string checksum1 = intdx1fnl.ToString("X");
                string checksum2 = intdx2fnl.ToString("X");

                byteArrayofReg28[0] = 0xdd;
                byteArrayofReg28[1] = 0x5a;
                byteArrayofReg28[2] = 0xaf;
                byteArrayofReg28[3] = 0x02;

                uint num = uint.Parse(str1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg28[4] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(str2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg28[5] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg28[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofReg28[7] = BitConverter.GetBytes(num)[0];

                byteArrayofReg28[8] = 0x77;
            }
            catch (Exception ex)
            {
            }
            return byteArrayofReg28;
        }
        public void ConvertHexToDex(string strInput1, string strInput2, string strInput3, string strInput4, ref string strOutPut1, ref string strOutPut2)
        {
            string[] str = new string[4];
            str[0] = strInput1;
            str[1] = strInput2;
            str[2] = strInput3;
            str[3] = strInput4;

            int ndeci1 = Convert.ToInt32(str[0], 16); // hex to decimal
            int ndeci2 = Convert.ToInt32(str[1], 16); // hex to decimal
            int ndeci3 = Convert.ToInt32(str[2], 16); // hex to decimal
            int ndeci4 = Convert.ToInt32(str[3], 16); // hex to decimal

            int TotalDeci = ndeci1 + ndeci2 + ndeci3 + ndeci4;

            string shx = TotalDeci.ToString("X");

            shx = shx.PadLeft(4, '0');

            string shx1fnl = shx.Substring(0, 2);
            string shx2fnl = shx.Substring(2, 2);

            int intdx1fnl = Convert.ToInt32(shx1fnl, 16);
            int intdx2fnl = Convert.ToInt32(shx2fnl, 16);

            intdx1fnl = 255 - intdx1fnl;

            intdx2fnl = 255 - intdx2fnl;

            if (TotalDeci > 255)
            {
                intdx2fnl = intdx2fnl + 1;
            }

            strOutPut1 = intdx1fnl.ToString("X");
            strOutPut2 = intdx2fnl.ToString("X");
        }
        public byte[] PrepareBytesForNewFirmwareMaxMinSOC(int MaxSoc, int MinSoc, string SOCProtection)
        {
            byte[] byteArrayofRegE4 = new byte[12];            
            try
            {
                string sMaxHex = MaxSoc.ToString("X");
                string sMinHex = MinSoc.ToString("X");

                sMaxHex = sMaxHex.PadLeft(2, '0');
                sMinHex = sMinHex.PadLeft(2, '0');

                int ndeci1 = Convert.ToInt32("e4", 16); // hex to decimal
                int ndeci2 = Convert.ToInt32("05", 16);
                int ndeci3 = Convert.ToInt32(SOCProtection, 16);
                int ndeci4 = Convert.ToInt32(sMaxHex, 16);
                int ndeci5 = Convert.ToInt32(sMinHex, 16);
                int ndeci6 = Convert.ToInt32("18", 16);
                int ndeci7 = Convert.ToInt32("83", 16);

                int TotalDeci = ndeci1 + ndeci2 + ndeci3 + ndeci4 + ndeci5 + ndeci6 + ndeci7;

                string shx = TotalDeci.ToString("X");

                shx = shx.PadLeft(4, '0');

                string shx1fnl = shx.Substring(0, 2);
                string shx2fnl = shx.Substring(2, 2);

                int intdx1fnl = Convert.ToInt32(shx1fnl, 16);
                int intdx2fnl = Convert.ToInt32(shx2fnl, 16);

                intdx1fnl = 255 - intdx1fnl;

                intdx2fnl = 256 - intdx2fnl;
                string checksum1 = intdx1fnl.ToString("X");
                string checksum2 = intdx2fnl.ToString("X");

                byteArrayofRegE4[0] = 0xdd;
                byteArrayofRegE4[1] = 0x5a;
                byteArrayofRegE4[2] = 0xe4;
                byteArrayofRegE4[3] = 0x05;
                byteArrayofRegE4[4] = 0x18;
                byteArrayofRegE4[5] = 0x83;

                uint num = uint.Parse(SOCProtection, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(sMaxHex, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[7] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(sMinHex, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[8] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[9] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[10] = BitConverter.GetBytes(num)[0];

                byteArrayofRegE4[11] = 0x77;
            }
            catch
            {
            }
            return byteArrayofRegE4;
        }
        public byte[] PrepareBytesForNewFirmwareStartAndStopTemp(int StartTemp, int StopTemp, string HeatingMode)
        {
            byte[] byteArrayofRegE4 = new byte[14];
            string StartTempSplit1 = string.Empty;
            string StartTempSplit2 = string.Empty;
            string StopTempSplit1 = string.Empty;
            string StopTempSplit2 = string.Empty;

            try
            {
                IntToHex(StartTemp, ref StartTempSplit1, ref StartTempSplit2);
                IntToHex(StopTemp, ref StopTempSplit1, ref StopTempSplit2);

                int ndeci1 = Convert.ToInt32("e4", 16); // hex to decimal
                int ndeci2 = Convert.ToInt32("07", 16);
                int ndeci3 = Convert.ToInt32(StartTempSplit1, 16);
                int ndeci4 = Convert.ToInt32(StartTempSplit2, 16);
                int ndeci5 = Convert.ToInt32(StopTempSplit1, 16);
                int ndeci6 = Convert.ToInt32(StopTempSplit2, 16);
                int ndeci7 = Convert.ToInt32(HeatingMode, 16);
                int ndeci8 = Convert.ToInt32("18", 16);
                int ndeci9 = Convert.ToInt32("81", 16);

                int TotalDeci = ndeci1 + ndeci2 + ndeci3 + ndeci4 + ndeci5 + ndeci6 + ndeci7 + ndeci8 + ndeci9;

                string shx = TotalDeci.ToString("X");

                shx = shx.PadLeft(4, '0');

                string shx1fnl = shx.Substring(0, 2);
                string shx2fnl = shx.Substring(2, 2);

                int intdx1fnl = Convert.ToInt32(shx1fnl, 16);
                int intdx2fnl = Convert.ToInt32(shx2fnl, 16);

                intdx1fnl = 255 - intdx1fnl;

                intdx2fnl = 256 - intdx2fnl;

                string checksum1 = intdx1fnl.ToString("X");
                string checksum2 = intdx2fnl.ToString("X");

                byteArrayofRegE4[0] = 0xdd;
                byteArrayofRegE4[1] = 0x5a;
                byteArrayofRegE4[2] = 0xe4;
                byteArrayofRegE4[3] = 0x07;
                byteArrayofRegE4[4] = 0x18;
                byteArrayofRegE4[5] = 0x81;

                uint num = uint.Parse(StartTempSplit1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[6] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(StartTempSplit2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[7] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(StopTempSplit1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[8] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(StopTempSplit2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[9] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(HeatingMode, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[10] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum1, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[11] = BitConverter.GetBytes(num)[0];

                num = uint.Parse(checksum2, System.Globalization.NumberStyles.AllowHexSpecifier);
                byteArrayofRegE4[12] = BitConverter.GetBytes(num)[0];

                byteArrayofRegE4[13] = 0x77;
            }
            catch
            {
            }
            return byteArrayofRegE4;
        }

        #endregion Prepare Register Bytes And Conversion                

        #region Not In Use in This Only Declare
        public byte[] CreateH2CustomCommand(string register, string length, uint Values = 0, string Command = "")
        {
            throw new NotImplementedException("Not supported in SFKV1");
        }
        public byte[] CreateBalanceParameterSetting(int value1, int value2, int value3, int value4)
        {
            throw new NotImplementedException("Not supported in SFKV1");
        }

        #endregion Not In Use in This
    }
}
