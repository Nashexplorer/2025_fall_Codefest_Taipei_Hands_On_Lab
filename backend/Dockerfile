# 使用 .NET 7 SDK 作為建置映像
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# 複製專案檔案並還原套件
COPY ["GongCanApi.csproj", "./"]
RUN dotnet restore "GongCanApi.csproj"

# 複製所有原始碼
COPY . .

# 建置專案
RUN dotnet build "GongCanApi.csproj" -c Release -o /app/build

# 發佈專案
FROM build AS publish
RUN dotnet publish "GongCanApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 使用 .NET 7 Runtime 作為執行映像
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# 複製發佈的檔案
COPY --from=publish /app/publish .

# Cloud Run 會注入 PORT 環境變數
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# 啟動應用程式
ENTRYPOINT ["dotnet", "GongCanApi.dll"]

