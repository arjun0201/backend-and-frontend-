using Microsoft.OpenApi.Models;
using ShopAdminTool.Api.Middleware;
using ShopAdminTool.Api.Resources;
using ShopAdminTool.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddLog4Net("log4net.config");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = GenericMessages.SwaggerTitle, Version = "v1.0.0" });

                    options.AddSecurityDefinition(GenericMessages.ApiKey, new OpenApiSecurityScheme
                    {
                        Description = GenericMessages.ApiKeyHeaderMessage,
                        Type = SecuritySchemeType.ApiKey,
                        Name = GenericMessages.ApiKey,
                        In = ParameterLocation.Header,
                        Scheme = "ApiKeyScheme"
                    });
                    var key = new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = GenericMessages.ApiKey
                        },
                        In = ParameterLocation.Header
                    };
                    var requirement = new OpenApiSecurityRequirement
                    {
                        { key, new List<string>() }
                    };
                    options.AddSecurityRequirement(requirement);
                });

builder.Services.ConfigureApplication();
builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceConfiguration)));//cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", GenericMessages.SwaggerTitle);
        c.RoutePrefix = string.Empty;
    });

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    
app.UseMiddleware<ApiKeyMiddleware>();
app.UseCustomExceptionHandler();

app.ConfigureApplication(builder.Environment.IsDevelopment());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
