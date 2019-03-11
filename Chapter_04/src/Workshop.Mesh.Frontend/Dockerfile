FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Workshop.Mesh.Frontend/Workshop.Mesh.Frontend.csproj Workshop.Mesh.Frontend/
RUN dotnet restore Workshop.Mesh.Frontend/Workshop.Mesh.Frontend.csproj
COPY . .
WORKDIR /src/Workshop.Mesh.Frontend
RUN dotnet build Workshop.Mesh.Frontend.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Workshop.Mesh.Frontend.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Workshop.Mesh.Frontend.dll"]