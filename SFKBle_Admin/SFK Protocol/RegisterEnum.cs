namespace SFKBle_Admin.SFK_Protocol
{
    public static class RegisterEnum 
    {
        public const string REG_ENTER_FACTORY = "00";      // 0 Decimal Value
        public const string REG_EXIT_FACTORY = "01";       // 1
        public const string REG_NEW_FIRMWARE = "02";       // 2
        public const string REG_GENERAL = "03";            // 3
        public const string REG_CELL = "04";               // 4
        public const string REG_HARDWARE = "05";           // 5
        public const string REG_USE_PASSWORD = "06";       // 6
        public const string REG_SET_PASSWORD = "07";       // 7
        public const string REG_FRESET = "0a";             // 10

        public const string REG_DESIGN_CAP = "10";         // 16
        public const string REG_CYCLE_CAP = "11";          // 17
        public const string REG_CAP_100 = "12";            // 18
        public const string REG_CAP_0 = "13";              // 19
        public const string REG_SELF_DSG_RATE = "14";      // 20
        public const string REG_MFG_DATE = "15";           // 21
        public const string REG_SERIAL_NUM = "16";         // 22
        public const string REG_CYCLE_CNT = "17";          // 23

        public const string REG_CHGOT = "18";              // 24
        public const string REG_CHGOT_REL = "19";          // 25
        public const string REG_CHGUT = "1a";              // 26
        public const string REG_CHGUT_REL = "1b";          // 27
        public const string REG_DSGOT = "1c";              // 28
        public const string REG_DSGOT_REL = "1d";          // 29
        public const string REG_DSGUT = "1e";              // 30
        public const string REG_DSGUT_REL = "1f";          // 31

        public const string REG_POVP = "20";               // 32
        public const string REG_POVP_REL = "21";           // 33
        public const string REG_PUVP = "22";               // 34
        public const string REG_PUVP_REL = "23";           // 35
        public const string REG_COVP = "24";               // 36
        public const string REG_COVP_REL = "25";           // 37
        public const string REG_CUVP = "26";               // 38
        public const string REG_CUVP_REL = "27";           // 39
        public const string REG_CHGOC = "28";              // 40
        public const string REG_DSGOC = "29";              // 41
        public const string REG_BAL_START = "2a";          // 42
        public const string REG_BAL_WINDOW = "2b";         // 43
        public const string REG_SHUNT_RES = "2c";          // 44
        public const string REG_FUNC_CONFIG = "2d";        // 45
        public const string REG_NTC_CONFIG = "2e";         // 46
        public const string REG_CELL_CNT = "2f";           // 47

        public const string REG_FET_TIME = "30";           // 48
        public const string REG_LED_TIME = "31";           // 49
        public const string REG_CAP_80 = "32";             // 50
        public const string REG_CAP_60 = "33";             // 51
        public const string REG_CAP_40 = "34";             // 52
        public const string REG_CAP_20 = "35";             // 53
        public const string REG_COVP_HIGH = "36";          // 54
        public const string REG_CUVP_HIGH = "37";          // 55
        public const string REG_SC_DSGOC2 = "38";          // 56
        public const string REG_CXVP_HIGH_DELAY_SC_REL = "39"; // 57
        public const string REG_CHG_T_DELAYS = "3a";       // 58
        public const string REG_DSG_T_DELAYS = "3b";       // 59
        public const string REG_PACK_V_DELAYS = "3c";      // 60
        public const string REG_CELL_V_DELAYS = "3d";      // 61
        public const string REG_CHGOC_DELAYS = "3e";       // 62
        public const string REG_DSGOC_DELAYS = "3f";       // 63

        public const string REG_GPS_OFF = "40";            // 64
        public const string REG_GPS_OFF_TIME = "41";       // 65
        public const string REG_CAP_90 = "42";             // 66
        public const string REG_CAP_70 = "43";             // 67
        public const string REG_CAP_50 = "44";             // 68
        public const string REG_CAP_30 = "45";             // 69
        public const string REG_CAP_10 = "46";             // 70
        public const string REG_CAP100 = "47";             // 71

        public const string REG_MFGNAME = "a0";            // 160
        public const string REG_MODEL = "a1";              // 161
        public const string REG_BARCODE = "a2";            // 162
        public const string REG_DEBUG_READ = "a3";         // 163
        public const string REG_ERROR_LOGS = "aa";         // 170

        public const string REG_MCUID = "0a";              // 10
        public const string REG_HEAT_TEST = "e4";          // 228
        public const string REG_HEAT_SOC_LIMITS = "09";    // 9

        public const string REG_CAL_CUR_IDLE = "ad";       // 173
        public const string REG_CAL_CUR_CHG = "ae";        // 174
        public const string REG_CAL_CUR_DSG = "af";        // 175

        public const string REG_CAL_V_CELL_01 = "b0";      // 176
        public const string REG_CAL_V_CELL_02 = "b1";      // 177
        public const string REG_CAL_V_CELL_03 = "b2";      // 178
        public const string REG_CAL_V_CELL_04 = "b3";      // 179
        public const string REG_CAL_V_CELL_05 = "b4";      // 180
        public const string REG_CAL_V_CELL_06 = "b5";      // 181
        public const string REG_CAL_V_CELL_07 = "b6";      // 182
        public const string REG_CAL_V_CELL_08 = "b7";      // 183
        public const string REG_CAL_V_CELL_09 = "b8";      // 184
        public const string REG_CAL_V_CELL_10 = "b9";      // 185
        public const string REG_CAL_V_CELL_11 = "ba";      // 186
        public const string REG_CAL_V_CELL_12 = "bb";      // 187
        public const string REG_CAL_V_CELL_13 = "bc";      // 188
        public const string REG_CAL_V_CELL_14 = "bd";      // 189
        public const string REG_CAL_V_CELL_15 = "be";      // 190
        public const string REG_CAL_V_CELL_16 = "bf";      // 191
        public const string REG_CAL_V_CELL_17 = "c0";      // 192
        public const string REG_CAL_V_CELL_18 = "c1";      // 193
        public const string REG_CAL_V_CELL_19 = "c2";      // 194
        public const string REG_CAL_V_CELL_20 = "c3";      // 195
        public const string REG_CAL_V_CELL_21 = "c4";      // 196
        public const string REG_CAL_V_CELL_22 = "c5";      // 197
        public const string REG_CAL_V_CELL_23 = "c6";      // 198
        public const string REG_CAL_V_CELL_24 = "c7";      // 199
        public const string REG_CAL_V_CELL_25 = "c8";      // 200
        public const string REG_CAL_V_CELL_26 = "c9";      // 201
        public const string REG_CAL_V_CELL_27 = "ca";      // 202
        public const string REG_CAL_V_CELL_28 = "cb";      // 203
        public const string REG_CAL_V_CELL_29 = "cc";      // 204
        public const string REG_CAL_V_CELL_30 = "cd";      // 205
        public const string REG_CAL_V_CELL_31 = "ce";      // 206
        public const string REG_CAL_V_CELL_32 = "cf";      // 207

        public const string REG_CAL_T_NTC_0 = "d0";        // 208
        public const string REG_CAL_T_NTC_1 = "d1";        // 209
        public const string REG_CAL_T_NTC_2 = "d2";        // 210
        public const string REG_CAL_T_NTC_3 = "d3";        // 211
        public const string REG_CAL_T_NTC_4 = "d4";        // 212
        public const string REG_CAL_T_NTC_5 = "d5";        // 213
        public const string REG_CAL_T_NTC_6 = "d6";        // 214
        public const string REG_CAL_T_NTC_7 = "d7";        // 215

        public const string REG_CAP_REMAINING = "e0";      // 224
        public const string REG_CTRL_MOSFET = "e1";        // 225
        public const string REG_CTRL_BALANCE = "e2";       // 226
        public const string REG_RESET = "e3";              // 227

        public const string REG_BAL = "D8";                // 216
        public const string REG_BAL_SET = "D9";            // 217
        public const string REG_E8 = "E8";                 // 232
        public const string REG_E3 = "E3";                 // 227
        public const string REG_SINCE_FULL = "F1";         // 241
        public const string REG_PULSE_SOC = "F2";          // 242
        public const string REG_PULSE = "F3";              // 243
        public const string REG_CHARGE_PULSE = "F4";       // 244
        public const string REG_BAL_STATUS = "F5";         // 245
        public const string REG_BLE_GAIN = "F6";           // 246
        public const string REG_CUT_OFF = "F7";            // 247
        public const string REG_FIRMWARE = "F8";           // 248

        public const string REG_WIRLESS_MODE = "E0";       // 224

        public const string REG_PANID_ZIGBEE = "E1";       // 225
        public const string REG_PANID_THREAD = "E5";       // 229

        public const string REG_CHANNEL_ZIGBEE = "E2";     // 226
        public const string REG_CHANNEL_THREAD = "E6";     // 230

        public const string REG_NETWORK_ZIGBEE = "E3";     // 227
        public const string REG_NETWORK_THREAD = "E7";     // 231

        public const string REG_NICID = "E9";             // 233

        public const string BATT_TYPE = "F9";             // 249

        public const string BIND_DEVICE = "B2";           // 178

        public const string DEBUG_MODE = "A1";            // 161

        public const string CHIPID_STATUS = "B5";         // 181

        public const string ESP_RESTART = "C4";           // 196

        public const string WRITE_CALIBRATION_ACCESS = "C5"; // 197

        public const string READ_CALIBRATION_ACCESS = "C6";  // 198

        public const string DAY_SINCE_DATE = "C7";        // 199

        public const string BLE_RESTART = "C8";           // 200

        public const string BLE_RESTART_READ = "C9";      // 201
    }

    public interface IProtocol
    {
        //Common Protocol Methods

        byte[] CreateMvByte(string Register, double UserDecValue);
        byte[] CreateMv10Byte(string Register, double UserDecValue);
        byte[] CreateTempratureByte(string Register, double UserDecValue);
        byte[] CreateReadCommand(string Register);
        byte[] CreateCustomReadCommand(string Register, string Length, uint strValues = 0);
        byte[] CreateCustomWriteCommand(string Register, string Length, uint strValues = 0);
        byte[] PrepareCustomerPassword(string sPassword);
        byte[] PrepareCustomerPassword_Reset(string soldPassword, string sPassword);
        byte[] PrepareSerialNumber(string SerialNumber);
        byte[] PrepareDefaultPin09();
        byte[] PrepareHeatSocLimit();
        int makeInt(byte b3, byte b2, byte b1, byte b0);
        byte[] PrepareRegister10Bytes(int nChargeValue);
        byte[] PrepareBytesForVoltageCalibration(byte RegisterValueinInt32, string RegisterValue, decimal VoltageValue);
        byte[] PrepareRegister11Bytes(double nChargeValue);
        void IntToHex(int number, ref string str1, ref string str2);
        byte[] GetTempratureBytes(string byteRegister, int nTemprature);
        byte[] GetIdleCurrentRegisterBytes(int Current);
        byte[] GetChargeCurrentRegisterBytes(int Current);
        byte[] GetDisChargeCurrentRegisterBytes(int Current);
        void ConvertHexToDex(string strInput1, string strInput2, string strInput3, string strInput4, ref string strOutPut1, ref string strOutPut2);
        byte[] PrepareBytesForNewFirmwareMaxMinSOC(int MaxSoc, int MinSoc, string SOCProtection);
        byte[] PrepareBytesForNewFirmwareStartAndStopTemp(int StartTemp, int StopTemp, string HeatingMode);
        byte[] StartandEndCommand(string Command = "start");

        // SFKV4Protocol
        byte[] CreateH2CustomCommand(string register, string length, uint Values = 0, string Command = "");
        public byte[] CreateBalanceParameterSetting(int value1, int value2, int value3, int value4);
    }
}
