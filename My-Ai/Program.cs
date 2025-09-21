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
    app.UseResponseCompression();
}

// Enable response caching
app.UseResponseCaching();

app.UseHttpsRedirection();

// Content Security Policy (before static files/endpoints)
app.Use(async (context, next) =>
{
    // Common directives
    var defaultSrc = "default-src 'self'; ";
    var baseUri = "base-uri 'self'; ";
    var imgSrc = "img-src 'self' data: https:; ";
    var styleSrc = "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; ";
    var fontSrc = "font-src 'self' https://fonts.gstatic.com; ";

    // Allow SODAR from adtrafficquality.google (apex + wildcard)
    var scriptSrc =
        "script-src 'self' 'unsafe-inline' " +
        "https://pagead2.googlesyndication.com " +
        "https://fundingchoicesmessages.google.com " +
        "https://www.google.com " +
        "https://www.gstatic.com " +
        "https://tpc.googlesyndication.com " +
        "https://googleads.g.doubleclick.net " +
        "https://adtrafficQuality.google https://*.adtrafficQuality.google; ";

    var scriptSrcElem =
        "script-src-elem 'self' 'unsafe-inline' " +
        "https://pagead2.googlesyndication.com " +
        "https://fundingchoicesmessages.google.com " +
        "https://www.google.com " +
        "https://www.gstatic.com " +
        "https://tpc.googlesyndication.com " +
        "https://googleads.g.doubleclick.net " +
        "https://adtrafficQuality.google https://*.adtrafficQuality.google; ";

    var frameSrc =
        "frame-src 'self' " +
        "https://googleads.g.doubleclick.net " +
        "https://tpc.googlesyndication.com " +
        "https://pagead2.googlesyndication.com " +
        "https://fundingchoicesmessages.google.com " +
        "https://www.google.com; ";

    // Connect sources differ for Dev vs Prod to support WebSockets
    var connectSrcCommon =
        " 'self' " +
        "https://pagead2.googlesyndication.com " +
        "https://fundingchoicesmessages.google.com " +
        "https://www.google.com " +
        "https://googleads.g.doubleclick.net " +
        "https://tpc.googlesyndication.com " +
        "https://adtrafficQuality.google https://*.adtrafficQuality.google";

    string connectSrc = app.Environment.IsDevelopment()
        ? $"connect-src{connectSrcCommon} ws: wss: wss://localhost:*; "
        : $"connect-src{connectSrcCommon} wss:; ";

    context.Response.Headers["Content-Security-Policy"] =
        defaultSrc + baseUri + imgSrc + styleSrc + fontSrc + connectSrc + scriptSrc + scriptSrcElem + frameSrc;

    await next();
});

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

app.Run();
