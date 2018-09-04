var isAdd = false;
var status = 100;
var isActual = 2;
$(document).ready(function () {

    $('#li-order').addClass('li-active');

    changePage(1);

    $('.s-status span').click(function () {
        $('.s-status span').removeClass('label-info').addClass('label-default');
        status = $(this).find('input').val();
        if (status == 3 || status == 5 || status == 7)
            $('.div-right2').css("display", "block");
        else
            $('.div-right2').css("display", "none");
        $(this).removeClass('label-default').addClass('label-info');
        changePage(1);

    })

    $('.s-isActual span').click(function () {
        $('.s-isActual span').removeClass('label-info').addClass('label-default');
        isActual = $(this).find('input').val();
        $(this).removeClass('label-default').addClass('label-info');
        //layer.msg(isActual);
        //isActual = true;
        //if (status == 21) {
        //    isActual = false;
        //    //status=1
        //}
        changePage(1);
    })

    //获取可分配的骑手
    $('body').delegate('.btn-assign', 'click', function () {
        loading();
        var e = $(this);
        var data = {
            orderId: e.prev().val()
        };

        jQuery.axpost('../riderajax/getAssignrider', JSON.stringify(data), function (data) {
            var res2 = data.data;
            var res = JSON.parse(res2)
            var l = res.length
            if (l < 1)
                layer.msg('附近没有骑手')
            else {
                h = '<select class="form-control fl">'
                for (var i = 0; i < res.length; i++) {
                    h += '<option value="' + res[i].riderId + '">' + res[i].riderName + '</option>'
                }
                h += '</select><button class="fl btn-assigned btn btn-success">分配</button><div class="fc"></div>'
                e.parent().append(h);
                e.remove();
            }
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    dataType: 'json',
        //    url: '/rider/GetAssignRider',
        //    cache: false,
        //    success: function (res) {

        //    },
        //    error: function (res) { }
        //})
        layer.close(loadingElement);
    })

    //点击分配
    $('body').delegate('.btn-assigned', 'click', function () {
        loading();
        var e = $(this);
        var riderId = e.prev().val();
        var orderId = e.prev().prev().val();
        var data = {
            orderId: orderId,
            riderId: riderId
        }

        jQuery.axpost('../orderajax/assignRider', JSON.stringify(data), function (data) {
            var res = data.data;
            layer.msg('指派成功');
            changePage(1);
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    dataType: 'json',
        //    url: '/order/AssignRider',
        //    cache: false,
        //    success: function (res) {
        //        if (res.status) {

        //        }
        //        else
        //            layer.msg(res.message);

        //    },
        //    error: function (res) { }
        //})
        layer.close(loadingElement);
    })

    $('body').delegate('.btn-agree', 'click', function () {
        loading();
        var orderId = $(this).prev().val();

        layer.confirm('确定取消吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                orderId: orderId
            }

            jQuery.axpost('../orderajax/AgreeRiderCancel', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('取消成功');
                changePage(1);
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    dataType: 'json',
            //    url: '/order/AgreeRiderCancel',
            //    cache: false,
            //    success: function (res) {
            //        if (res.status) {

            //        } else
            //            layer.msg(res.message);
            //    },
            //    error: function (res) {
            //        layer.msg('网络不稳定');
            //    }
            //})
            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })

        layer.close(loadingElement);

    })

    $('body').delegate('.btn-edit', 'click', function () {
        if (status == 3 || status == 5 || status == 7) {
            var id = $(this).prev().val();
            location.href = '/order/detail?id=' + id;
        }
    })

    $('.btn-add').click(function () {
        location.href = '/order/detail'
    })

    //删除
    $('body').delegate('.btn-del', 'click', function () {

        var orderId = $(this).parent().find('input').val();
        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            loading();
            var data = {
                orderId: orderId
            }

            jQuery.axpost('../orderajax/deleteOrder', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功', {
                    time: 1000,
                    end: function () {
                        var page = $('.pagination li[class="active"] a').text();
                        changePage(page);
                    }
                });
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/order/deleteOrder',
            //    cache: false,
            //    success: function (res) {


            //    },
            //    error: function (res) {
            //        layer.msg('error');
            //    }
            //})
            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })
    })

    //批量删除
    $('.btn-batchDel').click(function () {
        var ids = new Array(); $('tbody div[class="icheckbox_minimal checked"]').each(function () {
            ids.push($(this).parent().parent().find('.orderId').val());
        })
        if (ids.length < 1) {
            layer.msg('请先选择')
            return;
        }
        loading();
        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                ids: ids
            }

            jQuery.axpost('../orderajax/batchdel', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg("删除成功");
                var page = $('.pagination li[class="active"] a').text();
                changePage(page)
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/order/BatchDel',
            //    cache: false,
            //    success: function (res) {

            //    },
            //    error: function (res) {
            //        layer.msg('error');
            //    }
            //})
            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })
        layer.close(loadingElement);
    })
})

