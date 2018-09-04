var batch = "0";
var isUse = 2;
var isRepeated = 2;

$(document).ready(function () {
    $('#li-coupon').addClass('li-active');
    //jQuery('#qrcode').qrcode("192.168.2.21:61/coupon/download#shjs12");
    $('.select-batch').change(function () {
        batch = $(this).val();
        changePage(1);
    })
    changePage(1);

    $('.btn-download').click(function () {
        location.href = "/coupon/downcoupon";
    })

    $('.s-isUse span').click(function () {
        $('.s-isUse span').removeClass('label-info').addClass('label-default');
        isUse = $(this).find('input').val();
        $(this).removeClass('label-default').addClass('label-info');

        changePage(1);
    })

    $('.s-isRepeat span').click(function () {
        $('.s-isRepeat span').removeClass('label-info').addClass('label-default');
        isRepeated = $(this).find('input').val();
        $(this).removeClass('label-default').addClass('label-info');

        changePage(1);
    })

    $('body').delegate('.td-couponNo', 'click', function () {
        var no = $(this).html();

        $('#qrcode canvas').remove();
        jQuery('#qrcode').qrcode("https://fan-di.com/coupon/download#" + no);

        $('#modal2').modal();
    })

    $('.btn-add').click(function () {
        $('#modal1').modal();
    })

    $('.btn-save').click(function () {

        var e = $(this).parent();
        if (IsInputEmpty(e)) {
            layer.msg('输入不能为空')
            return;
        }

        if (!IsPositive(e)) {
            layer.msg('数字格式不正确')
            return;
        }


        var name = $('#name').val();
        var amount = $('#amount').val();
        var couponMoney = $('#money').val();
        var isRepeat = $('#isRepeat').val();
        loading();
        var data = {
            name: name,
            amount: amount,
            isRepeat: isRepeat,
            couponMoney: couponMoney
        }

        jQuery.axpost('../couponAjax/createcode', JSON.stringify(data), function (data) {
            var res = data.data;
            layer.msg('添加成功', {
                time: 1000,
                end: function () {
                    location.href = '/coupon';
                }
            })
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    url: '/coupon/CreateCode',
        //    dataType: 'json',
        //    cache: false,
        //    success: function (res) {
        //        if (!res.status) {
        //            layer.msg(res.message)
        //            layer.close(loadingElement);
        //        }
        //        else {

        //        }
        //        //changePage(1);
        //        //$('#modal1').modal('hide')
        //        //layer.close(loadingElement);
        //    },
        //    error: function (res) {
        //        layer.close(loadingElement);
        //    }
        //})

    })

    $('body').delegate('.btn-del', 'click', function () {
        loading();
        var id = $(this).parent().find('input').val();
        layer.confirm('确认作废吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                couponId: id
            }

            jQuery.axpost('../couponAjax/delete', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('已作废');
                changePage(1)
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/coupon/Delete',
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

    $('.btn-batchDel').click(function () {
        var ids = new Array(); $('tbody div[class="icheckbox_minimal checked"]').each(function () {
            ids.push($(this).parent().parent().find('#couponId').val());
        })
        if (ids.length < 1) {
            layer.msg('请先选择')
            return;
        }
        loading();
        layer.confirm('确认作废这些吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                ids: ids
            }
            jQuery.axpost('../couponAjax/batchdel', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('已作废');
                changePage(1)
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/coupon/BatchDel',
            //    cache: false,
            //    success: function (res) {

            //    },
            //    error: function (res) {
            //        alert(res);
            //    }
            //})
            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })
        layer.close(loadingElement);
    })

    $('.btn-delTs').click(function () {

        var ts = $(this).parent().prev().val();
        if (ts != '0') {
            loading();
            layer.confirm('确认作废该批次吗？', {
                btn: ['确定', '取消']
            }, function () {
                var data = {
                    timestamp: ts
                }

                jQuery.axpost('../couponAjax/deletebyts', JSON.stringify(data), function (data) {
                    var res = data.data;
                    layer.msg('已作废');
                    location.href = "/coupon";
                })

                //$.ajax({
                //    type: 'post',
                //    data: data,
                //    url: '/coupon/DeleteByTs',
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
        } else
            layer.msg('请选择批次')
    })

})

function getTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="1%"><div class="td-div ellipsis td-" title="金额">金额</div></th><th width="4%"><div class="td-div ellipsis td-" title="批次">批次</div></th><th width="2%"><div class="td-div ellipsis td-" title="已使用">已使用</div></th><th width="4%"><div class="td-div ellipsis td-" title="兑换码(可点击预览)">兑换码(可点击预览)</div></th><th width="4%"><div class="td-div ellipsis td-" title="创建时间">创建时间</div></th><th width="4%"><div class="td-div ellipsis td-" title="过期时间">过期时间</div></th><th width="4%"><div class="td-div ellipsis td-" title="是否允许重复">是否允许重复</div></th><th width="4%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        var price = json[i].money;
        var name = json[i].name;
        var isUse = json[i].isUse ? '已使用' : '未使用'
        var couponNo = json[i].couponNo;
        var createTime = json[i].createTime;
        var endTime = json[i].endTime;
        var isRepeat = json[i].isRepeat ? '可重复' : '不重复';
        h += '<tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-" title="' + price + '">' + price + '</div></td><td><div class="td-div ellipsis td-" title="' + name + '">' + name + '</div></td><td><div class="td-div ellipsis td-" title="' + isUse + '">' + isUse + '</div></td><td><div class="td-div ellipsis td-couponNo" title="' + couponNo + '">' + couponNo + '</div></td><td><div class="td-div ellipsis td-" title="' + createTime + '">' + createTime + '</div></td><td><div class="td-div ellipsis td-" title="' + endTime + '">' + endTime + '</div></td><td><div class="td-div ellipsis td-" title="' + isRepeat + '">' + isRepeat + '</div></td><td><input id="couponId" type="hidden" value="' + json[i].id + '"/><button class="btn btn-primary btn-del">作废</button></td></tr>'
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

function changePage(page) {
    loading();

    var data = {
        search: $('#div-search input').val(),
        index: page,
        batch: batch,
        isUse: isUse,
        isRepeat: isRepeated
    }

    jQuery.axpost('../couponAjax/get', JSON.stringify(data), function (data) {
        var res = data.data;
        getTable(res);
        layer.close(loadingElement);
    })

    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    url: '/coupon/get',
    //    cache: false,
    //    success: function (res) {

    //    },
    //    error: function (res) {
    //        layer.close(loadingElement);
    //    }
    //})

}


function testdownload() {
    exportCanvasAsPNG($("#qrcode").find("canvas")[0], "qrcoder.png");
};

function exportCanvasAsPNG(canvas, fileName) {
    var MIME_TYPE = "image/png";
    var dlLink = document.createElement('a');
    dlLink.download = fileName;
    dlLink.href = canvas.toDataURL("image/png");
    dlLink.dataset.downloadurl = [MIME_TYPE, dlLink.href].join(':');
    document.body.appendChild(dlLink);
    dlLink.click();
    document.body.removeChild(dlLink);
}


//function test() {
//    loading();
//    var data = {
//        lat: 30,
//        lng: 120
//    }



//    $.ajax({
//        type: 'post',
//        data: data,
//        dataType: 'json',
//        url: '/api/homepage/getarea',
//        cache: false,
//        success: function (res) {
//            layer.msg(res.data[0].areaId);
//        },
//        error: function (res) {

//        }
//    })
//    layer.close(loadingElement);
//}