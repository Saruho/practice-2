using Microsoft.AspNetCore.Mvc;
using StorageApi.Models;
using StorageApi.Services;

namespace StorageApi.Controllers;

[ApiController]
[Route("api/image")]
[Produces("application/json")]
public class StoreController : ControllerBase
{
    private readonly FileStore _store;

    public StoreController(FileStore store)
    {
        _store = store;
    }

    // POST api/image/add — добавить файл
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddFileRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Path))
            return BadRequest(new { error = "Укажите путь к файлу." });

        var (ok, message, record) = await _store.AddAsync(req.Path);

        if (!ok)
            return BadRequest(new { error = message });

        return StatusCode(201, new { message, file = record });
    }

    // GET api/image/info?path=... — информация о файле
    [HttpGet("info")]
    public async Task<IActionResult> Info([FromQuery] string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest(new { error = "Укажите путь к файлу." });

        var (ok, message, details) = await _store.GetInfoAsync(path);

        if (!ok)
            return NotFound(new { error = message });

        return Ok(details);
    }

    // PUT api/image/change/name — переименовать файл
    [HttpPut("change/name")]
    public async Task<IActionResult> Rename([FromBody] RenameFileRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Path))
            return BadRequest(new { error = "Укажите путь к файлу." });

        if (string.IsNullOrWhiteSpace(req.NewName))
            return BadRequest(new { error = "Укажите новое имя." });

        var (ok, message) = await _store.RenameAsync(req.Path, req.NewName);

        if (!ok)
            return BadRequest(new { error = message });

        return Ok(new { message });
    }

    // GET api/image — все файлы в хранилище
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _store.GetAllAsync();
        return Ok(list);
    }
}
