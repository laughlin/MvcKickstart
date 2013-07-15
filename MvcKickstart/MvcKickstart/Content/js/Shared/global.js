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

if (window['coalesce'] == undefined) {
	window.coalesce = function() {
		for (var i = 0; i < arguments.length; i++) {
			if (null !== arguments[i] && arguments[i] !== undefined) {
				return arguments[i];
			}
		}

		return null; // No non-null values found, return null
	};
	if (window['getValueOrDefault'] == undefined) {
		window.getValueOrDefault = window.coalesce.prototype;
	}
}

Array.prototype.findFirst = function (predicateCallback) {
    if (typeof predicateCallback !== 'function') {
        return undefined;
    }

    for (var i = 0; i < this.length; i++) {
        if (i in this && predicateCallback(this[i])) return this[i];
    }

    return undefined;
};

String.prototype.trim = function () {
	return this.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
};
if (typeof String.prototype.startsWith != 'function') {
	String.prototype.startsWith = function (str){
		return this.slice(0, str.length) == str;
	};
}

// ============================================================
// Bootstrap and MVC integration
// ============================================================

$(function() {
	// Enable tab behavior on all lists marked with the 'tabs-controller' class
	$('.tabs-controller a:first').tab('show');
	$('.tabs-controller a').click(function(e) {
		e.preventDefault();
		$(this).tab('show');
	});

	$('span.field-validation-valid, span.field-validation-error').each(function() {
		// All validation message should be decorated with 'help-inline' class for bootstrap styling
		$(this).addClass('help-inline');
	});

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

	// Allow any link or button to trigger a modal confirmation dialog with a simple data-confirm attribute
	$('a[data-confirm], button[data-confirm]').on('click', function(event) {
		var self = $(this);
		if (self.data('confirm-bypass')) {
			return true;
		}
		if ($('#dataConfirmModal').length == 0) {
			$('body').append('' +
				'<div id="dataConfirmModal" class="modal" role="dialog" aria-labelledby="dataConfirmLabel" aria-hidden="true">' +
				'	<div class="modal-header">' +
				'		<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>' +
				'		<h3 id="dataConfirmLabel">Please Confirm</h3>' +
				'	</div>' +
				'	<div class="modal-body"></div>' +
				'	<div class="modal-footer">' +
				'		<button class="btn" data-dismiss="modal" aria-hidden="true">Cancel</button>' +
				'		<a class="btn btn-primary" data-proceed="modal">OK</a>' +
				'	</div>' +
				'</div>');
		}
		var container = $('#dataConfirmModal');
		$('[data-proceed]', container).click(function() {
			self.data('confirm-bypass', true);
			self.trigger('click');
			self.data('confirm-bypass', false);
		});
		container.find('.modal-body').text($(this).attr('data-confirm'));
		container.modal({ show: true });

		event.preventDefault();
		return false;
	});
	
	// Allow for ajax content to be defined within the bootstrap modal system
	var anonymousModalTargets = 0;
	$('a[data-toggle=modal-ajax]').on('click', function(event) {
		var self = $(this);
		var target = self.attr('data-target');
		if (target == null) {
			target = '__modalTarget' + (++anonymousModalTargets);
			self.attr('data-target', target);
		}

		var container = $('#' + target);
		if (container.length == 0) {
			container = $('<div class="modal fade" id="' + target + '"></div>').appendTo(document);
		}

		container.load(self.attr('href'), function() {
			container.modal('toggle');
			
			// Find any angular applications and bootstrap them
			if (window['angular']) {
				container.on('shown', function() {
					$('[ng-app]', container).each(function() {
						var modules = $(this).attr('ng-app').split(' ');
						angular.bootstrap(this, modules);
					});
				});
			}
		});
		
		event.preventDefault();
		return false;
	});
});
