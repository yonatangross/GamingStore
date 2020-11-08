const width = 600;
const height = 700;
const margin = { top: 20, right: 20, bottom: 100, left: 100 };
const graphWidth = width - margin.left - margin.right;
const graphHeight = height - margin.top - margin.bottom;

var format = d3.format(",.2r");
const svg = d3.select(".barchart")
    .append("svg")
    .attr("width", width)
    .attr("height", height);
const graph = svg.append("g")
    .attr("width", graphWidth)
    .attr("height", graphHeight)
    .attr("transform", `translate(${margin.left}, ${margin.top})`);
const gXAxis = graph.append("g")
    .attr("transform", `translate(0, ${graphHeight})`);
const gYAxis = graph.append("g");

d3.json("/data/BarChartData.json").then(data => {
    const x = d3.scaleLinear()
        .domain([0, d3.max(data, d => d.Value)])
        .range([0, graphWidth]);

    const y = d3.scaleBand()
        .domain(data.map(item => item.Date))
        .range([0, graphHeight])
        .paddingInner(0.1)
        .paddingOuter(0.2);

    const rects = graph.selectAll("rect")
        .data(data);

    rects.attr("class", "bar-rect")
        .attr("height", y.bandwidth)
        .attr("width", d =>  x(d.Value)-x(0))
        .attr("x", d => x(0))
        .attr("y", d => y(d.Date));

    rects.enter()
        .append("rect")
        .attr("class", "bar-rect")
        .attr("height", y.bandwidth)
        .attr("width", d=> x(d.Value) - x(0))
        .attr("x", d => x(0))
        .attr("y", d => y(d.Date));

    rects.enter()
        .append("text")
        .attr("class", "text-rect")
        .attr("fill", "white")
        .attr("text-anchor", "end")
        .attr("font-family", "sans-serif")
        .attr("font-size", 10)
        .attr("x", d => x(d.Value))
        .attr("y", (d) => y(d.Date))
        .attr("dy", "1.30em")
        .attr("dx", -4)
        .text(d => format(d.Value)+" $" )
        .text(d => format(d.Value)+" $" )
        .call(text => text.filter(d => d.Value > 0) // short bars for values 
            .attr("dx", +4)
            .attr("fill", "black")
            .attr("text-anchor", "start"));


    const xAxis = d3.axisBottom(x)
        .ticks(5)
        .tickFormat(d => `${d}$`);

    const yAxis = d3.axisLeft(y);


    gXAxis.call(xAxis);
    gYAxis.call(yAxis);
    gXAxis.selectAll("text")
        .attr("transform", "translate(-10,10)rotate(-45)")
        .style("text-anchor", "end")
        .style("font-size", 15);

    gYAxis.selectAll("text")
        .style("font-size", 10).style("text-anchor", "start").style("font-weight", "bold")
        .attr('transform', () => { return 'translate(-80,0)' });
});