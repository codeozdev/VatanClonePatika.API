using System.Net;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace VatanClonePatika.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CustomBaseController : ControllerBase
{
    [NonAction]
    public IActionResult CreateActionResult<T>(ServiceResult<T> result)
    {
        if (result.Status == HttpStatusCode.NoContent)
        {
            return NoContent();
        }

     
        if (result.Status == HttpStatusCode.Created)
        {
            return Created(result.UrlAsCreated, result); 
        }

        return new ObjectResult(result) { StatusCode = result.Status.GetHashCode() };
    }

    [NonAction]
    public IActionResult CreateActionResult(ServiceResult result)
    {
        if (result.Status == HttpStatusCode.NoContent)
        {
            return NoContent();
        }

        return new ObjectResult(result) { StatusCode = result.Status.GetHashCode() };
    }
}