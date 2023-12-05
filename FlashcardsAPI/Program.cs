using Castle.DynamicProxy;
using FlashcardsAPI.Services;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

// For Entity Framework
var connectionString = builder.Configuration.GetConnectionString("FlashcardsAppContextConnection") ?? throw new InvalidOperationException("Connection string 'FlashcardsAppContextConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// For Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the LoggingInterceptor
builder.Services.AddTransient<LoggingInterceptor>();

// Register your services with interceptors
InterceptWith<LoggingInterceptor, IFlashcardCollectionService, FlashcardCollectionService>(builder.Services);
InterceptWith<LoggingInterceptor, IFlashcardService, FlashcardService>(builder.Services);
InterceptWith<LoggingInterceptor, ICommentService, CommentService>(builder.Services);
InterceptWith<LoggingInterceptor, IReactionService, ReactionService>(builder.Services);
InterceptWith<LoggingInterceptor, IProfileService, ProfileService>(builder.Services);

builder.Services.AddSingleton<IFlashcardsStorageService, FlashcardsStorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Extension method for service interception
static void InterceptWith<TInterceptor, TInterface, TImplementation>(IServiceCollection services)
    where TInterceptor : class, IInterceptor
    where TInterface : class
    where TImplementation : class, TInterface
{
    services.AddTransient<TInterface>(serviceProvider =>
    {
        var proxyGenerator = new ProxyGenerator();
        var actualService = serviceProvider.GetRequiredService<TImplementation>();
        var interceptor = serviceProvider.GetRequiredService<TInterceptor>();
        return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(actualService, interceptor);
    });

    services.AddTransient<TImplementation>();
}
