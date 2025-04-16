using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["GlobalConfiguration:SecretKey"]))
        };
    });

builder.Services.AddOcelot();

builder.Services.AddCors(options =>
{
    options.AddPolicy("OcelotPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMetricServer();
app.UseHttpMetrics(options =>
{
    options.AddCustomLabel("http_route", context => context.Request.Path.Value ?? "unknown");
    options.AddCustomLabel("http_method", context => context.Request.Method);
    options.AddCustomLabel("http_status_code", context => context.Response.StatusCode.ToString());
});

app.UseCors("OcelotPolicy");

await app.UseOcelot();

app.Run();
