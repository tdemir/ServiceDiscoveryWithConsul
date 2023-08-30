using ProductApi.BackgrounServices;
using ProductApi.Extensions;



var builder = WebApplication.CreateBuilder(args);
if (args != null && args.Length > 0)
{
    var apiNoData = args.FirstOrDefault(x => x != null && x.Length > ProductApi.Constants.API_NUMBER_PREFIX.Length && x.StartsWith(ProductApi.Constants.API_NUMBER_PREFIX));
    if (!string.IsNullOrEmpty(apiNoData))
    {
        ProductApi.Constants.API_NUMBER = apiNoData.Substring(ProductApi.Constants.API_NUMBER_PREFIX.Length);
    }    
}
Console.WriteLine("ApiNumber => " + ProductApi.Constants.API_NUMBER);

// Add services to the container.
if (ProductApi.Constants.IS_CONSUL_ENABLED)
{
    builder.Services.AddConsulConfig(builder.Configuration);
    builder.Services.AddHostedService<ServiceDiscoveryHostedService>();
}


builder.Services
    .AddHealthChecks();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");


//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
