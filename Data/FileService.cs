using LiteDB;

namespace Shard.Data;

public class FileService(GlobalData data, IConfiguration configuration)
{
    public List<KeyValuePair<string, int>> GetScores()
    {
        if (data.ActivePath == null) return [];

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<MediaFile>("files");
        var pics = col.Query().Where(x => x.Type == FolderType.Pictures).Count();
        var vids = col.Query().Where(x => x.Type == FolderType.Videos).Count();

        return [new KeyValuePair<string, int>("Картинки", pics), new KeyValuePair<string, int>("Видосы", vids)];
    }

    public List<MediaFile> GetFiles(FolderType folderType)
    {
        if (data.ActivePath == null) return [];

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<MediaFile>("files");

        col.EnsureIndex(x => x.Path);

        var results = col.Query()
            .Where(x => x.Type == folderType)
            .OrderBy(x => x.Name)
            .ToList();

        return results;
    }

    public int SyncFiles(FolderType folderType)
    {
        if (data.ActivePath == null) return 0;

        using var db = new LiteDatabase(data.ActivePath);
        var foldersCol = db.GetCollection<Folder>("folders");
        var filesCol = db.GetCollection<MediaFile>("files");
        var folders = foldersCol.Query()
            .Where(x => x.Type == folderType)
            .ToList();
        var real = filesCol.Query()
            .Where(x => x.Type == folderType)
            .ToList();

        Func<MediaFile, bool> filter =
            folderType switch
            {
                FolderType.Pictures => x =>
                    x.Name.EndsWith(".jpg") || x.Name.EndsWith(".png") || x.Name.EndsWith(".gif"),
                FolderType.Videos => x => x.Name.EndsWith(".webm") || x.Name.EndsWith(".mp4"),
                _ => throw new ArgumentOutOfRangeException(nameof(folderType), folderType, null)
            };

        var counter = 0;
        foreach (var folder in folders)
        {
            var files = Directory
                .GetFiles(folder.Path)
                .Select(MediaFile.FromPath)
                .Where(filter)
                .ToArray();

            foreach (var file in files)
            {
                if (real.Exists(x => x.Path == file.Path)) continue;
                file.Type = folderType;
                filesCol.Insert(file);
                if (folderType == FolderType.Videos)
                {
                    var contentRoot = configuration.GetValue<string>("Storage");
                    var filePath = $"{contentRoot}\\Thumbnails\\{file.Id}.png";
                    if (!File.Exists(filePath))
                    {
                        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                        ffMpeg.GetVideoThumbnail(file.Path, filePath);
                    }
                }

                counter++;
            }
        }

        return counter;
    }

    public void SaveFile(MediaFile file)
    {
        if (data.ActivePath == null) return;

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<MediaFile>("files");
        col.Update(file);
    }
}