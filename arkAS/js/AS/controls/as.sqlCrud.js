var as = as || {};

as.sqlCrud = {
    init: function () {
        var f = 1;
        $(document).delegate("th", "click", function () {
            var code = $(".as-sqlCrud").attr("data-code");
            var english = /^[A-Za-z0-9]*$/;
            if (!english.test(code)) {
                as.sys.bootstrapAlert("Такой код не существует.Возможно вы сменили раскладку клавиатуры", { type: "success" });
            } else {
                f *= -1;
                as.sqlCrud.showSqlCrud($(".as-sqlCrud"), code, parseInt($(".as-sqlCrud").attr("data-paging")), 1, $(this).index(), f);

            }
        });
        $(".as-sqlCrud").on("click", function () {
            var code = $(".as-sqlCrud").attr("data-code");
            var english = /^[A-Za-z0-9]*$/;
            if (!english.test(code)) {
                as.sys.bootstrapAlert("Такой код не существует.Возможно вы сменили раскладку клавиатуры", { type: "success" });
            } else {
                as.sqlCrud.showSqlCrud($(".as-sqlCrud"), code, parseInt($(".as-sqlCrud").attr("data-paging")), 1, 0, 1);
            }

        });
        
    },
    getsort: function () {

        var table = $(this).parents("table");
        var rows = table.find("tr:gt(0)").toArray().sort(as.sqlCrud.compar($(this).index()));
        this.asc = !this.asc;
        if (!this.asc) { rows = rows.reverse(); }

        $.each(rows, function (i) { table.append(rows[i]); });
    },
    compar: function comparer(index) {
        return function (a, b) {
            var valA = as.sqlCrud.getval(a, index),
                valB = as.sqlCrud.getval(b, index);
            return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.localeCompare(valB);
        }
    },
    getval: function getCellValue(row, index) { return row[index] },
    showSqlCrud: function (cont, code, pageSize, page, sorter, f) {
        var params = { code: code, pageSize: pageSize, page: page };
        as.sys.ajaxSend("/Demo/GetSqlCommand", params, function (data) {
            if (data.hasOwnProperty("Table")) {
                var s = [];
                s.push("<table id=\"tblsql\" class=\"table table-hover table-bordered table-condensed\"><thead><tr>");
                $.each(data.Table.columns, function (k, col) {
                    s.push("<th><a class='crdSort'>" + col + "</a></th>");
                });
                s.push("</tr></thead><tbody>");
                var rows = data.Table.fullRows.sort(as.sqlCrud.compar(sorter));
                var edc = $("#tblsql th")[sorter];
                if (!(typeof edc === "undefined")) {
                    if (f == 1) { rows = rows.reverse(); }
                }
                var ddd = data.Table.fullRows.length;
                var start = (pageSize * (page - 1));
                var end = start + pageSize;
                rows = rows.slice(start, end);
                $.each(rows, function (r, row) {
                    s.push("<tr>");
                    $.each(data.Table.columns, function (rr) {
                        s.push("<td>" + row[rr] + "</td>");
                    });
                    s.push("</tr>");
                });
                s.push("</tbody></table>");
                s.push("<div id=\"pagingwrapper\" class=\"pagingwrapper\"><div id=\"pagingcontainer\" class=\"pagingcontainer\">&nbsp;</div></div>");
                if (cont.find(".as-sqlCrud-details").length > 0) {
                    cont = cont.find(".as-sqlCrud-details");
                }
                cont.html(s.join(""));
                as.sqlCrud.setPaging($(this), code, page, parseInt(pageSize), ddd, sorter, f);
            } else {
                if (data.S == "sql") {
                    as.sys.bootstrapAlert("Скрипт имеет неверный формат", { type: "success" });
                } else {
                    as.sys.bootstrapAlert("Вы не имеете прав для запуска скрипта", { type: "success" });
                }
            }
        });

    },
    setPaging: function (cont, code, page, pageSize, total, sorter, f) {

        function initPagination() {
            $("#pagingcontainer").pagination(total, {
                items_per_page: parseInt(pageSize),
                callback: function (page_index, pagination_container) {
                    as.crud.options.page = (parseInt(page_index) + 1);
                    cont.html("");
                    as.sqlCrud.showSqlCrud($(".as-sqlCrud"), code, pageSize, as.crud.options.page, sorter, f);
                },
                prev_text: as.resources.crud_initPagination_prev_text,
                next_text: as.resources.crud_initPagination_next_text,
                num_display_entries: 10,
                num_edge_entries: 1,
                current_page: page - 1
            });
            var pagingcontainerwidth = 252 + 20 * total / pageSize;
            $("#pagingcontainer").css("width", "" + pagingcontainerwidth + "px");
        }
        as.sys.loadLib("/JS/pagination/jquery.pagination.js", "/JS/pagination/pagination.css", $("#pagingcontainer").pagination, initPagination);

    }

};