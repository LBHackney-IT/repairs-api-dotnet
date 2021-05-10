#!/bin/bash
failed=0
dotnet test /p:CollectCoverage=true /p:Threshold=90 /p:ThresholdType=line /p:ExcludeByFile="**/Migrations/**/*.*%2c**/Generated/**/*.cs" --no-build --filter FullyQualifiedName~V2 || failed=1
dotnet test --filter FullyQualifiedName~E2ETests || failed=1

echo $failed
exit $failed