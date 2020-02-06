var keyitems = [];
as.hotKeys = {
    options: {
        GetHotKeysLink: '',
        AccessLink:''
    },
    init: function (options) {
        as.hotKeys.options = $.extend(as.hotKeys.options, options);
        $(document).ready(function () {
            setTimeout(function () {
                as.sys.ajaxSend(as.hotKeys.options.GetHotKeysLink, { id: 1 }, function (data) {
                    var obj = data;
                    if (data.d != undefined) obj = data.d;
                    response = obj;
                    as.hotKeys.readyKey(response);
                });
            }, 1500);
        });
        $(document).keydown(function (event) {

            if (keyitems[event.keyCode] != undefined && event.ctrlKey == keyitems[event.keyCode].isCtrl
                && event.altKey == keyitems[event.keyCode].isAlt && event.shiftKey == keyitems[event.keyCode].isShift) {
                as.sys.ajaxSend(as.hotKeys.options.AccessLink, { id: keyitems[event.keyCode].id }, function (data) {
                    var obj = data;
                    if (obj == true) {
                        console.log('fine');
                        if (keyitems[event.keyCode].url && keyitems[event.keyCode].url.trim() != '') {
                            document.location.href = keyitems[event.keyCode].url;
                        }
                        else {
                            try {
                                eval(keyitems[event.keyCode].js);
                            }
                            catch (err) { }
                        }
                    }
                    else {
                        as.sys.bootstrapAlert("Запрещенное сочетание клавиш!", { type: 'danger' });
                    }
                });

            }
        });
    },
    readyKey: function (response) {
        if (response.total) {
            console.log(response.result + "*" + response.items.length);
            for (var i = 0; i < response.items.length; i++) {
                keyitems[response.items[i].key] = response.items[i];
            }
        }
    }
}