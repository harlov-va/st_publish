var as = as || {};

as.audioRecorder = {
    options: {
        log: null
        , isSavelocal: null

    },
    vars: {
        audio_context: null,
        recorder: null
    },
    init: function (options) {
        as.audioRecorder.options = $.extend(as.audioRecorder.options, options);       
    },

    startUserMedia: function (stream) {

        var inputPoint = as.audioRecorder.vars.audio_context.createGain();
        window.source = as.audioRecorder.vars.audio_context.createMediaStreamSource(stream);
        source.connect(inputPoint);
        as.audioRecorder.vars.recorder = new Recorder(inputPoint);
    },

    getMedia: function (successCallback) {

        try {
            window.AudioContext = window.AudioContext || window.webkitAudioContext;

            navigator.getUserMedia = (navigator.getUserMedia ||
                   navigator.webkitGetUserMedia ||
                   navigator.mozGetUserMedia ||
                   navigator.msGetUserMedia);

            window.URL = window.URL || window.webkitURL;

            as.audioRecorder.vars.audio_context = new AudioContext;

        } catch (e) {
            //alert('В этом браузере нет веб-поддержки аудио');
            as.sys.bootstrapAlert(as.resources.crud_getComments_NoWebAudioSupportError, { type: 'danger' });
        }

        navigator.getUserMedia({ audio: true }, function (stream) {
            as.audioRecorder.startUserMedia(stream);
            successCallback();
        }, function (e) {
            //alert('Нет аудио входа: ' + e);
            as.sys.bootstrapAlert(as.resources.crud_getComments_NoAudioInputError, { type: 'danger' });
        });

    },

    startRecording: function () {
        as.audioRecorder.vars.recorder && as.audioRecorder.vars.recorder.record();
      },

    stopRecording: function (saveElem, dataAttrName) {
          as.audioRecorder.vars.recorder && as.audioRecorder.vars.recorder.stop();
          as.audioRecorder.vars.recorder && as.audioRecorder.vars.recorder.exportWAV(function (blob) {
              saveElem.data(dataAttrName, blob);
          });

          as.audioRecorder.vars.recorder.clear();
      },

      cancelRecording: function () {
          if (as.audioRecorder.vars.recorder) {
              as.audioRecorder.vars.recorder.clear();
          }            

      },


      ajaxSend: function (url, data, callback, noProgressBar, btn) {

          $.ajax({
              type: 'POST',
              url: url,
              data: data,
              processData: false,
              contentType: false,
              success: function (data, status) {
                  var response = data;
                  if (data.d != undefined) response = data.d;
                  if (typeof (response) != "object") response = eval('(' + response + ')');
                  if (callback) callback(response);
              },
              complete: function () {
                  if (!noProgressBar) as.sys.closeProgressBar();
                  if (btn) {
                      btn.removeClass('disabled');
                      btn.removeAttr('disabled');
                  }
              },
              error: function (jqXHR, textStatus, errorThrown) {

                  if (as.sys.logJSError) {
                      as.sys.logJSError(url + JSON.stringify(params) + ": " + textStatus + ", " + errorThrown);
                  }
              }
          });
      }

};
