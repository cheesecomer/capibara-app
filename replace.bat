@echo off

set outpath=%1
set inpath=%outpath%.work
set before=%2
set after=%3

copy /Y %outpath% %inpath%
type nul > %outpath%
setlocal enabledelayedexpansion
for /f "tokens=1* delims=: eol=" %%a in ('findstr /n "^" %inpath%') do (
  set line=%%b
  if not "!line!" == "" (
    set line=!line:%before%=%after%!
  )
  echo.!line!>> %outpath%
)
endlocal

del /Y %inpath%
exit /B 0
