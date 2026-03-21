namespace StorageApi.Models;

// Одна запись о файле в базе данных
public class FileRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Format { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Path { get; set; } = string.Empty;
}
