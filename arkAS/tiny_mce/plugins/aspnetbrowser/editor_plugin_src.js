/**
* editor_plugin_src.js
*/

(function () {
    tinymce.create('tinymce.plugins.AspNetBrowser', {
        init: function (ed, url) {
            // Register commands
            ed.addCommand('mceAspNetBrowser', function () {
                ed.windowManager.open({
                    file: url + '/imgmanager.aspx',
                    width: 600 + parseInt(ed.getLang('aspnetbrowser.delta_width', 0)),
                    height: 500 + parseInt(ed.getLang('aspnetbrowser.delta_height', 0)),
                    inline: 1
                }, {
                    plugin_url: url
                });
            });

            // Register buttons
            /*
            You also have to put this line of css in the ui.css file of the 
            themes-advanced-skins-default folder
            .defaultSkin span.mce_aspnetbrowser {background:url(img/aspnetbrowser.png) no-repeat center}
            and copy the aspnetbrowser.png to the themes-advanced-skins-default-img folder of the default theme,
            */
            ed.addButton('aspnetbrowser', {
                title: 'Asp.NET Image Manager',
                cmd: 'mceAspNetBrowser'
            });
        },

        getInfo: function () {
            return {
                longname: 'Asp.NET Image Browser/Manager',
                author: 'Mark Angus',
                authorurl: 'http://sugnatech.codeplex.com',
                infourl: 'http://sugnatech.codeplex.com',
                version: tinymce.majorVersion + "." + tinymce.minorVersion
            };
        }
    });

    // Register plugin
    tinymce.PluginManager.add('aspnetbrowser', tinymce.plugins.AspNetBrowser);
})();