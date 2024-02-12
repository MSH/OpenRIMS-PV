namespace OpenRIMS.PV.Main.Core.Entities
{
    public class SystemLog : AuditedEntityBase
    {
        public SystemLog(string sender, string eventType, string exceptionCode, string exceptionMessage)
        {
            Sender = sender;
            EventType = eventType;
            ExceptionCode = exceptionCode;
            ExceptionMessage = exceptionMessage;
        }

        public string Sender { get; set; }
        public string EventType { get; set; }
        public string ExceptionCode { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string InnerExceptionStackTrace { get; set; }
        public string RemoteIpAddress { get; set; }
    }
}
