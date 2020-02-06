var as = as || {};
// Компонент-обертка для Maplace.Js - plugin for jQuery.

as.maplace = {
    options: {
        cities: [],
        map_div: "",
        controls_title: "Выберите город",
        controls_type: "dropdown",
        view_all_text: "Все города"
    },
    maplace: undefined,
    init: function(options) {
        as.maplace.options = $.extend(as.maplace.options, options);
        this.initMaplace();
    },
    initMaplace: function() {
        var cityCollection = [];
        for (var i = 0; i < this.options.cities.length; i++) {
            cityCollection[i] = {
                title: this.options.cities[i].name,
                lat: this.options.cities[i].lat,
                lon: this.options.cities[i].lon,
                html: this.options.cities[i].html,
                icon: this.options.cities[i].icon
            }
        }
        this.maplace = new Maplace({
            locations: cityCollection,
            map_div: this.options.map_div,
            controls_title: this.options.controls_title,
            controls_type: this.options.controls_type,
            view_all_text: this.options.view_all_text
        });
    },
    show: function() {
        this.maplace.Load();
    }
}
