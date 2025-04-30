namespace Topicality.Client.Application.Dto;

public class WeaviateProperties
{
    public string Description { get; set; }
    public string ChunkType { get; set; }
    public DateTime DateCreated { get; set; }
    public string User { get; set; }
    public string Extension { get; set; }
    public long CategoryId { get; set; }
    public string Filename { get; set; }
    public string Text { get; set; }
    public int? ChunkIndex { get; set; }
    public string Source { get; set; }
    public string FilePath { get; set; }
    public string Category { get; set; }
    public string Title { get; set; }
    public DateTime DateUpdated { get; set; }
    public DateTime DocumentCreated { get; set; }
}