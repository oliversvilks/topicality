namespace Topicality.Client.Application.Dto;

public class WeaviateMetadata
{
    public DateTime? CreationTime { get; set; }
    public DateTime? LastUpdateTime { get; set; }
    public double? Distance { get; set; }
    public double? Certainty { get; set; }
    public double? Score { get; set; }
    public string ExplainScore { get; set; }
    public bool? IsConsistent { get; set; }
    public double? RerankScore { get; set; }
}