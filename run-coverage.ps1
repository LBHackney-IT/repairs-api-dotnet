dotnet test /p:CollectCoverage=true /p:Threshold=75 /p:ThresholdType=line /p:ExcludeByFile="**/Migrations/**/*.*%%2C**/Generated/**/*.cs" /p:CoverletOutputFormat="cobertura" --filter FullyQualifiedName~V2
reportgenerator "-reports:RepairsApi.Tests/coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html
