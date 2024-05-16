using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using TAWDotNetCore.RestApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
    {
        DataSource = ".",
        InitialCatalog = "TAWDotNetCore",
        UserID = "sa",
        Password = "sa@1234",
        TrustServerCertificate = true
    };

builder.Services.AddDbContext<AppDbContext>(opt =>
{

    opt.UseSqlServer(sqlConnectionStringBuilder.ConnectionString);
}, ServiceLifetime.Transient, ServiceLifetime.Transient);

builder.Services.AddScoped<IDbConnection>(n => new SqlConnection(sqlConnectionStringBuilder.ConnectionString));
builder.Services.AddScoped<SqlConnection>(n => new SqlConnection(sqlConnectionStringBuilder.ConnectionString));

var app = builder.Build();

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
