SET FORMS_LOCAL_NUGET=c:\nuget\

IF NOT EXIST docs\Xamarin.Forms.Core.xml (CALL "c:\Program Files (x86)\Mono-3.2.3\bin\mdoc" export-msxdoc -o docs\Xamarin.Forms.Core.xml docs\Xamarin.Forms.Core)
IF NOT EXIST docs\Xamarin.Forms.Xaml.xml (CALL "c:\Program Files (x86)\Mono-3.2.3\bin\mdoc" export-msxdoc -o docs\Xamarin.Forms.Xaml.xml docs\Xamarin.Forms.Xaml)
IF NOT EXIST docs\Xamarin.Forms.Maps.xml (CALL "c:\Program Files (x86)\Mono-3.2.3\bin\mdoc" export-msxdoc -o docs\Xamarin.Forms.Maps.xml docs\Xamarin.Forms.Maps)

SETLOCAL
    SET VERSION_BAT=%FORMS_LOCAL_NUGET%version.bat
    ECHO VERSION_BAT=%VERSION_BAT%

    IF EXIST %VERSION_BAT% (CALL %VERSION_BAT%) ELSE (SET FORMS_LOCAL_VERSION=1000)
    ECHO Forms Local Version = %FORMS_LOCAL_VERSION%

    nuget pack .nuspec\Xamarin.Forms.nuspec -outputdirectory %FORMS_LOCAL_NUGET% -version 1.2.3.%FORMS_LOCAL_VERSION% -properties configuration=debug -properties -Platform=AnyCPU -properties idappend=.Master
    nuget pack .nuspec\Xamarin.Forms.Maps.nuspec -outputdirectory %FORMS_LOCAL_NUGET% -version 1.2.3.%FORMS_LOCAL_VERSION% -properties configuration=debug -properties -Platform=AnyCPU -properties idappend=.Master

    SET /A FORMS_LOCAL_VERSION=%FORMS_LOCAL_VERSION% + 1
    ECHO SET FORMS_LOCAL_VERSION=%FORMS_LOCAL_VERSION% > %VERSION_BAT%
ENDLOCAL

nuget update %1