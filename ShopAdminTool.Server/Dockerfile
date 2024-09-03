FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
EXPOSE 5248 
EXPOSE 7103

COPY *.sln .
COPY . .
RUN dotnet restore

# testing
FROM build AS testing
WORKDIR /src
RUN dotnet build
WORKDIR /src
RUN dotnet test

# publish
FROM testing AS publish
WORKDIR /src/ShopAdminTool.Api
RUN dotnet publish -c Release -o /src/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=publish /src/publish .


ENTRYPOINT ["dotnet", "ShopAdminTool.Api.dll"]