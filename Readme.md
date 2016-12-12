# Heisenslaught

A custom drafting tool for (On|Off)slaught.

## Local Setup

### Windows
1. Install [Visual Studio 2017](https://www.visualstudio.com/vs/visual-studio-2017-rc/)
	1. Make sure to select Web Development when installing
1. Install [node](https://nodejs.org)
1. Setup static file compilation
    1. In `src/Heisenslaught` run `npm install`
    1. Open `Heisenslaught.sln` 
    1. In the Task Runner run the Gulpfile task `default`
        1. If you cannot find the Task Runner it can be found under `View->Other Windows->Task Runner Explorer`
        1. Under `Gulpfile` right click `default` and select `Run`
1. Press `F5` to launch the application

### OS X / Linux
1. If you are upgrading from a previous version of dotnet you may need to remove your previous version. Helpful scripts can be found [here](https://github.com/dotnet/cli/tree/rel/1.0.0/scripts/obtain/uninstall).
1. Install [Preview 3](https://github.com/dotnet/core/blob/master/release-notes/preview3-download.md) (or later)
1. Run `dotnet restore`
1. Install [node](https://nodejs.org/en/download/package-manager/)
1. Setup static file compilation
    1. In `src/Heisenslaught` run `npm install`
    1. `./node_modules/.bin/gulp`
1. In `src/Heisenslaught` run `ASPNETCORE_ENVIRONMENT=Development dotnet run`

If you are on Arch Linux then you can figure it out yourself :) (or wait until [this](https://aur.archlinux.org/packages/dotnet-cli/) is updated).

## Deployment
Please refer to [ops/Readme.md](https://github.com/chetjan/heisenslaught/tree/master/ops).
