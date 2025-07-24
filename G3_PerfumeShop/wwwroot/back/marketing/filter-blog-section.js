document.addEventListener('DOMContentLoaded', function () {
    RenderBlogDashboard();
})

var filter;





function RenderBlogDashboard() {
    $.ajax({
        url: '/Marketing/GetBlogCountMonth',
        type: 'GET',
        dataType: 'json',
        data: {
            anchorDate: new Date(2024, 10, 15)
        },
        success: function (data) {
            filter = data;

            TestDraw("count-blog-in-month", data.Holder);

        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
        }
    });
}

function TestDraw(holder, realData) {
    const root = am5.Root.new(holder);
    const myTheme = am5.Theme.new(root);

    myTheme.rule("AxisLabel", ["minor"]).setAll({
        dy: 1
    });

    // Set themes
    // https://www.amcharts.com/docs/v5/concepts/themes/
    root.setThemes([
        am5themes_Animated.new(root),
        myTheme,
        am5themes_Responsive.new(root)
    ]);


    // Create chart
    // https://www.amcharts.com/docs/v5/charts/xy-chart/
    var chart = root.container.children.push(am5xy.XYChart.new(root, {
        panX: false,
        panY: false,
        wheelX: "panX",
        wheelY: "zoomX",
        paddingLeft: 0
    }));


    // Add cursor
    // https://www.amcharts.com/docs/v5/charts/xy-chart/cursor/
    var cursor = chart.set("cursor", am5xy.XYCursor.new(root, {
        behavior: "zoomX"
    }));
    cursor.lineY.set("visible", false);

    var date = new Date();
    date.setHours(0, 0, 0, 0);
    var value = 100;

    function generateData() {
        value = Math.round((Math.random() * 10 - 5) + value);
        am5.time.add(date, "day", 1);
        return {
            date: date.getTime(),
            value: value
        };
    }

    function generateDatas(count) {
        var data = [];
        for (var i = 0; i < count; ++i) {
            data.push(generateData());
        }
        return data;
    }


    // Create axes
    // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
    //var xAxis = chart.xAxes.push(am5xy.DateAxis.new(root, {
    //    maxDeviation: 0,
    //    baseInterval: {
    //        timeUnit: "day",
    //        count: 1
    //    },
    //    renderer: am5xy.AxisRendererX.new(root, {
    //        minorGridEnabled: true,
    //        minorLabelsEnabled: true
    //    }),
    //    tooltip: am5.Tooltip.new(root, {})
    //}));

    //xAxis.set("minorDateFormats", {
    //    "day": "dd",
    //    "month": "MM"
    //});

    var xAis = chart.xAxis.push(
        am5xy.ValueAxis.new(root, {
            min: 0,
            max: 30,
            renderer: am5xy.AxisRendererY.new(root, {})
        })
    );


    var yAxis = chart.yAxes.push(
        am5xy.ValueAxis.new(root, {
            min: 0,
            max: 50,
            renderer: am5xy.AxisRendererY.new(root, {})
        })
    );


    // Add series
    // https://www.amcharts.com/docs/v5/charts/xy-chart/series/
    var series = chart.series.push(am5xy.ColumnSeries.new(root, {
        name: "Series",
        xAxis: xAxis,
        yAxis: yAxis,
        valueYField: "Count",
        valueXField: "Day",
        tooltip: am5.Tooltip.new(root, {
            labelText: "{valueY}"
        })
    }));

    series.columns.template.setAll({ strokeOpacity: 0 })


    // Add scrollbar
    // https://www.amcharts.com/docs/v5/charts/xy-chart/scrollbars/
    chart.set("scrollbarX", am5.Scrollbar.new(root, {
        orientation: "horizontal"
    }));

    var data = generateDatas(30);
    console.log("data");
    console.log(data);
    console.log(realData);
    series.data.setAll(realData);


    // Make stuff animate on load
    // https://www.amcharts.com/docs/v5/concepts/animations/
    series.appear(1000);
    chart.appear(1000, 100);

}

