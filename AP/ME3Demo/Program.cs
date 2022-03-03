using Dtmcli;
using ME3AppService;
using WebApiClientCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDtmcli(x =>
{
    x.DtmUrl = "http://localhost:36789";
    // request timeout for DTM server, unit is milliseconds
    x.DtmTimeout = 10000;

    // request timeout for trans branch, unit is milliseconds
    x.BranchTimeout = 10000;
    x.DBType = "sqlserver";
    x.BarrierTableName = "[dbo].[barrier]";
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpApi<IOrderApi>(o => o.HttpHost = new Uri("http://localhost:32004"));
builder.Services.AddHttpApi<IStockApi>(o => o.HttpHost = new Uri("http://localhost:32007"));

builder.Services.AddTransient<ISaleOrderAppService, SaleOrderAppService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
