using Microsoft.AspNetCore.ResponseCompression;
using My_Ai.Components;
using My_Ai.Services;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Performance optimizations
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.SmallestSize);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.SmallestSize);

builder.Services.AddResponseCaching();

// minimal DI
builder.Services.AddScoped<IClient, ChatGPTClient>();
builder.Services.AddScoped<IImageClient, OpenAIImageGenerationClient>();    
builder.Services.AddScoped<IProcessRequest, ProcessRequest>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Apply compression early so it covers static files too
app.UseResponseCompression();

// Enable response caching
app.UseResponseCaching();

app.UseHttpsRedirection();

// Add static file caching headers
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 365; // 1 year
        ctx.Context.Response.Headers.CacheControl = $"public,max-age={durationInSeconds}";
    }
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add CSP headers for security
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' https://pagead2.googlesyndication.com; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self';");

    await next();
});

app.Run();
