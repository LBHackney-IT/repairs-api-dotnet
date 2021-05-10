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

# Copy everything else and build
COPY . .

RUN dotnet build RepairsApi.Tests/RepairsApi.Tests.csproj

CMD dotnet test /p:CollectCoverage=true /p:Threshold=70 /p:ThresholdType=line /p:ExcludeByFile="**/Migrations/**/*.*%2c**/Generated/**/*.cs" --no-build --filter FullyQualifiedName~V2
