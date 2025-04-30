namespace Topicality.Client.Application.Dto;

public class ContextFilterDto
{
    public List<string> Collections { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}