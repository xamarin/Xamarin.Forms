@ECHO OFF
SETLOCAL EnableDelayedExpansion
ECHO adding changes to index...
git add .
ECHO committing...
SET commandLineArgs1=%1
SET commandLineArgs1=!commandLineArgs1:~1,-1!
SET normalComment="updated @ %date%_%time%"
SET normalComment=!normalComment:~1,-1!
IF [%1]==[] (
    GOTO :CommitWithoutComment
) ELSE ( 
    GOTO :CommitWithComment
)

:CommitWithoutComment
    git commit -m "%normalComment%"
    GOTO :PushCode

:CommitWithComment
    SET comment="%commandLineArgs1%, %normalComment%"
    git commit -m %comment%
    GOTO :PushCode

:PushCode
    ECHO pushing...
    git push 
    GOTO :end1

:end1