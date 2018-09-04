//

$(document).ready(function () {
    $('#li-area').addClass('li-active');

    $('.btn-save').click(function () {
        var e = $(this).parent().parent();
        if (IsInputEmpty(e)) {
            layer.msg('输入不能为空');
            return;
        }
        loading();
        var name = $('.ig-areaName input').val()
        var lat1 = $('#lat1').val();
        var lng1 = $('#lng1').val();
        var lat2 = $('#lat2').val();
        var lng2 = $('#lng2').val();
        var no = $('.ig-areaNo input').val();
        if (areaId == '') {
            var data = {
                areaName: name,
                lat1: lat1,
                lat2: lat2,
                lng1: lng1,
                lng2: lng2,
                areaNo: no
            }

            jQuery.axpost('../areaajax/add', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('添加成功', {
                    time: 1000,
                    end: function () {
                        window.location.href = '/area'
                    }
                })
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/area/add',
            //    cache: false,
            //    success: function (res) {

            //    },
            //    error: function (res) {
            //        alert(res);
            //    }
            //})
        } else {
            var data = {
                areaName: name,
                lat1: lat1,
                lat2: lat2,
                lng1: lng1,
                lng2: lng2,
                areaId: areaId,
                areaNo: no
            }

            jQuery.axpost('../areaajax/update', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('更新成功', {
                    time: 1000,
                    end: function () {
                        window.location.href = '/area'
                    }
                })
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/area/update',
            //    cache: false,
            //    success: function (res) {

            //    },
            //    error: function (res) {
            //        alert(res);
            //    }
            //})
        }
        layer.close(loadingElement);
    })

    $('.btn-del').click(function () {
        loading();
        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                areaId: areaId
            }

            jQuery.axpost('../areaajax/delete', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功', {
                    time: 1000,
                    end: function () {
                        window.location.href = '/area'
                    }
                })
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/area/Delete',
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

    $('.ig-areaNo input').change(function () {
        loading();
        var no = $(this).val();
        var data = {
            areaNo: no,
            areaId: areaId
        }

        jQuery.axpost('../areaAjax/isnoexists', JSON.stringify(data), function (data) {
            var res = data.data;
            $('.no-tips').html('');
            if (res == '1') {
                $('.no-tips').html('已存在该编号');
            }
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    url: '/area/isNoExists',
        //    cache: false,
        //    success: function (res) {

        //    },
        //    error: function (res) {
        //        alert(res);
        //    }
        //})
        layer.close(loadingElement);
    })

    $('.ig-areaName input').change(function () {
        loading();
        var name = $(this).val().trim();
        var data = {
            areaName: name,
            areaId: areaId
        }

        jQuery.axpost('../areaajax/isNameExists', JSON.stringify(data), function (data) {
            var res = data.data;
            $('.name-tips').html('');
            if (res == '1') {
                layer.msg('已存在该名称');
                //$('.name-tips').html('已存在该名称');
            }
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    url: '/area/isNameExists',
        //    cache: false,
        //    success: function (res) {

        //    },
        //    error: function (res) {
        //        alert(res);
        //    }
        //})
        layer.close(loadingElement);
    })
})

//在地图上绘制四边形
function init2(y1, x1, y2, x2) {
    var center = new qq.maps.LatLng((y1 + y2) / 2, (x1 + x2) / 2);

    //var map = new qq.maps.Map(document.getElementById("container"), {
    //    center: center,
    //    zoom: 14
    //});
    map.center = center;
    ll1 = new qq.maps.LatLng(y1, x1);
    ll2 = new qq.maps.LatLng(y1, x2);
    ll3 = new qq.maps.LatLng(y2, x2);
    ll4 = new qq.maps.LatLng(y2, x1);
    //设置折线
    polyGon = new qq.maps.Polygon({
        path: [ll1, ll2, ll3, ll4],
        map: map
    });
}