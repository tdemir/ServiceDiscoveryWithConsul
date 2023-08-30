using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;


new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureAppConfiguration((_context, _config) =>
        {
            Console.WriteLine("ContentRootPath: " + _context.HostingEnvironment.ContentRootPath);
            _config.SetBasePath(_context.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{_context.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddJsonFile("ocelot.json")
                .AddEnvironmentVariables();
        })
        .ConfigureServices(_service =>
        {
            _service.AddOcelot().AddConsul();
        })
        .ConfigureLogging((_hostingContext, _logging) =>
        {
            _logging.AddConsole();
        })
        //.UseIISIntegration()
        .Configure(async _app =>
        {
            _app = await _app.UseOcelot();
        })
        .Build()
        .Run();

