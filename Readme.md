# Heisenslaught

A custom drafting tool for (On|Off)slaught.

## Local Setup

1. Install [node](https://nodejs.org)
1. Install `npm install --global typings@0.8.1`
	
### UI
1. Install [angular cli](https://github.com/angular/angular-cli/)
1. In `src/HeisenslaughtUI` run `npm install`
1. In `src/HeisenslaughtUI` run `ng build`
Please complete these steps before running the server

### Windows Server
1. Install [Visual Studio 2017](https://www.visualstudio.com/vs/visual-studio-2017-rc/)
	1. Select the following Workloads when installing
		1. Web Development
		1. .NET Core and Docker (Preview)

1. Setup static file compilation
    1. In `src/Heisenslaught` run `npm install`
    1. Open `Heisenslaught.sln` 

1. Press `F5` to launch the application

### OS X / Linux Server
1. If you are upgrading from a previous version of dotnet you may need to remove your previous version. Helpful scripts can be found [here](https://github.com/dotnet/cli/tree/rel/1.0.0/scripts/obtain/uninstall).
1. Install [Preview 3](https://github.com/dotnet/core/blob/master/release-notes/preview3-download.md) (or later)
1. Run `dotnet restore`
1. In `src/Heisenslaught` run `ASPNETCORE_ENVIRONMENT=Development dotnet run`

If you are on Arch Linux then you can figure it out yourself :) (or wait until [this](https://aur.archlinux.org/packages/dotnet-cli/) is updated).

## Deployment
Please refer to [ops/Readme.md](https://github.com/chetjan/heisenslaught/tree/master/ops).
