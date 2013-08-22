/*
@reference ~/Content/js/_lib/jquery
@reference ~/Content/js/_lib/bootstrap
 */

if (window['jQuery']) {
	jQuery.fn.reset = function(fn) {
		return fn ? this.bind("reset", fn) : this.trigger("reset");
	};
	
	if (jQuery['validator']) {
		jQuery.validator.addMethod("requirechecked", function(value, element, param) {
			return element.checked;
		});
		if (jQuery.validator['unobtrusive']) {
			jQuery.validator.unobtrusive.adapters.add("requirechecked", function(options) {
				options.rules['requirechecked'] = true;
				options.messages['requirechecked'] = options.message;
			});
		}
	}
}

// ============================================================
// Bootstrap and MVC integration
// ============================================================

$(function() {

	$('form').submit(function() {
		if (jQuery().validate) {
			// All control groups need to be decorated with 'error' class if not valid
			if ($(this).valid()) {
				$(this).find('div.control-group').each(function() {
					if ($(this).find('span.field-validation-error').length == 0) {
						$(this).removeClass('error');
					}
				});
			} else {
				$(this).find('div.control-group').each(function() {
					if ($(this).find('span.field-validation-error').length > 0) {
						$(this).addClass('error');
					}
				});
			}
		}
	});

	$('form').each(function() {
		// All control groups need to be decorated with 'error' class if not valid
		$(this).find('div.control-group').each(function() {
			if ($(this).find('span.field-validation-error').length > 0) {
				$(this).addClass('error');
			}
		});
	});

});
