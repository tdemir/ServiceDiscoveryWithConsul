using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;


IWebHostBuilder builder = new WebHostBuilder();
builder = builder.UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory());
builder = builder.ConfigureAppConfiguration((_context, _config) =>
        {
            _config.SetBasePath(_context.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{_context.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddJsonFile("ocelot.json")
                .AddEnvironmentVariables();
        });
builder = builder.ConfigureServices(_service =>
        {
            _service.AddOcelot().AddConsul();
        });
builder = builder.ConfigureLogging((_hostingContext, _logging) =>
{
    _logging.AddConsole();
});
//builder = builder.UseIISIntegration();
builder = builder.Configure(async _app =>
{
    _app = await _app.UseOcelot();
});
var webHost = builder.Build();
webHost.Run();

