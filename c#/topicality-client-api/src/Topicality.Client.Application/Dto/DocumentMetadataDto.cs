namespace Topicality.Client.Application.Dto;

public class DocumentMetadataDto
{
    public string Title { get; set; }
    public string Filename { get; set; }
    public string UserId { get; set; }
    public string Category { get; set; }
    public string CategoryId { get; set; }
    public string Description { get; set; }
    public DateTime DocumentCreated { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public string Extension { get; set; }
    public string Source { get; set; }
}