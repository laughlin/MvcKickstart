@model MvcKickstart.Analytics.ViewModels.Widgets.Analytics
<script>
	var analyticsLabel = [@for (var i = 1; i <= Model.Visits.Count(); i++) {if (i > 1 && i % 4 == 0) {var item = Model.Visits.ElementAt(i - 1);if (i > 4) {<text>,</text>}<text>[@i,"@item.Key.ToString("MMM d")"]</text>}}];
	var analyticsData = [@for (var i = 0; i < Model.Visits.Count(); i++) {var item = Model.Visits.ElementAt(i);if (i > 0) {<text>,</text>}<text>[@(i + 1),@item.Value]</text>}];
	var analyticsDataLabels = [@for (var i = 0; i < Model.Visits.Count(); i++) {var item = Model.Visits.ElementAt(i);if (i > 0) {<text>,</text>}<text>"@item.Key.ToString("dddd, MMM d, yyyy")"</text>}];
</script>
<div id="AnalyticsSummaryWidgetContent">
	<div class="row">
		<div class="col-sm-6">
			<h4>@Model.Profile.Name</h4>
		</div>
		<div class="col-sm-6">
			<div class="text-right">
				<img id="AnalyticsLoading" src="@Url.Image("loading.gif")" alt="..." class="hide" />
				@Html.DropDownListFor(x => x.Duration, new [] {
					new SelectListItem { Text = "Past 30 days", Value = "30" },
					new SelectListItem { Text = "Past 60 days", Value = "60" },
					new SelectListItem { Text = "Yesterday", Value = "1" }
				}, new { @class = "form-control analyticsTimeframe" })
			</div>
		</div>
	</div>
	<div id="AnalyticsGraph"></div>

	<h3>Site Usage</h3>
	<table class="table table-bordered table-condensed">
		<tbody>
			<tr>
				<th><div rel="tooltip" title="Total number of visits">Visits</div></th>
				<td>@Model.TotalVisits.ToString("N0")</td>
				<th><div rel="tooltip" title="The percentage of single-page visits (i.e. visits in which the person left your site from the entrance page)">Entrance Bounce Rate</div></th>
				<td>@Model.EntranceBounceRate.ToString("N2")%</td>
			</tr>
			<tr>
				<th><div rel="tooltip" title="The total number of pageviews for the website">Page Views</div></th>
				<td>@Model.TotalPageViews.ToString("N0")</td>
				<th><div rel="tooltip" title="The percentage of site exits that occurred out of the total page views">Exit Rate Percentage</div></th>
				<td>@Model.PercentExitRate.ToString("N2")%</td>
			</tr>
			<tr>
				<th><div rel="tooltip" title="The average number of pages viewed during a visit to your site. Repeated views of a single page are counted">Pages Per Visit</div></th>
				<td>@Model.PageviewsPerVisit.ToString("N2")</td>
				<th><div rel="tooltip" title="The average amount of time visitors spent on the site">Average Time On Site</div></th>
				<td>@Model.AverageTimeOnSite.ToSmallTime()</td>
			</tr>
			<tr>
				<th><div rel="tooltip" title="The percentage of visits by people who had never visited the site before">% New Visits</div></th>
				<td>@Model.PercentNewVisits.ToString("N2")%</td>
				<th><div rel="tooltip" title="The average amount of time it takes for pages to load, from initiation of the pageview (e.g. click on a page link) to load completion in the browser.">Average Page Load Time</div></th>
				<td>@Model.AveragePageLoadTime.ToSmallTime(true)</td>
			</tr>
		</tbody>

	</table>

	<h3>Top Pages</h3>
	@if (Model.PageViews.Any()) {
		<table class="table table-bordered table-condensed">
			<thead>
				<tr>
					<th>Page</th>
					<th># of Visits</th>
				</tr>
			</thead>
			<tbody>
				@foreach (var page in Model.PageViews.OrderByDescending(x => x.Value).Take(5)) {
					<tr>
						<th><div rel="tooltip" title="@Model.PageTitles[page.Key]"><a href="@page.Key">@page.Key</a></div></th>
						<td>@page.Value.ToString("N0")</td>
					</tr>
				}
			</tbody>
		</table>
	} else {
		<p>There were no pageviews discovered :(</p>
	}
	<div class="row">
		<div class="col-sm-6">
			<h3>Top Referrers</h3>
			@if (Model.TopReferrers.Any()) {
				<table class="table table-bordered table-condensed">
					<thead>
						<tr>
							<th>Url</th>
							<th># of Visits</th>
						</tr>
					</thead>
					<tbody>
						@foreach (var item in Model.TopReferrers.OrderByDescending(x => x.Value).Take(5)) {
							<tr>
								<th><a href="http://@item.Key">@item.Key</a></th>
								<td>@item.Value.ToString("N0")</td>
							</tr>
						}
					</tbody>
				</table>
			} else {
				<p>There were no referrers discovered :(</p>
			}	
		</div>
		<div class="col-sm-6">
			<h3>Top Searches</h3>
			@if (Model.TopSearches.Any()) {
				<table class="table table-bordered table-condensed">
					<thead>
						<tr>
							<th>Term</th>
							<th># of Searches</th>
						</tr>
					</thead>
					<tbody>
						@foreach (var item in Model.TopSearches.OrderByDescending(x => x.Value).Take(5)) {
							<tr>
								<th>@item.Key</th>
								<td>@item.Value.ToString("N0")</td>
							</tr>
						}
					</tbody>
				</table>
			} else {
				<p>There were no searches discovered :(</p>
			}	
		</div>
	</div>

	<small class="footerRight">Generated at @DateTime.Now.ToSmallTime()</small>
</div>