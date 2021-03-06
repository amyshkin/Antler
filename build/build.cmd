@echo off
set path=%path%;C:/Windows/Microsoft.NET/Framework/v4.0.30319;
set version=1.8
set skipTests=false
set skipPublishing=false

::++++++++++++++++++++++ Building +++++++++++++++++++++++++++++++++++++++++++++++++++++
echo Building project...

msbuild.exe ..\src\Antler.sln /nologo /v:q /p:Configuration=Release /t:Clean
msbuild.exe ..\src\Antler.sln /nologo /v:q /p:Configuration=Release /clp:ErrorsOnly

::++++++++++++++++++++++ Running tests++++++++++++++++++++++++++++++++++++++++++++++++++

if %skipTests%==true goto skipTests
echo Running tests...
if exist output rmdir /s /q output
mkdir output

..\tools\nunit.runner\nunit-console.exe /work:output ..\src\specs\Antler-Domain-Specs\bin\Release\Antler.Domain.Specs.dll ..\src\specs\Antler-EntityFramework-SqlCe-Specs\bin\Release\Antler.EntityFramework.SqlCe.Specs.dll ..\src\specs\Antler-NHibernate-Sqlite-Specs\bin\Release\Antler.NHibernate.Sqlite.Specs.dll ..\src\specs\Antler-Storages-Specs\bin\Release\Antler.Storages.Specs.dll ..\src\specs\Antler-Windsor-Specs\bin\Release\Antler.Windsor.Specs.dll
if %ERRORLEVEL% neq 0 goto end
:skipTests

::++++++++++++++++++++++ Copying built assemblies+++++++++++++++++++++++++++++++++++++++
echo Copying assemblies...

if exist core\output rmdir /s /q core\output
if exist nh-sqlite\output rmdir /s /q nh-sqlite\output
if exist ef-sqlce\output rmdir /s /q ef-sqlce\output
if exist windsor\output rmdir /s /q windsor\output

mkdir core\output\lib\net40
mkdir nh-sqlite\output\lib\net40
mkdir ef-sqlce\output\lib\net40
mkdir windsor\output\lib\net40

::Core
copy ..\src\main\Antler-Core\bin\Release\Antler.Core.* core\output\lib\net40

::NHibernate + Sqlite adapter
copy ..\src\main\Antler-NHibernate\bin\Release\Antler.NHibernate.* nh-sqlite\output\lib\net40
copy ..\src\main\Antler-NHibernate-Sqlite\bin\Release\Antler.NHibernate.Sqlite.* nh-sqlite\output\lib\net40

::EntityFramework + SqlCe adapter
copy ..\src\main\Antler-EntityFramework\bin\Release\Antler.EntityFramework.* ef-sqlce\output\lib\net40
copy ..\src\main\Antler-EntityFramework-SqlCe\bin\Release\Antler.EntityFramework.SqlCe.* ef-sqlce\output\lib\net40

::Windsor adapter
copy ..\src\main\Antler-Windsor\bin\Release\Antler.Windsor.* windsor\output\lib\net40

::++++++++++++++++++++ Creating Nuget packages+++++++++++++++++++++++++++++++++++++++++++++
echo Creating NuGet packages...

copy core\Antler.Core.dll.nuspec core\output
..\src\.nuget\Nuget.exe pack core\output\Antler.Core.dll.nuspec -properties version=%version%
move Antler.Core*.nupkg core\output

copy nh-sqlite\Antler.NHibernate.Sqlite.dll.nuspec nh-sqlite\output
..\src\.nuget\Nuget.exe pack nh-sqlite\output\Antler.NHibernate.Sqlite.dll.nuspec -properties version=%version%
move Antler.NHibernate.Sqlite*.nupkg nh-sqlite\output

copy ef-sqlce\Antler.EntityFramework.SqlCe.dll.nuspec ef-sqlce\output
..\src\.nuget\Nuget.exe pack ef-sqlce\output\Antler.EntityFramework.SqlCe.dll.nuspec -properties version=%version%
move Antler.EntityFramework.SqlCe*.nupkg ef-sqlce\output

copy windsor\Antler.Windsor.dll.nuspec windsor\output
..\src\.nuget\Nuget.exe pack windsor\output\Antler.Windsor.dll.nuspec -properties version=%version%
move Antler.Windsor*.nupkg windsor\output

::+++++++++++++++++++++ Publishing NuGet packages+++++++++++++++++++++++++++++++++++++++++++
if %skipPublishing%==true goto end
echo Publishing NuGet packages...
..\src\.nuget\Nuget.exe push core\output\Antler.Core.%version%.nupkg
..\src\.nuget\Nuget.exe push windsor\output\Antler.Windsor.%version%.nupkg
..\src\.nuget\Nuget.exe push ef-sqlce\output\Antler.EntityFramework.SqlCe.%version%.nupkg
::++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
: end

echo Done.

pause