using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Topicality.Domain.Entities;

public class DocumentMetadata
{
    [Key]
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Filename { get; set; }
    public string? UserId { get; set; }
    public DateTime? DocumentCreated { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public string? Source { get; set; }
    public string? Extension { get; set; }
    public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime? DateUpdated { get; set; }

    [ForeignKey(nameof(Category))]
    public long? CategoryId { get; set; }
    public UserCategory? Category { get; set; }

    public List<CategoryDocument>? CategoryDocuments { get; set; } = new();
    public List<SharedDocument>? SharedDocuments { get; set; } = new();
}