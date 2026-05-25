FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY M-TAU.Domain/M-TAU.Domain.csproj M-TAU.Domain/
COPY M-TAU.Application/M-TAU.Application.csproj M-TAU.Application/
COPY M-TAU.Infrastructure/M-TAU.Infrastructure.csproj M-TAU.Infrastructure/
COPY M-TAU.API/M-TAU.API.csproj M-TAU.API/
RUN dotnet restore M-TAU.API/M-TAU.API.csproj

COPY M-TAU.Domain/ M-TAU.Domain/
COPY M-TAU.Application/ M-TAU.Application/
COPY M-TAU.Infrastructure/ M-TAU.Infrastructure/
COPY M-TAU.API/ M-TAU.API/

RUN dotnet publish M-TAU.API/M-TAU.API.csproj -c Release -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
RUN mkdir -p /app/wwwroot/uploads
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "M-TAU.API.dll"]
