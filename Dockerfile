FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /source

COPY *.sln .
COPY CtServer/*.csproj ./CtServer/
COPY CtServer.Data/*.csproj ./CtServer.Data/
RUN dotnet restore

COPY . .
RUN dotnet publish --no-restore -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /source/out .
ENTRYPOINT ["dotnet", "CtServer.dll"]
