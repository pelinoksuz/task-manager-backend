using Microsoft.AspNetCore.Mvc; //MVC = Model – View – Controller

[ApiController] // Request validation otomatik olur, JSON binding düzgün olur, Swagger entegrasyonu kolaylaşır
[Route("api/[controller]")] // [controller] → class adı - controller isminin değişmesine karşın
public class HealthController : ControllerBase
{
    [HttpGet] // [HttpPost] [HttpPut] [HttpDelete] gibi HTTP metodlarını belirtir
    public IActionResult Get() // IActionResult → Esnek response döneriz: Ok() , NotFound() , BadRequest() gibi
    {
        return Ok(new { status = "ok" }); // 200 OK status kodu ile JSON response döner
    }
}
