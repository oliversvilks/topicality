namespace Topicality.Client.Application.Dto;

public class QueryRequest
{
    public string Query { get; set; }
    public string Prompt { get; set; }
    public List<string> Users { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}