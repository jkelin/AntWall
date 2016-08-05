var packager = require('electron-packager');
var path = require('path');
var fs = require('fs');
var exec = require('child_process').exec;
var ncp = require('ncp').ncp;

var monoBuildPromise = new Promise(function(resolve, reject){
    var cmd = 'xbuild /p:Configuration=Release ' + path.join(__dirname, "..", "AntWall.sln");
    console.log(cmd)
    var mono = exec(cmd, function(error, stdout, stderr){
        if(error) return reject(error);

        console.log(stdout);

        var releaseFolder = path.join(__dirname, "..", "AntWall", "bin", "Release");
        fs.readdir(releaseFolder, function(err, files){
            if(err) return reject(err);
            resolve(releaseFolder);
        });
    });
});

function afterExtract(buildPath, electronVersion, platform, arch, callback){
    monoBuildPromise.then(function(releaseFolder){
        ncp(releaseFolder, buildPath, function (err) {
            if (err) {
                console.error(err);
            }

            callback();
        });
    });
}

packager({
    dir: __dirname,
    all: true,
    out: "out",
    prune: true,
    icon: process.platform === 'win32' ? "./icon.ico" : "./icon.png",
    afterExtract: [afterExtract]
}, function(){console.log("done")});