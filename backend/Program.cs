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

// 配置 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3001", policy =>
    {
        policy.WithOrigins("http://localhost:3001", "https://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 配置資料庫連接
builder.Services.AddDatabaseContext(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// 啟用 CORS
app.UseCors("AllowLocalhost3001");

// Cloud Run 使用 HTTPS，不需要在應用層重定向
// app.UseHttpsRedirection();

// 使用 Controllers
app.MapControllers();

app.Run();
