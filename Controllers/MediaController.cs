using Microsoft.AspNetCore.Mvc;

namespace Tamashi.Controllers;

[Route("api/media")]
public class MediaController(IConfiguration configuration) : Controller
{
    [HttpGet]
    [Route("image")]
    public IActionResult GetImage([FromQuery] string path)
    {
        var bytes = System.IO.File.ReadAllBytes(path);
        return File(bytes, "image/png");
    }

    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }
    }

    [HttpGet]
    [Route("thumbnail")]
    public IActionResult GetThumbnail([FromQuery] string path)
    {
        var hash = CreateMD5(path);
        var contentRoot = configuration.GetValue<string>("Storage");
        var filePath = $"{contentRoot}\\Thumbnails\\{hash}.png";
        if (!System.IO.File.Exists(filePath))
        {
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            ffMpeg.GetVideoThumbnail(path, filePath);
        }

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, "image/png");
    }
}