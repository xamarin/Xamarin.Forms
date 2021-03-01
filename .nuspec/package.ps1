param ($packageVersion, $configuration)

& dotnet pack $PSScriptRoot\..\src\Core\src\Core-net6.csproj -c:$configuration -p:PackageVersion=$packageVersion
& dotnet pack $PSScriptRoot\..\src\Controls\src\Xaml\Controls.Xaml-net6.csproj -c:$configuration -p:PackageVersion=$packageVersion
& dotnet pack $PSScriptRoot\..\src\Controls\src\Core\Controls.Core-net6.csproj -c:$configuration -p:PackageVersion=$packageVersion
& dotnet pack $PSScriptRoot\..\src\Controls\src\Build.Tasks\Controls.Build.Tasks-net6.csproj -c:$configuration -p:PackageVersion=$packageVersion

& dotnet pack $PSScriptRoot\..\src\SingleProject\Resizetizer\src\Resizetizer.csproj -c:$configuration -p:PackageVersion=$packageVersion

& dotnet pack $PSScriptRoot\..\.nuspec\Microsoft.Maui.Package.csproj -c:$configuration -p:PackageVersion=$packageVersion -bl:$PSScriptRoot\..\artifacts\package.binlog