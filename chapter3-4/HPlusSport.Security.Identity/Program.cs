using HPlusSport.Security.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/*
builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
});
*/

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(o =>
{
    o.Events = new CookieAuthenticationEvents()
    {
        OnRedirectToLogin = (ctx) =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
            {
                ctx.Response.StatusCode = 401;
            }

            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = (ctx) =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
            {
                ctx.Response.StatusCode = 403;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    await next.Invoke();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapSwagger();

app.MapGet("/api/hello", (HttpContext httpContext) =>
{
    return $"Hello {httpContext.User?.Identity?.Name}";
})
    .WithOpenApi()
    .RequireAuthorization();

app.Run();
