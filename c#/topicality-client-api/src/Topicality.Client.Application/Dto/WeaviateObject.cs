namespace Topicality.Client.Application.Dto;

public class WeaviateObject
{
    public WeaviateMetadata Metadata { get; set; }
    public WeaviateProperties Properties { get; set; }
    public object References { get; set; } // Assuming references can be of any type
    public object Vector { get; set; } // Assuming vector can be of any type
    public string Collection { get; set; }
}