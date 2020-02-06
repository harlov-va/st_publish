var as = as || {};


as.text = {
    curElement: {},
    ajax:
    {
        UrlSave: "/ArkStuff/ArkImgSave/",
        UrlExist: "/ArkStuff/ArkImgExist/"
    },
    width: "auto",
    height: "200px",

    init: function () {
        if ($("div.as-text").length > 0) {

            as.text.showTexts();
            as.text.delegateCanEdit('.as-text-canEdit', 'as-text-click');

            as.text.delegateClickEdit('a.as-text-click', "div.as-text-canEdit");

            as.fe.options.ajax.fileEditor_Caption = "Caption";
            as.fe.options.ajax.loadFileEditor = "/ArkStuff/loadFileEditor";
            as.fe.options.ajax.loadDir = "/ArkStuff/loadDir";
            as.fe.options.ajax.uploadFiles = "/Handlers/feUploadFilesAsArkImage.ashx";
            as.fe.options.ajax.createDir = "/ArkStuff/createDir";
            as.fe.options.ajax.deleteFile = "/ArkStuff/deleteFile";
            as.fe.options.ajax.renameFile = "/ArkStuff/renameFile";
            as.fe.options.ajax.renameDir = "/ArkStuff/renameDir";
            as.fe.options.ajax.deleteDir = "/ArkStuff/deleteDir";
            as.fe.options.ajax.getTextFile = "/ArkStuff/getTextFile";
            as.fe.options.ajax.saveTextFile = "/ArkStuff/saveTextFile";
            as.fe.init();
        }
    },

    delegateCanEdit: function (canEdit, asClick) {
        $(document).delegate(canEdit, 'mouseenter mouseleave', function (event) {
            if (event.type == 'mouseenter') {
                var Sel = $(this);
                if (Sel.find("div.as-hover-toolbar").length == 0) {
                    Sel.append("<div class='as-hover-toolbar'></div>");

                }
                Sel.find("div.as-hover-toolbar").append("<div class='as-edit'>" +
                                                    "<a href='#' class=" + asClick + "><i class='glyphicon glyphicon-edit' title='" + as.resources.ark_image_init_title + "' alt='" + as.resources.ark_image_init_alt + "' ></i></a>" +
                                                    "</div>");
            }
            else {
                $(this).find("div.as-hover-toolbar").remove();
            }
        });
    },

    delegateClickEdit: function (click, divParent) {
        $(document).delegate(click, 'click', function (e) {
            e.preventDefault();
            as.text.curElement = $(this).parent("div.as-edit").parent("div.as-hover-toolbar").parent(divParent);
            if (as.text.curElement.attr("data-mode") == "image")
                as.text.editImage();
            else as.text.editText();
        });
    },

    showTexts: function () {
        var codes = [];
        $('.as-text').each(function () {
            if ($(this).attr('data-seotext') != "1") {
                var code = $(this).attr('data-code');
                if (code) codes.push(code);
            }
        });
        as.sys.ajaxSend("/ArkStuff/GetTexts", { codes: codes }, function (data) {
            $.each(data.items, function (i, item) {
                var el = $('.as-text[data-code=' + item.code + ']');
                el.html(item.text);
            });
            if (data.canEdit) $('.as-text').addClass('as-text-canEdit');
        });
    },

    editImage: function () {
        if (as.text.curElement.attr("data-mode") == "image" && as.text.curElement.attr("data-mode") != "undefined") {
            as.text.curElement.find("div.as-hover-toolbar").remove();
            as.fe.showFileExplorerDialog("Менеджер изображений", "image/jpeg, image/png, image/gif", "Выбрать", $('#asModal'), function (url, curElementData) {
                var alt = $('input.feAlt').val();
                if (alt == "" || alt == "undefined") {
                    alt = curElementData.attr('data-name');
                }
                var NameCodeDiv = "";
                var dc = as.text.curElement.attr("data-code");
                as.sys.ajaxSend(as.text.ajax.UrlExist, { codeDiv: dc }, function (dataIs) {
                    if (dataIs.STATUS == "NotExist") {
                        NameCodeDiv = prompt('Введите название блока', dc);
                    }
                    if (dataIs.STATUS == "ItExists" || dataIs.STATUS == "NotExist") {
                        as.sys.ajaxSend(as.text.ajax.UrlSave, { ImgText: "|||" + url + "|||" + alt, codeDiv: dc, NameDiv: NameCodeDiv }, function (data) {

                            if (data.STATUS == "OK") {
                                as.text.curElement.html("<img id='" + dc + "' src='" + url + "' alt='" + alt + "' width='" + as.text.width + "' height='" + as.text.height + "'>");
                            }
                        }, false, false);
                    }

                }, false, false);
            });
        }
    },
    editText: function (btn) {
        var s = [];
        var t = as.text.curElement;
        as.text.curElement.find("div.as-hover-toolbar", t).remove();

        var code = t.attr('data-code');
        var text = t.html();
        s.push("<textarea class='as-html as-text-text' rows='15' cols='120' style='max-width: 500px;'>" + text + "</textarea>");


        function CloseText() {
            as.sys.closeDialog();
            $("div.modal-body").html("");
        }
        as.sys.showDialog(as.resources.ark_editText_showDialogEdit, s.join(""), as.resources.ark_editText_showDialogSave, function () {

            var mce1 = tinyMCE.get($('textarea.as-text-text').attr('id'));
            var text1 = mce1.getContent ? mce1.getContent() : $('textarea.as-text-text').val();
            var params = { text: text1, code: code };

            as.sys.ajaxSend("/ArkStuff/SaveText", params, function (data) {
                if (typeof (data) != "object") data = eval('(' + data + ')');
                if (data.result) {
                    as.sys.bootstrapAlert(as.resources.ark_editText_AlertSuccess, { type: 'success' });
                    t.html(text1);
                } else {
                    as.sys.bootstrapAlert(data.msg || as.resources.ark_editText_AlertError, { type: 'danger' });
                }
            });
            CloseText();
        }, false, function () { },
        function () {
            CloseText();
        }
        );
    }
};