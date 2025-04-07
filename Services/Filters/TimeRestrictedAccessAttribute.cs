using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Services.Filters;

public class TimeRestrictedAccessAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var currentTime = DateTime.Now.TimeOfDay;
        var startTime = TimeSpan.FromHours(WorkingHoursSettings.StartHour);
        var endTime = TimeSpan.FromHours(WorkingHoursSettings.EndHour);

        if (currentTime < startTime || currentTime > endTime)
        {
            context.Result = new JsonResult(new
            {
                Success = false,
                Message = "Mağazamız şu anda kapalıdır.",
                WorkingHours = new
                {
                    OpenTime = $"{WorkingHoursSettings.StartHour:00}:00",
                    CloseTime = $"{WorkingHoursSettings.EndHour:00}:00",
                    Note = $"Online mağazamız her gün {WorkingHoursSettings.StartHour:00}:00 - {WorkingHoursSettings.EndHour:00}:00 saatleri arasında hizmet vermektedir."
                }
            })
            {
                StatusCode = 403
            };
        }
    }
}