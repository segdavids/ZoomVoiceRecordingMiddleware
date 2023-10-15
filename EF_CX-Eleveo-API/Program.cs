using Eleveo_EFCX_Connector_API.Contracts;
using Eleveo_EFCX_Connector_API.Data;
using Eleveo_EFCX_Connector_API.Repository;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);
string? cstring = Eleveo_EFCX_Connector_API.Data.Helper.GetConnectionString();
string hosturl = Eleveo_EFCX_Connector_API.Data.Helper.gethost();
if (string.IsNullOrWhiteSpace(cstring))
{
    Console.WriteLine($"Database settings not correctly set. Please setup DB in the configuration file and try again...");
    return;
}

builder.Services.AddDbContext<EleveoEFCXDBContext>(options =>
options.UseSqlServer(cstring)
);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options=>
{
    options.AddPolicy("AllowAll", b=>b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});
builder.Services.AddScoped<IEleveoConnector, EleveoConnector>();
builder.Services.AddScoped<IHelper, Eleveo_EFCX_Connector_API.Repository.Helper>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




//app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
