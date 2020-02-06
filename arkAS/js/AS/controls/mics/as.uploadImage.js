var as = as || {};

as.uploadImage = {
    init: function (options) {
        $(".cropped").html('<input type="file" name="file_upload" id="file_upload" />');
        $("#file_upload").uploadify({
            'swf': '/js/uploadify/uploadify.swf',
            'uploader': '/Demo/UploadImage',
            //'uploader': '@(Url.Action("Demo", "Uploadify"))',
            'onUploadSuccess': function (file, data, responce) {

                as.uploadImage.cropper(options);
                $('#SWFUpload_0_0').remove();

                var s = [];
                s.push("<img id='image' src='" + data + "' alt='Uploaded Image' />");
                s.push('<b>Выберите область:  </b><button id="size1" class="btn btn-default">Размер 1</button><button id="size2" class="btn btn-default">Размер 2</button><button id="size3" class="btn btn-default">Размер 3</button><button id="size4" class="btn btn-default">Размер 4</button>');
                as.sys.showDialog("Редактируйте изображение", s.join(""), "Сохранить", function () {
                    var canvas = $('#image').cropper('getCroppedCanvas');
                    var url = canvas.toDataURL('image/jpeg');
                    $('.cropped').html("<img id='cropped_image' src='" + url + "' alt='Cropped Image' />")
                    as.sys.closeDialog();
                }, null, function () {$('#image').cropper('crop');});
            }
        });
    },
    cropper: function (options) {
        $(document).delegate('#size1', 'click', function () {
            $('#image').cropper("setCropBoxData", options.size1);
            //alert(size1);
        });

        $(document).delegate('#size2', 'click', function () {
            $('#image').cropper("setCropBoxData", options.size2);
        });

        $(document).delegate('#size3', 'click', function () {
            $('#image').cropper("setCropBoxData", options.size3);
        });

        $(document).delegate('#size4', 'click', function () {
            $('#image').cropper("setCropBoxData", options.size4);
        });
    }
}
