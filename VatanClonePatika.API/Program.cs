using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Repositories.Extensions;
using Repositories.Identity;
using Scalar.AspNetCore;
using Services;
using Services.Auth;
using Services.Extensions;
using Services.Orders;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services
    .AddIdentity<AppUser, IdentityRole>() // ASP.NET Core uygulamasına Identity sistemini ekler
    .AddEntityFrameworkStores<AppDbContext>()  // database bağlantısını içerir
    .AddDefaultTokenProviders();               // token üretmek için eklenir

builder.Services.Configure<IdentityOptions>(options =>
{
    // Şifre gereksinimleri
    options.Password.RequireDigit = true;           // En az bir rakam içermeli
    options.Password.RequireLowercase = true;       // En az bir küçük harf içermeli
    options.Password.RequireUppercase = true;       // En az bir büyük harf içermeli
    options.Password.RequireNonAlphanumeric = true; // En az bir özel karakter içermeli
    options.Password.RequiredLength = 6;            // Minimum şifre uzunluğu

    // Hesap kilitleme ayarları
    options.Lockout.MaxFailedAccessAttempts = 5;    // Maksimum başarısız giriş denemesi
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Kilit süresi
    options.Lockout.AllowedForNewUsers = true;      // Yeni kullanıcılar için de kilit aktif
});

// JWT kismi
builder.Services.AddAuthentication(options =>
{
    // burayi neden ekledigimizi tam anlamadim
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}
    )
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });




// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddRepositories(builder.Configuration).AddServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
