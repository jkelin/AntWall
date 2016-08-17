# AntWall
+ AntWall - HTML5 live wallpaper. Uses electron. You can use file:// url. You can use Node.js modules and you can use require("") in your wallpapers.
+ AntWallServer - .NET server that displays info about current environment. Also acts as hack to make AntWall window into a wallpaper (thanks to https://github.com/Foohy/Wallpainter).

## Usage
1. Start AntWallServer.exe (.NET executable).
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
+ AntWallServer works only on .NET but it should be easy to port it to mono with minor modifications.
+ Only tested on Windows

Downloads: https://github.com/fireantik/AntWall/releases/
