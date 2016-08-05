(function () {
    function get(path){
        fetch("http://localhost:39583/api/" + path).then(function (response) {
            return response.json();
        });
    };

    AntWall = document.AntWall = window.AntWall = Object.freeze({
        getBasicInfo: get.bind(null, "basic_info.json"),
        getProcesses: get.bind(null, "processes.json"),
        getCPUInfo: get.bind(null, "cpu_info.json"),
    });
})();