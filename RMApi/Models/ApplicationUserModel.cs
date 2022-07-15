namespace RMApi.Models
{
    public class ApplicationUserModel
    {
        public string? Id { get; set; }
        public string? EmailAddress { get; set; }
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();

    }
}
