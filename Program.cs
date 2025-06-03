using ToDoListApi.Models;
using ToDoListApi.Services;
using ToDoListApi.Mappers;
using ToDoListApi.Settings;
using ToDoListApi.Validators;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ToDoListDatabaseSettings>(
    builder.Configuration.GetSection("ToDoListDatabase"));

    builder.Services.AddSingleton<ItemsService>();
    builder.Services.AddSingleton<CategoriesService>();
    builder.Services.AddSingleton<UsersService>();

    builder.Services.AddSingleton<ItemMapper>();
    builder.Services.AddSingleton<CategoryMapper>();
    builder.Services.AddSingleton<UserMapper>();

    builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

    builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey!)),
            ValidateIssuerSigningKey = true
        };
    });

    builder.Services.AddAuthorization();

    builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
