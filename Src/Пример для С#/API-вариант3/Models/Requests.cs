namespace StorageApi.Models;

// Запрос — добавить файл
public class AddFileRequest
{
    public string Path { get; set; } = string.Empty;
}

// Запрос — переименовать файл
public class RenameFileRequest
{
    public string Path { get; set; } = string.Empty;
    public string NewName { get; set; } = string.Empty;
}

// Ответ — подробная информация о файле
public class FileDetails
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string SizeText { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Path { get; set; } = string.Empty;
}

// Ответ — краткая информация (для списка)
public class FileShort
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
