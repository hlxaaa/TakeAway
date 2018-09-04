var type;
var riderId;
var isAdd;
$(document).ready(function () {
    $('#li-rider').addClass('li-active');
    //debugger;
    toggleMap(type);

    //$('#riderType3').css('display', 'none');
    $('#select-type').change(function () {
        type = $(this).val();
        toggleMap(type);
    })

    $('.btn-save').click(function () {

        var extraElement = $('#riderType3')
        var e = $('#riderInfo')

        if (IsInputEmpty(e)) {
            layer.msg('输入不能为空')
            return;
        }
        if (!IsPositive(e)) {
            layer.msg('数字格式不正确')
            return;
        }

        var latLng = $('#poi_cur').val();
        var addr = $('#addr_cur').val();
        var type = $('#select-type').val();
        if (type == 2) {

            if (IsInputEmpty(extraElement)) {
                layer.msg('输入不能为空')
                return;
            }
        }
        var account = $('#account').val();
        var pwd = $('#pwd').val();
        var riderName = $('#riderName').val();
        var status = $('#status').val();
        var areaId = $('#select-area').val();
        var stars = $('#stars').val();
        var starCount = $('#starCount').val();
        var sendCount = $('#sendCount').val();
        var phone = $('#riderPhone').val();
        var arr = latLng.split(',');
        if (type == 2 && arr.length < 1) {
            layer.msg('请先选择地址');
            return;
        }
        loading();
        var lat = '';
        var lng = '';
        if (type == 2) {
            lat = arr[0];
            lng = arr[1];
        }


        if (isAdd == 1) {//新增
            var data = {
                lat: lat,
                lng: lng,
                mapAddress: addr,
                riderType: type,
                account: account,
                pwd: pwd,
                riderName: riderName,
                status: status,
                riderAreaId: areaId,
                stars: stars,
                starCount: starCount,
                sendCount: sendCount,
                phone: phone
            }

            jQuery.axpost('../riderajax/add', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('添加成功', {
                    time: 1000,
                    end: function () {
                        location.href = '/rider'
                    }
                });
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    dataType: 'json',
            //    url: '/rider/add',
            //    cache: false,
            //    success: function (res) {
            //        if (res.status) {

            //        } else {
            //            layer.msg(res.message);
            //            layer.close(loadingElement);
            //        }
            //    },
            //    error: function () {
            //        layer.close(loadingElement);
            //    }
            //})
        } else {//更新
            var data = {
                riderId: riderId,
                lat: lat,
                lng: lng,
                mapAddress: addr,
                riderType: type,
                account: account,
                pwd: pwd,
                riderName: riderName,
                status: status,
                riderAreaId: areaId,
                stars: stars,
                starCount: starCount,
                sendCount: sendCount,
                phone: phone
            }
            jQuery.axpost('../riderajax/update', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('更新成功', {
                    time: 1000,
                    end: function () {
                        location.href = '/rider'
                    }
                });
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    dataType: 'json',
            //    url: '/rider/update',
            //    cache: false,
            //    success: function (res) {
            //        if (res.status) {

            //        } else {
            //            layer.msg(res.message);
            //        }
            //    },
            //    error: function () {

            //    }
            //})
            layer.close(loadingElement);
        }
    })
})


function toggleMap(type) {
    if (type == 2)
        $('#riderType3').css('display', 'block');
    else
        $('#riderType3').css('display', 'none');
}

function test() {

    $('#riderType3').css('display', 'none');

}