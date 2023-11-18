using Microsoft.Extensions.Caching.Distributed;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Redis_Demo_";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

User[] users = new[]
{
    new User(1, "Emre AKA", "aka.emre@hotmail.com", "cranberry"),
    new User(2, "Emine AKA", "aka.emine@hotmail.com", "orange"),
    new User(3, "Ridvan AKA", "aka.ridvan@hotmail.com", "banana"),
    new User(4, "Eray AKA", "aka.eray@hotmail.com", "dirty-ass"),
};

var userGroup = app.MapGroup("users").WithTags("Users");

userGroup.MapGet("/", async (IDistributedCache distributedCache) =>
{
    var cache = await distributedCache.GetRecordAsync<User[]>("Users");
    if (cache is not null) return Results.Ok(cache);
    
    await Task.Delay(2500);

    await distributedCache.SetRecordAsync<User[]>("Users", users, TimeSpan.FromMinutes(10));
    
    return Results.Ok(users);
});

app.Run();

internal record class User(int Id, string FullName, string Email, string Username);