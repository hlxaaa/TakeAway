var tab = 0;
var thisLv;

$(document).ready(function () {
    $('#li-settings').addClass('li-active');

    $('body').delegate('.btn-del', 'click', function () {
        loading();
        var input = $(this).prev().prev().prev();
        var id = input.val();
        if (id == 0) {
            input.parent().parent().remove();
        } else {
            layer.confirm('确认删除吗？', {
                btn: ['确定', '取消']
            }, function () {
                var data = {
                    id: id
                }

                jQuery.axpost('../settingsajax/delete', JSON.stringify(data), function (data) {
                    var res = data.data;
                    layer.msg('删除成功')
                    changePage();
                })

                //$.ajax({
                //    type: 'post',
                //    data: data,
                //    dataType: 'json',
                //    url: '/settings/delete',
                //    cache: false,
                //    success: function () {

                //    },
                //    error: function () {

                //    }
                //})
                layer.closeAll('dialog');
            }, function () {
                layer.closeAll('dialog');
            })
        }
        layer.close(loadingElement);
    })

    $('body').delegate('.btn-save', 'click', function () {

        var e = $(this).parent().parent();

        if (IsInputEmpty(e)) {
            layer.msg('输入不能为空');
            return;
        }
        if (!IsPositive(e)) {
            layer.msg('数字格式不正确');
            return;
        }
        loading();
        var inputs = e.find('input');
        var account = inputs.eq(1).val();
        var pwd = inputs.eq(2).val();
        var lv = inputs.eq(3).val();

        //layer.msg(parseInt(lv));
        //return;
        var id = inputs.eq(4).val();
        //layer.msg(account + pwd + lv);
        if (id != 0) {
            var data = {
                account: account,
                pwd: pwd,
                lv: lv,
                id: id
            }

            jQuery.axpost('../settingsajax/update', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('更新成功');
                changePage();
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    dataType: 'json',
            //    url: '/settings/update',
            //    cache: false,
            //    success: function (res) {
            //        if (res.status) {

            //        } else
            //            layer.msg(res.message);
            //    },
            //    error: function () { }
            //})
        } else {
            var data = {
                account: account,
                pwd: pwd,
                lv: lv
            }
            jQuery.axpost('../settingsajax/add', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('添加成功');
                changePage();
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    dataType: 'json',
            //    url: '/settings/add',
            //    cache: false,
            //    success: function (res) {
            //        if (res.status) {

            //        } else
            //            layer.msg(res.message);
            //    },
            //    error: function () { }
            //})
        }
        layer.close(loadingElement);
    })

    $('body').delegate('.btn-add', 'click', function () {
        if (tab == 1) {
            var h = '<tr><td><input type="hidden" /></td><td><input class="form-control inputNotEmpty" value=""></td><td><input class="form-control inputNotEmpty" value="123456"></td><td><input class="form-control inputNotEmpty onlyInt" value="1"></td><td><input type="hidden" value="0"><button class="btn btn-success btn-save">确定</button><button class="btn btn-primary btn-edit hide">编辑</button><button class="btn btn-primary btn-del">删除</button></td></tr>';
            $('#main-table tbody tr').last().after(h);
            addOnlyInt();
        }
    })

    $('body').delegate('.btn-edit', 'click', function () {
        var e = $(this).parent().parent();
        var lv = e.find('.td-lv').html();
        lv = parseInt(lv);
        thisLv = parseInt(thisLv);
        if (thisLv < lv) {
            layer.msg('没有权限');
            return;
        }

        var eAccout = e.find('.td-account');
        var ePwd = e.find('.td-pwd');
        var eLv = e.find('.td-lv');
        tdToInput(eAccout, '')
        tdToInput(ePwd, '')
        tdToInput(eLv, 'onlyInt')
        e.find('.btn-save').removeClass('hide');
        e.find('.btn-edit').addClass('hide');
        addOnlyInt();
    })

    $('body').delegate('.btn-updateAll', 'click', function () {
        loading();
        var couponSaveDays = $('#couponSaveDays').val();
        var riderCancelTips = $('#riderCancelTips').val();
        var recoveryBoxTips = $('#recoveryBoxTips').val();
        var orderSendingTips = $('#orderSendingTips').val();
        var orderArrivelTips = $('#orderArrivelTips').val();
        var boxGetTips = $('#boxGetTips').val();
        var serverAssignRiderTips = $('#serverAssignRiderTips').val();
        var shopNewOrderTips = $('#shopNewOrderTips').val();
        var shopNewReserveOrderTips = $('#shopNewReserveOrderTips').val();
        var discount = $('#discount').val();
        var data = {
            couponSaveDays: couponSaveDays,
            riderCancelTips: riderCancelTips,
            recoveryBoxTips: recoveryBoxTips,
            orderSendingTips: orderSendingTips,
            orderArrivelTips: orderArrivelTips,
            boxGetTips: boxGetTips,
            serverAssignRiderTips: serverAssignRiderTips,
            shopNewOrderTips: shopNewOrderTips,
            shopNewReserveOrderTips: shopNewReserveOrderTips,
            discount: discount
        }

        jQuery.axpost('../settingsajax/updateall', JSON.stringify(data), function (data) {
            var res = data.data;
            layer.msg('更新成功', {
                time: 1000,
                end: function () {
                    location.href = '/settings'
                }
            });
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    url: '/settings/Updateall',
        //    cache: false,
        //    success: function (res) {

        //        layer.close(loadingElement);
        //    },
        //    error: function (res) {
        //        layer.close(loadingElement);
        //    }
        //})
    })

    $('.s-tab span').click(function () {
        $('.s-tab span').removeClass('label-info').addClass('label-default');
        tab = $(this).find('input').val();
        $(this).removeClass('label-default').addClass('label-info');

        changePage();
    })

})

function changePage() {
    loading();
    var statusTemp = status;
    var data = {
        tab: tab
    }

    jQuery.axpost('../settingsajax/get', JSON.stringify(data), function (data) {
        var res2 = data.data;
        var res = JSON.parse(res2)
        if (tab == 0) {
            var h = '<div class="input-group"><span class="input-group-addon">二维码有效时间(天):</span><input type="text" class="form-control" id="couponSaveDays" value="' + res.couponSaveDays + '"></div><div class="input-group"><span class="input-group-addon">骑手取消提示词:</span><input type="text" class="form-control" id="riderCancelTips" value="' + res.riderCancelTips + '"></div><div class="input-group"><span class="input-group-addon">提醒骑手回收餐盒提示词:</span><input type="text" class="form-control" id="recoveryBoxTips" value="' + res.recoveryBoxTips + '"></div><div class="input-group"><span class="input-group-addon">订单配送中提示词:</span><input type="text" class="form-control" id="orderSendingTips" value="' + res.orderSendingTips + '"></div><div class="input-group"><span class="input-group-addon">订单送达提示词:</span><input type="text" class="form-control" id="orderArrivelTips" value="' + res.orderArrivelTips + '"></div><div class="input-group"><span class="input-group-addon">餐盒已回收提示词:</span><input type="text" class="form-control" id="boxGetTips" value="' + res.boxGetTips + '"></div><div class="input-group"><span class="input-group-addon">后台指派订单给骑手提示词:</span><input type="text" class="form-control" id="serverAssignRiderTips" value="' + res.serverAssignRiderTips + '"></div><div class="input-group"><span class="input-group-addon">自提点实时订单提示词:</span><input type="text" class="form-control" id="shopNewOrderTips" value="' + res.shopNewOrderTips + '"></div><div class="input-group"><span class="input-group-addon">自提点预订订单提示词:</span><input type="text" class="form-control" id="shopNewReserveOrderTips" value="' + res.shopNewReserveOrderTips + '"></div><div class="input-group"><span class="input-group-addon">优惠券抵扣百分比:</span><input type="text" class="form-control" id="discount" value="' + res.discount + '"></div><div class="fc"></div><div><button class="btn btn-primary btn-updateAll" type="button">更新</button></div>';
            $('#content-white').empty();
            $('#content-white').append(h);
            layer.close(loadingElement);
        } else if (tab == 1) {
            getTable(res2);
            layer.close(loadingElement);
        }
    })

    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    dataType: 'json',
    //    url: '/settings/get',
    //    cache: false,
    //    success: function (res) {


    //        //layer.close(loadingElement);
    //    },
    //    error: function (res) {
    //        layer.close(loadingElement);
    //    }
    //})

}

function getTable(jsonstr) {
    var json = JSON.parse(jsonstr);
    var h = '<div id="table-wapper"><table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="账号">账号</div></th><th width="4%"><div class="td-div ellipsis td-" title="密码">密码</div></th><th width="4%"><div class="td-div ellipsis td-" title="等级">等级</div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        var id = json[i].id;
        var account = json[i].account;
        var pwd = json[i].pwd;
        var level = json[i].level;
        h += ' <tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-account" title="' + account + '">' + account + '</div></td><td><div class="td-div ellipsis td-pwd" title="' + pwd + '">' + pwd + '</div></td><td><div class="td-div ellipsis td-lv" title="' + level + '">' + level + '</div></td><td><input type="hidden" value="' + id + '"/><button class="btn btn-success btn-save hide">确定</button><button class="btn btn-primary btn-edit">编辑</button><button class="btn btn-primary btn-del">删除</button></td></tr>'
    }
    h += '</tbody></table></div>';
    $('#content-white').empty();
    $('#content-white').append(h);

    //var h = getPageHtml(parseInt(pages), parseInt(index))
    //$('#content nav').remove();
    //$('#content-white').after(h);
    InputGetStyle();
    allselectToggle();
}

function tdToInput(e, c) {
    var h = '<input class="form-control inputNotEmpty ' + c + '" value="' + e.html() + '" >'
    var e2 = e.parent()
    e2.empty();
    e2.append(h);
}