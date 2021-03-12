dotnet build SSharp.Net -c=Release /p:Platform="Any CPU" /p:TargetFramework="netcoreapp3.1" --no-incremental
dotnet build SSharp.Net -c=Release_NET4 /p:Platform="Any CPU" /p:TargetFramework="net48" --no-incremental
dotnet pack SSharp.Net -c=Release_NET4 --no-build -o=./

powershell "Get-FileHash .\SSharp.Net.4.0.1.nupkg  -Algorithm SHA512 | Format-List"