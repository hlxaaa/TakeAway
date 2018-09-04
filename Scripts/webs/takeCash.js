$(document).ready(function () {
    $('#li-takeCash').addClass('li-active');
    changePage(1);

    $('body').delegate('.btn-doTake', 'click', function () {
        loading();
        var upId = $(this).prev().val();

        layer.confirm('确认提现吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                upId: upId
            }

            jQuery.axpost('../takecashajax/dotake', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('提现成功')
                changePage(1)
            })

            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })
        layer.close(loadingElement);
    })
})

function getTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    //debugger;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="2%"><div class="td-div ellipsis td-" title="用户id">用户id</div></th><th width="4%"><div class="td-div ellipsis td-" title="用户姓名">用户姓名</div></th><th width="4%"><div class="td-div ellipsis td-" title="联系方式">联系方式</div></th><th width="2%"><div class="td-div ellipsis td-" title="提现金额">提现金额</div></th><th width="4%"><div class="td-div ellipsis td-" title="提现方式">提现方式</div></th><th width="3%"><div class="td-div ellipsis td-" title="提现账号">提现账号</div></th><th width="4%"><div class="td-div ellipsis td-" title="申请时间">申请时间</div></th><th width="5%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        var userId = json[i].userId;
        var userName = json[i].userName;
        var phone = json[i].userPhone;
        var takeType = json[i].takeTypeName;
        var takeAccount = json[i].takeAccount;
        var takeTime = json[i].createTime;
        var status = json[i].status;
        var money = json[i].money;

        h += '<tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-" title="' + userId + '">' + userId + '</div></td><td><div class="td-div ellipsis td-" title="' + userName + '">' + userName + '</div></td><td><div class="td-div ellipsis td-" title="' + phone + '">' + phone + '</div></td><td><div class="td-div ellipsis td-" title="' + money + '">' + money + '</div></td><td><div class="td-div ellipsis td-" title="' + takeType + '">' + takeType + '</div></td><td><div class="td-div ellipsis td-" title="' + takeAccount + '">' + takeAccount + '</div></td><td><div class="td-div ellipsis td-" title="' + takeTime + '">' + takeTime + '</div></td><td><input class="userPayId" type="hidden" value="' + json[i].id + '"/>'
        if (status)
            h += '<button class="btn btn-primary btn-taked" disabled="disabled">已确认</button>'
        else
            h += '<button class="btn btn-success btn-doTake">确认</button>'
        h += '</td></tr>'
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
    }
    jQuery.axpost('../takecashajax/get', JSON.stringify(data), function (data) {
        var res = data.data;

        getTable(res);
    })

    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    dataType:'json',
    //    url: '/takecash/get',
    //    cache: false,
    //    success: function (res) {
    //        getTable(res);
    //        layer.close(loadingElement);
    //    },
    //    error: function (res) {
    //        layer.close(loadingElement);
    //    }
    //})

}