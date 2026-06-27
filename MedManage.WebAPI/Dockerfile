FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json ./
COPY MedManage.WebAPI/MedManage.WebAPI.csproj MedManage.WebAPI/
COPY MedManage.Core/MedManage.Domain/MedManage.Domain.csproj MedManage.Core/MedManage.Domain/
COPY MedManage.Core/MedManage.Application/MedManage.Application.csproj MedManage.Core/MedManage.Application/
COPY MedManage.Persistence/MedManage.Persistence.csproj MedManage.Persistence/
COPY MedManage.Indentity/MedManage.Indentity.csproj MedManage.Indentity/
RUN dotnet restore MedManage.WebAPI/MedManage.WebAPI.csproj

COPY . .
RUN dotnet publish MedManage.WebAPI/MedManage.WebAPI.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
EXPOSE 8080
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MedManage.WebAPI.dll"]
