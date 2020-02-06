var as = as || {};

as.kanban = {

    option: {
        ajaxURLEditCategory: '/Demo/EditSetting',
        ajaxURLSave: '/Demo/SaveSetting',
        params: '',
        params1: '',
        count: 'kanban'
    },

    init: function (option) {
        as.kanban.option = $.extend(as.kanban.option, option);
        as.kanban.createKanban();
        as.kanban.initCallbacks();
    },

    initCallbacks: function () {
        $('.portlet-header').on('dblclick', as.kanban.editItem);
        $('.btnAdd').on('click', as.kanban.createItem);
        $('.btnFilter').on('click', as.kanban.filter);
    },

    createKanban: function () {

        $(as.kanban.option.count).before('<input class="btn-info btnFilter" type="button" value="Фильтр">')

        as.kanban.option.params.forEach(function (item, i, params) {
            $(as.kanban.option.count).append('<div id="' + item.ID + '" class="column' + item.ID + ' connectedSortable kanbanColumn column"> <h4 class="columnTitle"> ' + item.Text + '(<span class="count">0</span>)</h4 ><input id="' + item.ID + '" class="btn-success btnAdd ui-sortable" type="button" value="Создать"></div > ');         
        });
       
        as.kanban.option.params1.forEach(function (item, i, params1) {
            $('.column' + item.CategoryID).append('<div id="' + item.ID + '" class="ui-sortable sort col-md-2 kanbanElement portlet"><div class="portlet-header">' + item.Name + '</div><div class="portlet-content">' + item.Text +'</div></div > ');
            var val = parseInt($('.column' + item.CategoryID).find('.count').text());
            $('.column' + item.CategoryID).find('.count').text(val + 1);
        });

        $('.connectedSortable').sortable({
            connectWith: ".connectedSortable",
            handle: ".portlet-header",
            cancel: ".portlet-toggle",
            placeholder: "portlet-placeholder ui-corner-all",
            items: ".sort",
            opacity:0.5,
            start: function (event, ui) {
                var id = ui.item[0].parentElement.id;
                $(this).attr('data-parent', id);
                var val = parseInt($('.column' + id).find('.count').text());
                $('.column' + id).find('.count').text(val - 1);            
            },
            beforeStop: function (event, ui) {
                var newParent = ui.item[0].parentElement.id;
                var val = parseInt($('.column' + newParent).find('.count').text());
                $('.column' + newParent).find('.count').text(val + 1);

                var oldParent = $(this).attr('data-parent');
                var id = $(ui.item).attr('id');
                $(this).removeAttr('data-parent');
                if (newParent != oldParent) {
                    var params = { id: id, fieldValue: newParent, fieldName: "categoryName" }
                    as.kanban.ajaxSend(params);
                }
            }
        });

        $(".portlet")
            .addClass("ui-widget ui-widget-content ui-helper-clearfix ui-corner-all")
            .find(".portlet-header")
            .addClass("ui-widget-header ui-corner-all")
            .prepend("<span class='ui-icon ui-icon-minusthick portlet-toggle'></span>");

        as.kanban.portletFunction();
    },

    portletFunction: function () {
        $(".portlet")
            .addClass("ui-widget ui-widget-content ui-helper-clearfix ui-corner-all")
            .find(".portlet-header")
            .addClass("ui-widget-header ui-corner-all")
            .prepend("<span class='ui-icon ui-icon-minusthick portlet-toggle'></span>");

        $(".portlet-toggle").on("click", function () {
            var icon = $(this);
            icon.toggleClass("ui-icon-minusthick ui-icon-plusthick");
            icon.closest(".portlet").find(".portlet-content").toggle();
        });
    },

    createItem: function () {
        var s = [];
        s.push("<span>Код</span><input type='text' class='itemCode form-control' /><br />");
        s.push("<span>Значение</span><input type='text' class='itemValue form-control' /><br />");

        var elem = this;
        var a = $('.categoryID [value="' + this.id + '"]').attr('selected', 'selected');
        $('.categoryID [value="' + this.id + '"]').attr('selected', 'selected');
        console.log(a);
        s.push("<div class='category'><span class='gCap'>Категория</span><select class='itemCategory form-control'>" + $('.categoryID').html() + "</select></div>");

        s.push("<span>Значение2</span><input type='text' class='itemValue2 form-control' /><br />");
        s.push("<span>Название</span><input type='text' class='itemName form-control' /><br />");       

        $('.categoryID [value="' + this.id + '"]').removeAttr('selected');

        as.sys.showDialog("Создание", s.join(""), "Сохранить", function () {
            var code = $('.itemCode').val();
            var value = $('.itemValue').val();
            var category = $('.itemCategory option:selected').val();
            var value2 = $('.itemValue2').val();
            var name = $('.itemName').val();

            if (!name) {
                as.sys.bootstrapAlert("Укажите пожалуйста название", { type: 'warning' });
                $('.itemName').focus();
                return;
            }

            if (!value) {
                as.sys.bootstrapAlert("Укажите пожалуйста значение", { type: 'warning' });
                $('.itemValue').focus();
                return;
            }
            

            var params = {
                code: code,
                value: value,
                category: category,
                value2: value2,
                name:name
            }
            as.sys.ajaxSend(as.kanban.option.ajaxURLSave, params, function (data) {
                if (typeof (data) != "object") data = eval('(' + data + ')');
                if (data.result) {
                    as.sys.bootstrapAlert("Изменения сохранены", { type: 'success' });
                    var idForNew = 0;
                    as.kanban.option.params1.forEach(function (item, i, params1) {
                        if (item.ID > idForNew) idForNew = item.ID
                    });
                    idForNew++;
                    as.kanban.option.params1 += { ID: idForNew, Name: name, Text: value };                    
                    $('.column' + category).append('<div id="' + idForNew + '" class="ui-sortable sort col-md-2 kanbanElement portlet"><div class="portlet-header">' + name + '</div><div class="portlet-content">' + value + '</div></div > ');
                    as.kanban.portletFunction();
                } else {
                    as.sys.bootstrapAlert("Возникли ошибки при выполнении операции!", { type: 'danger' });
                }
            }, true);
        });
    },

    editItem: function (e) {
        var e = e || window.event;
        var target = e.target || e.srcElement;
        if (this == target) {
            var elem = this;
            var s = [];
            s.push('<span>Название</span><input type="text" class="form-control newName" value="' + elem.innerText + '"/><br />');
            s.push('<span>Значение</span><input type="text" class="form-control newValue" value="' + elem.offsetParent.lastChild.innerText + '" /><br />');

            as.sys.showDialog("Редактирование", s.join(""), "Сохранить", function () {
                if (elem.innerText != $('.newName').val() || elem.offsetParent.lastChild.innerText != $('.newValue').val()){
                    if (elem.innerText != $('.newName').val()) {
                        elem.innerText = $('.newName').val();
                        var params = { id: elem.offsetParent.id, fieldValue: $('.newName').val(), fieldName: "name" }
                        as.kanban.ajaxSend(params);
                    }

                    if (elem.offsetParent.lastChild.innerText != $('.newValue').val()) {
                        elem.offsetParent.lastChild.innerText = $('.newValue').val();
                        var params = { id: elem.offsetParent.id, fieldValue: $('.newValue').val(), fieldName: "value" }
                        as.kanban.ajaxSend(params);
                    }
                    as.kanban.portletFunction();
                }
                else {
                    as.sys.bootstrapAlert("Никаких изменений не произошло!", { type: 'info' });
                }
            });
        }
    },

    filter: function () {
        var s = [];
        as.kanban.option.params.forEach(function (item, i, params) {
            if ($(".column" + item.ID).hasClass("hide")) {
                s.push('<div class="checkbox"><label> <input type="checkbox" value="column' + item.ID + '" onChange="as.kanban.changeFunction(this)">' + item.Text + '</label></div >');
            }
            else {
                s.push('<div class="checkbox"><label> <input type="checkbox" value="column' + item.ID + '" onChange="as.kanban.changeFunction(this)" checked>' + item.Text + '</label></div >');
            }
            
        });
        as.sys.showDialog("Колонки", s.join(""), "", function () {
            as.sys.closeDialog();
        });

    },

    changeFunction: function (elem) {
        if (!elem.checked) {
            $('.' + elem.value).addClass('hide');
        }
        else {
            $('.' + elem.value).removeClass('hide');
        }
    },

    ajaxSend: function (params) {
        as.sys.ajaxSend(as.kanban.option.ajaxURLEditCategory, params, function (data) {
            if (typeof (data) != "object") data = eval('(' + data + ')');
            if (data.result) {
                as.sys.bootstrapAlert("Изменения сохранены", { type: 'success' });
            } else {
                as.sys.bootstrapAlert("Возникли ошибки при выполнении операции!", { type: 'danger' });
            }
        }, true);
    }
};