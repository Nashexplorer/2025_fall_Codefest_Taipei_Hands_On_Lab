using GongCanApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 在開發環境中啟用 User Secrets（用於存儲敏感資訊如 SMTP 密碼）
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

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
builder.Services.AddHttpClient();
builder.Services.AddScoped<GongCanApi.Services.EmailService>();

// 配置 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3001", policy =>
    {
        policy.WithOrigins("http://localhost:3001", "https://localhost:3001", "http://10.0.2.2:3001", "https://10.0.2.2:3001"
            , "https://2025-fall-codefest-taipei-hands-on-lab-w8n6-f2sxa98nn.vercel.app", "http://172.31.128.1:3001")
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
