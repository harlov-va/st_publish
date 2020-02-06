var as = as || {};
var isLogin = false;

as.postVK = {
    options: {
        ajax: {
            sendWallPost: "/postVK/sendWallPost",
            showWallElement: "/postVK/showWallElement",
        },
    },
    init: function (param) {
        VK.init({
            apiId: param,  //id Вашего приложения
        });

        VK.Auth.getLoginStatus(function (response) {
            if (response.status == 'connected') {
                isLogin = true;
            }
        });

        $(document).delegate('#vkButton', 'click', as.postVK.sendWallPost);
        as.postVK.showWallElement();

    },

    onClick: function () {
        if (isLogin) {
            as.postVK.sendWallPost();
        }
        else {
            as.postVK.doLogin();
        }
    },

    doLogin: function () {
        VK.Auth.login(function (response) {
            if (response.status == 'connected') {
                isLogin = true;
            }
        })
    },

    sendWallPost: function () {
        VK.api("wall.post", {
            message: $('.as-vk-data').val()
        }, function (data) {
            if (data.response) {
                as.sys.bootstrapAlert(as.resources.postVK_Success, { type: 'success' });
            }
            else {
                as.sys.bootstrapAlert(as.resources.postVK_Error, { type: 'danger' });
            }
        });
    },

    showWallElement: function () {
        var s = [];
        s.push('<div class="row"><textarea class="form-control as-vk-data" id="text" rows="4" cols="35" name="text"></textarea></div>');
        s.push('<div class="as-vk-post"><button style="margin:10px 0 15px 0" class="btn" id="vkButton" value="Send">' + as.resources.postVK_Post + '</button></div>');
        $('.as-vk-post').html(s.join(''));
    },
};

