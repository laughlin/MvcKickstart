/*
@reference ~/Content/js/_lib/jquery
@reference ~/Content/js/_lib/jquery.validate
@reference ~/Content/js/_lib/jquery.typewatch
 */

 $(function() {
	 $("#Username").typeWatch({
		wait:750,
		captureLength: 0,
		callback: function(value) {
			$.post(Url.validateUsername, {
				username: value
			}).done(function(data) {
				
				if (data) {
					$(".username-valid").show();
					$(".username-invalid").hide();
				} else {
					$(".username-valid").hide();
					$(".username-invalid").show();
				}
			});
		}
	 });
 });