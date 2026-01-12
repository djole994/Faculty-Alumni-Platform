using Microsoft.EntityFrameworkCore;
using AlumniApi.Data; // ili tvoj namespace
using AlumniApi.Services.Geocoding;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AlumniContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpClient<IGeocodingService, Geocoding>(client =>
{
    client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("AlumniAppPortfolio/1.0 (contact: developer@example.com)");
});
builder.Services.AddEmailing(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
