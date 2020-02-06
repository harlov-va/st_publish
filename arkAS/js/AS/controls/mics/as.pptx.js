var as = as || {};

as.pptx = {
    options: [
        { appendTo: 'body' },
        {appendTo:'body'}
    ],
    init: function (options) {
        as.pptx.options = $.extend(as.pptx.options, options);
        console.log(as.pptx.options);
    },
    setParameters: function (arrayParameters, numSlide) {
        $.ajax({
            url: '/Pptx/SetParameters',
            type: 'Get',
            dataType: 'json',
            traditional:true,
            data: { 'arrayParameters': arrayParameters,'numberSlide':numSlide},
            success: function (data) {
                as.pptx.succesParam();
            }
        });
    },
    chekData: function () {
        var s = "";
        s += '<div class="alert alert-warning alert-dismissible">';
        s += 'Заполните все поля <button class="close" data-dismiss="alert">&times;</button> </div>';
        $(as.pptx.options.option[0].appendTo).empty().append(s);
        setTimeout(function () {
            $('.alert').hide('slow', function () {
                $(this).remove();
            });
        },
         3000);
    },
    addSlide: function (count,select) {
        $.ajax({
            url: '/Pptx/AddSlide',
            type: 'Get',
            dataType: 'json',
            data: {'Count':count},
            success: function (data) {
                as.pptx.addCountSlide(data,select);
                as.pptx.succesParam();
            }
        });
    },

    addCountSlide: function (data,select) {
        if (data == true)
        {
            var count = select.size();
            count++;
            var s = [];
            s += '<option value="' + count + '">' + count + '</option>';
            $(as.pptx.options.option[1].appendTo).append(s);
            $(as.pptx.options.option[2].appendTo).html(count);
            $(as.pptx.options.option[3].appendTo).append(s);

        }
        
    },
    delitSlide: function (count, select, numSelect, numDelitSelect) {
        var numSlide = select.val();
        $.ajax({
            url: '/Pptx/DelitSlide',
            type: 'Get',
            dataType: 'json',
            data: { 'numSlide': numSlide, 'count':count },
            success: function (data) {
                if (data == false)
                    as.pptx.errorParam();
                else {
                    as.pptx.countSlide(count, data, numSelect, numDelitSelect);
                    as.pptx.succesParam();
                }
                
            }
        });

    },
    countSlide: function (count, data, numSelect, numDelitSelect) {
        if (data == true)
        {
            count--;
            $(as.pptx.options.option[2].appendTo).html(count);
            $(numSelect).remove();
            $(numDelitSelect).remove();

        }
    },
    succesParam: function () {
        var s = "";
        s += '<div class="alert alert-success alert-dismissible">';
        s += 'Выполенно <button class="close" data-dismiss="alert">&times;</button> </div>';
        $(as.pptx.options.option[0].appendTo).empty().append(s);
        setTimeout(function () {
            $('.alert').hide('slow', function () {
                $(this).remove();
            });
        },
         3000);
    },
    errorParam: function () {
        var s = "";
        s += '<div class="alert alert-danger alert-dismissible">';
        s += 'Не выполенно <button class="close" data-dismiss="alert">&times;</button> </div>';
        $(as.pptx.options.option[0].appendTo).empty().append(s);
        setTimeout(function () {
            $('.alert').hide('slow', function () {
                $(this).remove();
            });
        },
         3000);
    }
}