param ($configuration)

& dotnet restore $PSScriptRoot\..\Microsoft.Maui-net6.sln -c:$configuration
& dotnet pack $PSScriptRoot\..\Microsoft.Maui-net6.sln -c:$configuration -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg -bl:$PSScriptRoot\..\artifacts\maui.binlog