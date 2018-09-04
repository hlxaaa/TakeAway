
$(document).ready(function () {
    $('#li-food').addClass('li-active');

    $('#btn-addImg').click(function () {
        $('#upImg').val('');
        $('#upImg').click();
    })

    $('.btn-save').click(function () {

        var e = $(this).parent().parent();
        if (IsInputEmpty(e)) {
            layer.msg('输入不能为空')
            return;
        }
        if (!IsPositive(e)) {
            layer.msg('数字格式不正确')
            return;
        }
        var foodStars = $('#stars').val();
        var foodName = $('#foodName').val();
        var foodPrice = $('#foodPrice').val();
        var foodTypeId = $('#foodType select').val();
        var foodText = $('#foodText textarea').val();
        var tempImg = $('.img-show').attr('src');
        if (tempImg == null) {
            layer.msg('请先选择图片')
            return;
        }
        var isMain = $('#isMain select').val();
        var imgName = tempImg.substring(tempImg.lastIndexOf('/') + 1);
        var foodImg = '/Img//food//' + imgName;
        var deposit = $('#deposit').val();
        var isCycle = $('#isCycle select').val();
        var isThisWeek = $('#isThisWeek').val();
        loading();
        if (foodId == '') {//新增
            var data;
            if (isweek == 'False') {
                var data = {
                    foodName: foodName,
                    foodPrice: foodPrice,
                    foodTypeId: foodTypeId,
                    foodText: foodText,
                    foodImg: foodImg,
                    tempImg: tempImg,
                    isMain: isMain,
                    deposit: deposit,
                    isCycle: isCycle,
                    foodStars: foodStars
                }
            } else {
                var data = {
                    foodName: foodName,
                    foodPrice: foodPrice,
                    foodTypeId: foodTypeId,
                    foodText: foodText,
                    foodImg: foodImg,
                    tempImg: tempImg,
                    isMain: isMain,
                    deposit: deposit,
                    isCycle: isCycle,
                    foodStars: foodStars,
                    isThisWeek: isThisWeek,
                    secondTag: $('#secondTag').val()
                }
            }

            jQuery.axpost('../foodajax/add', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('添加成功', {
                    time: 1000,
                    end: function () {
                        location.href = '/food';
                    }
                })//-txy
            })
        }
        else {//更新
            if (isweek == 'False') {
                var data = {
                    foodName: foodName,
                    foodPrice: foodPrice,
                    foodTypeId: foodTypeId,
                    foodText: foodText,
                    foodImg: foodImg,
                    foodId: foodId,
                    tempImg: tempImg,
                    isMain: isMain,
                    foodStars: foodStars,
                    deposit: deposit,

                    isCycle: isCycle
                }
            } else {
                var data = {
                    foodName: foodName,
                    foodPrice: foodPrice,
                    foodTypeId: foodTypeId,
                    foodText: foodText,
                    foodImg: foodImg,
                    foodId: foodId,
                    tempImg: tempImg,
                    isMain: isMain,
                    foodStars: foodStars,
                    deposit: deposit,
                    isCycle: isCycle,
                    isThisWeek: isThisWeek,
                    secondTag: $('#secondTag').val()
                }
            }

            jQuery.axpost('../foodajax/update', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('更新成功', {
                    time: 1000,
                    end: function () {
                        location.href = '/food';
                    }
                })//-txy
            })
        }
        layer.close(loadingElement);
    })

    $('.btn-back').click(function () {
        history.back();
    })

    $('.btn-del').click(function () {

        loading();
        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                foodId: foodId
            }

            jQuery.axpost('../foodajax/delete', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功', {
                    time: 1000,
                    end: function () {
                        location.href = '/food';
                    }
                })//-txy
            })


            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/food/Delete',
            //    cache: false,
            //    success: function (res) {

            //    },
            //    error: function (res) {

            //    }
            //})
            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })
        layer.close(loadingElement);
    })
})

function filechange(event) {
    var obj = document.getElementById("upImg");
    var length = obj.files.length;
    var isPic = true;
    for (var i = 0; i < obj.files.length; i++) {
        var temp = obj.files[i].name;
        var fileTarr = temp.split('.');
        var filetype = fileTarr[fileTarr.length - 1];
        if (filetype != 'png' && filetype != 'jpg' && filetype != 'jpeg') {
            layer.msg('上传文件必须为图片(后缀名为png,jpg,jpeg)');
            isPic = false;
        } else {
            var size = obj.files[i].size / 1024;
            if (parseInt(size) > 2048) {
                layer.msg("图片大小不能超过2MB");
                isPic = false;
            }
        }
        if (!isPic)
            break;
    }
    if (!isPic)
        return;

    $("#formid").ajaxSubmit(function (res) {
        var h = '<img class="img-show" src="' + res + '" alt="img">';
        $('.img-show').remove();
        $('#divImgs').prepend(h);
    });
}