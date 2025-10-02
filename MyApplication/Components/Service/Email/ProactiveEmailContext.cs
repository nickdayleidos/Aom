namespace MyApplication.Components.Services.Email
{
    public sealed class ProactiveEmailContext
    {
        public DateTime ProactiveTime { get; init; } 
        public string UsnInjectionAnnouncement { get; init; } = string.Empty;
        public string UsnSiteAnnouncement { get; init; } = string.Empty;
        public string UsnStatusAnnouncement { get; init; } = string.Empty;
    }
}
