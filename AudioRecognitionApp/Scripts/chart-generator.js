function createChart(labels, dataArray) {



    var canvas = document.getElementById("chart-canvas");
    var ctx = canvas.getContext("2d");
    var options = {
        scales: {
            xAxes: [{
                ticks: {
                    Rotation: 0 // angle in degrees
                }
            }]
        },
        legend: {
            display: false
        }
    };
    var data = {
        labels: labels,
        datasets: [{
            backgroundColor: "#87CEFF",
            data: dataArray
        }]
    };


    var myNewChart = new Chart(ctx, {
        type: "bar",
        data: data,
        options: options
    });
}