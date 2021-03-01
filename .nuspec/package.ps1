param ($packageVersion, $configuration)

& dotnet pack $PSScriptRoot\..\src\Core\src\Core-net6.csproj -c:$configuration -p:PackageVersion=$packageVersion -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
& dotnet pack $PSScriptRoot\..\src\Controls\src\Xaml\Controls.Xaml-net6.csproj -c:$configuration -p:PackageVersion=$packageVersion -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
& dotnet pack $PSScriptRoot\..\src\Controls\src\Core\Controls.Core-net6.csproj -c:$configuration -p:PackageVersion=$packageVersion -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

# no symbols for build tasks
& dotnet pack $PSScriptRoot\..\src\Controls\src\Build.Tasks\Controls.Build.Tasks-net6.csproj -c:$configuration -p:PackageVersion=$packageVersion
& dotnet pack $PSScriptRoot\..\src\SingleProject\Resizetizer\src\Resizetizer.csproj -c:$configuration -p:PackageVersion=$packageVersion

# head package has no code to have symbols for
& dotnet pack $PSScriptRoot\..\src\Package\Package-net6.csproj -c:$configuration -p:PackageVersion=$packageVersion