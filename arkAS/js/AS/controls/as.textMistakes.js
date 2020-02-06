// Компонент редактирования ошибки в тексте
// Выделяем ошибку в тексте, нажимаем ctrl + Enter, вводим (необязательно) комментарий, отравляем в базу

var as = as || {};

as.textMistakes = {

    options: {
        ajaxURLFormat: "/TextMistakes/TextMistakesSave",
    },

    init: function () {
        $("body").prepend("<a id='as-form-simpleTextMistakes'></a> ");

        $("body").keydown(function(e){
            if (e.ctrlKey && e.which == 13 && (as.textMistakes.getSelectionText() != "")) {
                e.preventDefault();
                var tm = as.textMistakes.getSelectionText();
                var a = $("#as-form-simpleTextMistakes").attr({
                    "data-subtitle": "<h4>Ошибка:</h4> " + tm,
                    "class": "as-form-simple",
                    "data-code": "textMistakes",
                    "data-text": as.resources.textMistakes_init_datatext,
                    "data-text-placeholder": as.resources.textMistakes_init_placeholder,
                    "data-button": as.resources.textMistakes_init_btnSave,
                    "data-title": as.resources.textMistakes_init_dataTitle
                });
                var m = as.simpleForm.getSimpleFormMakeup(a, true);
                as.sys.showDialog(a.attr('data-title'), m, a.attr('data-button'), function () {
                    var cont = $('.modal').find(".as-form-simple-form");
                    var text = $(".as-form-simple-text", cont);
                    if (text.attr('required') && !text.val()) {
                        as.sys.bootstrapAlert("" + as.resources.textMistakes_init_AlertRequiredText + " <b>" + text.prev().text() + "</b>", { type: "warning" });
                        text.focus();
                        return;
                    }
                    var code = $(cont).attr('data-code') || "";        
                    var params = { code: code, comment: text.val(), selectText: tm, url: window.location.href };
                    as.sys.ajaxSend(as.textMistakes.options.ajaxURLFormat, params, function (data) {
                        if (typeof (data) != "object") data = eval('(' + data + ')');
                        if (data.result) {
                            as.sys.bootstrapAlert(data.msg || as.resources.textMistakes_init_AlertSavedError, { type: 'success' });
                            as.sys.closeDialog();
                            setTimeout(function () {
                                $(cont).val('');
                            }, 500);               
                        } else {
                            as.sys.bootstrapAlert(data.msg || as.resources.textMistakes_init_AlertError, { type: 'danger' });
                        }
                    }, false, $(".as-form-simple-save", cont));
                })
            }
        })
    },

    getSelectionText: function () {
        var txt = '';
        if (txt = window.getSelection){ // Не IE, используем метод getSelection
            txt = window.getSelection().toString();
        } else { // IE, используем объект selection
            txt = document.selection.createRange().text;
        }
        return txt;
    }
}
