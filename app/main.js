const electron = require('electron');
const path = require('path');
const app = electron.app;
const BrowserWindow = electron.BrowserWindow;
const Tray = electron.Tray;
const Menu = electron.Menu;

const port = 39583;

const argv = require('yargs')
            .usage('Usage: -m [monitor id] -u [url to load] [-d]')
            .options('u', {
                alias : 'url',
                demand : true,
                describe: "Url to load (can be file://)",
                type: "string"
            })
            .options('m', {
                alias : 'monitor',
                demand : true,
                describe: "Monitor id to run on",
                default: 0,
                type: "number"
            })
            .options('d', {
                alias : 'debug',
                describe: "Run as debug window",
                type: "boolean"
            })
            .parse(process.argv);

const mon = argv.monitor;
const url = argv.url;
const debug = argv.debug;
const icon = process.platform === 'win32' ? "./icon.ico" : "./icon.png";

if(debug){
    require('electron-debug')({enabled: true, showDevTools: true});
}

console.log("opening", url, "on monitor", mon, debug ? "in debug mode" : "");

let mainWindow;

function makeid()
{
    var text = "";
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for(var i = 0; i < 5; i++){
        text += possible.charAt(Math.floor(Math.random() * possible.length));
    }

    return text;
}

app.on('ready', function(){
    var displays = electron.screen.getAllDisplays();
    var targetDisplay = displays[mon];

    var title = makeid();

    if (debug) {
        mainWindow = new BrowserWindow({
            title: title, 
            hasShadow: false,
            icon: icon,
            allowRunningInsecureContent: true
        });
    }
    else {
        mainWindow = new BrowserWindow({
            skipTaskbar: true, 
            title: title, 
            frame: false, 
            transparent: true,
            focusable: false,
            hasShadow: false,
            x: targetDisplay.bounds.x,
            y: targetDisplay.bounds.y,
            width: targetDisplay.bounds.width,
            height: targetDisplay.bounds.height - 1, //-1 is a windows hack that fixes taskbar icon sometimes showing
            type: "desktop",
            icon: icon,
            allowRunningInsecureContent: true
        });

        mainWindow.setIgnoreMouseEvents(true);

        if (process.platform === 'win32') {
            //windows hack for fullscreen
            require('http').get(`http://localhost:${port}/api/fullscreen/${mon}/${title}`);
        }
    }

    mainWindow.loadURL(url);
});

app.on('window-all-closed', function () {
    app.quit();
});