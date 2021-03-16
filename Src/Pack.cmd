dotnet build SSharp.Net -c=Release
dotnet pack SSharp.Net -c=Release --no-build -o=./

powershell "Get-FileHash .\SSharp.Net.4.1.1.nupkg  -Algorithm SHA512 | Format-List"