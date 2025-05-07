namespace Topicality.Client.Application.Dto;

public class FlowSubmissionDto
{
    public string question { get; set; }
    public List<FlowEntry> flow { get; set; } = new List<FlowEntry>();
}