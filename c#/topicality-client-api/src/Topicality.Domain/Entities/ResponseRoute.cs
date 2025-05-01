using System.ComponentModel.DataAnnotations;

namespace Topicality.Domain.Entities;

public class ResponseRoute
{
    [Key]
    public long Id { get; set; }
    public string? Collection { get; set; } = null!;
    public List<string>? Categories { get; set; } = new();
}