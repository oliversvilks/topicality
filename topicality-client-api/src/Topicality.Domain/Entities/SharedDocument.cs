using System.ComponentModel.DataAnnotations.Schema;

namespace Topicality.Domain.Entities;

public class SharedDocument
{
    public long Id { get; set; }

    [ForeignKey(nameof(DocumentShare))]
    public long DocumentShareId { get; set; }
    public DocumentShare DocumentShare { get; set; } = null!;

    [ForeignKey(nameof(DocumentMetadata))]
    public long DocumentMetadataId { get; set; }
    public DocumentMetadata DocumentMetadata { get; set; } = null!;
}