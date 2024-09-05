using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using CQRS_Microservice.Data;
using AutoMapper;
using CQRS_Microservice.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using CQRS_Microservice;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using CQRS_Microservice.Models;
using CQRS_Microservice.Helper;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

// Configure database context with Postgres
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register MediatR services
builder.Services.AddApplicationMediatR();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Configure authorization policies for ProductController
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PermissionHelper.CanCreateProduct, policy =>
        policy.RequireClaim(PermissionHelper.Permissions, PermissionHelper.CreateProduct));

    options.AddPolicy(PermissionHelper.CanReadProduct, policy =>
        policy.RequireClaim(PermissionHelper.Permissions, PermissionHelper.ReadProduct));

    options.AddPolicy(PermissionHelper.CanUpdateProduct, policy =>
        policy.RequireClaim(PermissionHelper.Permissions, PermissionHelper.UpdateProduct));

    options.AddPolicy(PermissionHelper.CanDeleteProduct, policy =>
        policy.RequireClaim(PermissionHelper.Permissions, PermissionHelper.DeleteProduct));
});

// Register TokenService
builder.Services.AddSingleton<TokenService>();

// Register password hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Register AutoMapper services
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Add controllers
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Ensure authentication is used
app.UseAuthorization();  // Ensure authorization is used
app.MapControllers();

app.Run();
