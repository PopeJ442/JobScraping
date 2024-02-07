namespace NabsGroup.Models
{
    public class Job
    {
        public required string Id { get; set; }
        public string? JobTitle { get; set; }
        public string? JobUrl { get; set; }
        public string? JobDescription { get; set; }
        public string? Location { get; set; }
        public DateOnly CreatedAt { get; set; }
    }
}
