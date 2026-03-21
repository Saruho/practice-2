using Microsoft.EntityFrameworkCore;
using StorageApi.Data;
using StorageApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// База данных SQLite
builder.Services.AddDbContext<StoreContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=store.db"));

// Сервис для работы с файлами
builder.Services.AddScoped<FileStore>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Создать таблицы при запуске
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StoreContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Storage API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
