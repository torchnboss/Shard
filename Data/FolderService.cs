using LiteDB;

namespace Shard.Data;

public class FolderService(GlobalData data)
{
    public List<Folder> GetFolders(FolderType folderType)
    {
        if(data.ActivePath == null) return [];
        
        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<Folder>("folders");

        col.EnsureIndex(x => x.Path);

        var results = col.Query()
            .Where(x => x.Type == folderType)
            .OrderBy(x => x.Name)
            .ToList();

        return results;
    }

    public Folder? AddFolder(string path, FolderType folderType)
    {
        if(data.ActivePath == null) return null;
        
        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<Folder>("folders");
        var real = col.Query().Where(x => x.Path == path && x.Type == folderType).FirstOrDefault();
        if (real != null) return null;
        var folder = new Folder
        {
            Id = Guid.NewGuid(),
            Path = path,
            Name = path.Split(new [] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).Last(),
            Type = folderType
        };

        col.Insert(folder);

        return folder;
    }

    public void DeleteFolder(Guid folderId)
    {
        if(data.ActivePath == null) return;
        
        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<Folder>("folders");
        col.Delete(folderId);
    }
}