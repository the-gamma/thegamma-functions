@echo off
.paket\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)
if not exist paket.lock (
  .paket\paket.exe install
) else (
  .paket\paket.exe restore
)
if errorlevel 1 (
  exit /b %errorlevel%
)
if not defined SCM_COMMIT_ID (
  packages\FAKE\tools\FAKE.exe build.fsx run
) else (
  packages\FAKE\tools\FAKE.exe build.fsx generate
)
