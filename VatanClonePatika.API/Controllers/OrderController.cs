using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Orders;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Services.Orders.Dtos;
using Services.Filters;
using Repositories.Orders;

namespace VatanClonePatika.API.Controllers;


[Authorize]
public class OrderController(IOrderService orderService) : CustomBaseController
{


    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        // Tüm claim'leri kontrol edelim
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"UserId from token: {userId}");

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { error = "Kullanıcı girişi yapılmamış." });

        try
        {
            var result = await orderService.CreateOrderAsync(request, userId);
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
            var orders = await orderService.GetUserOrdersAsync(userId);
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

        return CreateActionResult(await orderService.DeleteOrderAsync(id, userId));
    }
}