function changePage(page) {
    loading();
    var statusTemp = status;
    var data = {
        search: $('#div-search input').val(),
        index: page,
        status: statusTemp,
        isActual: isActual
    }

    jQuery.axpost('../orderajax/get', JSON.stringify(data), function (data) {
        var res = data.data;
        if (status == 0 || status == 100)
            getNotPayTable(res);
        if (status == 1) {
            if (isActual == 0)
                getWaitTable2(res)
            else
                getWaitTable(res);
        }
        if (status == 2) {
            getSendingTable(res);
        }
        if (status == 3)
            getArrivalTable(res);
        if (status == 7)
            getArrivalTable(res);
        if (status == 4)
            getFoodGetTable(res);
        if (status == 5)
            getCancelledTable(res);
        if (status == 99)
            getRiderCancelTable(res);
        if (status == 21)
            getWaitTable2(res);
        if (status == 8 || status == 9)
            getArrivalTable(res);
    })

    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    url: '/order/get',
    //    cache: false,
    //    success: function (res) {

    //        layer.close(loadingElement);
    //    },
    //    error: function (res) {
    //        layer.close(loadingElement);
    //    }
    //})

}

function getNotPayTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    //alert(index);
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="用户">用户</div></th><th width="4%"><div class="td-div ellipsis td-" title="菜单">菜单</div></th><th width="4%"><div class="td-div ellipsis td-" title="区域">区域</div></th><th width="4%"><div class="td-div ellipsis td-" title="创建时间">创建时间</div></th><th width="4%"><div class="td-div ellipsis td-" title="备注">备注</div></th><th width="4%"><div class="td-div ellipsis td-" title="订单状态">订单状态</div></th><th width="4%"><div class="td-div ellipsis td-" title="配送地址">配送地址</div></th><th width="4%"><div class="td-div ellipsis td-" title="联系电话">联系电话</div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>'
    for (var i = 0; i < json.length; i++) {
        var areaName = json[i].areaName == null ? "" : json[i].areaName;
        var phone = json[i].contactPhone;
        phone = phone == null ? "" : phone
        h += '<tr><td><input type="checkbox"></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].userName + '">' + json[i].userName + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].foods + '">' + json[i].foods + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + areaName + '">' + areaName + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].createTime + '">' + json[i].createTime + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].remarks + '">' + json[i].remarks + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].statusType + '">' + json[i].statusType + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].address + '">' + json[i].address + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + phone + '">' + phone + '</div></td>'

        if (json[i].status == 1) {
            h += '<td><div class="div-assign"><input type="hidden" class="orderId" value="' + json[i].orderId + '"/><button class="btn btn-primary btn-assign">指派骑手</button></div></td>'
        }
        else
            h += '<td><input type="hidden" value="' + json[i].orderId + '"/></td></tr>'
    }
    h += '</tbody></table>';
    $('#main-table').remove();
    $('#table-wapper').append(h);

    var h = getPageHtml(parseInt(pages), parseInt(index))
    $('#content nav').remove();
    $('#content-white').after(h);
    InputGetStyle();
    allselectToggle();
}

