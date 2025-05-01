using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Topicality.Domain.Entities;

public class UserCategory
{
    [Key]
    public long Id { get; set; }
    public string UserEmail { get; set; } = null!;
    public string? Name { get; set; }
    public Guid? Uuid { get; set; } = Guid.NewGuid();
    public string? Description { get; set; } = null!;

    [InverseProperty(nameof(SchemaDefinition.Category))]
    public SchemaDefinition? SchemaDefinition { get; set; }

    [InverseProperty(nameof(CategoryDocument.Category))]
    public List<CategoryDocument>? CategoryDocuments { get; set; } = new();
}