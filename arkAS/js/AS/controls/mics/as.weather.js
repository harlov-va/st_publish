var as = as || {};

as.weather = {
    options: {
        appendTo: 'body',
    },

    init: function (options) {
        as.weather.options = $.extend(as.weather.options, options);
    },
    loadByLocation: function (longitude, latitude) {
        var opts = as.weather.options;
        $.ajax({
            type: 'GET',
            dataType: 'json',
            url: "/Weather/GetWeatherByLocation",
            data: { lon: longitude, lat: latitude },
            success: function (data) {
                if (data.result) {
                    as.weather.viewResult(data.result);
                }
            },
        });
    },
    loadByCity: function (city) {
       
        $.ajax({
            type: 'GET',
            dataType: 'json',
            url: "/Weather/GetWeatherByCity",
            data: { city: city },
            success: function (data) {
                if (data.result) {
                    as.weather.viewResult(data.result);
                }
            },
        });
    },
    viewResult: function (result) {
        var obj = JSON.parse(result);
        var s = [];
        s.push("<div>" + obj.name + ", " + obj.sys.country + "</div>");
        s.push("<div><img src='http://openweathermap.org/img/w/" + obj.weather[0].icon + ".png' title='" + obj.weather[0].description + "'>");
        s.push(obj.main.temp + " °C</div>");
        $(as.weather.options.appendTo).empty().append(s.join(""));
    },
    getLocation: function(result) {
        $.ajax({
            url: '//freegeoip.net/json/',
            type: 'POST',
            dataType: 'jsonp',
            success: function (location) {
                result(location);
            }
        });
    }
};
