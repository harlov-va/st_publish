/**
* aspnetbrowswer.js
*/
var AspNetBrowserDialog = {

    preInit: function (ed) {
        tinyMCEPopup.requireLangPack();
    },

    init: function (ed) {
        tinyMCEPopup.resizeToInnerSize();
    },

    insert: function (file, title) {
        var ed = tinyMCEPopup.editor, dom = ed.dom;

        tinyMCEPopup.execCommand('mceInsertContent', false, dom.createHTML('img', {
			src : file,
			alt : title,
			title : title,
			border : 0
		}));
        
        tinyMCEPopup.editor.execCommand('mceRepaint');
        tinyMCEPopup.editor.focus();
        tinyMCEPopup.close();
    }
};
AspNetBrowserDialog.preInit();
tinyMCEPopup.onInit.add(AspNetBrowserDialog.init, AspNetBrowserDialog);
