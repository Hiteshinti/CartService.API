namespace CartService.Core.Options
{
    public sealed class ApiSettingsOptions
    {
        public const string SectionName = "ApiSettings";

        public string UserApi { get; init; } = string.Empty;
    }
}
