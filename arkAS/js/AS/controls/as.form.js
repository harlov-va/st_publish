// v0.01
// метод load
// комбики и чекбоксы (sources)
// проверка через ajax 

var as = as || {};

as.form = {
    options: {
        editID: "",
        title: '',
        subtitle: '',
        exampleText: as.resources.formExampleText,
        fieldIsRequiredText: as.resources.formFieldIsRequiredText,
        cont: null,
        buttonText: as.resources.formButtonText,
        ajaxURLFormat: "/serv/form.aspx/{0}",  // get, save   
        cookiePrefix: 'as-form-',
        fields: [
         { code: "", title: "", tooltip: "", example: "", datatype: "", visible: true, isRequired: false, checkCallback: function () { alert('asd') } },
         { code: "", title: "", tooltip: "", example: "", datatype: "", visible: true, isRequired: false, checkCallback: function () { alert('asd') } },
         { code: "", title: "", tooltip: "", example: "", datatype: "", visible: true, isRequired: false, checkCallback: function () { alert('asd') } }
        ],
        showCallback: function () { },
        saveCallback: function (data) { }
    },
    errors: [],
    fieldErrors: [],
    init: function (options) {
        if (as.form.runInit) return;
        as.form.runInit = true;

        as.form.options = $.extend(as.form.options, options);
        if (typeof String.prototype.format != 'function') {
            as.sys.init();
        }

        $(document).delegate('.as-form-submit', 'click', function (e) {
            e.preventDefault();
            as.form.save();
        });
        $(document).delegate('.as-form-error-link', 'click', function (e) {
            e.preventDefault();
            var code = $(this).attr('data-code');
            var el = $(".as-form-item[data-code=" + code + "]", as.form.options.cont);
            $(":input:first", el).focus();
           
            $('html,body').animate({ scrollTop: parseInt( el.offset().top) +200 + "px"}, 'slow');
        });

        as.form.processErrors();

        if (as.form.options.cont) {
            as.form.show();
        }
    },
    show: function () {
        function setMakeup(data) {
            var s = [];
            s.push("<div class='as-form-cont'>");
            s.push("<h1>" + as.form.options.title + "</h1>");
            s.push("<p>" + as.form.options.subtitle + "</p>");

            $.each(as.form.options.fields, function (i, field) {
                s.push("<div class='as-form-item " + (!field.visible ? "hide" : "") + "' data-code='" + field.code + "' data-datatype='" + field.datatype + "'  data-isRequired='" + (field.isRequired ? 1 : 0) + "' title='" + field.tooltip + "'>");
                s.push("<span class='as-form-cap'>" + field.title + (field.isRequired ? "<span class='as-form-star'>*</span>" : "") + "</span>");
                s.push("<div class='as-form-item-el'>");
                s.push(as.makeup.getControlMakeupByDataType(field.datatype, data[field.code] || "", field.sources || [], "form-control"));
                if (field.example) {
                    s.push("<div class='alert alert-warning as-form-example'>"+as.form.options.exampleText + field.example + "</div>");
                }
                s.push("</div>");
                s.push("</div>");
            });

            s.push("<div class='as-form-btn-cont'><a href='#' class='btn btn-primary as-form-submit'>" + as.form.options.buttonText + "</a></div>");
            s.push("<div class='as-form-res hide'></div>");

            s.push("</div>");
            as.form.options.cont.html(s.join(""));
            if (as.form.options.showCallback) {
                as.form.options.showCallback();
            }
        }

        if (!as.form.options.editID!="") {
            setMakeup({});
        } else {
            var params = {
                id: as.form.options.editID
            };
            as.sys.ajaxSend(as.form.options.ajaxURLFormat.format("get"), params, function (data) {
                if (typeof (data) != "object") data = eval('(' + data + ')');
                setMakeup(data);
            });
        }

        

    },
    save: function () {
        $('.as-form-res').addClass('hide').html('');
        as.form.errors = [];
        as.form.errors.push.apply(as.form.errors, as.form.fieldErrors || []) || [];
        var ar = [];
        $(".as-form-item").each(function () {
            var val = as.makeup.getValueFromControlMakeup($(this).attr('data-datatype'), $(this));
            ar.push({ code: $(this).attr('data-code'), value: val || '' });
            if ($(this).attr("data-isRequired") == "1" && !val) {
                as.form.errors.push({ text: as.form.options.fieldIsRequiredText + $(".as-form-cap", this).text(), code: $(this).attr('data-code'), el: $(">:input", this) });
            }
        });


        if (as.form.errors && as.form.errors.length > 0) {
            var s = [];
            s.push("<ul class='as-form-errors'>");
            var needShowMessage = false;
            $.each(as.form.errors, function (i, item) {
                if (item) {
                    needShowMessage = true;
                    s.push("<li><a href='#' data-code='" + item.code + "' class='as-form-error-link'>" + item.text + "</a></li>");
                }
            });
            s.push("</ul>");
            if (as.form.errors[0]) {
                $(as.form.errors[0].el).focus();
            }
            if (needShowMessage) {
                as.form.showMessage(s.join(""), 'alert alert-danger');
                return;
            }
        }
        var params = {
            fields: ar
        };
        as.sys.ajaxSend(as.form.options.ajaxURLFormat.format("save"), params, function (data) {
            if (typeof (data) != "object") data = eval('(' + data + ')');

            if (data.result) {
                // as.sys.bootstrapAlert(data.msg || "Сохранено!", { type: 'success' });
                as.form.showMessage(data.msg || as.resources.form_save_showMessageSaved, 'alert alert-success');

            } else {
                as.form.showMessage(data.msg || as.resources.form_save_showMessageErrorSaved, 'alert alert-danger');
            }
            if (as.form.options.saveCallback) {
                as.form.options.saveCallback(data);
            }
        });
    },
    showMessage: function (msg, class1) {
        $('.as-form-res').removeClass('alert-success alert-error alert-danger').addClass(class1).removeClass('hide').html(msg);
    },
    processErrors: function () {
        $.each(as.form.options.fields, function (i, field) {
            if (field.checkCallback) {
                $(document).delegate(".as-form-item[data-code=" + field.code + "] :input", 'change', function () {
                    var el = $(this);
                    el.removeClass('as-form-error-field');
                    field.checkCallback(el, function (msg) {
                        if (msg) {
                            as.sys.bootstrapAlert(msg, { type: "warning" });
                            el.addClass('as-form-error-field');
                        }
                        var notfound = true;
                        $.each(as.form.fieldErrors, function (i, er) {
                            if (er.code == field.code) {
                                notfound = false;
                                if (msg == "") { // then delete error                                    
                                    delete as.form.fieldErrors[i];
                                } else {
                                    er.text = msg;
                                }
                            }
                        });
                        if (notfound) as.form.fieldErrors.push({ text: msg, code: field.code, el: $(">:input", el) });
                    });
                });
            }
        });
    }
  
};
