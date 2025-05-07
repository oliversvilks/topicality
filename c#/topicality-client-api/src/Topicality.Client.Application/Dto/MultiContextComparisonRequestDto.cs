namespace Topicality.Client.Application.Dto;

public class MultiContextComparisonRequestDto
{
    public string Question { get; set; }
    public List<ContextFilterDto> Contexts { get; set; } = new();
}