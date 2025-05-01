namespace Topicality.Domain.Entities;

public class DocumentShare
{
    public long Id { get; set; }
    public string UserEmail { get; set; } = null!;
    public DateTime? ShareCreated { get; set; } = DateTime.UtcNow;
    public string? ShareMessage { get; set; } = null!;
    public List<SharedDocument> SharedDocuments { get; set; } = new();
    public string? Prompt { get; set; } = null!;
    public string? Answer { get; set; } = null!;
    public bool ShowAsWeb { get; set; } = false;
    public string? WebTemplate { get; set; } = null!;
    public Guid? Uuid { get; set; } = Guid.NewGuid();
}