using GongCanApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 支援 Cloud Run 的 PORT 環境變數
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置資料庫連接
builder.Services.AddDatabaseContext(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Cloud Run 使用 HTTPS，不需要在應用層重定向
// app.UseHttpsRedirection();

// 使用 Controllers
app.MapControllers();

app.Run();
