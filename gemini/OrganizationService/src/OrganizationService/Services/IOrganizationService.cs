using OrganizationService.DTOs;
using OrganizationService.Entities;

namespace OrganizationService.Services;

public interface IOrganizationService
{
    Task<OrganizationNode?> GetNodeByIdAsync(Guid id);
    Task<NodeViewDto?> GetSubtreeAsDtoAsync(Guid rootNodeId);
    Task<OrganizationNode> AddNodeAsync(NodeCreateDto createDto);
    Task<bool> UpdateNodeAsync(Guid id, NodeUpdateDto updateDto);
    Task<bool> DeleteNodeAsync(Guid id);
}
