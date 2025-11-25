using apiPrisma.Context;
using apiPrisma.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("CadenaOracle"))
    );
builder.Services.AddDbContext<AppDbContextSeg>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("CadenaOraSeg"))
    );

builder.Services.AddScoped<UsuariosService>();
builder.Services.AddScoped<ParametrosService>();
builder.Services.AddScoped <FirmaPrismaService>();

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = builder.Configuration["MiAplicacion:SecretKey"];

    var key = Encoding.UTF8.GetBytes(secretKey);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["MiAplicacion:Issuer"],
        ValidAudience = builder.Configuration["MiAplicacion:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


var app = builder.Build();

app.UseCors(options =>
options.WithOrigins(
    "http://localhost:4200",
    "https://localhost:4200",
    "https://preprod-marndigital-fe.marn.gob.gt",
    "http://preprod-marndigital-fe.marn.gob.gt",
    "https://saga.marn.gob.gt",
    "https://marndigital.marn.gob.gt",
    "https://marndigital.marn.gob.gt/participacion-ciudadana",
    "https://bkmarndigital.marn.gob.gt",
    "http://127.0.0.1:5500",
    "http://172.16.27.30",
    "https://172.16.27.30",
    "http://104.26.12.170",
    "https://104.26.12.170",
    "http://45.191.247.74",
    "https://45.191.247.74",
    "http://45.191.247.67",
    "https://45.191.247.67",
    "http://104.26.13.170",
    "https://104.26.13.170",
    "http://172.67.71.155",
    "https://172.67.71.155",
    "http://104.26.12.170",
    "https://104.26.12.170")
.AllowAnyMethod()
.AllowAnyHeader()
.AllowCredentials()
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
