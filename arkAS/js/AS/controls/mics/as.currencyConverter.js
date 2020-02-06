var as = as || {};

as.currencyConverter = {
    options: {
        appendTo: 'body',
        currencies: ["RUB", "USD", "EUR", "UAH"]
    },

    init: function (options) {
        as.currencyConverter.options = $.extend(as.currencyConverter.options, options);
        var opts = as.currencyConverter.options;

        as.currencyConverter.appendView();
        as.currencyConverter.load();
    },
    appendView: function () {
        var opts = as.currencyConverter.options;
        var s = [];
        s.push("<div class='input-group'>");
        s.push("<input type='number' id='count' value='100' class='form-control' />");

        s.push("<div class='input-group-btn'>");
        s.push("<select id='currency-from' style='height: 34px;' class='btn btn-default dropdown-toggle'></select>");
        s.push("</div>");
        
        s.push("</div>");
        s.push("<br />");
        s.push("<div id='result'></div>")
        $(opts.appendTo).empty().append(s.join(""));
    },
    load: function () {
        var opts = as.currency.options;
        $.ajax({
            type: 'GET',
            dataType: 'xml',
            url: "/Currency/GetCurrency",
            success: function (data) {
                if (data) {
                    as.currencyConverter.fill(data);
                }
            },
        });
    },
    fill: function(data)
    {
        var opts = as.currencyConverter.options;
        var cur_base = "RUB";
        var cal_curs = [];

        opts.currencies.forEach(function (cur_name) {
            $("#currency-from").append("<option>" + cur_name + "</option>");
        })

        $("#currency-from").change(function (cur) {
            fillResult();
        })
        $("#count").on('input', function (e) {
            fillResult();
        });

        function fillResult() {
            var s = [];
            var cur_from = $("#currency-from").val();
            var count = $("#count").val();
            opts.currencies.forEach(function (cur_to) {
                if (cur_from != cur_to) {
                    s.push("<div class='currency-line input-group'>");
                    var value = calculate(cur_from, cur_to, count);
                    s.push("<input type='number' class='form-control' readonly value='" + value + "' /> ");
                    s.push("<span class='input-group-addon currency-title'>" + cur_to + "</span>");
                    s.push("</div>");
                }
            });
            $("#result").empty().append(s.join(""));
        }

        function calculate(cur_from, cur_to, count) {
            var rate = $.grep(cal_curs, function (e) {
                return e.from == cur_from && e.to == cur_to;
            });
            var res = (count * rate[0].value).toFixed(2);
            return res;
        }

        function loadData() {
            opts.currencies.forEach(function (cur_name) {
                if (cur_name != cur_base) {
                    var cur = $(data).find("Valute").has("CharCode:contains('" + cur_name + "')");
                    var cur_value = cur.find("Value").html().replace(",", ".");
                    var cur_nominal = cur.find("Nominal").html();
                    cal_curs.push({
                        from: cur_base,
                        to: cur_name,
                        value: 1 / cur_value * cur_nominal
                    });
                    cal_curs.push({
                        from: cur_name,
                        to: cur_base,
                        value: cur_value / cur_nominal
                    });

                    opts.currencies.forEach(function (cur_name2) {
                        if (cur_name2 != cur_base && cur_name != cur_name2) {
                            var cur2 = $(data).find("Valute").has("CharCode:contains('" + cur_name2 + "')");
                            var cur2_value = cur2.find("Value").html().replace(",", ".");
                            var cur2_nominal = cur2.find("Nominal").html();
                            cal_curs.push({
                                from: cur_name,
                                to: cur_name2,
                                value: (cur_value / cur_nominal) * (1 / cur2_value * cur2_nominal)
                            });
                        }
                    });
                }
            })
        }

        loadData();
        fillResult();
        calculate();
    }
    
};
