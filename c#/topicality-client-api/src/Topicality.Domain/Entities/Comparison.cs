using System.ComponentModel.DataAnnotations;

namespace Topicality.Domain.Entities;

public class Comparison
{
    [Key]
    public long Id { get; set; }
    public DateTime? ComparisonDate { get; set; } = DateTime.UtcNow;
    public List<ComparisonSet>? ComparisonSets { get; set; } = new();
    public string? ComparisonMessage { get; set; } = null!;
    public string? ComparisonAnswer { get; set; } = null!;
}