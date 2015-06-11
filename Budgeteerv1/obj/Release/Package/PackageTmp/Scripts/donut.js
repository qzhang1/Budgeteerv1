$(function () {

	var data, chartOptions

	data = [
		{ label: "Product 1", data:  250 }, 
		{ label: "Product 2", data: Math.floor (350) }, 
		{ label: "Product 3", data: Math.floor (650) }, 
		{ label: "Product 4", data: Math.floor (750) },
		{ label: "Product 5", data: Math.floor (250) }
	]

	chartOptions = {		
		series: {
			pie: {
				show: true,  
				innerRadius: .5, 
				stroke: {
					width: 4
				}
			}
		}, 
		legend: {
			position: 'ne'
		}, 
		tooltip: true,
		tooltipOpts: {
			content: '%s, %p.0%'
		},
		grid: {
			hoverable: true
		},
		colors: mvpready_core.layoutColors
	}


	var holder = $('#donut-chart')

	if (holder.length) {
		$.plot(holder, data, chartOptions )
	}

})