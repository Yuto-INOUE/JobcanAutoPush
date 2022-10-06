SHELL       := powershell.exe
.SHELLFLAGS := -NoProfile -Command

init:
	Copy-Item -Path Credentials.Template.txt -Destination Credentials.cs

build: init
	dotnet build -c Release
