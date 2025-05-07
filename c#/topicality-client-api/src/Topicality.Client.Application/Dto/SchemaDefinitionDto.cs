namespace Topicality.Client.Application.Dto;

public class SchemaDefinitionDto
{
    public string Title { get; set; }
    public string Filename { get; set; }
    public string User { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}