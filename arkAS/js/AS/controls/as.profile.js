var as = as || {};

as.profile =
        {
            options:
            {
            },
            maxFileSize: 2000000,
            init: function () {
                as.profile.InitProfile();

            },
            loadImage: function (path, width, height, target, uniqueId) {
                $("<img id='as-simpleImage' src='" + path + "?uniqueId=" + uniqueId + "'/>").load(function () {
                    $(this).width(width).height(height);
                    $(this).attr("uniqueId", uniqueId);
                    $(target).html(this);
                });
            },

            RefreshImg: function (imgFile)
            {
                var textId = "";
                var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                for (var i = 0; i < Math.random() * 16 + 8; i++)
                    textId += possible.charAt(Math.floor(Math.random() * possible.length));

                as.profile.loadImage(imgFile, 160, 240, "div.as-simpleImage-item", textId);

            },
            InitProfile: function () {
                $("button#BtnDownload").click(function () {

                    $("input#imagesFileUpload").trigger("click");
                });
                $("input#imagesFileUpload").change(function () {
                    if (this.files.length == 1) {
                        var fileUpl = this.files[0];
                        var fileUplName = fileUpl.name;
                        var fileUplSize = fileUpl.size;
                        var fileUplType = fileUpl.type;
                        if (fileUplSize <= as.profile.maxFileSize) {
                            var fd = new FormData();
                            fd.append("UserGuid", $("div.as-simpleImage-manager").attr("data-userguid"));
                            fd.append("FileImg", fileUpl);
                            $.ajax({
                                type: 'POST',
                                url: "/Demo/ProfileSaveImage",
                                processData: false,
                                contentType: false,
                                cache: false,
                                data: fd,
                                dataType: "json",
                                success: function (data) {
                                    if (typeof (data) != "object") data = eval('(' + data + ')');

                                    if (data.error == "Success") {


                                        as.profile.RefreshImg(data.imgFile);
                                        as.sys.bootstrapAlert("Сохранено!", { type: 'success' });
                                    }
                                    else {
                                        as.sys.bootstrapAlert(data.msg || "Возникли ошибки при выполнении операции!", { type: 'danger' });
                                    }
                                }
                            });
                        }
                    }
                });

                $("div.as-simpleImage-item").click(function () {
                    as.sys.ajaxSend("/Demo/ProfileGetUserData", {}, function (data) {
                        if (typeof (data) != "object") data = eval('(' + data + ')');
                        var body = [];
                        body.push("<div id='container-fluid'>");
                        body.push("<div class='row'>");
                        body.push("<div class='col-md-3'><span><label for='UserSurname'>Фамилия</label></span></div><div class='col-md-8'> <span><input type='text' id='UserSurname' class='form-control' name='UserSurname' value='" + data.UserSurname + "' data-name='Фамилия'/></span> </div><div id='ErUserSurname' class='col-md-1 DivEr'></div>");
                        body.push("</div>");
                        body.push("<div class='row'>");
                        body.push("<div class='col-md-3'> <span><label for='UserName'>Имя</label></span></div><div class='col-md-8'> <span><input type='text' id='UserName' class='form-control' name='UserName' value='" + data.UserName + "' data-name='Имя'/></span> </div><div id='ErUserName' class='col-md-1'></div>");
                        body.push("</div>");
                        body.push("<div class='row'>");
                        body.push("<div class='col-md-3'> <span><label for='UserPatronymic'>Отчество</label></span></div><div class='col-md-8'> <span><input type='text' class='form-control' id='UserPatronymic' name='UserPatronymic' value='" + data.UserPatronymic + "' data-name='Отчество'/></span> </div><div id='ErUserPatronymic' class='col-md-1'></div>");
                        body.push("</div>");
                        body.push("<div class='row'>");
                        body.push("<div class='col-md-3'> <span><label for='UserEmail'>Email</label></span></div><div class='col-md-8'> <span><input type='text' class='form-control' id='UserEmail' name='UserEmail' value='" + data.UserEmail + "' data-name='Email'/></span> </div><div id='ErUserEmail' class='col-md-1'></div>");
                        body.push("</div>");
                        body.push("<div class='row'>");
                        body.push("<div class='col-md-3'> <span><label for='UserSkype'>Skype</label></span></div><div class='col-md-8'> <span><input type='text' class='form-control' id='UserSkype' name='UserSkype' value='" + data.UserSkype + "' data-name='Skype'/></span> </div><div id='ErUserSkype' class='col-md-1'></div>");
                        body.push("</div>");
                        body.push("<div class='row'>");
                        body.push("<div class='col-md-3'> <span><label for='UserPhoneTwo'>Телефон</label></span></div><div class='col-md-8'> <span> <input id='UserPhoneTwo' name='UserPhoneTwo' class='form-control' type='tel' value='" + data.UserPhoneTwo + "' data-name='Телефон'/></span> </div><div id='ErUserPhoneTwo' class='col-md-1'></div>");
                        body.push("</div>");
                        body.push("</div>");
                        as.sys.showDialog("Контактные данные пользователя", body.join(""), "Сохранить", function () {
                            var inputSel = $("div.modal-body input");
                            var hasError = 0;
                            var inputNameError = [];
                            for (var i = 0; i < inputSel.length; i++)
                            {
                                if(inputSel.eq(i).hasClass("has-error"))
                                {
                                    hasError++;
                                    inputNameError[hasError] = inputSel.eq(i).attr("data-name") + "<br>";
                                    as.sys.bootstrapAlert("Заполните поле " + inputNameError[hasError], { type: 'danger' });

                                }
                            }
                            var error = $("input#UserPhoneTwo").intlTelInput("getValidationError");

                            if (hasError == 0)
                            {
                                var UserSurname = $("input#UserSurname").val();
                                var UserName = $("input#UserName").val();
                                var UserPatronymic = $("input#UserPatronymic").val();
                                var UserEmail = $("input#UserEmail").val();
                                var UserSkype = $("input#UserSkype").val();
                                var UserPhoneTwo = $("input#UserPhoneTwo").intlTelInput("getNumber");
                                var UserGuid = $("div.as-simpleImage-manager").attr("data-userguid")
                                var params = { UserGuid: UserGuid, UserSurname: UserSurname, UserName: UserName, UserPatronymic: UserPatronymic, UserEmail: UserEmail, UserSkype: UserSkype, UserPhoneTwo: UserPhoneTwo };
                                as.sys.ajaxSend("/Demo/ProfileContactDataSave", params, function (data) {
                                    if (typeof (data) != "object") data = eval('(' + data + ')');
                                    if (data.error == "Success") {
                                        as.sys.closeDialog();
                                        as.sys.bootstrapAlert("Сохранено!", { type: 'success' });
                                    }
                                    else {
                                        as.sys.bootstrapAlert(data.msg || "Возникли ошибки при выполнении операции!", { type: 'danger' });
                                    }
                                });
                            }

                        }, false, function () {

                            $("input#UserSurname").inputmask("*{4,512}",
                              {
                                  "oncomplete": function () {
                                      $("input#UserSurname").removeClass('has-error');
                                      $("input#UserSurname").addClass('has-success');
                                      $("div#ErUserSurname").html(" <span class='glyphicon glyphicon-ok'></span>");
                                  },
                                  "onincomplete": function () {
                                      $("input#ErUserSurname").removeClass('has-success');
                                      $("input#UserSurname").addClass('has-error');
                                      $("div#ErUserSurname").html(" <span class='glyphicon glyphicon-remove'></span>");
                                  },

                              });
                            $("input#UserName").inputmask("*{4,512}",
                              {
                                  "oncomplete": function () {
                                      $("input#UserName").removeClass('has-error');
                                      $("input#UserName").addClass('has-success');
                                      $("div#ErUserName").html(" <span class='glyphicon glyphicon-ok'></span>");
                                  },
                                  "onincomplete": function () {
                                      $("input#UserName").removeClass('has-success');
                                      $("input#UserName").addClass('has-error');
                                      $("div#ErUserName").html(" <span class='glyphicon glyphicon-remove'></span>");
                                  },

                              });
                            $("input#UserPatronymic").inputmask("*{4,512}",
                              {
                                  "oncomplete": function () {
                                      $("input#UserPatronymic").removeClass('has-error');
                                      $("input#UserPatronymic").addClass('has-success');
                                      $("div#ErUserPatronymic").html(" <span class='glyphicon glyphicon-ok'></span>");
                                  },
                                  "onincomplete": function () {
                                      $("input#UserPatronymic").removeClass('has-success');
                                      $("input#UserPatronymic").addClass('has-error');
                                      $("div#ErUserPatronymic").html(" <span class='glyphicon glyphicon-remove'></span>");
                                  },

                              });

                            $("input#UserEmail").inputmask(
                            {
                                mask: "e{1,1}m{1,40}@@i{1,40}.l{2,6}",
                                greedy: false,
                                definitions:
                                {
                                    'e': {
                                        validator: "[a-zA-Z_]",
                                        cardinality: 1,
                                        casing: "lower"
                                    },
                                    'm': {
                                        validator: "[a-zA-Z0-9._%-]",
                                        cardinality: 1,
                                        casing: "lower"
                                    },
                                    'i': {
                                        validator: "[a-zA-Z0-9-]",
                                        cardinality: 1,
                                        casing: "lower"
                                    },
                                    'l': {
                                        validator: "[a-zA-Z]",
                                        cardinality: 1,
                                        casing: "lower"
                                    }
                                },
                                "oncomplete": function () {
                                    $("input#UserEmail").removeClass('has-error');
                                    $("input#UserEmail").addClass('has-success');
                                    $("div#ErUserEmail").html(" <span class='glyphicon glyphicon-ok'></span>");
                                },
                                "onincomplete": function () {
                                    $("input#UserEmail").removeClass('has-success');
                                    $("input#UserEmail").addClass('has-error');
                                    $("div#ErUserEmail").html(" <span class='glyphicon glyphicon-remove'></span>");
                                }
                            });

                            $("input#UserSkype").inputmask("*{4,512}",
                           {
                               "oncomplete": function () {
                                   $("input#UserSkype").removeClass('has-error');
                                   $("input#UserSkype").addClass('has-success');
                                   $("div#ErUserSkype").html(" <span class='glyphicon glyphicon-ok'></span>");
                               },
                               "onincomplete": function () {
                                   $("input#UserSkype").removeClass('has-success');
                                   $("input#UserSkype").addClass('has-error');
                                   $("div#ErUserSkype").html(" <span class='glyphicon glyphicon-remove'></span>");
                               },

                           });

                            $("input#UserPhoneTwo").intlTelInput(
                              {
                                  nationalMode: true,
                                  defaultCountry: "auto",
                                  geoIpLookup: function (callback) {
                                      $.get('http://ipinfo.io', function () { }, "jsonp").always(function (resp) {
                                          var countryCode = (resp && resp.country) ? resp.country : "";
                                          callback("ru");
                                      });
                                  },
                                  utilsScript: "/js/intl-tel-input-master/lib/libphonenumber/build/utils.js"
                              });

                            $('div.modal-body input').focus(function () {
                                $(this).removeClass('has-error');
                                $(this).removeClass('has-success');
                                $(this).addClass("focus");
                            });

                            $('div.modal-body input').blur(function () {
                                $(this).removeClass("focus");
                            });


                            $("input#UserPhoneTwo").blur(function () {
                                if ($.trim($("input#UserPhoneTwo").val())) {
                                    if ($("input#UserPhoneTwo").intlTelInput("isValidNumber")) {
                                        $("input#UserPhoneTwo").removeClass('has-error');
                                        $("input#UserPhoneTwo").addClass('has-success');
                                        $("div#ErUserPhoneTwo").html(" <span class='glyphicon glyphicon-ok'></span>");
                                    } else {
                                        $("input#UserPhoneTwo").removeClass('has-success');
                                        $("input#UserPhoneTwo").addClass('has-error');
                                        $("div#ErUserPhoneTwo").html(" <span class='glyphicon glyphicon-remove'></span>");
                                    }
                                }
                                var intlNumber = $("input#UserPhoneTwo").intlTelInput("getNumber");

                            });
                            $("input#UserPhoneTwo").keyup(function () {
                                $("input#UserPhoneTwo").removeClass('has-error');
                                $("input#UserPhoneTwo").removeClass('has-success');
                                if ($("input#UserPhoneTwo").intlTelInput("isValidNumber")) {
                                    $("input#UserPhoneTwo").addClass('has-success');
                                }
                                else
                                {
                                    $("input#UserPhoneTwo").removeClass('has-success');
                                }
                            });

                        }, function () {
                            $("div.modal-body").empty();
                            $("div.modal-title").empty();
                        });
                    });
                });
            }
        };