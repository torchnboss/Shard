namespace Shard.Data;

public class GlobalData(IConfiguration configuration)
{
    public string? ActiveShard { get; set; }

    public string? ActivePath => ActiveShard == null ? null : configuration.GetValue<string>("Storage") + $"\\Shards\\{ActiveShard}.db";

    public List<string> GetShards()
    {
        var path = configuration.GetValue<string>("Storage") + "\\Shards";
        var files = Directory.GetFiles(path, "*.db").Select(x => Path.GetFileNameWithoutExtension(x)!).Order().ToList();
        if (ActiveShard == null && files.Count > 0)
            ActiveShard = files[0];
        return files;
    }

    public void CreateShard(string name)
    {
        if (ActivePath == null) return;
        
        ActiveShard = name;
        File.Create(ActivePath).Close();
    }
}