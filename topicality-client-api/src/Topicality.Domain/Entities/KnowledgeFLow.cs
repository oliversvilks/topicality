using System.ComponentModel.DataAnnotations;

namespace Topicality.Domain.Entities;

public class KnowledgeFlow
{
    [Key]
    public long Id { get; set; }
    public string? Name { get; set; } = null!;
    public string? UserEmail { get; set; } = null!;
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime? ExecutionTime { get; set; }
    public bool Recurring { get; set; } = false;

    public List<KnowledgeFlowStep>? KnowledgeFlowSteps { get; set; } = new();
}