# AntWall
+ AntWall - HTML5 live wallpaper, supports transparency and displays behind icons. Uses electron. Can be run with file:// url. Can utilize Node.js modules and can use require("") in wallpapers. Flash is not supported but HTML5 video works.
+ AntWallServer - Mono/.NET server that displays info about current environment. Also acts as hack to make AntWall window into a wallpaper (thanks to https://github.com/Foohy/Wallpainter).

Example wallpaper: https://github.com/fireantik/AntWallSample

Downloads: https://github.com/fireantik/AntWall/releases/

## Usage
1. Start AntWallServer.exe (Mono/.NET executable).
2. Start AntWall for each monitor (with proper -m id) that you want it to display on.

## AntWall command line arguments
```
-u, --url      Url to load (can be file://)  [string] [required]
-m, --monitor  Monitor id to run on  [number] [required] [default: 0]
-d, --debug    Run as debug window  [boolean]
--help         Display help screen.
```

## AntWallServer command line arguments
```
-p, --port  (Default: 39583) Port to listen on
--help      Display help screen.
```

## Current project state
+ AntWallServer cpu info is mostly broken.
+ AntWallServer works only on Windows, it will need minor tweaks for other platforms (different counter names).
+ Only tested on Windows

## Developer info
+ To build the project you need node, npm and mono xbuild in your path.

## Building and packaging
Delete app/out and then:
```
cd app
npm run pkg
```

