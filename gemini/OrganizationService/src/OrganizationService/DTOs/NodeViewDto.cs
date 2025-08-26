namespace OrganizationService.DTOs;

public class NodeViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Level { get; set; }
    public Guid? ParentId { get; set; }
    public List<NodeViewDto> Children { get; set; } = new();
}
