using HMS.API.Extensions;
using HMS.API.Middleware;
using HMS.Application.Common;
using HMS.Application.Interfaces;
using HMS.Infrastructure.Data.Context;
using HMS.Infrastructure.Data.Seed;
using HMS.Infrastructure.DependencyInjection;
using HMS.Infrastructure.Email;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ---------- Service registration (everything before Build) ----------

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddHmsSwagger();
builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddHmsAuthentication(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHealthChecks();


var app = builder.Build();

// ---------- Middleware pipeline (everything after Build) ----------

app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serves wwwroot/confirm-email.html, the page the verification link opens.
app.UseStaticFiles();

app.UseAuthentication();   // must come before UseAuthorization
app.UseAuthorization();
app.MapHealthChecks("/health");

app.MapControllers();

// ---------- Migrate + seed ----------

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<HmsDbContext>();
    var passwordHasher = services.GetRequiredService<IPasswordHasher>();

    await HmsDbSeeder.SeedAsync(context, passwordHasher);
}

app.Run();
