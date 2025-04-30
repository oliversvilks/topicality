namespace Topicality.Client.Application.Dto;

public class CopyDocumentRequestDto
{
    public string TargetCollection { get; set; }
    public string SourceCategory { get; set; }
    public string TargetCategory { get; set; }
    public List<string> Documents { get; set; } = new();
    public string Description { get; set; }
}