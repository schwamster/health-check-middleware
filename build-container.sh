#!bin/bash
set -e
dotnet restore
dotnet test test/health-check-middleware.test/health-check-middleware.test.csproj --test-adapter-path:. --logger:xunit
rm -rf $(pwd)/package
dotnet pack src/health-check-middleware/health-check-middleware.csproj -c release -o $(pwd)/package --version-suffix=${BuildNumber}
mkdir $(pwd)/symbols
cp $(pwd)/package/*.symbols.nupkg $(pwd)/symbols
rm $(pwd)/package/*.symbols.nupkg