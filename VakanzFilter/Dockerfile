FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["VakanzFilter/VakanzFilter.csproj", "VakanzFilter/"]
RUN dotnet restore "VakanzFilter/VakanzFilter.csproj"
COPY . .
WORKDIR "/src/VakanzFilter"
RUN dotnet build "VakanzFilter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VakanzFilter.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VakanzFilter.dll"]
