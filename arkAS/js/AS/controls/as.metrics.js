var as = as || {};

as.metrics = {   
    options: {
        ajaxURLFormat: "/Metrics/{0}",  // get, save           
    },
    init: function (options) {
        as.metrics.options = $.extend(as.metrics.options, options);
        $(document).delegate('a.as-metrics', 'click', function (e) {
            e.preventDefault();
            as.metrics.showMetricTypes($(this));
        });
        $(document).delegate('.as-metrics-type', 'click', function (e) {
            e.preventDefault();
            as.metrics.showMetrics($(this));
        });
        $(document).delegate('.as-metrics-metric-link', 'click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var cont = btn.closest(".as-metrics-metric-item").find(".as-metrics-metric-table");
            if (btn.hasClass("collapsed")) {
                cont.show(30); btn.removeClass("collapsed");
            }
            else {
                cont.hide(30); btn.addClass("collapsed");
            }
            if (cont.text() != "" || btn.hasClass("collapsed")) return;
            var metricID = parseInt(btn.attr('data-metricID'));

            as.metrics.showMetric(cont, metricID);
        });
        $(document).delegate('.as-metrics-metric-details-link', 'click', function (e) {
            e.preventDefault();
            var par = "";
            $('.as-metrics-sel', $(this).closest("table")).removeClass("as-metrics-sel");
            var td = $(this);
            if ($(this).is("span")) {
                td = $(this).closest("td");
                par = td.attr("colName");
            }

            var details = $(this).closest('.as-metrics-metric-tbl-item').next();
            details.toggleClass('hide');
            if (!details.hasClass('hide')) {
                td.addClass('as-metrics-sel');

                var metricID = $(this).attr('data-metricID');
                var row = [];
                details.prev().find('td[colName]').each(function () {
                    row.push({ colname: $(this).attr('colName'), value: $(this).text() });
                });
                as.metrics.showMetric(details,metricID, row);
            } else {
                td.removeClass('as-metrics-sel');
            }
          
        });
        
    },


    showMetric: function (cont, metricID, row) {
        row = row || [];
     
        var params = { metricID: metricID, row: row };
        console.log(params);
        as.sys.ajaxSend(as.metrics.options.ajaxURLFormat.format("GetMetric"), params, function (data) {
            if (typeof (data) != "object") data = eval('(' + data + ')');
            if (data.result) {
                var s = [];
               
                s.push('<table data-metricID="' + metricID + '" class="table table-hover table-bordered table-striped table-condensed as-metrics-tbl">');
                s.push('<thead><tr>');
               
                $.each(data.Table.columns, function (k, col) {
                    s.push('<td ' + (col.startsWith("hide_") ? ' style="display:none;" ' : '') + ' ><b>' + col + '</b></td>');
                });
                s.push('</tr></thead>');
                s.push('<tbody>');

                $.each(data.Table.rows, function (r, row) {
                    s.push('<tr class="as-metrics-metric-tbl-item">');
                    
                    $.each(data.Table.columns, function (k, col) {
                        var value = "<span>"+ row.fields[k] +"</span>";
                        var p = as.metrics.colIsParameter(col, data.Parameters);
                        if(p){
                            value = "<a href='#' class='as-metrics-metric-details-link' data-metricID='"+p.id+"' title='"+p.title+"'>"+ row.fields[k] +"</a>";
                        }

                        s.push('<td ' + (col.startsWith("hide_") ? ' style="display:none;" ' : '') + ' colName="' + col + '" >' + value + '</td>');
                    });
                    s.push('</tr>');
                    s.push('<tr class="as-metric-details-tr hide">' +
                        '<td colspan="' + data.Table.columns.length + '">' +
                            '<div class="as-metric-details"></div>' +
                       '</td>' +
                    '</tr>');
                });
                s.push('</tbody>');
                s.push('</table>');

                if (cont.find(".as-metric-details").length > 0) {
                    cont = cont.find(".as-metric-details");
                }
                
                cont.html(s.join(""));

             
            }
        });
    },
    showMetrics: function (btn) {
        var cont = btn.closest(".panel").find(".panel-body");       
        if (cont.text() != "" || btn.hasClass("collapsed")) return;
        var typeID = parseInt(btn.attr('data-typeID'));
        var params = { typeID: typeID };
        as.sys.ajaxSend(as.metrics.options.ajaxURLFormat.format("GetMetrics"), params, function (data) {
            if (typeof (data) != "object") data = eval('(' + data + ')');
            if (data.result) {
                var s = [];
                if (data.items.length > 0) {
                    $.each(data.items, function (i, item) {
                        s.push("<div class='as-metrics-metric-item'>");
                        s.push('<h5><a class="as-metrics-metric-link collapsed" data-metricID="' + item.id + '" href="#">' + item.title + '</a></h5>');
                        s.push('<p>'+item.subtitle+'</p>');
                        s.push('<div class="as-metrics-metric-table"></div>');
                        s.push("</div>");
                    });
                } else {
                    s.push("<div class='alert alert-info'>" + as.resources.metrics_showMetrics_NotReportCurrCateg + "</div>");
                }
                cont.html(s.join(""));
            }
        });
    },   
    showMetricTypes: function (btn) {
        var code = btn.attr('data-code') || "";
        var params = { code: code };
        as.sys.ajaxSend(as.metrics.options.ajaxURLFormat.format("gettypes"), params, function (data) {
            if (typeof (data) != "object") data = eval('(' + data + ')');
            if (data.result) {
                var s = [];
                s.push("<div class='as-metric-cont'>");
                if (data.items.length > 0) {
                    $.each(data.items, function (i, item) {
                        s.push('<div class="panel panel-default">' +
                                       '<div class="panel-heading">' +
                                           '<h4 class="panel-title"><a class="as-metrics-type" data-typeID="'+item.id+'" data-toggle="collapse" data-parent="#accordion" href="#collapse' + item.id + '">' + item.name + '</a></h4>' +
                                       '</div>' +
                                       '<div id="collapse' + item.id + '" class="panel-collapse collapse">' +
                                       '<div class="panel-body"></div>');
                        s.push("</div></div>");

                    });
                    s.push("</div>");
                } else {
                    s.push("<div class='alert alert-info'>" + as.resources.metrics_showMetricTypes_NotReport + "</div>");
                }               
                as.sys.showDialog(btn.attr('data-title') || as.resources.metrics_showMetricTypes_showDialog, s.join(""), "", function () { }, true);
            } else {
                as.sys.bootstrapAlert(data.msg || as.resources.metrics_showMetricTypes_showDialogError, { type: 'danger' });
            }
        });
    },
    showDetails: function(details, par) {
        var row = [];
        details.prev().find('td[colName]').each(function () {
            row.push({  colname: $(this).attr('colName'), value: $(this).text()  });
        });

        var params = {
            row: row,
            repID: details.closest('.kvcTable').attr('repID'),
            par: par || ''
        };
       // console.log(params);
        as.sys.ajaxSend(as.metrics.options.ajax.getDetails, params, function (data) {
            if (data.d != undefined) data = data.d;
            data = eval('(' + data + ')');

            var s = [];
            if (data.Table && data.Table.rows) {
                s.push("<h5>" + data.DetailsNote + "</h5>");
                s.push('<table class="table table-hover table-bordered table-striped table-condensed">');
                s.push('<thead><tr>');
                $.each(data.Table.columns, function (k, col) {
                    s.push('<td><b>'+col+'</b></td>');
                });
                s.push('</tr></thead>');

                s.push("<tbody>");
                $.each(data.Table.rows, function (i, row) {
                    s.push('<tr class="kvcItem">');
                    $.each(data.Table.columns, function (k, col) {
                        s.push('<td colName="' + col + '">' + row.fields[k] + '</td>');
                    });
                    s.push('</tr>');
                });
                s.push("</tbody>");
                s.push('</table>');
            }          
            $('.kvcDetailsCont', details).html(s.join(""));
        });       
    },
    colIsParameter: function (colName, parameters) {
        var res = false;
        for (var i = 0; i < parameters.length; i++) {            
            if (parameters[i].parName == colName) {
                res = parameters[i];
                break;
            }
        }        
        return res;
    }
};