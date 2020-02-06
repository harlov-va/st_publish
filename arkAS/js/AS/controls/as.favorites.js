var as = as || {};

as.favorites = {
    options: {
        ajax: {
            setFavorite: "/Favorites/SetFavorite",
            delFavorite: "/Favorites/DelFavorite",
            getFavorites: "/Favorites/GetFavoritesItems"
        },
        cont: ".as-favorite",
        title: as.resources.favoritesTitle,
        toolbarAdditional: as.resources.favoritesToolbarAdditional,
        cols: [
            { code: "idFavorite", title: "ID ", tooltip: as.resources.favorites_idFavorite_tooltip, isVisible: false, isPK: true, callback: null },
            { code: "appName", title: as.resources.favorites_appName_tooltip, tooltip: as.resources.favorites_appName_tooltip, isVisible: true, isPK: false, callback: null },
            {
                code: "itemId", title: as.resources.favorites_itemId_title, tooltip: as.resources.favorites_itemId_tooltip, isVisible: true, isPK: false,
                userCallBack: function (e, callback) {
                    var params = {};
                    params["itemId"] = e.attr("itemId");
                    as.sys.ajaxSend("/Favorites/GetAdditionData", params, function (data) {
                        if (typeof (data) != "object") data = eval('(' + data + ')');
                        if (data.result) {
                            var s = [];
                            s.push("<td>" + as.resources.favorites_client + "</td>");
                            s.push("<td>"+data.item.fio + "</td>");
                            s.push("<td>" + as.resources.favorites_status + "</td>");
                            s.push("<td>" + data.item.status + "</td>");
                            if (callback) {
                                callback(s.join(""));
                            };
                        } else {
                            as.sys.bootstrapAlert(data.msg || as.resources.favorites_userCallBack_ErrMsg, { type: "danger" });
                        };
                    });
                },
                callback: function (e, col) {
                    var tr = e.closest("tr");
                    if (tr.next().hasClass("as-favorites-add")) {
                        tr.next().remove();
                    } else {
                        if (col.userCallBack) {
                            col.userCallBack(e, function (m) {
                                var s = [];
                                s.push("<tr class='as-favorites-add'>");
                                s.push(m);
                                s.push("</tr>");
                                $(s.join("")).insertAfter(tr);
                            });
                        }
                    }
                }
            },
            { code: "userGuid", title: "UserGuid", tooltip: as.resources.favorites_callback_userGuidTooltip, isVisible: false, isPK: false, loadData: "", callback: null },
            { code: "created", title: as.resources.favorites_callback_createdTitle, tooltip: as.resources.favorites_callback_createdTooltip, isVisible: true, isPK: false, loadData: "", callback: null }
        ]
    },

    init: function (options) {
        if (as.favorites.runInit) return;
        as.favorites.runInit = true;
        as.favorites.options = $.extend(as.favorites.options, options);
        as.favorites.loadFavorites();
        $(document).delegate(".as-favorites-set", 'click', function (e) {
            e.preventDefault();
            as.favorites.setFavorite($(this));
        });
        $(document).delegate(".as-favorites-del", 'click', function (e) {
            e.preventDefault();
            as.favorites.delFavorite($(this));
        });
        $(document).delegate(".as-favorites-link", 'click', function (e) {
            e.preventDefault();
            var el = $(this);
            var code = $(this).closest("td").attr('data-code');
            var stop = false;
            $.each(as.favorites.options.cols, function (i, col) {
                if (stop) return;
                if (col.code == code) {
                    if (col.callback) col.callback(el, col);
                    stop = true;
                };
            });
        });
    },

    setFavorite: function (fav) {
        var params = {};
        $.each(as.favorites.options.cols, function (i, col) {
            if (fav.attr(col.code)) {
                params[col.code] = fav.attr(col.code);
            };
        });
        as.sys.ajaxSend(as.favorites.options.ajax.setFavorite, params, function (data) {
            if (typeof (data) != "object") data = eval('(' + data + ')');
            if (data.result) {
                as.sys.bootstrapAlert(data.msg || as.resources.favorites_setFavorite_AddMsg);
            } else {
                as.sys.bootstrapAlert(data.msg || as.resources.favorites_setFavorite_ErrorMsg, { type: "danger" });
            };
        });
    },

    delFavorite: function (fav) {
        if (confirm(as.resources.favorites_delFavorite_Confirm) == false) return;
        var params = {};
        var a = fav.closest("tr");
        if (a) {
            $.each(as.favorites.options.cols, function (i, col) {
                if ((a.attr(col.code)) && (col.isPK)) {
                    params[col.code] = a.attr(col.code);
                };
            });
            as.sys.ajaxSend(as.favorites.options.ajax.delFavorite, params, function (data) {
                if (typeof (data) != "object") data = eval('(' + data + ')');
                if (data.result) {
                    as.sys.bootstrapAlert(data.msg || as.resources.favorites_delFavorite_AlertSuccess);
                    if (a.next().hasClass("as-favorites-add")) {
                        a.next().remove();
                    }
                    a.remove();
                } else {
                    as.sys.bootstrapAlert(data.msg || as.resources.favorites_delFavorite_AlertError, { type: "danger" });
                };
            });
        };
    },

    loadFavorites: function () {
        var fav = $(as.favorites.options.cont);
        var f = [];
        var fItem = [];
        if (fav.length == 0) {
            return;  //рисовать негде
        } else {
            //Рисуем шапку компоненты
            f.push("<h2>" + as.favorites.options.title + "</h2>");
            f.push("<div class='crdToolbar well well-sm'>");
            f.push(as.favorites.options.toolbarAdditional);
            f.push("</div>");
            fav.append(f.join(""));
        };
        var params;
        as.sys.ajaxSend(as.favorites.options.ajax.getFavorites, params, function (data) {
            if (typeof (data) != "object") data = eval('(' + data + ')');
            if (data.result) {
                f.length = 0;
                f.push("<table class='crdTable table table-hover table-condensed table-stripped table-bordered'>");
                f.push("<thead><tr>");
                $.each(as.favorites.options.cols, function (i, col) {
                    if (col.isVisible) {
                        f.push("<th>" + col.title + "</th>");
                    }
                });
                f.push("<th>" + as.resources.favorites_loadFavorites_Delete + "</th>")
                f.push("</tr></thead>");
                $.each(data.items, function (j, item) {
                    fItem.length = 0;
                    f.push("<tr");
                    $.each(as.favorites.options.cols, function (i, col) {
                        if (item[col.code]) {
                            if (col.isVisible) {
                                if (col.callback) {
                                    fItem.push("<td data-code='" + col.code + "'><a class='as-favorites-link' " + col.code + "='" + item[col.code] + "'  title='" + col.tooltip + "' href='#'>" + item[col.code] + "</a></td>");
                                } else {
                                    fItem.push("<td data-code='" + col.code + "' title='" + col.tooltip + "'>" + item[col.code] + "</td>");
                                }
                            };
                            if (col.isPK) {
                                f.push(" " + col.code + "='" + item[col.code] + "' ");
                            };
                        } else {
                            fItem.push("<td></td>");
                        };
                    });
                    f.push(">");
                    f.push(fItem.join(""));
                    f.push("<td><button type='button' class='btn btn-link as-favorites-del' title='" + as.resources.favorites_loadFavorites_Title + "'><span class='glyphicon glyphicon-remove'></span></button></td>");
                    f.push("</tr>");
                });
                f.push("</table>");
                fav.append(f.join(""));
            } else {
                as.sys.bootstrapAlert(data.msg || as.resources.favorites_loadFavorites_ALlertError, { type: "danger" });
            };
        });
    }
};