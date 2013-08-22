/*
@reference ~/Content/css/admin/widgets/analytics.css
@reference ~/Content/js/_lib/jquery.flot
 */

 (function() {
	$.fn.UseTooltip =  function () {
		var previousPoint = null;
		
		$(this).bind("plothover", function (event, pos, item) {         
			if (item) {
				if (previousPoint != item.dataIndex) {
					previousPoint = item.dataIndex;
	
					$("#vumtooltip").remove();
					
					var x = item.datapoint[0];
					var y = item.datapoint[1];      
					showTooltip(item.pageX, item.pageY, "<b>" + analyticsDataLabels[x - 1] + "</b><br/>" + item.series.label + ": <strong>" + y + "</strong>");
				}
			}
			else {
				$("#vumtooltip").remove();
				previousPoint = null;
			}
		});
	};
	function showTooltip(x, y, contents) {
		$('<div id="vumtooltip">' + contents + '</div>').css({
			position: 'absolute',
			display: 'none',
			top: y + 5,
			left: x + 20,
			border: '1px solid #D0D0D0',
			padding: '6px',     
			size: '9',   
			'background-color': '#fff',
			opacity: 0.80
		}).appendTo("body").fadeIn(200);
	}
 })();

 $(function() {
	getAnalytics();
	 
	function getAnalytics() {
		$("#AnalyticsLoading").removeClass('hide');
		
		$.post(Url.widgetsAnalytics, {
			duration: $("#Duration").val()
		}).done(function(result) {
			 
			$("#AnalyticsSummaryWidget .content").html(result);

			var graph = $("#AnalyticsGraph");

			var options = {
				grid: {
					show: true,
					aboveData: true,
					color: "#3f3f3f" ,
					labelMargin: 5,
					axisMargin: 0, 
					borderWidth: 0,
					borderColor:null,
					minBorderMargin: 5 ,
					clickable: true, 
					hoverable: true,
					autoHighlight: true,
					mouseActiveRadius: 10
				},
				series: {
					grow: {
						 active:false
					},
					lines: {
						show: true,
						fill: true,
						lineWidth: 2,
						steps: false
						},
					points: {
						show:true
					}
				},
				legend: {
					 position: "se"
				},
				yaxis: {
					 min: 0
				},
				xaxis: {
					ticks : analyticsLabel, // injected to the page from analytics ajax
					tickDecimals : 0
				},
				colors: ['#88bbc8', '#ed7a53', '#9FC569', '#bbdce3', '#9a3b1b', '#5a8022', '#2c7282'],
				shadowSize: 1,
				tooltip: false, //activate tooltip
			};

			$.plot(graph, [{
				label: 'Visits',
				data: analyticsData, // injected to the page from Analytics ajax
				lines: {
					fillColor: "#f2f7f9"},
					points: {fillColor: "#88bbc8"}
				}], options);
			 
			graph.UseTooltip();
			
			$("#AnalyticsSummaryWidgetContent [rel='tooltip']").tooltip();
			// Hookup ajax update based on duration dropdown changing
			$("#Duration").on('change', function() {
				getAnalytics();
			});
		 }).fail(function() {
			alert("Problem getting analytics information."); 
		 });
	 }
 });