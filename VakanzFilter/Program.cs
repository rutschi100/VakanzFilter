using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using VakanzFilter.Data;
using VakanzFilter.Services;
using VakanzFilter.ViewModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddTransient<IndexViewModel>();
builder.Services.AddTransient<IDataService, DataService>();

ConfigureServices(builder.Services);


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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


void ConfigureServices(IServiceCollection services)
{
    //services.AddTransient<IDbTemplateService, DbTemplateService>();
    //...
    RegisterDbContext(services);
}


void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
{
    _serviceStartup.ConfigureApplication(app, serviceProvider);

    ConfigureDbContext(serviceProvider);

    // Register all service specific things to do on app builder after chassis configuration.
    // ...
}

void RegisterDbContext(IServiceCollection services)
{
    services.AddTransient<ILaboratoryContextFactory, LaboratoryContextFactory>();
    //Context should be Transient, because we use a self written Factory and use a Context as
    //a minimal lifetime by the Methods and not overall in a Class Injection.
    services.AddDbContext<LaboratoryWidgetContext>(
        builder => builder.UseNpgsql(Configuration.GetConnectionString("EfCoreSettings")),
        ServiceLifetime.Transient
    );
}

static void ConfigureDbContext(IServiceProvider serviceProvider)
{
    using var context = serviceProvider.GetRequiredService<LaboratoryWidgetContext>();
    if (context.Database.IsInMemory()) return;

    if (Environment.GetEnvironmentVariable(EnvironmentVariables.Env_VarName)
     != EnvironmentVariables.Env_Production)
    {
        context.Database.Migrate();
    }
    else
    {
        context.Database.EnsureCreated();
    }
}