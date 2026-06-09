namespace Shard.Data;

public enum FolderType
{
    Files,
    Pictures,
    Videos,
}

public class Folder
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;
    public FolderType Type { get; set; }
}