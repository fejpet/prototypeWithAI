using Microsoft.AspNetCore.Mvc;
using Moq;
using OrganizationService.Controllers;
using OrganizationService.DTOs;
using OrganizationService.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OrganizationService.Tests;

public class OrganizationControllerTests
{
    private readonly Mock<IOrganizationService> _mockService;
    private readonly OrganizationController _controller;

    public OrganizationControllerTests()
    {
        _mockService = new Mock<IOrganizationService>();
        _controller = new OrganizationController(_mockService.Object);
    }

    [Fact]
    public async Task GetSubtree_ReturnsNotFound_WhenSubtreeDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockService.Setup(s => s.GetSubtreeAsDtoAsync(nonExistentId))
            .ReturnsAsync((NodeViewDto?)null);

        // Act
        var result = await _controller.GetSubtree(nonExistentId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetSubtree_ReturnsOk_WhenSubtreeExists()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var fakeSubtree = new NodeViewDto { Id = existingId, Name = "Test Root" };
        _mockService.Setup(s => s.GetSubtreeAsDtoAsync(existingId))
            .ReturnsAsync(fakeSubtree);

        // Act
        var result = await _controller.GetSubtree(existingId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<NodeViewDto>(okResult.Value);
        Assert.Equal(existingId, returnedDto.Id);
    }
}
