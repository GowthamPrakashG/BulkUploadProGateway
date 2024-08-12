using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service;
using ClientSchemaHub.Service.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Register database services
builder.Services.AddScoped<ITimescaleService, TimescaleService>();
builder.Services.AddScoped<IPostgreSQLService, PostgreSQLService>();
builder.Services.AddScoped<IMySQLService, MySQLService>();
builder.Services.AddScoped<IMSSQLService, MSSQLService>();
builder.Services.AddScoped<IGeneralDatabaseService, GeneralDatabaseService>();
builder.Services.AddScoped<IDynamoDbService, DynamoDbService>();
builder.Services.AddScoped<IScyllaService, ScyllaService>();


// Register configuration options
builder.Services.Configure<DBConnectionDTO>(builder.Configuration.GetSection("AWS"));

// Register DynamoDB service

// Configure CORS
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
