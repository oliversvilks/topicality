using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Topicality.Domain.Entities;

public class ComparisonSet
{
    [Key]
    public long Id { get; set; }

    [ForeignKey(nameof(Comparison))]
    public long ComparisonId { get; set; }
    public Comparison? Comparison { get; set; } = null!;

    public List<DocumentMetadata>? Documents { get; set; } = new();
}