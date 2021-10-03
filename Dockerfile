# Stage 1
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /build
COPY . .
#RUN dotnet dev-certs https -ep /app/certs.pfx -p password
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Stage 2
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final
WORKDIR /app
EXPOSE 5000
#EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
ENV ASPNETCORE_ENIRONMENT=Development
#ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"
#ENV ASPNETCORE_Kestrel__Certificates__Default__Path=certs.pfx
COPY --from=build /app .
ENTRYPOINT ["dotnet", "sommersoftware.dk.dll"]
