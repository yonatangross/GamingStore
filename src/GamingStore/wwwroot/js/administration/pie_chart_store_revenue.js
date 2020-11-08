
var format = d3.format(",.2r");

d3.json("/data/PieChart.json").then(data => {
    const pieHeight = Math.min(600, 600);


    const color = d3.scaleOrdinal()
        .domain(data.map(d => d.Name))
        .range(d3.quantize(t => d3.interpolateSpectral(t * 0.8 + 0.1), data.length).reverse());

    const arc = d3.arc()
        .innerRadius(0)
        .outerRadius(Math.min(width, pieHeight) / 2 - 1);

    const arcLabelRadius = Math.min(width, pieHeight) / 2 * 0.8;

    const arcLabel = d3.arc().innerRadius(arcLabelRadius).outerRadius(arcLabelRadius);

    const pie = d3.pie()
        .sort(null)
        .value(d => d.Value);

    const arcs = pie(data);

    const svgPie = d3.select(".piechart")
        .append("svg")
        .attr("width", 800)
        .attr("height", pieHeight);

    svgPie.append("g")
        .attr("class", "pie-container")
        .attr("stroke", "white")
        .attr("transform", "translate(250,250)")
        .selectAll("path")
        .data(arcs)
        .join("path")
        .attr("fill", d => color(d.data.Name))
        .attr("d", arc)
        .append("title")
        .text(d => `${d.Name}: ${format(d.Value)}`);

    svgPie.append("g")
        .attr("font-family", "sans-serif")
        .attr("font-size", 12)
        .attr("text-anchor", "middle")
        .selectAll("text")
        .data(arcs)
        .join("text")
        .attr("transform", d => `translate(${arcLabel.centroid(d)})`)
        .call(text => text.append("tspan")
            .attr("y", "-0.4em")
            .attr("font-weight", "bold")
            .text(d => d.data.Name))
        .call(text => text.filter(d => (d.endAngle - d.startAngle) > 0.25).append("tspan")
            .attr("x", 0)
            .attr("y", "0.7em")
            .attr("fill-opacity", 0.7)
            .text(d => format(d.data.Value)));
});