using ProductApi.Extensions;

const bool IS_CONSUL_ENABLED = true;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if (IS_CONSUL_ENABLED)
{
    builder.Services.AddConsulConfig(builder.Configuration);
}


builder.Services
    .AddHealthChecks();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (IS_CONSUL_ENABLED)
{
    app.UseConsul();
}

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
