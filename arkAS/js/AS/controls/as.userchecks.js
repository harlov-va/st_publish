var as = as || {};

as.userchecks =
{
    options: {
        text: "Hello world!!!",
        containerClass: null
    },
    init: function (options) {

        as.userchecks.options = $.extend(as.userchecks.options, options);

        if (as.userchecks.options.containerClass) {
            as.userchecks.ShowUserChecks();
        }

    },
    getSelectParams: function () {
        var res = {
            page: 1,
            pageSize: 10,
            sort: '',
            direction: '',
            mode: '',
            filter: ''

        };
        return res;
    },
    ShowUserChecks: function () {
        var container = $(as.userchecks.options.containerClass);
        var params = as.userchecks.getSelectParams();

        var s = [];

        if (container) {

            as.sys.ajaxSend("/Admin/UserChecks_getItems", params, function (data) {
                if (typeof (data) != "object") data = eval('(' + data + ')');

                if (data.msg) {
                    as.sys.showMessage(data.msg);
                }
                if (data.url) {
                    if (data.url.trim() != "#") {
                        location.href = data.url;
                    }
                    return;
                }

                if (data.items.length > 0) {

                    var columns = data.columns;

                    //Header
                    s.push("<table class='crdTable table table-hover table-condensed table-stripped table-bordered'>");
                    s.push("<thead><tr>");

                    s.push("<th><b>#</b></th>");
                    for (var i = 0; i < columns; i++) {
                        s.push("<th><b>" + data.items[i].nameCheckItem + "</b></br><a href='#' data-checkid='" + data.items[i].idCheckItem + "' class='usCloseColumn btn btn-default btn-sm'>" + as.resources.userchecks_ShowUserChecks_close + "</a></th>");
                    }
                    s.push("</tr></thead>");

                    var curColumn = 0;
                    var curRow = 1;

                    //Body
                    s.push("<tbody>");
                    $.each(data.items, function (i, item) {


                        if (curColumn == 0) {
                            s.push("<tr><td><b>" + item.nameUser + "</b></td>");
                            curColumn++;
                        }

                        s.push(as.userchecks.getCellHTML(item.idCheckItem, item.nameUser, item.userHasRights, item.isSended, item.modified, item.isClosed));

                        curColumn++;
                        if (curColumn > columns) {
                            curColumn = 0;
                            curRow++;
                            s.push("</tr>");
                        }
                    });
                    s.push("</tbody>");
                    s.push("</table>");

                    container.html(s.join(""));
                }

            });

        }

    },
    getCellHTML: function (idCheckItem, nameUser, userHasRights, isSended, modified, isClosed) {
        var s = [];

        if (userHasRights == true) {

            var curClass = "";

            if (isSended == true) {
                curClass = "warning";
            }
            if (isClosed == true) {
                curClass = "success";
            }

            s.push("<td class = '" + curClass + "'>");

            if (isSended == true) {
                s.push("<span class='usModified'>" + modified + "</span></br>");
            }

            s.push("<label><input type='checkbox' " + (isClosed == true ? 'checked' : '') + " data-username='" + nameUser + "'  data-checkid='" + idCheckItem + "' class='usClosed'>" + as.resources.userchecks_getCellHTML_closed + "</label></br>");
            s.push("<a href='#' data-username='" + nameUser + "'  data-checkid='" + idCheckItem + "' class='usSendMail btn btn-default btn-sm'>" + as.resources.userchecks_getCellHTML_send + "</a>");

            s.push("</td>");
        } else {
            s.push("<td></td>");
        }

        return s.join("");
    }

}