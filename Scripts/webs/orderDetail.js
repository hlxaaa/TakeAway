var orderId;
var isAdd;
$(document).ready(function () {
    $('#li-order').addClass('li-active');
    GetDateTool('#createTime');
    GetDateTool('#endTime');
    GetDateTool('#payTime');

    $('.btn-del').click(function () {
        //layer.msg(orderId);
        //return;
        loading();
        if (!isAdd) {
            layer.confirm('确认删除吗？', {
                btn: ['确定', '取消']
            }, function () {
                var data = {
                    orderId: orderId
                }

                jQuery.axpost('../orderajax/deleteOrder', JSON.stringify(data), function (data) {
                    var res = data.data;
                    layer.msg('删除成功', {
                        time: 1000,
                        end: function () {
                            location.href = '/order';
                        }
                    });
                })

                //$.ajax({
                //    type: 'post',
                //    data: data,
                //    url: '/order/deleteOrder',
                //    cache: false,
                //    success: function (res) {
                //        //layer.msg('删除成功');


                //    },
                //    error: function (res) {
                //        layer.msg('error');
                //    }
                //})
                layer.closeAll('dialog');
            }, function () {
                layer.closeAll('dialog');
            })
        }
        layer.close(loadingElement);
    })

    $('#select-rider').change(function () {
        loading();
        var riderId = $(this).val();
        var data = {
            riderId: riderId
        }

        jQuery.axpost('../riderajax/getRiderPhone', JSON.stringify(data), function (data) {
            var res = data.data;
            $('#riderPhone').val(res);
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    //dataType:'json',
        //    url: '/rider/getriderphone',
        //    cache: false,
        //    success: function (riderPhone) {

        //    },
        //    error: function (res) { }
        //})
        layer.close(loadingElement);
    })

    $('#userId').change(function () {
        loading();
        var userId = $(this).val();
        var data = {
            userId: userId
        }

        jQuery.axpost('../orderajax/getRecentlyAddress', JSON.stringify(data), function (data) {
            var res = data.data;
            if (ua != null) {
                var address = ua.mapAddress + ua.detail;
                var name = ua.name;
                var phone = ua.phone;
                $('#address').val(address);
                $('#contactName').val(name);
                $('#contactPhone').val(phone);
            } else {
                $('#address').val('');
                $('#contactName').val('');
                $('#contactPhone').val('');
            }
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    dataType: 'json',
        //    url: '/order/GetRecentlyAddress',
        //    cache: false,
        //    success: function (ua) {

        //    },
        //    error: function (res) { }
        //})
        layer.close(loadingElement);
    })

    $('body').delegate('.select-food', 'change', function () {
        loading();
        var e = $(this)
        var div = e.parent();
        var foodId = e.val();
        var data = {
            foodId: foodId
        }

        jQuery.axpost('../Foodajax/getFoodInfo', JSON.stringify(data), function (data) {
            var food2 = data.data;
            var food = JSON.parse(food2);
            div.next().next().find('input').val(food.foodPrice);
            //layer.msg(food.isMain);
            if (food.isMain)
                div.next().next().next().find('input').val("主菜");
            else
                div.next().next().next().find('input').val("配菜");
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    dataType: 'json',
        //    url: '/food/getFoodInfo',
        //    cache: false,
        //    success: function (food) {

        //    },
        //    error: function (res) { }
        //})
        layer.close(loadingElement);
    })

    $('body').delegate('.btn-delFood', 'click', function () {
        var e = $(this);
        var thisIsMain = e.parent().find('.foodIsMain').next().val();
        //layer.msg(thisIsMain);
        var isHaveMain = true;
        if (thisIsMain == '主菜') {
            var count = -1;
            $('.foodWrap').each(function () {
                var isMain = $(this).find('.foodIsMain').next().val()
                if (isMain == '主菜')
                    count++;
            })
            if (count < 1)
                isHaveMain = false;
        }
        if ($('.foodWrap').length > 1 && isHaveMain)
            e.parent().remove();
        else
            layer.msg('至少保留一个主菜');


    })

    $('body').delegate('.btn-addFood', 'click', function () {
        loading();
        var data = {};

        jQuery.axpost('../foodajax/getFoodDictForweb', JSON.stringify(data), function (data) {
            var food2 = data.data;
            var food = JSON.parse(food2)
            var h = '';
            h = '<div class="foodWrap"><div class="input-group fl "><span class="input-group-addon">菜品:</span><select class="form-control select-food">'
            for (var i = 0; i < food.length; i++) {
                h += '<option value="' + food[i].foodId + '">' + food[i].foodName + '</option>'
            }
            var isMain = '配菜'
            if (food[0].isMain)
                isMain = '主菜'
            h += '</select></div><div class="input-group fl "><span class="input-group-addon">数量:</span><input type="number" id="" class="form-control amount" value="1" ></div><div class="input-group fl "><span class="input-group-addon">单价:</span><input readonly="readonly" id="" class="form-control foodPrice" value="' + food[0].foodPrice + '"></div><div class="input-group fl "><span class="input-group-addon foodIsMain">主/配菜:</span><input id="" class="form-control" readonly="readonly" value="' + isMain + '"></div><span class="glyphicon glyphicon-minus-sign fl btn-delFood" style="color: rgb(212, 106, 64);"></span></div>'
            $('.foodWrap').eq(-1).last().after(h);
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    dataType: 'json',
        //    url: '/food/GetFoodDictForWeb',
        //    cache: false,
        //    success: function (food) {

        //    },
        //    error: function (res) {
        //        layer.msg('error');
        //    }
        //})
        layer.close(loadingElement);
    })

    $('.btn-save').click(function () {
        var userId = $('#userId').val();
        if (userId == 0) {
            layer.msg('请选择用户');
            return;
        }
        var createTime = $('#createTime').val();
        if (createTime.trim() == '') {
            layer.msg('请选择订单创建时间');
        }
        var status = $('#select-status').val();
        var isActual = $('#select-isActual').val();
        //layer.msg(isActual);
        var remarks = $('#remarks').val().toString();

        var riderId = $('#select-rider').val();
        if (riderId == 0) {
            layer.msg('请选择骑手');
            return;
        }
        var orderAreaId = $('#areaId').val();
        var endTime = $('#endTime').val();
        var riderComment = $('#comment').val();
        var useBalance = $('#useBalance').val();
        var useCoupon = $('#useCoupon').val();
        var deposit = $('#deposit').val();
        var payTime = $('#payTime').val();
        var payType = $('#payType').val();
        var payMoney = $('#payMoney').val();
        var foodIds = new Array();
        var amounts = new Array();
        var isMains = new Array();
        var foodError = 1;
        $('.foodWrap').each(function () {
            var e = $(this);
            var foodId = e.find('.select-food').val();
            if (foodId == 0) {
                foodError = 0;
            }
            var amount = e.find('.amount').val();
            if (parseInt(amount) <= 0)
                foodError = 3
            foodIds.push(foodId);
            amounts.push(amount);
            isMains.push(e.find('.isMain').val());
        })
        if (foodError == 0) {
            layer.msg('请选择菜品');
            return;
        }
        if (foodError == 3) {
            layer.msg('菜品数量不正确');
            return;
        }
        foodError = 2;
        for (var i = 0; i < isMains.length; i++) {
            if (isMains[i] == '主菜')
                foodError = 1;
        }
        if (foodError == 2) {
            layer.msg('至少选择一个主菜');
            return;
        }


        loading();

        if (!isAdd) {
            var data = {
                userId: userId,
                orderId: orderId,
                createTime: createTime,
                status: status,
                isActual: isActual,
                remarks: remarks,
                riderId: riderId,
                orderAreaId: orderAreaId,
                endTime: endTime,
                riderComment: riderComment,
                useBalance: useBalance,
                useCoupon: useCoupon,
                deposit: deposit,
                payTime: payTime,
                payType: payType,
                payMoney: payMoney,
                foodIds: foodIds,
                amounts: amounts
            }

            jQuery.axpost('../orderajax/update', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('更新成功');
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    //dataType: 'json',
            //    url: '/order/update',
            //    cache: false,
            //    success: function (food) {

            //    },
            //    error: function (res) {
            //        layer.msg('error');
            //    }
            //})
        } else {
            var data = {
                userId: userId,
                createTime: createTime,
                status: status,
                isActual: isActual,
                remarks: remarks,
                riderId: riderId,
                orderAreaId: orderAreaId,
                endTime: endTime,
                riderComment: riderComment,
                useBalance: useBalance,
                useCoupon: useCoupon,
                deposit: deposit,
                payTime: payTime,
                payType: payType,
                payMoney: payMoney,
                foodIds: foodIds,
                amounts: amounts
            }

            jQuery.axpost('../orderajax/add', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('添加成功', {
                    time: 1000,
                    end: function () {
                        location.href = '/order'
                    }
                });
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/order/add',
            //    cache: false,
            //    success: function (food) {

            //    },
            //    error: function (res) {
            //        layer.msg('error');
            //    }
            //})


        }
    })
    layer.close(loadingElement);
})


function GetDateTool(e) {
    $(e).datetimepicker({
        format: 'yyyy-mm-dd hh:ii:ss',
        language: 'ch',
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        forceParse: 0,
        showMeridian: 1
    });
}