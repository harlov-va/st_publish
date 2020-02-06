var as = as || {};

as.graphic = {
    options: {
        type: null,
        method: null,
    },

    init: function (options) {

        as.graphic.options = $.extend(as.graphic.options, options);

        switch (as.graphic.options.type) {
            case "as-graphic": as.graphic.showGraphic('as-graphic', 'StatisticsByWeeks'); break;
            case "as-funnel": as.graphic.showGraphic('as-funnel', 'GraphFunnel'); break;
            case "as-graphic-pie": as.graphic.showGraphic('as-graphic-pie', 'GraphFunnel'); break;
        }

    },

    getData: function (method, callback) {
        as.sys.ajaxSend("/Graphic/" + method, {}, function (data) {
            if (typeof (data) != "object") data = eval('(' + data + ')');
            if (typeof callback !== 'undefined') {
                callback(data);
            }
        });
    },

    showGraphic: function (type, method) {
        switch (type) {
            case 'as-graphic':
                as.graphic.getData(method, function (data) {
                    var graphData = [];

                    $.each(data.item, function (object) {
                        var week = this.shift();
                        var cnt = this.shift();
                        graphData.push([week.Value, cnt.Value]);
                    });

                    as.graphic.addGraphic(type,
                    [
                        { label: "Кол-во писем", color: "#00A8F0", data: graphData, "mouse": { "track": true, trackFormatter: function (a) { return "Кол-во писем (" + a.x + " , " + a.y + ")" }, relative: true } },
                    ]);
                });
                break;

            case 'as-funnel':
                as.graphic.getData(method, function (data) {
                    as.graphic.addFunnel(type,
                    [
                        {
                            name: "Воронка",
                            dataLabels: {
                                enabled: true,
                                format: "<b>{point.name}</b><br>({point.y:,.0f})",
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || "black"
                            },
                            data:
                            [
                                ['Визиты на сайт', parseInt(data.item.visits)],
                                ['Скачивание', data.item.downloads],
                                ['Запрос прайса', data.item.querys],
                                ['Оформление чека', data.item.cheques],
                                ['Оплата', data.item.pays]
                            ]
                        }
                    ]);
                });
                break;

            case 'as-graphic-pie':
                as.graphic.getData(method, function (data) {
                    as.graphic.addGraphic(type,
                        [
                            { label: "Visits site", color: "red", data: [[0, data.item.visits]] },
                            { label: "Downloadings", data: [[0, data.item.downloads]] },
                            {
                                label: "Queries",
                                data: [[0, data.item.querys]],
                                pie: { explode: 15 }
                            },
                            { label: "Paid", color: "#AAA", data: [[0, data.item.pays]] }
                        ],
                        {
                            pie: true,
                            mouse: { track: true }
                        }
                    );

                });
                break;
        }

        $('.graphic').hide();
        $('.graphic[id="' + type + '"]').show();
    },

    addGraphic: function (element, data, params) {
        var getType = {};
        var dataVal = null;

        function getDataValue(d, start, t) {
            var dval = null;
            if (d && getType.toString.call(d) === '[object Function]') {
                if (params['animate']) {
                    if (start != undefined && t != undefined) {
                        dval = d(start, t);
                    } else return [];
                } else
                    dval = d();
            } else {
                if (!Array.isArray(d)) {
                    dval = [d];
                } else {
                    dval = d;
                }
            }
            return dval;
        }

        dataVal = getDataValue(data);

        if (typeof element === "string") {
            element = document.getElementById(element);
        }

        var el = $(element);
        var width = el.attr("as-graphic-width");
        if (width) {
            el.css("width", width + "px");
        }
        var height = el.attr("as-graphic-height");
        if (height) {
            el.css("height", height + "px");
        }

        var pie;
        params = params ? params : {};
        if (params['pie']) {
            if (params.pie == true)
                params.pie = {};
            pie = $.extend(params.pie, { show: true, explode: 2 });
        }

        var grid = $.extend(pie ? { verticalLines: false, horizontalLines: false } : {}, params.grid);

        params.legend = params.legend ? params.legend : {};
        params.xaxis = params.xaxis ? params.xaxis : {};
        params.yaxis = params.yaxis ? params.yaxis : {};
        params.points = params.points ? params.points : {};
        params.lines = params.lines ? params.lines : {};

        var drawParameters = $.extend(params,
        {
            title: el.attr("as-graphic-title"),
            subtitle: el.attr("as-graphic-subtitle"),
            legend: $.extend(true, { show: el.attr("as-graphic-showlegent") || "true" }, params.legend),
            xaxis: $.extend(true, { showLabels: pie ? false : true, title: el.attr("as-graphic-xtitle") }, params.xaxis),
            yaxis: $.extend(true, { showLabels: pie ? false : true, title: el.attr("as-graphic-ytitle") }, params.yaxis),
            points: $.extend(true, { show: pie ? false : true, radius: 2, fill: false }, params.points),
            lines: $.extend(true, { show: pie ? false : true }, params.lines),
            pie: pie,
        });

        if (params['animate']) {
            var period = params.animate.period ? params.animate.period : 50;
            var start = 0; //как вариант: (new Date).getTime();
            function animate(t) {
                dataVal = getDataValue(data, start, t);

                Flotr.draw(element, dataVal, drawParameters);
                setTimeout(function () {
                    animate(t + period / 1000);
                }, period);
            }

            animate(start);
        } else {
            Flotr.draw(element, dataVal, drawParameters);
        }
    },

    addFunnel: function (element, data) {
        if (!Array.isArray(data)) {
            data = [data];
        }

        if (typeof element === "string") {
            element = document.getElementById(element);
        }

        var el = $(element);
        var width = el.attr("as-graphic-width");
        if (width) {
            el.css("width", width + "px");
        }

        var height = el.attr("as-graphic-height");
        if (height) {
            el.css("height", height + "px");
        }

        el.highcharts({
            chart: {
                type: "funnel",
                backgroundColor: el.attr("as-graphic-backgroundcolor"),
                borderColor: el.attr("as-graphic-bordercolor"),
                marginRight: el.attr("as-graphic-marginright"),
                marginBottom: el.attr("as-graphic-marginbottom"),
                marginTop: el.attr("as-graphic-margintop"),
                marginLeft: el.attr("as-graphic-marginleft"),
            },
            title: {
                text: el.attr("as-graphic-title")
            },
            subtitle: {
                text: el.attr("as-graphic-subtitle")
            },
            series: data
        });
    },
};