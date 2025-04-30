namespace Topicality.Client.Application.Dto;

public class WeaviateQueryResponse
{
    public string status { get; set; }
    public List<string> categories_filtered { get; set; }
    public WeaviateResponseData response { get; set; }
}