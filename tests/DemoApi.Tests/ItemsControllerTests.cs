using DemoApi.Controllers;
using DemoApi.Data;
using DemoApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DemoApi.Tests;

public class ItemsControllerTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Create_Then_GetAll_ReturnsTheCreatedItem()
    {
        var context = CreateContext();
        var controller = new ItemsController(context);

        await controller.Create(new Item { Name = "Test item" });
        var result = await controller.GetAll();

        Assert.Single(result.Value!);
        Assert.Equal("Test item", result.Value!.First().Name);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var context = CreateContext();
        var controller = new ItemsController(context);

        var result = await controller.GetById(999);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Delete_ExistingItem_RemovesIt()
    {
        var context = CreateContext();
        var controller = new ItemsController(context);
        var created = await controller.Create(new Item { Name = "To delete" });
        var createdItem = (Item)((Microsoft.AspNetCore.Mvc.CreatedAtActionResult)created.Result!).Value!;

        await controller.Delete(createdItem.Id);
        var result = await controller.GetAll();

        Assert.Empty(result.Value!);
    }
}
