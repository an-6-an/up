using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
// метод для получения списка всех студентов/справок
app.MapGet("/api/users", async (ApplicationDbContext db) => await db.Users.ToListAsync());
app.MapGet("/api/spravki", async (ApplicationDbContext db) => await db.Spravki.ToListAsync());
// метод для создания данных студента
app.MapPost("/api/users", async (User user, ApplicationDbContext db) =>
{
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    return user;
});
// справки
app.MapPost("/api/spravki", async (Spravki spravki, ApplicationDbContext db) =>
{
    await db.Spravki.AddAsync(spravki);
    await db.SaveChangesAsync();
    return spravki;
});
// метод для получения данных студента
app.MapGet("/api/users/{id}", async (ApplicationDbContext db, int id) =>
{
    var user = await db.Users.FindAsync(id); // получаем студента по id
    if (user == null)
    return Results.NotFound(); // если не найден => вывод ошибки
    return Results.Ok(user); // если найден => отправляем
});
// справки
app.MapGet("/api/ spravki /{id}", async (ApplicationDbContext db, int id) =>
{
    var spravki = await db.Spravki.FindAsync(id);
    if (spravki == null)
    return Results.NotFound();
    return Results.Ok(spravki);
});
// метод для обновления данных студента
app.MapPut("/api/users", async (User userData, ApplicationDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userData.Id); // получаем пользователя по id
    if (user == null)
    return Results.NotFound(); // если не найден, отправляем статусный код и сообщение об ошибке
    user.Name = userData.Name;
    await db.SaveChangesAsync();
    return Results.Json(user); // если пользователь найден, изменяем его данные и отправляем обратно
});
// справки
app.MapPut("/api/spravki", async (Spravki spravkiData, ApplicationDbContext db) =>
{
    var spravki = await db.Spravki.FirstOrDefaultAsync(u => u.Id == spravkiData.Id);
    if (spravki == null)
        return Results.NotFound();
    spravki.User_id = spravkiData.User_id;
    spravki.Nomer_spravki = spravkiData.Nomer_spravki;
    await db.SaveChangesAsync();
    return Results.Json(spravki);
});
// метод для удаления данных студента
app.MapDelete("/api/users/{id}", async (ApplicationDbContext db, int id) =>
{
    var user = await db.Users.FindAsync(id); // получаем студента по id
    if (user == null)
    return Results.NotFound(); // если не найден => вывод ошибки
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Ok(); // если найден => удаляем
});
// справки
app.MapDelete("/api/spravki/{id}", async (ApplicationDbContext db, int id) =>
{
    var spravki = await db.Spravki.FindAsync(id);
    if (spravki == null)
        return Results.NotFound();
    db.Spravki.Remove(spravki);
    await db.SaveChangesAsync();
    return Results.Ok();
});
app.Run();
public class User
{
    public int Id { get; set; } // ID пользователя
    public string Name { get; set; } // имя пользователя
    public string Groop { get; set; } // группа
}
public class Spravki
{
    public int Id { get; set; } // ID счёта
    public int User_id { get; set; } // ID пользователя
    public int Nomer_spravki { get; set; } // номер справки
}
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
   base(options)
    { }
    public DbSet<User> Users { get; set; }
    public DbSet<Spravki> Spravki { get; set; }
}