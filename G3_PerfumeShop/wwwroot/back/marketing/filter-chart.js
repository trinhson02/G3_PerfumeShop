// Create root and chart
var root = am5.Root.new("sales-analytics"); 

root.setThemes([
  am5themes_Animated.new(root)
]);

var chart = root.container.children.push( 
  am5xy.XYChart.new(root, {
    wheelY: "zoomX"
  }) 
);

// Define data
var data = [{
  date: new Date(2021, 0, 1).getTime(),
  value: 100
}, {
  date: new Date(2021, 0, 2).getTime(),
  value: 320
}, {
  date: new Date(2021, 0, 3).getTime(),
  value: 216
}, {
  date: new Date(2021, 0, 4).getTime(),
  value: 150
}, {
  date: new Date(2021, 0, 5).getTime(),
  value: 156
}, {
  date: new Date(2021, 0, 6).getTime(),
  value: 199
}, {
  date: new Date(2021, 0, 7).getTime(),
  value: 114
}, {
  date: new Date(2021, 0, 8).getTime(),
  value: 269
}, {
  date: new Date(2021, 0, 9).getTime(),
  value: 190
}, {
  date: new Date(2021, 0, 10).getTime(),
  value: 380
}, {
  date: new Date(2021, 0, 11).getTime(),
  value: 250
}, {
  date: new Date(2021, 0, 12).getTime(),
  value: 110
}, {
  date: new Date(2021, 0, 13).getTime(),
  value: 185
}, {
  date: new Date(2021, 0, 14).getTime(),
  value: 105
}];

// Create Y-axis
var yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, {
  extraTooltipPrecision: 1,
  renderer: am5xy.AxisRendererY.new(root, {
    minGridDistance: 30
  })
}));

// Create X-Axis
var xAxis = chart.xAxes.push(am5xy.DateAxis.new(root, {
  baseInterval: { timeUnit: "day", count: 1 },
  renderer: am5xy.AxisRendererX.new(root, {
    minGridDistance: 20,
    cellStartLocation: 0.2,
    cellEndLocation: 0.8
  })
}));

// Create series
function createSeries(name, field) {
  var series = chart.series.push(am5xy.SmoothedXLineSeries.new(root, { 
    name: name,
    xAxis: xAxis, 
    yAxis: yAxis, 
    valueYField: field, 
    valueXField: "date",
    tooltip: am5.Tooltip.new(root, {}),
    clustered: true
  }));
  
  series.strokes.template.setAll({
    strokeWidth: 3
  });
  
  series.fills.template.setAll({
    fillOpacity: 0.5,
    visible: true
  });
  
  series.get("tooltip").label.set("text", "[bold]{name}[/]\n{valueX.formatDate()}: {valueY}")
  series.data.setAll(data);
  
  return series;
}

var series1 = createSeries("Series #1", "value");

// Create axis ranges
function createRange(series, value, endValue, color) {
  var rangeDataItem = xAxis.makeDataItem({
    value: new Date(2021, 0, 4).getTime(),
    endValue: new Date(2021, 0, 10).getTime()
  });
  
  var range = series.createAxisRange(rangeDataItem);
  
  range.strokes.template.setAll({
    stroke: color,
    strokeWidth: 3
  });
  
  range.fills.template.setAll({
    fill: color,
    fillOpacity: 0.8,
    visible: true
  });
  
  rangeDataItem.get("axisFill").setAll({
    fill: color,
    fillOpacity: 0.05,
    visible: true
  });
 
}

createRange(series1, 125, 275, am5.color(0xff621f));

// Add cursor
chart.set("cursor", am5xy.XYCursor.new(root, {
  behavior: "zoomX",
  xAxis: xAxis
}));

xAxis.set("tooltip", am5.Tooltip.new(root, {
  themeTags: ["axis"]
}));

yAxis.set("tooltip", am5.Tooltip.new(root, {
  themeTags: ["axis"]
}));