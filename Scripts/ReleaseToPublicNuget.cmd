Echo Creates Release Packages

set packages="..\packages\release"
set program="..\src\Slugent.APIInfo"

dotnet msbuild /p:Configuration=Release %program%
dotnet pack -o %packages% %program%

REM for %%n in (..\packages\*.nupkg) do  dotnet nuget push -s d:\a_dev\LocalNugetPackages "%%n"
