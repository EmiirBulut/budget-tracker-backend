using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using BudgetTracker.Application;
using BudgetTracker.Infrastructure;
using BudgetTracker.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Infrastructure services — DbContext, repositories, TokenService, JwtSettings
// ---------------------------------------------------------------------------
builder.Services.AddInfrastructureServices(builder.Configuration);

// ---------------------------------------------------------------------------
// Application services — MediatR, FluentValidation, pipeline behaviors
// ---------------------------------------------------------------------------
builder.Services.AddApplicationServices();

// ---------------------------------------------------------------------------
// Authentication — JWT Bearer
// ---------------------------------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero // access tokens expire exactly at ExpiresAt
        };
    });

builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------
// CORS — allow Vite dev server and production frontend
// ---------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:5173"];
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------------------------------------------------------------------------
// API layer
// ---------------------------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddOpenApi();

// ---------------------------------------------------------------------------
// HTTP pipeline
// ---------------------------------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Budget Tracker API";
        options.Theme = ScalarTheme.Purple;
    });
}

app.UseCors("FrontendPolicy");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
