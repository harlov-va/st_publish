var as = as || {};

as.location = {
    options: {
        ctrlCountry: "",
        ctrlCity: "",

        rCountry: "",
        rCountryId: "",
        rCity: "",
        rCityId: "",
        changeLocation: function (country, countryId, city, cityId) { },

        country_div: "",
        country_input: "",
        country_result_list: "",

        city_div: "",
        city_input: "",
        city_result_list: "",
    },

    init: function (options) {
        as.location.options = $.extend(as.location.options, options);
        var opts = as.location.options;

        $(opts.ctrlCountry).append(as.location.createInput("Выбор страны").join("")).addClass("div_country");
        $(opts.ctrlCity).append(as.location.createInput("Выбор города").join("")).addClass("div_city");

        opts.country_div = $(".div_country");
        opts.country_input = opts.country_div.find(".selector_input");
        opts.country_result_list = opts.country_div.find(".result_list");

        opts.city_div = $(".div_city");
        opts.city_input = opts.city_div.find(".selector_input");
        opts.city_result_list = opts.city_div.find(".result_list");

        as.location.cropper();
    },
    update: function () {
        var opts = as.location.options;
        opts.changeLocation(opts.rCountry, opts.rCountryId, opts.rCity, opts.rCityId);
    },
    createInput: function (title) {
        var s = [];
        s.push("<div class='input-group'>");
        s.push("<input type='text' class='form-control selector_input' placeholder='" + title + "'>");
        s.push("<span class='input-group-addon selector_dropdown'><i class='fa fa-caret-down' aria-hidden='true'></i></span>");
        s.push("</div>");

        s.push("<div class='result_list scroll-outer-border result_list_scrollable'>");
        s.push("<ul></ul>");
        s.push("</div>");

        return s;
    },
    selectItem: function(li) {
        if ($(li).parents('.div_country').length > 0) {
            as.location.selectCountry(li);
        }
        else {
            as.location.selectCity(li);
        }
    },
    selectCountry: function (li) {
        var opts = as.location.options;
        if (li && $(li).data("country")) {
            opts.rCountry = $(li).data("country");
            opts.rCountryId = $(li).data("country-id");
            opts.country_input.val(opts.rCountry);
            opts.country_result_list.hide();
        } else {
            opts.country_input.val("");
             opts.rCountry = "";
             opts.rCountryId = "";
         }
        as.location.updateCities();
        as.location.update();
    },

    selectCity: function (li) {
        var opts = as.location.options
        if (li && $(li).data("city")) {
            opts.rCity = $(li).data("city");
            opts.rCityId = $(li).data("cityId");
            opts.city_input.val(opts.rCity);
            opts.city_result_list.hide();
        } else {
            opts.city_input.val("");
            opts.rCity = "";
            opts.rCityId = "";
        }
        
        as.location.update();
    },

    updateCities: function () {
        var opts = as.location.options
        as.location.selectCity();
        if (!opts.rCountryId) {
            opts.city_result_list.find("ul").empty();
            opts.city_div.slideUp(100);
        }
        else {
            opts.city_div.slideDown(100);
            $.ajax({
                type: 'GET',
                dataType: 'jsonp',
                url: "http://api.vk.com/method/database.getCities",
                data: { v: 5.5, offset: 0, need_all: 0, count: 1000, country_id: opts.rCountryId },
                success: function (data) {
                    if (data.response) {
                        var s = [];
                        s.push("<li disabled>Выбор города</li>");
                        $.each(data.response.items, function (index, item) {
                            s.push("<li data-city=" + item.title + " data-city-id=" + item.id + ">" + item.title + "</li>");
                        })
                        opts.city_result_list.find("ul").empty().append(s.join("")).removeClass("dividing_line");
                    }
                }
            });
        }
    },
    cropper: function () {
        var opts = as.location.options
        opts.city_div.hide();

        opts.country_input.focus(function () {
            opts.country_result_list.show();
        });

        opts.city_input.focus(function () {
            opts.city_result_list.show();
        });

        opts.country_div.focusout(function () {
            setTimeout(function () { opts.country_result_list.hide() }, 200);
        })

        opts.city_div.focusout(function () {
            setTimeout(function () { opts.city_result_list.hide() }, 200);
        })

        $(document).click(function (e) {
            if ($(e.target).parents('.div_country').length == 0) {
                opts.country_result_list.hide();
            }
            if ($(e.target).parents('.div_city').length == 0) {
                opts.city_result_list.hide();
            }
        });

        opts.country_div.find(".selector_dropdown").click(function () {
            opts.country_result_list.toggle();
        })
        opts.city_div.find(".selector_dropdown").click(function () {
            opts.city_result_list.toggle();
        })

        opts.country_input.on('input', function (e) {
            var items = opts.country_result_list.find("li");
            var term = this.value.toLowerCase();
            if (this.value.length == 0) {
                items.show();
            } else {
                $.each(items, function (index, item) {
                    if ($(item).data("country") && $(item).data("country").toLowerCase().indexOf(term) >= 0)
                        $(item).show();
                    else
                        $(item).hide();
                });
            }
        });

        opts.city_input.on('input', function (e) {
            if ($(this).val() == "")
                as.location.updateCities();
        });

        $(".result_list").delegate("li", "mouseover", function () {
            $(this).parent().find("li").removeClass("active");
            $(this).addClass("active");
        });
        opts.country_result_list.delegate("li", "click", function () {
            as.location.selectCountry($(this));
        });
        opts.city_result_list.delegate("li", "click", function () {
            as.location.selectCity($(this));
        });

        $(document).keydown(function (e) {
            var list = $(".result_list:visible");
            if (list) {
                if (e.keyCode == 13) { // enter
                    var selected = list.find(".active");
                    if (selected.length > 0) {
                        as.location.selectItem(selected);
                    }
                    return false;
                }
                if (e.keyCode == 38) { // up
                    var selected = list.find(".active");
                    if (selected.length == 0) {
                        list.find("li:first").addClass("active");
                    }
                    else if (selected.prev().length > 0) {
                        list.find("li").removeClass("active");
                        selected.prev().addClass("active");
                    }
                    return false;
                }
                if (e.keyCode == 40) { // down
                    var selected = list.find(".active");
                    if (selected.length == 0) {
                        list.find("li:first").addClass("active");
                    }
                    else if (selected.next().length > 0) {
                        list.find("li").removeClass("active");
                        selected.next().addClass("active");
                    }
                    return false;
                }
            }
        });

        $.ajax({
            type: 'GET',
            dataType: 'jsonp',
            url: "http://api.vk.com/method/database.getCountries",
            data: { v: 5.5, offset: 0, need_all: 0, count: 20 },
            success: function (data) {
                if (data.response) {
                    var s = [];
                    s.push("<li disabled>Выбор страны</li>");
                    $.each(data.response.items, function (index, item) {
                        s.push("<li data-country=" + item.title + " data-country-id=" + item.id + ">" + item.title + "</li>");
                    })
                    opts.country_result_list.find("ul").empty().append(s.join(""));
                }
            }
        });

        opts.city_input.autocomplete({
            source: function (request, response) {
                if (opts.city_input.data("term") != this.term) {
                    opts.city_input.data("term", this.term);
                    var countryID = as.location.options.rCountryId;
                    $.ajax({
                        type: 'GET',
                        dataType: 'jsonp',
                        url: "http://api.vk.com/method/database.getCities",
                        data: { v: 5.5, offset: 0, need_all: 0, count: 1000, country_id: countryID, q: this.term },
                        success: function (data) {
                            if (data.response) {
                                var s = [];
                                $.each(data.response.items, function (index, item) {
                                    s.push("<li data-city=" + item.title + " data-city-id=" + item.id + ">" + item.title);
                                    s.push("<br><span>");
                                    if (item.area)
                                        s.push(item.area + ",<br>");
                                    if (item.region)
                                        s.push(item.region);
                                    s.push("</span></li>");
                                })
                                opts.city_result_list.find("ul").empty().append(s.join("")).addClass("dividing_line");
                            }
                        }

                    });
                }
            }
        });
    }
    
};
