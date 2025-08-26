using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganizationService.Entities;

[Table("organization_nodes")]
public class OrganizationNode
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Lft { get; set; }
    public int Rgt { get; set; }
    public int Level { get; set; } // A fa mélysége, 0 a gyökér

    public Guid? ParentId { get; set; }
}
