
var as = as || {};
as.jubilee = {
	options: {
	    dayOfHundredYears: 36400,
        days : 500
	},

	init: function (options) {
	    as.jubilee.options = $.extend(as.jubilee.options, options);
	    var formatter = new Intl.DateTimeFormat("ru", {
	        year: "numeric",
	        month: "long",
	        day: "numeric"
	    });
	    $(document).delegate(".asBtnJubilee", "click", function () {
		//$('.as-jubilee').delegate(".asBtnJubilee", "click", function () {
		    var dateOfBirthStr = $('.asDateOfBirth').val(); 
		    var dateOfBirthArr = dateOfBirthStr.split('.'); 
		    var dateOfBirth = new Date(dateOfBirthArr[2], +dateOfBirthArr[1] - 1, +dateOfBirthArr[0]);

		    var todayDate = new Date();
		    var counter=0;
			
			if (as.jubilee.validateDate()) {
				
				var s = [];
				s.push("<div class ='container'>");
				s.push("<div class ='row' style=''><div class ='col-md-8 col-xs-12 col-sm-10'  style='background-color: azure;font-family:italic;padding-top: 15px;'><span class='daysOfJubilee'>"
                 + "Даты 500-дневныx юбилеев" + "</span></div></div>");
				
				        var dateOfNextAnyversary = new Date(dateOfBirth);
				        var today = new Date(todayDate); var today1 = new Date(todayDate);
				        var minDate = new Date(today.setDate(todayDate.getDate() - 1500));
				        var maxDate = new Date(today1.setDate(todayDate.getDate() + 1500));
				       
				        s.push("<div class ='row text-center' style=''><div class ='col-md-4 col-xs-12 col-sm-6 text-center'  style='background-color: azure;text-align:center;font-size:18px;border-top: 0px;padding-bottom: 15px;    padding-left: 15px;' >" +
                        "<img class='pull-left   img-responsive'  src='/Content/images/controls/jubilee2.jpg' alt='photo' width='auto'>" + "</div><div class ='col-md-4 col-xs-12 col-sm-4'style='background-color: azure;border-top: 0px;border-left: 0px;' >");
				        for (var i = 0; i <= as.jubilee.options.dayOfHundredYears; i += as.jubilee.options.days) {
				            var date = new Date(dateOfNextAnyversary.setDate(dateOfNextAnyversary.getDate() + as.jubilee.options.days));
				           		if (date < maxDate && date > minDate) {		
				           		    var counterDays = as.jubilee.options.days + i;
				           		    res = formatter.format(date);
				           		    str = counterDays.toString();
				           		    s.push("<div class ='row' style=''><div class ='col-md-12 col-xs-12 col-sm-12' style='background-color: azure;border-top: 0px; text-align: left;padding-bottom: 15px;' ><span class ='resultText'>" + as.jubilee.number_format(str) + " дней  - " + res + "</span></div></div>");
				              
				            }
				        }
				        s.push("</div></div>");
                                
				s.push("</div>");
				$('.table').html(s.join(""));
			}
		});
	},
	validateDate: function () {
		var valid = true;
		var date = $('.asDateOfBirth');
		if (date.attr('required') && !date.val()) {
			as.sys.bootstrapAlert("Выберите дату <b>" + date.prev().text() + "</b>", { type: "warning" });
			date.focus();
			valid = false;
		}
		return valid;
	},

	number_format: function ( str ){
   return str.replace(/(\s)+/g, '').replace(/(\d{1,3})(?=(?:\d{3})+$)/g, '$1 ');
}
};