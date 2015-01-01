(function ($) {
    $.fn.checkFileTypeAndSize = function (options) {

        //默认设置
        var defaults = {
            allowedExtensions: [],
            maxSize: 1024, //单位是KB，默认最大文件尺寸1MB=1024KB
            success: function () { },
            extensionerror: function () { },
            sizeerror: function () { }
        };

        //合并设置
        options = $.extend(defaults, options);

        //遍历元素
        return this.each(function () {

            $(this).on('change', function () {

                //获取文件路径
                var filePath = $(this).val();

                //小写字母的文件路径
                var fileLowerPath = filePath.toLowerCase();

                //获取文件的后缀名
                var extension = fileLowerPath.substring(fileLowerPath.lastIndexOf('.') + 1);

                //判断后缀名是否包含在预先设置的、所允许的后缀名数组中
                if ($.inArray(extension, options.allowedExtensions) == -1) {
                    options.extensionerror();
                    $(this).focus();
                } else {
                    try {
                        var size = 0;

                        if ($.browser.msie) {//ie旧版浏览器
                            var fileMgr = new ActiveXObject("Scripting.FileSystemObject");
                            var fileObj = fileMgr.getFile(filePath);
                            size = fileObj.size; //byte
                            size = size / 1024;//kb
                            //size = size / 1024;//mb
                        } else {//其它浏览器
                            size = $(this)[0].files[0].size;//byte
                            size = size / 1024;//kb
                            //size = size / 1024;//mb
                        }

                        if (size > options.maxSize) {
                            options.sizeerror();
                        } else {
                            options.success();
                        }

                    } catch (e) {
                        alert("错误：" + e);
                    }


                }
            });
        });
    };
})(jQuery);