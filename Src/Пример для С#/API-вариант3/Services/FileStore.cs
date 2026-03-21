using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using StorageApi.Data;
using StorageApi.Models;

namespace StorageApi.Services;

public class FileStore
{
    private readonly StoreContext _db;

    public FileStore(StoreContext db)
    {
        _db = db;
    }

    // Добавить файл в хранилище
    public async Task<(bool Ok, string Message, FileRecord? Record)> AddAsync(string path)
    {
        if (!File.Exists(path))
            return (false, "Файл не найден: " + path, null);

        ImageInfo info;
        try
        {
            info = await Image.IdentifyAsync(path);
        }
        catch (Exception ex)
        {
            return (false, "Не удалось прочитать файл: " + ex.Message, null);
        }

        var file = new FileInfo(path);

        var record = new FileRecord
        {
            Name      = file.Name,
            Size      = file.Length,
            Width     = info.Width,
            Height    = info.Height,
            Format    = file.Extension.TrimStart('.').ToLower(),
            CreatedAt = DateTime.UtcNow,
            Path      = System.IO.Path.GetFullPath(path)
        };

        _db.Files.Add(record);
        await _db.SaveChangesAsync();

        return (true, "Файл добавлен.", record);
    }

    // Получить информацию о файле
    public async Task<(bool Ok, string Message, FileDetails? Details)> GetInfoAsync(string path)
    {
        if (!File.Exists(path))
            return (false, "Файл не найден: " + path, null);

        ImageInfo info;
        try
        {
            info = await Image.IdentifyAsync(path);
        }
        catch (Exception ex)
        {
            return (false, "Не удалось прочитать файл: " + ex.Message, null);
        }

        var file    = new FileInfo(path);
        var fullPath = System.IO.Path.GetFullPath(path);
        var record  = await _db.Files.FirstOrDefaultAsync(x => x.Path == fullPath);

        var details = new FileDetails
        {
            Id         = record?.Id ?? 0,
            Name       = file.Name,
            SizeBytes  = file.Length,
            SizeText   = FormatSize(file.Length),
            Width      = info.Width,
            Height     = info.Height,
            Resolution = $"{info.Width}x{info.Height}",
            Format     = file.Extension.TrimStart('.').ToLower(),
            CreatedAt  = record?.CreatedAt ?? file.CreationTimeUtc,
            Path       = fullPath
        };

        return (true, "OK", details);
    }

    // Переименовать файл
    public async Task<(bool Ok, string Message)> RenameAsync(string path, string newName)
    {
        if (!File.Exists(path))
            return (false, "Файл не найден: " + path);

        var file    = new FileInfo(path);
        var folder  = file.DirectoryName!;
        var ext     = file.Extension;
        var final   = System.IO.Path.HasExtension(newName) ? newName : newName + ext;
        var newPath = System.IO.Path.Combine(folder, final);

        if (File.Exists(newPath))
            return (false, $"Файл '{final}' уже существует.");

        try
        {
            File.Move(path, newPath);
        }
        catch (Exception ex)
        {
            return (false, "Ошибка при переименовании: " + ex.Message);
        }

        // Обновить запись в БД если есть
        var fullPath = System.IO.Path.GetFullPath(path);
        var record   = await _db.Files.FirstOrDefaultAsync(x => x.Path == fullPath);
        if (record != null)
        {
            record.Name = final;
            record.Path = System.IO.Path.GetFullPath(newPath);
            await _db.SaveChangesAsync();
        }

        return (true, "Файл переименован: " + newPath);
    }

    // Получить все файлы из хранилища
    public async Task<List<FileShort>> GetAllAsync()
    {
        var list = await _db.Files.ToListAsync();

        return list.Select(x => new FileShort
        {
            Id         = x.Id,
            Name       = x.Name,
            Path       = x.Path,
            Format     = x.Format,
            SizeBytes  = x.Size,
            Resolution = $"{x.Width}x{x.Height}",
            CreatedAt  = x.CreatedAt
        }).ToList();
    }

    // Перевод байт в читаемый размер
    private static string FormatSize(long bytes)
    {
        string[] units = { "Б", "КБ", "МБ", "ГБ" };
        double size = bytes;
        int i = 0;
        while (size >= 1024 && i < units.Length - 1) { size /= 1024; i++; }
        return $"{size:0.##} {units[i]}";
    }
}
