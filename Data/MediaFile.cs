using System.Text.Json.Serialization;

namespace Shard.Data;

public class MediaFile
{
    public Guid Id { get; set; }
    public string Path { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Caption { get; set; }
    public FolderType Type { get; set; }
    public DateTime Created { get; set; }
    public long Size { get; set; }
    public bool IsSpoiler { get; set; }

    public string GetImagePath() => $"/api/media/image?path={Uri.EscapeDataString(Path)}";
    public string GetVideoPath() => $"/api/media/video?path={Uri.EscapeDataString(Path)}";
    public string GetThumbnailPath() => $"/api/media/thumbnail?fileId={Id}";
    
    public static MediaFile FromPath(string path)
    {
        var file = new FileInfo(path);
        return new MediaFile
        {
            Id =  Guid.NewGuid(),
            Name = file.Name,
            Path = file.FullName,
            Created = file.CreationTimeUtc,
            Size = file.Length
        };
    }
}