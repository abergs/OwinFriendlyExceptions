:: Script by Anders, 2017-01-22
:: nuget push .\OwinFriendlyExceptions.1.1.0.0.nupkg -Source https://www.nuget.org
:: run nuget setApiKey KEY to set your nuget apikey first
@echo off
setlocal
:PROMPT
echo .
echo .
echo .
echo Will run this:
echo nuget push %1 -Source https://www.nuget.org
SET /P AREYOUSURE=Are you sure (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END

nuget push %1 -Source https://www.nuget.org

:END
endlocal