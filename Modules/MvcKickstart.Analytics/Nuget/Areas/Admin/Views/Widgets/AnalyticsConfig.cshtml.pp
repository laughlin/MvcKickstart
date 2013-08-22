@model MvcKickstart.Analytics.ViewModels.Widgets.Config

@using (Html.BeginRouteForm("MvcKickstart_Analytics_Widgets_AnalyticsConfig")) {
	<div class="form-group">
		@Html.LabelFor(x => x.ProfileId)
		<select name="ProfileId" id="ProfileId" class="form-control">
			@foreach (var account in Model.Accounts.OrderBy(x => x.Name)) {
				if (!Model.Profiles.Any(x => x.AccountId == account.Id)) {
					continue;
				}
				<optgroup label="@account.Name">
					@foreach (var profile in Model.Profiles.Where(x => x.AccountId == account.Id).OrderBy(x => x.Name)) {
						<option value="@profile.Id">@profile.Name</option>
					}
				</optgroup>
			}
		</select>
	</div>
	<div class="form-group">
		<button type="submit" class="btn btn-primary">Submit</button>
	</div>
}