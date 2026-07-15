using Microsoft.AspNet.SignalR.Client;
using SFKBle_Admin;
using SFKBle_Admin.SFK_Protocol;
using System.Collections;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static Android.Net.Wifi.WifiConfiguration;

namespace SFKBle.Models
{
    public class RemoteSessionService : IRemoteSessionService
    {
        HubConnection _conn; IHubProxy _hub; bool _started; string _deviceId;

        private bool _peerDisconnected;
        public bool IsPeerDisconnected
        {
            get
            {
                return _peerDisconnected;
            }
        }
        private DateTime _lastPongUtc = DateTime.UtcNow; private System.Timers.Timer _pingTimer; private readonly TimeSpan _timeout = TimeSpan.FromSeconds(25);
        //public const string SERVER_URL = "https://sunfunkits.com:8085";
        public const string SERVER_URL = "http://192.168.0.111/comrogenapi";
        const string SignalR_URL = SERVER_URL + "/signalr";
        string ReceiverID = string.Empty;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        public event Action<string, string, string> OnReceive;
        public bool IsConnected
        {
            get
            {
                return _started && _conn != null && _conn.State == ConnectionState.Connected;
            }
        }
        public async Task StartAsync(string sReceiverID = "")
        {
            try
            {
                if (_conn != null) { try { _conn.Stop(); _conn.Dispose(); } catch { } _conn = null; _hub = null; _started = false; }

                ReceiverID = sReceiverID;

                if (IsConnected) return;

                _deviceId = ReceiverID + "_ADMIN";

                _conn = new HubConnection(SignalR_URL);
                _hub = _conn.CreateHubProxy("SFKRemoteHub");

                _hub.On<string, string, string>("Receive",
                async (from, type, json) =>
                {
                    if (type == RemoteSessionReceiveDataTypes.Ping.ToString())
                    {
                        SendPingAsync(RemoteSessionSendCommandTypes.Pong, null);

                        return;
                    }

                    if (type == RemoteSessionReceiveDataTypes.Pong.ToString())
                    {
                        _lastPongUtc = DateTime.UtcNow;
                        _peerDisconnected = false;
                        return;
                    }

                    OnReceive?.Invoke(from, type, json);
                });

                _conn.Reconnected += async () =>
                {
                    if (_conn.State != ConnectionState.Connected)
                    {
                        return;
                    }

                    await _sendLock.WaitAsync();

                    try
                    {
                        await _hub.Invoke("Register", _deviceId);
                    }
                    finally
                    {
                        _sendLock.Release();
                    }
                };

                _conn.Closed += () => _started = false;

                await _conn.Start();
                await _hub.Invoke("Register", _deviceId);

                _started = true;

                _lastPongUtc = DateTime.UtcNow;
                _peerDisconnected = false;

                StartPingTimer();
            }
            catch (Exception ex)
            {
                _started = false;
            }
        }
        public async Task StopAsync()
        {
            try
            {
                if (_conn != null)
                {
                    _conn.Stop();
                    _conn.Dispose();
                }

                _hub = null;
                _conn = null;
                _started = false;
                ReceiverID = string.Empty;
                RemoteSessionDetails.RemoteSessionDeviceGUID = null;
                StopPingTimer();
            }
            catch (Exception ex)
            {
            }
        }
        private void StartPingTimer()
        {
            try
            {
                _pingTimer = new System.Timers.Timer();

                _pingTimer.Interval = 10000;

                _pingTimer.AutoReset = true;

                _pingTimer.Elapsed += async (s, e) =>
                {
                    if (_conn == null || _conn.State != ConnectionState.Connected)
                    {
                        return;
                    }

                    if (_lastPongUtc != DateTime.MinValue && (DateTime.UtcNow - _lastPongUtc > _timeout))
                    {
                        _peerDisconnected = true;
                        StopPingTimer();
                    }

                    SendPingAsync(RemoteSessionSendCommandTypes.Ping, null);
                };

                _pingTimer.Start();
            }
            catch (Exception ex)
            {
            }
        }
        private void StopPingTimer()
        {
            try
            {
                if (_pingTimer == null)
                {
                    return;
                }

                _pingTimer.Stop();

                _pingTimer.Dispose();

                _pingTimer = null;

                _lastPongUtc = DateTime.MinValue;
            }
            catch (Exception ex)
            {
            }
        }
        public async Task SendAsync(RemoteSessionSendCommandTypes type, object data)
        {
            try
            {
                if (_conn == null || _conn.State != ConnectionState.Connected || string.IsNullOrWhiteSpace(ReceiverID))
                {
                    return;
                }

                await _sendLock.WaitAsync();

                try
                {
                    var json = JsonSerializer.Serialize(data);

                    await _hub.Invoke("Relay", _deviceId, ReceiverID, type.ToString(), json);
                }
                catch (InvalidOperationException)
                {
                    if (_conn.State == ConnectionState.Reconnecting)
                    {
                        return;
                    }
                }
                finally
                {
                    _sendLock.Release();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public async Task SendPingAsync(RemoteSessionSendCommandTypes type, object data)
        {
            try
            {
                if (_conn == null || _conn.State != ConnectionState.Connected || string.IsNullOrWhiteSpace(_deviceId))
                {
                    return;
                }

                var json = JsonSerializer.Serialize(data);
                await _hub.Invoke("Relay", _deviceId, ReceiverID, type.ToString(), json);
            }
            catch (Exception ex)
            {
            }
        }
    }
    public interface IRemoteSessionService
    {
        event Action<string, string, string> OnReceive;
        Task StartAsync(string sReceiverID = "");
        Task StopAsync();
        Task SendAsync(RemoteSessionSendCommandTypes type, object data);
        bool IsConnected { get; }
        bool IsPeerDisconnected { get; }
    }
    public enum RemoteSessionReceiveDataTypes
    {
        VirtualDeviceList,
        BluetoothDeviceList,
        ViewAllDevicesList,
        USBDeviceList,
        SelectedDevice,
        BSelectedDevice,
        VSelectedDevice,
        byteCommand,
        DeviceConnectionStatus,
        ProtocolType,
        ReEstablishConnection,
        SuccessDetailPage,
        SuccessMultiPage,
        GetProtocolType,
        RemoteSessionEnd,
        bFirmwareUpdated,
        Error,
        Ping,
        Pong,
        DisconnectDevice
    }
    public enum RemoteSessionSendCommandTypes
    {
        Scan,
        RestoreOrForgotPINDeviceConnect,
        Disconnect,
        GetProtocolType,
        ViewAllDevice,
        ConnectDevices,
        DiscounnectDevice,
        CheckDeviceConnectionStatus,
        ProceedForDetailPage,
        SubmitBatterySetup,
        ProceedForMultiView,
        CheckBox_Check,
        BusyIndicator,
        ShowDisplayPopup,
        DisplayPopup_Clicked,
        TabSwitch,
        ComfirmPassword,
        ScrollEvent,
        BatteryConnectionSetup,
        RemoteSessionEnd,
        ActiveBalancerToolTab,
        byteCommand,
        ToolTabUpdate,
        DeatilTabSegement,
        VoltageCalibration,
        TemperatureCalibraion,
        DeclineRequest,
        CaseTemperatureToggle,
        SwitchGraphToggle,
        SelectWirelessMode,
        CheckBoxValueChange,
        SfSliderValueChange,
        RadioButtonValueChange,
        FirmWareCheckForUpdate,
        FirmWareVersionSelectedIndex,
        FirmWareProceedForUpdate,
        Ping,
        Pong
    }
    public static class Serializer
    {
        public static string Serialize<T>(T data) =>
            JsonSerializer.Serialize(data);

        public static T Deserialize<T>(string json) =>
            JsonSerializer.Deserialize<T>(json);
    }
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status;

            if (value == null)
            {
                return Colors.Red;
            }

            status = value.ToString();

            if (string.Equals(status, "PENDING", StringComparison.OrdinalIgnoreCase))
            {
                return Colors.DarkOrange;
            }

            if (string.Equals(status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            {
                return Colors.Green;
            }

            if (string.Equals(status, "VERIFIED", StringComparison.OrdinalIgnoreCase))
            {
                return Colors.Green;
            }

            if (string.Equals(status, "CLOSED", StringComparison.OrdinalIgnoreCase))
            {
                return Colors.Red;
            }

            return Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public static class RemoteSessionDetails
    {
        public static RemoteSessionService _remote;
        public static string CustomerName { get; set; }
        public static string RemoteSessionDeviceGUID { get; set; }
        public static int SetFailCount = 0;
        public static async Task SendDataToRemoteSession(RemoteSessionSendCommandTypes DataType, object DeviceData)
        {
            try
            {
                if (_remote.IsConnected)
                {
                    await _remote.SendAsync(DataType, DeviceData);
                }
                else
                {
                    if (SetFailCount < 5)
                    {
                        SetFailCount++;
                    }
                    else
                    {
                        SetFailCount = 0;
                        await _remote.StartAsync();
                    }
                }
            }
            catch { }
        }
        public static async Task StopRemoteSessionService()
        {
            try
            {
                _remote = App.Services.GetService<RemoteSessionService>();

                if (_remote.IsConnected)
                {
                    await UpdateRemoteSession("END");
                    await _remote.StopAsync();
                }
            }
            catch (Exception ex) { }
        }
        public static bool CheckRemoteSessionPingStatus()
        {
            try
            {
                if (_remote.IsPeerDisconnected)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
            }
            return true;
        }
        public static async Task UpdateRemoteSession(string Command)
        {
            string Result = string.Empty;
            APIExtensions_Response ApiActionResponse = new APIExtensions_Response();
            try
            {
                string ApiURL = RemoteSessionService.SERVER_URL + "/APIExtensions/Execute/ComrogenAPI_Extensions_SFKRemoteAPP/UpdateRemoteSession";
                ArrayList objParam = new ArrayList();
                objParam.Add(RemoteSessionDeviceGUID);
                objParam.Add(Command);
                HttpResponseMessage response = await PostApiAsync(ApiURL, objParam);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ApiActionResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<APIExtensions_Response>(data);
                    if (ApiActionResponse.ActionResponse.Response.ResultType == ResultType.Success)
                    {
                        Result = ApiActionResponse.ActionResponse.Response.message;
                    }
                    else
                    {
                        Result = ApiActionResponse.ActionResponse.Response.message;
                    }
                }
            }
            catch (Exception ex)
            {
                Result = "Fail";
            }
        }
        public static async Task<string> GetIPAddress()
        {
            string IpAddress = string.Empty;
            try
            {
                string[] PublicIpApis = new[] { "https://api.ipify.org", "https://ifconfig.me", "https://ipinfo.io/ip", "https://icanhazip.com", "https://checkip.amazonaws.com" };
                using var httpClient = new HttpClient();
                foreach (var api in PublicIpApis)
                {
                    try
                    {
                        string publicIp = await httpClient.GetStringAsync(api);
                        if (!string.IsNullOrWhiteSpace(publicIp))
                        {
                            IpAddress = publicIp.Trim();
                            break;
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex) { }
            return IpAddress;
        }


        public static async Task<HttpResponseMessage?> PostApiAsync(string apiUrl, ArrayList payload)
        {
            try
            {
                var handler = new HttpClientHandler();

                // ⚠️ TEMP FIX for Fire Tablet SSL issue (REMOVE in production)
                handler.ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.PostAsJsonAsync(apiUrl, payload).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"HTTP ERROR: {response.StatusCode}");
                        return null;
                    }

                    var data = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(data))
                    {
                        System.Diagnostics.Debug.WriteLine("EMPTY RESPONSE BODY");
                        return null;
                    }

                    return response;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public static async Task<HttpResponseMessage> PostApiAsync(string apiUrl, ArrayList payload)
        //{
        //    HttpResponseMessage _HttpResponseMessage = new HttpResponseMessage();
        //    try
        //    {
        //        HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        //        _client.BaseAddress = new System.Uri(apiUrl);
        //        _client.DefaultRequestHeaders.Accept.Clear();
        //        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var response = await _client.PostAsJsonAsync(apiUrl, payload).ConfigureAwait(false);
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            return _HttpResponseMessage;
        //        }
        //        var data = await response.Content.ReadAsStringAsync();
        //        if (string.IsNullOrWhiteSpace(data))
        //        {
        //            return _HttpResponseMessage;
        //        }
        //        _HttpResponseMessage = response;
        //    }
        //    catch (Exception ex) { }
        //    return _HttpResponseMessage;
        //}
    }

}