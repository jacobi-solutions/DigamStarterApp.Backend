using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using DigamStarterApp.Backend.API.Services;
using DigamStarterApp.Backend.API.Settings;
using Microsoft.Extensions.Options;
using DigamStarterApp.Backend.API.Repos;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

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

// Bind Settings
var mongoDbSettings = builder.Configuration.GetSection(AppConstants.MongoDbSettings).Get<MongoDBSettings>();
builder.Services.AddSingleton(mongoDbSettings);

var firebaseSettings = builder.Configuration.GetSection(AppConstants.FirebaseSettings).Get<FirebaseSettings>();
builder.Services.AddSingleton(firebaseSettings);


// Database
// ✅ Setup MongoDB Conventions
var pack = new ConventionPack
{
    new CamelCaseElementNameConvention(),
    new IgnoreIfNullConvention(true),
    new IgnoreExtraElementsConvention(true)
};
ConventionRegistry.Register("My Custom Conventions", pack, t => true);


// ✅ Register MongoDB Client & Database as Singleton
var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton<IMongoDatabase>(x => mongoClient.GetDatabase(mongoDbSettings.DatabaseName));



builder.Services.AddSingleton<AccountsService>();
builder.Services.AddSingleton<AccountsRepo>();

// ✅ Initialize Firebase Admin SDK (Google Application Default Credentials)
var firebaseProjectId = firebaseSettings.ProjectId;
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.GetApplicationDefault(),
    ProjectId = firebaseSettings.ProjectId
});

builder.Services.AddAuthentication(x =>
{
x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{

    options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
        ValidateAudience = true,
        ValidAudience = firebaseProjectId,
        ValidateLifetime = true
    };
});


// ✅ Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Build the app
var app = builder.Build();

// ✅ Use Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Use Swagger in dev environment
if (true) // Keeping Swagger enabled
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowLocalhost");
app.MapControllers();
app.Run();