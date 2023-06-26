namespace PVIMS.API.Infrastructure.Configs.Settings
{
    public class EventBusSettings
    {
        public string SubscriptionClientName { get; set; }
        public string EventBusConnection { get; set; }
        public int EventBusRetryCount { get; set; }
        public string EventBusUserName { get; set; }
        public string EventBusPassword { get; set; }
    }
}
