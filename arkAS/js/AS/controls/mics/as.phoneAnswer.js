var as = as || {};

as.phoneAnswer = {
    options: {
        appendTo:'body',
        files:[]
    },
    init: function(options) {
        as.phoneAnswer.options = $.extend(as.phoneAnswer.options, options);
        var opts = as.phoneAnswer.options;

        var target = $(opts.appendTo);
        
        for (var i in opts.files) {
            target.append("<div>"+opts.files[i].Name+"</div>");
            target.append("<div style='margin-top:1px'><audio src='"+opts.files[i].File+"' preload='none' /></div>");
        }

        as.sys.loadLib("/js/audiojs/audio.min.js", "/js/audiojs/index.css", false, function () {
            audiojs.settings.imageLocation = "/js/audiojs/player-graphics.gif";
            audiojs.settings.swfLocation = "/js/audiojs/audiojs.swf";
            audiojs.events.ready(function() {
                var as = audiojs.createAll();
            });
        });
    }
}