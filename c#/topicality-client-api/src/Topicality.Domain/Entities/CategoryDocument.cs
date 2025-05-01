using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Topicality.Domain.Entities;

public class CategoryDocument
{
    [Key]
    public long Id { get; set; }

    [ForeignKey(nameof(Category))]
    public long? CategoryId { get; set; }
    public UserCategory? Category { get; set; }

    [ForeignKey(nameof(DocumentMetadata))]
    public long? DocumentMetadataId { get; set; }
    public DocumentMetadata? DocumentMetadata { get; set; }

    public Guid? Uuid { get; set; } = Guid.NewGuid();
}