using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Topicality.Domain.Entities;

public class SchemaDefinition
{
    [Key]
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Filename { get; set; }
    public string? User { get; set; }
    public string? Description { get; set; }

    [ForeignKey(nameof(Category))]
    public long? CategoryId { get; set; }
    public UserCategory? Category { get; set; }

    public DateTime? DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
}