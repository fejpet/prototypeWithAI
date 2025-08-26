using Microsoft.EntityFrameworkCore;
using OrganizationService.Data;
using OrganizationService.DTOs;
using OrganizationService.Entities;

namespace OrganizationService.Services;

public class OrganizationService : IOrganizationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrganizationService> _logger;

    public OrganizationService(AppDbContext context, ILogger<OrganizationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OrganizationNode?> GetNodeByIdAsync(Guid id)
    {
        return await _context.OrganizationNodes.FindAsync(id);
    }

    public async Task<NodeViewDto?> GetSubtreeAsDtoAsync(Guid rootNodeId)
    {
        var rootNode = await _context.OrganizationNodes.FindAsync(rootNodeId);
        if (rootNode == null)
        {
            return null;
        }

        var descendants = await _context.OrganizationNodes
            .Where(n => n.Lft >= rootNode.Lft && n.Rgt <= rootNode.Rgt)
            .OrderBy(n => n.Lft)
            .ToListAsync();

        return BuildTree(descendants);
    }

    private NodeViewDto? BuildTree(List<OrganizationNode> flatList)
    {
        if (flatList.Count == 0) return null;

        var nodeMap = flatList.ToDictionary(n => n.Id, n => new NodeViewDto
        {
            Id = n.Id,
            Name = n.Name,
            Description = n.Description,
            Level = n.Level,
            ParentId = n.ParentId
        });

        var root = nodeMap.Values.First(n => n.ParentId == null || !nodeMap.ContainsKey(n.ParentId.Value));

        foreach (var node in flatList.Where(n => n.ParentId != null))
        {
            if (nodeMap.TryGetValue(node.ParentId.Value, out var parentDto))
            {
                parentDto.Children.Add(nodeMap[node.Id]);
            }
        }
        
        return root;
    }

    public async Task<OrganizationNode> AddNodeAsync(NodeCreateDto createDto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var parent = await _context.OrganizationNodes.FindAsync(createDto.ParentId);
            if (parent == null)
            {
                throw new KeyNotFoundException("Szülő elem nem található.");
            }

            var myRight = parent.Rgt;

            // Hely csinálása az új elemnek
            await _context.OrganizationNodes
                .Where(n => n.Rgt >= myRight)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.Rgt, n => n.Rgt + 2));

            await _context.OrganizationNodes
                .Where(n => n.Lft >= myRight)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.Lft, n => n.Lft + 2));

            var newNode = new OrganizationNode
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Description = createDto.Description,
                Lft = myRight,
                Rgt = myRight + 1,
                Level = parent.Level + 1,
                ParentId = parent.Id
            };

            _context.OrganizationNodes.Add(newNode);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Új szervezeti egység hozzáadva: {NodeId}, {NodeName}", newNode.Id, newNode.Name);
            return newNode;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Hiba történt új elem hozzáadásakor.");
            throw;
        }
    }

    public async Task<bool> UpdateNodeAsync(Guid id, NodeUpdateDto updateDto)
    {
        var node = await _context.OrganizationNodes.FindAsync(id);
        if (node == null)
        {
            return false;
        }

        node.Name = updateDto.Name;
        node.Description = updateDto.Description;

        _context.OrganizationNodes.Update(node);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Szervezeti egység módosítva: {NodeId}", id);
        return true;
    }

    public async Task<bool> DeleteNodeAsync(Guid id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var nodeToDelete = await _context.OrganizationNodes.FindAsync(id);
            if (nodeToDelete == null || nodeToDelete.Lft == 1) // Gyökeret nem törölhetünk
            {
                await transaction.RollbackAsync();
                return false;
            }

            var myLeft = nodeToDelete.Lft;
            var myRight = nodeToDelete.Rgt;
            var width = myRight - myLeft + 1;

            // A csomópont és alatta lévők törlése
            await _context.OrganizationNodes
                .Where(n => n.Lft >= myLeft && n.Rgt <= myRight)
                .ExecuteDeleteAsync();

            // A fa "összezárása"
            await _context.OrganizationNodes
                .Where(n => n.Rgt > myRight)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.Rgt, n => n.Rgt - width));
                
            await _context.OrganizationNodes
                .Where(n => n.Lft > myRight)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.Lft, n => n.Lft - width));

            await transaction.CommitAsync();
            _logger.LogInformation("Szervezeti egység (és al-fája) törölve: {NodeId}", id);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Hiba történt elem törlésekor: {NodeId}", id);
            throw;
        }
    }
}
