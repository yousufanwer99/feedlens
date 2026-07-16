using Amazon.Runtime;
using Amazon.S3;
using FeedLens.Domain.Interfaces;
using FeedLens.Helpers;
using FeedLens.Repositories.Context;
using FeedLens.Repositories.Repositories;
using FeedLens.Services.Interfaces;
using FeedLens.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// ── Load secrets from AWS Parameter Store ───────────────
var bootstrapAccessKey = builder.Configuration["AWS:AccessKey"];
var bootstrapSecretKey = builder.Configuration["AWS:SecretKey"];
var awsRegion = builder.Configuration["AWS:Region"] ?? "us-east-1";

var secrets = await AwsParameterStoreHelper.LoadParametersAsync(
    bootstrapAccessKey!,
    bootstrapSecretKey!,
    awsRegion,
    new List<string>
    {
        "/feedlens/JWT_SECRET",
        "/feedlens/DB_CONNECTION",
        "/feedlens/AWS_ACCESS_KEY",
        "/feedlens/AWS_SECRET_KEY"
    }
);


// ── Inject secrets into configuration ───────────────────
var secretsDict = new Dictionary<string, string?>
{
    ["Jwt:Secret"] = secrets["/feedlens/JWT_SECRET"],
    ["ConnectionStrings:DefaultConnection"] = secrets["/feedlens/DB_CONNECTION"],
    ["AWS:AccessKey"] = secrets["/feedlens/AWS_ACCESS_KEY"],
    ["AWS:SecretKey"] = secrets["/feedlens/AWS_SECRET_KEY"]
};
builder.Configuration.AddInMemoryCollection(secretsDict);


var jwtSecret = secrets["/feedlens/JWT_SECRET"];
var dbConnection = secrets["/feedlens/DB_CONNECTION"];
var awsAccessKey = secrets["/feedlens/AWS_ACCESS_KEY"];
var awsSecretKey = secrets["/feedlens/AWS_SECRET_KEY"];

// ── Database ────────────────────────────────────────────
builder.Services.AddDbContext<FeedLensDbContext>(options =>
    options.UseSqlServer(dbConnection, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    )
);


// ── AWS ─────────────────────────────────────────────────
var awsCredentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
var awsConfig = new AmazonS3Config { RegionEndpoint = Amazon.RegionEndpoint.USEast1 };
builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsCredentials, awsConfig));

// ── JWT Auth ────────────────────────────────────────────
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ── Repositories ────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IWatchHistoryRepository, WatchHistoryRepository>();

// ── Services ────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IWatchHistoryService, WatchHistoryService>();

// ── CORS ────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FeedLensPolicy", policy =>
    {
            policy.WithOrigins
            (
                "http://localhost:4200",
                "https://feedlens-nine.vercel.app",
                "https://feedlens-production.up.railway.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ── Swagger ─────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FeedLens API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Enter: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// ── Middleware pipeline ──────────────────────────────────
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FeedLensPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();