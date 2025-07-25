using Absence.Domain.Context;
using Absence.Domain.Repository;
using Absence.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/* Configuraci�n de servicio */
builder.Services.AddCors(opts => opts.AddPolicy("PolicyCors", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
builder.Services.AddControllers();

/* Configuraci�n de BD */
builder.Services.AddDbContext<AbsenceContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

/* Configuraci�n JWT */
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opts =>
    {
        opts.SaveToken = true;
        opts.RequireHttpsMetadata = false;
        opts.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ValidAudience = "reader",
            ValidIssuer = "issuer",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        };
    });

/* Configuraci�n de UOW */
builder.Services.AddTransient<IAbsenceUnitOfWork, AbsenceUnitOfWork>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Configuraci�n de la App */
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("PolicyCors");
app.UseAuthentication();    

app.MapGet("/", () => "Ok");

app.UseEndpoints(ep =>
{
    ep.MapControllers();
});

await DataSeeder.SeedDefaultAdminAsync(app.Services);

app.Run();
