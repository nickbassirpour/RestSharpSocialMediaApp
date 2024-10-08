using DotNetEnv;
using RestSharpSocialMediaPosts.Reddit.Services;
using RestSharpSocialMediaPosts.Reddit.Services.Interfaces;
using RestSharpSocialMediaPosts.Token;
using RestSharpSocialMediaPosts.Tumblr.Services;
using RestSharpSocialMediaPosts.Tumblr.Services.Interfaces;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Add Services
builder.Services.AddSingleton<IRedditService, RedditService>();
builder.Services.AddSingleton<ITumblrService, TumblrService>();
builder.Services.AddHostedService<TokenRefreshService>();
builder.Services.AddSingleton<ITokenRefreshService, TokenRefreshService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
