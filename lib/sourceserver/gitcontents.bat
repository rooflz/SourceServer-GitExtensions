@echo off

if ()==(%1) exit /b
if ()==(%2) exit /b
if ()==(%3) exit /b
if ()==(%4) exit /b

setlocal
set GIT_NODE_URL=%1
set GIT_WORKING_DIR=%2
set GIT_OBJ_ID=%3
set GIT_CONTENTS_OUTPUT=%4

echo *Init*
if exist %GIT_WORKING_DIR% goto update

echo *Clone*
md %GIT_WORKING_DIR%
git --git-dir=%GIT_WORKING_DIR% init
git --git-dir=%GIT_WORKING_DIR% fetch "%GIT_NODE_URL%
goto show

:update
echo *Update*
for /f "usebackq delims=" %%i in (`git --no-pager --git-dir=%%GIT_WORKING_DIR%% show %%GIT_OBJ_ID%%`) do ( goto show )
git --git-dir=%GIT_WORKING_DIR% fetch "%GIT_NODE_URL%

:show
echo *Show*
if not exist %~dp4 mkdir %~dp4
git --no-pager --git-dir=%GIT_WORKING_DIR% show %GIT_OBJ_ID% > %GIT_CONTENTS_OUTPUT%

:cleanup
endlocal