FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./RepairsApi.sln ./
COPY ./RepairsApi/RepairsApi.csproj ./RepairsApi/
COPY ./RepairsApi.Tests/RepairsApi.Tests.csproj ./RepairsApi.Tests/

RUN dotnet restore ./RepairsApi/RepairsApi.csproj
RUN dotnet restore ./RepairsApi.Tests/RepairsApi.Tests.csproj
RUN dotnet tool install --global coverlet.console

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out RepairsApi.Tests/RepairsApi.Tests.csproj

CMD coverlet .\bin\Debug\netcoreapp3.1\RepairsApi.Tests.dll --target "dotnet" --targetargs "test --no-build" --exclude-by-file "**/Migrations/*.*" --threshold 80