function getWaitTable2(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="用户">用户</div></th><th width="4%"><div class="td-div ellipsis td-" title="菜单">菜单</div></th><th width="4%"><div class="td-div ellipsis td-" title="备注">备注</div></th><th width="4%"><div class="td-div ellipsis td-" title="订单状态">订单状态</div></th><th width="4%"><div class="td-div ellipsis td-" title="配送地址">配送地址</div></th><th width="4%"><div class="td-div ellipsis td-" title="联系电话">联系电话</div></th><th width="4%"><div class="td-div ellipsis td-" title="支付时间">支付时间</div></th><th width="4%"><div class="td-div ellipsis td-" title="使用余额">使用余额</div></th><th width="4%"><div class="td-div ellipsis td-" title="使用优惠券">使用优惠券</div></th><th width="4%"><div class="td-div ellipsis td-" title="支付方式">支付方式</div></th><th width="4%"><div class="td-div ellipsis td-" title="支付金额">支付金额</div></th><th width="4%"><div class="td-div ellipsis td-" title="送达时间">送达时间</div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>'
    for (var i = 0; i < json.length; i++) {

        var payType = json[i].payType == null ? "" : json[i].payType;
        h += '<tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-" title="' + json[i].userName + '">' + json[i].userName + '</div></td><td><div class="td-div ellipsis td-" title="' + json[i].foods + '">' + json[i].foods + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].remarks + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].statusType + '</div></td><td><div class="td-div ellipsis td-" title=""><div class="td-address ellipsis" title="' + json[i].address + '">' + json[i].address + '</div></div></td><td><div class="td-div ellipsis td-" title="' + json[i].contactPhone + '">' + json[i].contactPhone + '</div></td><td><div class="td-div ellipsis td-" title="' + json[i].payTime + '">' + json[i].payTime + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].useBalance + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].useCoupon + '</div></td><td><div class="td-div ellipsis td-" title="">' + payType + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].payMoney + '</div></td><td><div class="td-div ellipsis td-" title="' + json[i].arriveTime + '">' + json[i].arriveTime + '</div></td><td><div class="div-assign"><input type="hidden" class="orderId" value="' + json[i].orderId + '"/><button class="btn btn-primary btn-assign">指派骑手</button></div></tr>'
    }
    h += '</tbody></table>';
    $('#main-table').remove();
    $('#table-wapper').append(h);

    var h = getPageHtml(parseInt(pages), parseInt(index))
    $('#content nav').remove();
    $('#content-white').after(h);
    InputGetStyle();
    allselectToggle();
}

function getWaitTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="用户">用户</div></th><th width="4%"><div class="td-div ellipsis td-" title="菜单">菜单</div></th><th width="4%"><div class="td-div ellipsis td-" title="区域">区域</div></th><th width="4%"><div class="td-div ellipsis td-" title="送达时间">送达时间</div></th><th width="4%"><div class="td-div ellipsis td-" title="备注">备注</div></th><th width="4%"><div class="td-div ellipsis td-" title="订单状态">订单状态</div></th><th width="4%"><div class="td-div ellipsis td-" title="配送地址">配送地址</div></th><th width="4%"><div class="td-div ellipsis td-" title="联系电话">联系电话</div></th><th width="4%"><div class="td-div ellipsis td-" title="支付时间">支付时间</div></th><th width="4%"><div class="td-div ellipsis td-" title="使用余额">使用余额</div></th><th width="4%"><div class="td-div ellipsis td-" title="使用优惠券">使用优惠券</div></th><th width="4%"><div class="td-div ellipsis td-" title="支付方式">支付方式</div></th><th width="4%"><div class="td-div ellipsis td-" title="支付金额">支付金额</div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>'
    for (var i = 0; i < json.length; i++) {
        var payType = json[i].payType == null ? "" : json[i].payType;
        var areaName = json[i].areaName == null ? "" : json[i].areaName;
        h += '<tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-" title="' + json[i].userName + '">' + json[i].userName + '</div></td><td><div class="td-div ellipsis td-" title="' + json[i].foods + '">' + json[i].foods + '</div></td><td><div class="td-div ellipsis td-" title="' + areaName + '">' + areaName + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].timeArea + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].remarks + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].statusType + '</div></td><td><div class="td-div ellipsis td-" title=""><div class="td-address ellipsis" title="' + json[i].address + '">' + json[i].address + '</div></div></td><td><div class="td-div ellipsis td-" title="' + json[i].contactPhone + '">' + json[i].contactPhone + '</div></td><td><div class="td-div ellipsis td-" title="' + json[i].payTime + '">' + json[i].payTime + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].useBalance + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].useCoupon + '</div></td><td><div class="td-div ellipsis td-" title="">' + payType + '</div></td><td><div class="td-div ellipsis td-" title="">' + json[i].payMoney + '</div></td><td><div class="div-assign"><input type="hidden" class="orderId" value="' + json[i].orderId + '"/><button class="btn btn-primary btn-assign">指派骑手</button></div></tr>'
    }
    h += '</tbody></table>';
    $('#main-table').remove();
    $('#table-wapper').append(h);

    var h = getPageHtml(parseInt(pages), parseInt(index))
    $('#content nav').remove();
    $('#content-white').after(h);
    InputGetStyle();
    allselectToggle();
}

function getSendingTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th>'
        + '<th width="5%">用户</th>'
        + '<th width="5%">菜单</th>'
        + '<th width="5%">备注</th>'
        + '<th width="5%">订单状态</th>'
        + '<th width="5%">配送地址</th>'
        + '<th width="5%">联系电话</th>'
        + '<th width="5%">余额</th>'
        + '<th width="5%">优惠券</th>'
        + '<th width="5%">第三方支付</th>'
        + '<th width="5%">骑手信息</th>'

        + '<th width="20%">操作</th></tr></thead><tbody>';

    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="用户">用户</div></th><th width="4%"><div class="td-div ellipsis td-" title="菜单">菜单</div></th><th width="4%"><div class="td-div ellipsis td-" title="备注">备注</div></th><th width="4%"><div class="td-div ellipsis td-" title="订单状态">订单状态</div></th><th width="4%"><div class="td-div ellipsis td-" title="配送地址">配送地址</div></th><th width="4%"><div class="td-div ellipsis td-" title="联系电话">联系电话</div></th><th width="4%"><div class="td-div ellipsis td-" title="使用余额">使用余额</div></th><th width="4%"><div class="td-div ellipsis td-" title="使用优惠券">使用优惠券</div></th><th width="4%"><div class="td-div ellipsis td-" title="第三方信息">第三方信息</div></th><th width="4%"><div class="td-div ellipsis td-" title="骑手信息">骑手信息</div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>'
    for (var i = 0; i < json.length; i++) {
        h += '<tr><td><input type="checkbox"></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].userName + '">' + json[i].userName + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].foods + '">' + json[i].foods + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].remarks + '">' + json[i].remarks + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].statusType + '">' + json[i].statusType + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].address + '">' + json[i].address + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].contactPhone + '">' + json[i].contactPhone + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].useBalance + '">' + json[i].useBalance + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].useCoupon + '">' + json[i].useCoupon + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].payTypeAndMoney + '">' + json[i].payTypeAndMoney + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].riderInfo + '">' + json[i].riderInfo + '</div></td>'

            + '<td></td></tr>'
    }
    h += '</tbody></table>';
    $('#main-table').remove();
    $('#table-wapper').append(h);

    var h = getPageHtml(parseInt(pages), parseInt(index))
    $('#content nav').remove();
    $('#content-white').after(h);
    InputGetStyle();
    allselectToggle();
}

function getArrivalTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="用户">用户</div></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="菜单">菜单</div></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="备注">备注</div></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="订单状态">订单状态</div></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="配送地址">配送地址</div></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="联系电话">联系电话</div></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="使用余额">使用余额</div></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="使用优惠券">使用优惠券</div></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="第三方支付">第三方支付</div></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="骑手信息">骑手信息</div></th>'
        + '<th width="4%"><div class="td-div ellipsis td-" title="送达时间">送达时间</div></th>'
        + '<th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        h += '<tr><td><input type="checkbox"></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].userName + '">' + json[i].userName + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].foods + '">' + json[i].foods + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].remarks + '">' + json[i].remarks + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].statusType + '">' + json[i].statusType + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].address + '">' + json[i].address + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].contactPhone + '">' + json[i].contactPhone + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].useBalance + '">' + json[i].useBalance + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].useCoupon + '">' + json[i].useCoupon + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].payTypeAndMoney + '">' + json[i].payTypeAndMoney + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].riderInfo + '">' + json[i].riderInfo + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].endTime + '">' + json[i].endTime + '</div></td>'
        if (status != 8 && status != 9)
            h += '<td><input class="orderId" type="hidden" value="' + json[i].orderId + '" /><button class="btn btn-primary btn-edit">编辑</button><button class="btn btn-primary btn-del">删除</button></td></tr>'
    }
    h += '</tbody></table>';
    $('#main-table').remove();
    $('#table-wapper').append(h);

    var h = getPageHtml(parseInt(pages), parseInt(index))
    $('#content nav').remove();
    $('#content-white').after(h);
    InputGetStyle();
    allselectToggle();
}

function getFoodGetTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th>'
        + '<th width="5%">用户</th>'
        + '<th width="5%">菜单</th>'
        + '<th width="5%">订单状态</th>'
        + '<th width="5%">联系电话</th>'
        + '<th width="5%">使用余额</th>'
        + '<th width="5%">使用优惠券</th>'
        + '<th width="5%">第三方支付</th>'
        + '<th width="5%">骑手信息</th>'
        + '<th width="5%">确认时间</th>'
        + '<th width="5%">菜品评价</th>'
        + '<th width="5%">骑手评价</th>'

        + '<th width="20%">操作</th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        h += '<tr><td><input type="checkbox"></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].userName + '">' + json[i].userName + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].foods + '">' + json[i].foods + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].statusType + '">' + json[i].statusType + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].contactPhone + '">' + json[i].contactPhone + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].useBalance + '">' + json[i].useBalance + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].useCoupon + '">' + json[i].useCoupon + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].payTypeAndMoney + '">' + json[i].payTypeAndMoney + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].riderInfo + '">' + json[i].riderInfo + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].endTime + '">' + json[i].endTime + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].foodComment + '">' + json[i].foodComment + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].riderComment + '">' + json[i].riderComment + '</div></td>'


            + '<td></td></tr>'
    }
    h += '</tbody></table>';
    $('#main-table').remove();
    $('#table-wapper').append(h);

    var h = getPageHtml(parseInt(pages), parseInt(index))
    $('#content nav').remove();
    $('#content-white').after(h);
    InputGetStyle();
    allselectToggle();
}

function getCancelledTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th>'
        + '<th width="5%">用户</th>'
        + '<th width="5%">菜品</th>'
        + '<th width="5%">订单状态</th>'
        + '<th width="5%">联系电话</th>'
        + '<th width="5%">取消时间</th>'


        + '<th width="20%">操作</th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        h += '<tr><td><input type="checkbox"></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].userName + '">' + json[i].userName + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].foods + '">' + json[i].foods + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].statusType + '">' + json[i].statusType + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].contactPhone + '">' + json[i].contactPhone + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].endTime + '">' + json[i].endTime + '</div></td>'


            + '<td><input  class="orderId"  type="hidden"  value="' + json[i].orderId + '" /><button class="btn btn-primary btn-edit">编辑</button><button class="btn btn-primary btn-del">删除</button></td></tr>'
    }
    h += '</tbody></table>';
    $('#main-table').remove();
    $('#table-wapper').append(h);

    var h = getPageHtml(parseInt(pages), parseInt(index))
    $('#content nav').remove();
    $('#content-white').after(h);
    InputGetStyle();
    allselectToggle();
}

function getRiderCancelTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    //var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th>'
    //+ '<th width="5%">用户</th>'
    //+ '<th width="5%">菜单</th>'
    //+ '<th width="5%">备注</th>'
    //+ '<th width="5%">订单状态</th>'
    //+ '<th width="5%">配送地址</th>'
    // + '<th width="5%">联系电话</th>'
    //+ '<th width="5%">余额</th>'
    //+ '<th width="5%">优惠券</th>'
    //+ '<th width="5%">第三方支付</th>'
    //+ '<th width="5%">骑手信息</th>'

    //+ '<th width="20%">操作</th></tr></thead><tbody>';
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="用户">用户</div></th><th width="4%"><div class="td-div ellipsis td-" title="菜单">菜单</div></th><th width="4%"><div class="td-div ellipsis td-" title="备注">备注</div></th><th width="4%"><div class="td-div ellipsis td-" title="订单状态">订单状态</div></th><th width="4%"><div class="td-div ellipsis td-" title="配送地址">配送地址</div></th><th width="4%"><div class="td-div ellipsis td-" title="联系电话">联系电话</div></th><th width="4%"><div class="td-div ellipsis td-" title="使用余额">使用余额</div></th><th width="4%"><div class="td-div ellipsis td-" title="使用优惠券">使用优惠券</div></th><th width="4%"><div class="td-div ellipsis td-" title="第三方信息">第三方信息</div></th><th width="4%"><div class="td-div ellipsis td-" title="骑手信息">骑手信息</div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>'
    for (var i = 0; i < json.length; i++) {
        h += '<tr><td><input type="checkbox"></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].userName + '">' + json[i].userName + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].foods + '">' + json[i].foods + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].remarks + '">' + json[i].remarks + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].statusType + '">' + json[i].statusType + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].address + '">' + json[i].address + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].contactPhone + '">' + json[i].contactPhone + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].useBalance + '">' + json[i].useBalance + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].useCoupon + '">' + json[i].useCoupon + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].payTypeAndMoney + '">' + json[i].payTypeAndMoney + '</div></td>'
            + '<td><div class="td-div ellipsis td-" title="' + json[i].riderInfo + '">' + json[i].riderInfo + '</div></td>'

            + '<td><input type="hidden" value="' + json[i].orderId + '"/><button class="btn btn-primary btn-agree">同意</button></td></tr>'
    }
    h += '</tbody></table>';
    $('#main-table').remove();
    $('#table-wapper').append(h);

    var h = getPageHtml(parseInt(pages), parseInt(index))
    $('#content nav').remove();
    $('#content-white').after(h);
    InputGetStyle();
    allselectToggle();
}

function getPage(node) {
    var thePage = node.innerText;
    //alert(status);
    changePage(thePage);
}

function getPrePage() {
    $('.pagination li').each(function () {
        if ($(this).attr('class') == 'active') {

            var a = $(this).children('a').text();
            var pre = parseInt(a) - 1;
            changePage(pre);
            return false;
        }
    })
}

function getNextPage() {
    $('.pagination li').each(function () {
        if ($(this).attr('class') == 'active') {
            var a = $(this).children('a').text();
            var next = parseInt(a) + 1;
            changePage(next);
            return false;
        }
    })
}