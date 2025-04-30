using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Topicality.Domain.Entities;

public class KnowledgeFlowStep
{
    [Key]
    public long Id { get; set; }
    public string? FlowQuestion { get; set; } = null!;
    public string? FlowPrompt { get; set; } = null!;
    public int Order { get; set; }

    [ForeignKey(nameof(ComparisonSet))]
    public long? ComparisonSetId { get; set; }
    public ComparisonSet? ComparisonSet { get; set; } = null!;

    [ForeignKey(nameof(KnowledgeFlow))]
    public long? KnowledgeFlowId { get; set; }
    public KnowledgeFlow? KnowledgeFlow { get; set; } = null!;

    public bool IsStructured { get; set; } = false;
    public bool IsActive { get; set; } = true;
}