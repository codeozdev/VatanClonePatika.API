using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Filters;

namespace VatanClonePatika.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SettingsController : ControllerBase
{
    [HttpGet("working-hours")]
    public IActionResult GetWorkingHours()
    {
        return Ok(new
        {
            Success = true,
            Message = "Mağaza çalışma saatleri",
            WorkingHours = new
            {
                StartTime = $"{WorkingHoursSettings.StartHour:00}:00",
                EndTime = $"{WorkingHoursSettings.EndHour:00}:00",
                Note = $"Mağazamız her gün {WorkingHoursSettings.StartHour:00}:00 - {WorkingHoursSettings.EndHour:00}:00 saatleri arasında hizmet vermektedir."
            }
        });
    }

    [HttpPut("working-hours")]
    public IActionResult UpdateWorkingHours([FromBody] UpdateWorkingHoursRequest request)
    {
        if (request.StartHour < 0 || request.StartHour > 23 ||
            request.EndHour < 0 || request.EndHour > 23)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Çalışma saatleri 00-23 arasında olmalıdır."
            });
        }

        if (request.StartHour >= request.EndHour)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Başlangıç saati bitiş saatinden küçük olmalıdır."
            });
        }

        WorkingHoursSettings.StartHour = request.StartHour;
        WorkingHoursSettings.EndHour = request.EndHour;

        return Ok(new
        {
            Success = true,
            Message = "Çalışma saatleri güncellendi",
            WorkingHours = new
            {
                StartTime = $"{WorkingHoursSettings.StartHour:00}:00",
                EndTime = $"{WorkingHoursSettings.EndHour:00}:00",
                Note = $"Mağazamız her gün {WorkingHoursSettings.StartHour:00}:00 - {WorkingHoursSettings.EndHour:00}:00 saatleri arasında hizmet vermektedir."
            }
        });
    }
}

public class UpdateWorkingHoursRequest
{
    public int StartHour { get; set; }
    public int EndHour { get; set; }
}