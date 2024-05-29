using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json");



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOcelot();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseOcelot().Wait();

app.UseAuthorization();

app.MapControllers();

app.Run();

//using Ocelot.DependencyInjection;
//using Ocelot.Middleware;

//var builder = WebApplication.CreateBuilder(args);
//builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
//    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
//    .AddEnvironmentVariables();
//builder.Services.AddOcelot(builder.Configuration);

//var app = builder.Build();
//app.UseOcelot().Wait();
//app.Run();

