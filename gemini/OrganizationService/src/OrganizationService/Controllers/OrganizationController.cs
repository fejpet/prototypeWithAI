using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationService.DTOs;
using OrganizationService.Services;

namespace OrganizationService.Controllers;

[ApiController]
[Route("api/organization")]
// [Authorize] // Ezt a sort élesben visszakapcsolnánk!
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _service;

    public OrganizationController(IOrganizationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lekérdez egy teljes rész-fát a megadott csomóponttól kezdve.
    /// </summary>
    [HttpGet("{id}/subtree")]
    [ProducesResponseType(typeof(NodeViewDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSubtree(Guid id)
    {
        var subtree = await _service.GetSubtreeAsDtoAsync(id);
        if (subtree == null)
        {
            return NotFound("A megadott azonosítóval nem található elem.");
        }
        return Ok(subtree);
    }
    
    /// <summary>
    /// Lekérdez egyetlen csomópontot (csak az adatait, a gyerekeit nem).
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NodeViewDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetNode(Guid id)
    {
        var node = await _service.GetNodeByIdAsync(id);
        if (node == null)
        {
            return NotFound();
        }
        return Ok(node);
    }

    /// <summary>
    /// Új szervezeti egységet ad hozzá a fához.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NodeViewDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddNode([FromBody] NodeCreateDto createDto)
    {
        try
        {
            var newNode = await _service.AddNodeAsync(createDto);
            return CreatedAtAction(nameof(GetNode), new { id = newNode.Id }, newNode);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, "Szerver oldali hiba történt.");
        }
    }
    
    /// <summary>
    /// Módosítja egy meglévő szervezeti egység nevét és leírását.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateNode(Guid id, [FromBody] NodeUpdateDto updateDto)
    {
        var success = await _service.UpdateNodeAsync(id, updateDto);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Töröl egy szervezeti egységet és annak összes leszármazottját.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteNode(Guid id)
    {
        var success = await _service.DeleteNodeAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}
