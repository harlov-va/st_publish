var as = as || {};

as.currency = {
    options: {
        currency: [
            { title: 'USD ЦБ', code: 'R01235', appendTo: 'body', tooltip: "" },
            { title: 'EUR ЦБ', code: 'R01239', appendTo: 'body', tooltip: "" }
        ]
    },

    init: function (options) {
        as.currency.options = $.extend(as.currency.options, options);
        var opts = as.currency.options;
        as.currency.load();
        opts.currency.forEach(function (item) {
            as.currency.loadDynamic(item);
        });
    },
    load: function () {
        var opts = as.currency.options;
        $.ajax({
            type: 'GET',
            dataType: 'xml',
            url: "/Currency/GetCurrency",
            success: function (data) {
                if (data) {
                    as.currency.viewResult(data);
                }
            },
        });
    },
    viewResult: function (data) {
        
        var opts = as.currency.options;
        opts.currency.forEach(function (item, i) {
            var s = [];
            var valute = $(data).find("Valute[ID='" + item.code + "']");
            var value = valute.find("Value").html().replace(",", ".")
            var nominal = valute.find("Nominal").html();
            var rate = (value / nominal).toFixed(4);
            s.push("<span class='currency-title'>" + item.title + "</span><span class='currency-value'>" + rate + "</span>");
            $(item.appendTo).empty().append(s.join(""));
        });
    },
    loadDynamic: function (item) {
        var opts = as.currency.options;
        $.ajax({
            type: 'GET',
            dataType: 'xml',
            url: "/Currency/GetDynamic",
            data: { code: item.code },
            success: function (data) {
                if (data) {
                    var s = [];
                    s.push("<div class='wrapper-canvas'>");
                    s.push("<canvas class='currency-canvas' width=500 height=200></canvas></div>");
                    $(item.tooltip).empty().append(s.join(""));

                    var values = as.currency.parseValues(data);
                    var canvas = $(item.tooltip).find(".currency-canvas").get(0);
                    if (canvas)
                        as.currency.drawGraph(values, $(item.tooltip).find(".currency-canvas").get(0));
                }
            },
        });
    },
    parseValues: function (data) {
        var values = [];
        var opts = as.currency.options;
        $(data).find("Record").each(function () {
            var date = $(this).attr("Date").substr(0, 2);
            var value = $(this).find("Value").html().replace(",", ".");
            var nominal = $(this).find("Nominal").html();
            values.push({ X: date, Y: value, Nominal: nominal });
        });
        return values;
    },
    drawGraph: function (values, graph) {
        var c = graph.getContext("2d");

        var nominal = values[0].Nominal;
        var yy = parseInt(Math.max.apply(null, values.map(function (a) {
            return (Math.round(a.Y) / nominal).toString().length;
        })));
        
        var xPadding = yy * 5 + 20;
        var yPadding = 50;
        var padding = 5;

        var minY;
        var maxY;
        var minX;
        var maxX;

        
        function getXPixel(val) {
            return ((graph.width - xPadding - padding) / (values.length)) * val + xPadding + 10;
        }

        function getYPixel(val) {
            var count = maxY - minY;
            return graph.height - (((graph.height - yPadding - padding - 10) / count) * val) - yPadding;
        }

        function drawValuesX() {
            c.font = 'italic 8pt sans-serif';
            c.textAlign = "center";
            for (var i = 0; i < values.length; i++) {
                c.fillText(values[i].X, getXPixel(i), graph.height - yPadding + 20);
            }
        }

        function drawValuesY() {
            c.textAlign = "right"
            c.textBaseline = "middle";

            maxY = Math.max.apply(null, values.map(function (a) {
                return a.Y;
            }));
            minY = parseInt(Math.min.apply(null, values.map(function (a) {
                return a.Y;
            })));
            maxY = Math.round(maxY);
            minY = Math.round(minY);

            var step = 0;
            for (var i = minY; i <= maxY; i++) {
                c.fillText(i / nominal , xPadding - 10, getYPixel(step));
                step++;
            }
        }

        function drawLines() {
            c.lineWidth = 1;
            c.strokeStyle = '#333';
            c.beginPath();
            c.moveTo(xPadding, padding);
            c.lineTo(xPadding, graph.height - yPadding);
            c.lineTo(graph.width - padding, graph.height - yPadding);
            c.stroke();
        }

        function drawGraph() {
            c.lineWidth = 2;
            c.strokeStyle = '#669';
            c.beginPath();
            c.moveTo(getXPixel(0), getYPixel(values[0].Y - minY));
            for (var i = 1; i < values.length; i++) {
                c.lineTo(getXPixel(i), getYPixel(values[i].Y - minY));
            }
            c.stroke();
        }

        function drawDots() {
            c.fillStyle = '#333';
            for (var i = 0; i < values.length; i++) {
                c.beginPath();
                c.arc(getXPixel(i), getYPixel(values[i].Y - minY), 2.5, 0, Math.PI * 2, true);
                c.fill();
            }
        }

        drawLines();
        drawValuesX();
        drawValuesY();
        drawGraph();
        drawDots();

        c.font = 'italic 7pt sans-serif';
        c.textAlign = "left";
        c.fillText("По данным ЦБ РФ за последние 30 дней", 10, graph.height - yPadding + 40);
    }
};
