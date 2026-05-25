FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY M-TAU.Domain/M-TAU.Domain.csproj M-TAU.Domain/
COPY M-TAU.Application/M-TAU.Application.csproj M-TAU.Application/
COPY M-TAU.Client/M-TAU.Client.csproj M-TAU.Client/
RUN dotnet restore M-TAU.Client/M-TAU.Client.csproj

COPY M-TAU.Domain/ M-TAU.Domain/
COPY M-TAU.Application/ M-TAU.Application/
COPY M-TAU.Client/ M-TAU.Client/
RUN dotnet publish M-TAU.Client/M-TAU.Client.csproj -c Release -o /app

FROM nginx:alpine
COPY --from=build /app/wwwroot /usr/share/nginx/html
COPY M-TAU.Client/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
