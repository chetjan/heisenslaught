# Heisenslaught

A custom drafting tool for (On|Off|Cole)slaught.

## Local Setup

1. Install [node](https://nodejs.org)
1. Install [mongodb](https://www.mongodb.com/)
	
### App Configuration 
1. Copy src/Heisenslaught/appsettings.json to /opt/Heisenslaught/appsettings.json
1. Changes the values of 'ClientID' and 'ClientSecret' to match your battle.net application - [Create Here](https://dev.battle.net/)
1. Add any battleTags you wish to make super users to 'AutoGrantSuperUserToBattleTags' - they will be added to the SU role the first time they log in
	Example appsettings.json:
	```json
	{
	  "MongoDb": {
		"ConnectionString": "mongodb://localhost:27017",
		"Database": "Heisenslaught"
	  },
	  "Authentication": {
		"BattleNet": {
		  "ClientID": "HAN76GhJ87Jhgs",
		  "ClientSecret":  "MHHVS8s6b66dhs7dj"
		}
	  },
	  "UserCreation": {
		"AutoGrantSuperUserToBattleTags": [
			"John#1234",
			"SuperAsome#9876"
		]
	  }
	}
	```

### SSL Configuration
1. Instructions coming soon(tm)

### UI
1. In `src/HeisenslaughtUI` run `npm install`
    1. Note there is currently a bug in the version of node-sass used by angular-cli. After `npm install` you need to run `npm rebuild node-sass` to generate the vendor folder needed.
1. In `src/HeisenslaughtUI` run `./node_modules/bin/ng.cmd build` (Windows) or `./node_modules/.bin/ng build` (OS X / Linux)
    1. Alternatively you can install [angular cli](https://github.com/angular/angular-cli/) globally and just run `ng build`

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
