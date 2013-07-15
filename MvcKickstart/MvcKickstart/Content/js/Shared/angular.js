// @reference ~/content/js/_lib/angularjs
angular.addDirectives = {
	select2: function (app){
		app.directive('select2', function() {
			return function(scope, element, attrs) {
				var model = attrs.ngModel;
				$(element).select2().on('change', function(e) {
					if (scope.isLoadingData == undefined || !scope.isLoadingData) {
						if(Object.prototype.toString.call(e.val) === '[object Array]') {
							scope.$apply(model + '=[' + e.val.toString() + ']');
						}else {
							scope.$apply(model + '=' + e.val);
						}
					}
				});
		
				scope.$watch(model,function(value) {
					$(element).select2('val', value);
					if (scope.setWatchValue != undefined) {
						scope.setWatchValue(model, value);
					}
				});
			};
		});
	},
	slider: function (app) {
		app.directive('slider', function($location, $parse) {
			return {
				restrict: 'A',
				replace: true,
				template: '<div class="slider">' +
						  '		<div class="slider-control"></div>' +
						  '		<span class="slider-value label"></span>' +
						  '</div>',
				link: function(scope, element, attrs) {
					var model = attrs.ngModel;
					var step = Number(coalesce(attrs.step, 50));
					var minimum = Number(coalesce(attrs.min, 50));
					var maximum = Number(coalesce(attrs.max, 1000));

					//round min/max to nearest multiple of step (otherwise slider won't work)
					maximum = step * Math.ceil(maximum / step);
					minimum = step * Math.floor(minimum / step);

					// Get the current value on the model or the maximum to use as the initial slider value
					var value = coalesce($parse(attrs.ngModel)(scope), maximum);
					$('.slider-control', element).slider({
						min: minimum,
						max: maximum,
						value: value,
						range: 'min',
						orientation: "horizontal",
						step: step,
						slide: function(event, ui) {
							$(this).siblings('.slider-value').text(ui.value);
						},
						change: function(event, ui) {
							// The change occured based on user input from mouse or keyboard, so bind the model
							if (event.originalEvent) {
								scope.$apply(model + '=' + ui.value);
							}
						}
					});

					scope.$watch(attrs.ngModel, function(proposedValue) {
						var v = coalesce(proposedValue, maximum);
						$('.slider-control', element).slider('value', v);
						$('.slider-value', element).text(v);
					});

					$('.slider-value', element).text(value);
				}
			};
		});
	},
	popover: function (app) {
		app.directive('popover', function() {
			return {
				restrict: 'A',
				link: function(scope, element, attrs) {
					var deferredShowRequest = null;
					element.hover(
						function (event) {
							if (element.attr('data-content') == null) {
								var trigger = this;
								deferredShowRequest = $.get(this.href, function(data) {
									$(trigger).popover(scope.preview ? 'enable' : 'disable');
									$(trigger).attr('data-content', data);
									$(trigger).popover('show');
									deferredShowRequest = null;
								});
							} else {
								element.popover('show');
							}
						},
						function (event) {
							if (deferredShowRequest != null) {
								deferredShowRequest.abort();
								return;
							}
							element.popover('hide');
						}
					);

					scope.$watch('preview', function(preview) {
						if (element.attr('data-content')) {
							if (preview) {
								element.popover('enable');
							} else {
								element.popover('disable');
							}
						}
					});
				}
			};
		});
	}
};

angular.addFilters = {
	dictionary: function (app) {
		app.filter('dictionary', function() {
			return function(key, categories) {
				if (categories == null) {
					return key;
				}
				for (var i = 0; i < categories.length; i++) {
					var item = categories[i];
					if (item.value == key) {
						return item.text;
					}
				}
				return key;
			};
		});
	}
}