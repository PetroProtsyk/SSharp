dotnet build SSharp.Net -c=Release /p:ForNuget=true
dotnet pack SSharp.Net -c=Release /p:ForNuget=true --no-build -o=./

powershell "Get-FileHash .\SSharp.Net.4.1.1.nupkg  -Algorithm SHA512 | Format-List"