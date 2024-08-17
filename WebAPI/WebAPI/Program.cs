using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//mysql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))));

var mongoDBSettings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddDbContext<RestaurantReservationDbContext>(options =>
options.UseMongoDB(mongoDBSettings.ConnectionString ?? "", mongoDBSettings.DatabaseName ?? ""));


builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
  var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  dbContext.Database.Migrate();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
