//configuracion para utilizar Json Web Token
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//habillitar cors para consumir API
//configuracion de politica de CORS
var misReglasCors = "ReglasCors";

builder.Services.AddCors(option => option.AddPolicy(name: misReglasCors,
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    )
);
// Add services to the container.

//Autenticación con Json Web token

//obtener y convertir la secret key en bytes
builder.Configuration.AddJsonFile("appsettings.json"); //obtener el archivo appsettings
var secretKey = builder.Configuration.GetSection("Settings").GetSection("SecretKey").ToString(); //obtener la llave secreta
var keyBytes = Encoding.UTF8.GetBytes(secretKey); // conversion a bytes de la secret key


//Implementar json web token
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config => 
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//llamado a la politica de cors creada arribo
app.UseCors(misReglasCors);

//llamado a autentication para json web token
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapControllers();

app.Run();
