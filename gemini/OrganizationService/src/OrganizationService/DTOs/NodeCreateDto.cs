namespace OrganizationService.DTOs;

public class NodeCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ParentId { get; set; }
}
