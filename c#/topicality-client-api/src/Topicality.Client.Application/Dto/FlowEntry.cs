namespace Topicality.Client.Application.Dto;

public class FlowEntry
{
    public string entrance_category { get; set; }
    public List<FlowContext> contexts { get; set; } = new List<FlowContext>();
}