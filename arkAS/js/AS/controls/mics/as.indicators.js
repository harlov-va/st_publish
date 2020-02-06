var as = as || {};

as.indicators = {
    options: {
        appendTo: 'body',
        columns: 'col-md-2 col-sm-4 col-xs-6',
        indicators: [{
            title: 'Total Users',
            count: '2500',
            titleImg: 'fa fa-user',
            footer: 'From last Week',
            footerPercent: 4,
            footerColor: 'green',
            footerImg: 'fa fa-sort-asc'
        }]
    },

    init: function (options) {
        as.indicators.options = $.extend(as.indicators.options, options);
        var opts = as.indicators.options;

        var s = [];
        s.push("<div class='row tile_count'>");
        $.each(opts.indicators, function (i, item) {
            s.push("<div class='" + opts.columns + " tile_stats_count'>");
            s.push("<span class='count_top'><i class='" + item.titleImg + "'></i> " + item.title + "</span>");
            s.push("<div class='count " + item.countColor + "'>" + item.count + "</div>");
            s.push("<span class='count_bottom'>");

            s.push("<i class='" + item.footerColor + "'>");
            s.push("<i class='" + item.footerImg + "'></i>" + item.footerPercent + "% </i>" + item.footer);

            s.push("</span>");
            s.push("</div>");
        });
        s.push("</div>");

        $(opts.appendTo).append(s.join(""));
    }
};
