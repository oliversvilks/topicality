namespace Topicality.Client.Application.Dto;

public class ComparisonQueryRequestDto
{
    public ContextFilterDto Context1 { get; set; }
    public ContextFilterDto Context2 { get; set; }
    public string Question { get; set; }
}