using FamilyHub.IdentityServerHost.Extensions;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.AzureAppServices;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(logging => logging.AddAzureWebAppDiagnostics())
.ConfigureServices(serviceCollection => serviceCollection
    .Configure<AzureFileLoggerOptions>(options =>
    {
        options.FileName = "azure-diagnostics-";
        options.FileSizeLimit = 50 * 1024;
        options.RetainedFileCountLimit = 5;
    }).Configure<AzureBlobLoggerOptions>(options =>
    {
        options.BlobName = "log.txt";
    })
);

builder.Services.Configure<GovNotifySetting>(builder.Configuration.GetSection("GovNotifySetting"));

var connectionString = builder.Configuration.GetConnectionString("UserServiceConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

string useDbType = builder.Configuration.GetValue<string>("UseDbType");

switch (useDbType)
{
    default:
    case "UseInMemoryDatabase":
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("FH-IdentityDb"));
        break;

    case "UseSqlLite":
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("UserServiceConnection"),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        break;
    case "UseSqlServerDatabase":
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("UserServiceConnection"),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        break;
    case "UsePostgresDatabase":
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("UserServiceConnection"),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        break;

}


builder.Services.AddDefaultIdentity<ApplicationIdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient<IApplicationDbContext,ApplicationDbContext>();
builder.Services.AddTransient<ApplicationDbContextInitialiser>();
builder.Services.AddTransient<IOrganisationRepository, OrganisationRepository>();
builder.Services.AddTransient<AuthenticationDelegatingHandler>();
builder.Services.AddTransient<ITokenService, TokenService>();


bool isEmailEnabled = builder.Configuration.GetValue<bool>("EmailSetting:IsEmailEnabled");
if (isEmailEnabled)
{
    builder.Services.AddTransient<IEmailSender, EmailSender>();
}
else
{
    builder.Services.AddTransient<IEmailSender, GovNotifySender>();
}

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ServiceDirectoryUrl"));
}).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    try
    {

        //var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        // Seed Database
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync(builder.Configuration);
        await initialiser.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        if (logger != null)
            logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
}

app.Run();
