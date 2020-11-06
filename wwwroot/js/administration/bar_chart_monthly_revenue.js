// set the dimensions and margins of the graph
var margin = { top: 20, right: 30, bottom: 40, left: 90 },
    width = 500 - margin.left - margin.right,
    height = 500 - margin.top - margin.bottom;

// append the svg object to the body of the page
var svg = d3.select("svg")
    .append("svg")
    .attr("width", width + margin.left + margin.right)
    .attr("height", height + margin.top + margin.bottom)
    .append("g")
    .attr("transform",
        "translate(" + margin.left + "," + margin.top + ")");

// Parse the Data
d3.json("/data/BarChartData.json",
    function(data) {

        // Add X axis
        var x = d3.scaleLinear()
            .domain([0, 13000])
            .range([0, width]);

        svg.append("g")
            .attr("transform", "translate(0," + height + ")")
            .call(d3.axisBottom(x))
            .selectAll("text")
            .attr("transform", "translate(-10,0)rotate(-45)")
            .style("text-anchor", "end");

        svg.append("text") // text label for the x axis
            .attr("x", width / 2)
            .attr("y", height + margin.bottom)
            .style("text-anchor", "middle")
            .text("Value");


        // Y axis
        var y = d3.scaleBand()
            .range([0, height])
            .domain(data.map(function(d) { return d.Date; }))
            .padding(.1);

        svg.append("text")
            .attr("transform", "rotate(-90)")
            .attr("y", 0 - margin.left)
            .attr("x", 0 - (height / 2))
            .attr("dy", "1em")
            .style("text-anchor", "middle")
            .text("Date");

        svg.append("g")
            .call(d3.axisLeft(y));

        //Bars
        var bar = svg.selectAll("myRect")
            .data(data)
            .enter();
        bar.append("rect")
            .attr("x", x(0))
            .attr("y", function(d) { return y(d.Date); })
            .attr("width", function(d) { return x(d.Value); })
            .attr("height", y.bandwidth())
            .attr("fill", "#69b3a2");


        svg.append("text")
            .attr("x", (width / 2)+40)
            .attr("y", 22)
            .attr("text-anchor", "middle")
            .style("font-size", "22px")
            .style("text-decoration", "bold")
            .text("Sales in $ per month");
    });