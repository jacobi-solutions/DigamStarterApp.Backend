using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using DigamStarterApp.Backend.API.Auth;
using DigamStarterApp.Backend.API.Services;
using DigamStarterApp.Backend.API.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// If we are in development, force Kestrel to use only HTTP on port 5165
if (builder.Environment.IsDevelopment())
{
    // This overrides launchSettings.json for local dev
    builder.WebHost.UseUrls("http://localhost:5165");
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB settings
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddSingleton(sp =>
{
    var mongoSettings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return mongoSettings;
});

builder.Services.AddSingleton<AccountsService>();
builder.Services.AddSingleton<AccountsRepo>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        // For dev: allow any origin, or just "http://localhost:8100"
        // If you only want Ionic, do:
        // policy.WithOrigins("http://localhost:8100")
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Initialize Firebase Admin SDK (Google Application Default Credentials)
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.GetApplicationDefault()
});

// Build the app
var app = builder.Build();



// Use Swagger in dev environment
//if (app.Environment.IsDevelopment())
if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Note: In dev, we are NOT using HTTPS redirection
    // so we comment out or omit app.UseHttpsRedirection();
}
else
{
    // In production or staging, we DO force HTTPS
    app.UseHttpsRedirection();
}

app.UseCors("AllowLocalhost");

app.MapControllers();

app.Run();
