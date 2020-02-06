var as = as || {};

as.opinion = {
    options: {
        ajax: {
            upload: "/Opinion/Save",
            getOpinions: "/Opinion/GetCacheOpinions",
            cacheDuration: 1 // продолжительность кеширования мнения в минутах
        }
    },
    init: function () {

        as.sys.ajaxSend(as.opinion.options.ajax.getOpinions, null, function (data) {
            if (typeof (data) != "object") data = eval('(' + data + ')');
            if (data.result) {
                $('.as-user-opinion').each(function () {
                    if (data.items.indexOf($(this).data('itemid').toString()) == -1) {
                        as.opinion.addOpinionDialog($(this));
                    }
                });
            }
        });

        $(document).delegate('.as-opinion-add', 'click', as.opinion.showOpinionDialog);
        $(document).delegate('.as-opinion-save', 'click', as.opinion.saveOpinion);

    },

    addOpinionDialog: function (cont) {

        cont.append('<button class="btn btn-primary as-opinion-add"><span class="glyphicon glyphicon-star" aria-hidden="true"></span>   ' + as.resources.opinion_Add + '</button>');
        
        var s = [];

        s.push('<textarea style="width:223px; max-width:246px; min-width:210px;" class="form-control" placeholder="' + as.resources.opinion_Placeholder + '"></textarea>');
        s.push('<div><div style="float:left; width:49%"><button style="margin:10px 0 15px 0" class="btn btn-success btn-sm btn-block as-opinion-save" value="true"><span class="glyphicon glyphicon-thumbs-up" aria-hidden="true"></span>   ' + as.resources.opinion_Like + '</button></div>');
        s.push('<div style="float:right; width:49%"><button style="margin:10px 0 15px 0" class="btn btn-danger btn-sm btn-block as-opinion-save" value="false"><span class="glyphicon glyphicon-thumbs-down" aria-hidden="true"></span>   ' + as.resources.opinion_Dislike + '</button></div></div>');

        $('.as-opinion-add', cont).popover({
            html: true,
            //title: 'Мнение',
            content: s.join('')
        });
    },

    showOpinionDialog: function () {

        $(this).popover();

    },


    saveOpinion: function () {

        var cont = $(this).closest('.as-user-opinion');

        var params = {
            itemID: cont.data('itemid'),
            like: $(this).val(),
            comment: $('textarea', cont).val(),
            cacheDuration: as.opinion.options.ajax.cacheDuration,
        };

        as.sys.ajaxSend(as.opinion.options.ajax.upload, params, function (data) {
            if (typeof (data) != "object") data = eval('(' + data + ')');
            if (data.result) {
                cont.children().remove();
                as.sys.bootstrapAlert(as.resources.opinion_Message, { type: 'success' });
                //cont.after('<div style="position: absolute; right:100px; top:100px" class="alert alert-success fade in"><a href="#" style="margin-left: 30px" class="close" data-dismiss="alert" aria-label="close">&times;</a>' + as.resources.opinion_Message + '</div>');
            }
        });
    }
};