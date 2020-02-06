var as = as || {};

as.timeline = {
	options: {
		appendTo: 'body',
	},
	init: function (options) {
		as.timeline.options = $.extend(as.timeline.options, options);
		var opts = as.timeline.options;
		as.timeline.viewResult();

	},
	viewResult: function () {
	    var date = [new Date(2016, 7, 25), new Date(2016, 7, 20), new Date(2016, 9, 25), new Date(2016, 11, 1), new Date(2017, 11, 1)];
	    var month = ['Январь','Февраль','Март','Апрель','Май','Июнь','Июль','Август','Сентябрь','Октябрь','Ноябрь','Декабрь'];
	    var s = "";
	    var margin_bottom = 0;
	    if (date.length != 0) {
	        s += '<ul class="timeline">';
	        for (var i = 0; i < date.length; i++) {
	            margin_bottom = date[i + 1] - date[i];
	            margin_bottom = (((margin_bottom / 1000) / 60) / 60) / 24;
	            s += '<li><div class="timline-circl"></div>';
	            s += '<div class="timeline-badge"><p>' + date[i].getFullYear() + '</p><p class="month">' + month[date[i].getMonth()] + '</p><p class="Day">' + date[i].getDate() + '</p></div>';
	            s += '<div class="timeline-panel" style ="margin-bottom:' + margin_bottom + 'px"><div class="timeline-heading">';
	            s += '<h4 class="timeline-title">' + "Mussum ipsum cacilds" + '</h4>';
	            s += '</div><div class="timeline-body"><p>' + "Mussum ipsum cacilds, vidis litro abertis." + '</p> </div></li>';
	        }
	        s += '</ul>';
	        $(as.timeline.options.appendTo).empty().append(s);
	    }
	    else {
	        s += '<div class="well elementEmpty">' + "Нет данных" + '</div>'
	        $(as.timeline.options.appendTo).empty().append(s);
	    }
	}
}