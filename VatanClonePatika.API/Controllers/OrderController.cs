using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Orders;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Services.Orders.Dtos;
using Services.Filters;
using Repositories.Orders;

namespace VatanClonePatika.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[TimeRestrictedAccess] // default 09:00-22:00 arası sipariş alınır
public class OrderController : CustomBaseController
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { error = "Kullanıcı girişi yapılmamış." });

        try
        {
            var result = await _orderService.CreateOrderAsync(request, userId);
            return Ok(result);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new { error = ex.Message, innerError = ex.InnerException?.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUserOrders()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { error = "Kullanıcı girişi yapılmamış." });

        try
        {
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [ServiceFilter(typeof(NotFoundFilter<Order>))]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        return Ok(await _orderService.DeleteOrderAsync(id, userId));
    }
}