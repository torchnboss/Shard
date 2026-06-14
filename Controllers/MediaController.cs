using Microsoft.AspNetCore.Mvc;

namespace Tamashi.Controllers;

[Route("api/media")]
public class MediaController(IConfiguration configuration) : Controller
{
    [HttpGet]
    [Route("image")]
    public IActionResult GetImage([FromQuery] string path)
    {
        return PhysicalFile(path, "image/png");
    }

    [HttpGet]
    [Route("video")]
    public IActionResult GetVideo([FromQuery] string path)
    {
        return PhysicalFile(path, "video/webm", true);
    }

    [HttpGet]
    [Route("thumbnail")]
    public IActionResult GetThumbnail([FromQuery] Guid fileId)
    {
        var contentRoot = configuration.GetValue<string>("Storage");
        var filePath = $"{contentRoot}\\Thumbnails\\{fileId}.png";
        return PhysicalFile(filePath, "image/png");
    }
}