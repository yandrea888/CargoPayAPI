using CargoPayAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/fee")]
public class FeeController : ControllerBase
{
    [Authorize]
    [HttpGet("current")]
    public IActionResult GetCurrentFee()
    {
        decimal fee = FeeService.Instance.GetCurrentFee();
        return Ok(new { fee });
    }
}
