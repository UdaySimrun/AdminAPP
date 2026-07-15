using Java.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFKBle.Models
{
    public class APIExtensions_Response
    {
        public APIExtensions_Response()
        {
            ActionResponse = new ApiActionResponse();
        }

        public ApiActionResponse ActionResponse { get; set; }
    }

    public class APIExtensionsSettings_Response
    {
        public APIExtensionsSettings_Response()
        {
            ActionResponse = new ApiActionResponse();
        }

        public ApiActionResponse ActionResponse { get; set; }
    }
    public enum ResultType
    {
        Success = 1,
        Fail = 2,
        Error = 3
    }

    public class Response
    {
        public ResultType ResultType { get; set; }
        public string message { get; set; }
        public Exception exception { get; set; }
        public bool IsRequestRunning { get; set; }
    }

    public class ApiActionResponse
    {
        public ApiActionResponse()
        {
            Response = new Response();
        }

        public string RequestURL { get; set; }
        public Response Response { get; set; }
    }

    public class RemoteSession
    {
        public int ID { get; set; }
        public string DeviceID { get; set; }
        public string CustomerName { get; set; }
        public string VerificationPIN { get; set; }
        public string SessionStatus { get; set; }
        public string IPAddress { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? SessionStartDate { get; set; }
        public DateTime? SessionEndDate { get; set; }
        public TimeSpan TimeDiffrence { get; set; }
        public bool ClickEnable { get; set; } = false;
        public string EnableImage { get; set; } = "enable.png";
        public string DisableImage { get; set; } = "mute.png";
    }
}
