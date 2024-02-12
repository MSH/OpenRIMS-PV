namespace OpenRIMS.PV.Main.API.Infrastructure.Configs.Settings
{
    public class EventBusSettings
    {
        public bool Enabled { get; set; }
        public string SubscriptionClientName { get; set; }
        public string EventBusConnection { get; set; }
        public int EventBusRetryCount { get; set; }
        public string EventBusUserName { get; set; }
        public string EventBusPassword { get; set; }
    }
}
