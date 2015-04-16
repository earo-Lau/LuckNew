(function ($) {
    var defaultOpt = {
        codeLength: 4,
        url: '',
        validatefor: '',
        state: true
    };

    var code = '';
    var dic = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

    $.fn.verificationCode = function (options) {
        var opt = $.extend(defaultOpt, options);
        return $(this).each(function () {
            code = CreatCode(opt.codeLength);
            this.src = opt.url + "?code=" + code;

            $(this).on('click', function () {
                code = CreatCode(opt.codeLength);
                this.src = opt.url + "?code=" + code;
            });
            var img = $(this);
            var validate = $('#' + opt.validatefor);
            if (validate.length > 0) {

                validate.on('blur', function (e) {
                    var txtCode = $(this).val();
                    $(this).parent().find('.icon-warning, .icon-success').remove();
                    if (txtCode.toLowerCase() == code.toLowerCase()) {
                        img.after('<span class="icon icon-success" title="验证码正确"></span>');
                        defaultOpt = true;
                    }
                    else {
                        img.after('<span class="icon icon-warning" title="验证码错误"></span>');
                        defaultOpt = false;
                    }
                });
            }
        });
    }
    $.fn.verificationCode.State = function () {
        return defaultOpt.state
    }

    function CreatCode(length) {
        var res = "";
        for (var i = 0; i < length ; i++) {
            var id = Math.ceil(Math.random() * 62);
            res += dic[id];
        }

        return res;
    }

    function preImage(url, code, callback) {
        var img = new Image(); //创建一个Image对象，实现图片的预下载
        

        if (img.complete) { // 如果图片已经存在于浏览器缓存，直接调用回调函数
            callback.call(img);
            return; // 直接返回，不用再处理onload事件
        }

        img.onload = function () { //图片下载完毕时异步调用callback函数。
            callback.call(img);//将回调函数的this替换为Image对象
        };
    }

})(jQuery)