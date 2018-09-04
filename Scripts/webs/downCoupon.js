var isRepeat = 2;
$(document).ready(function () {
    $('#li-coupon').addClass('li-active');
    changePage(1)
})

function changePage(page) {
    loading();

    var data = {
        search: $('#div-search input').val(),
        index: page,
        isRepeat: isRepeat
    }

    jQuery.axpost('../couponAjax/getdownloadList', JSON.stringify(data), function (data) {
        var res = data.data;
        getTable(res);
        layer.close(loadingElement);
    })

    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    url: '/coupon/GetDownloadList',
    //    cache: false,
    //    success: function (res) {

    //    },
    //    error: function (res) {
    //        layer.close(loadingElement);
    //    }
    //})

}

function getTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div th-div ellipsis td-name" title="名称">名称</div></th><th width="4%"><div class="td-div th-div ellipsis td-isRepeat" title="是否允许重复">是否允许重复</div></th><th width="4%"><div class="td-div th-div ellipsis td-createTime" title="创建时间">创建时间</div></th><th width="4%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        var name = json[i].name;
        var url = json[i].url;
        var isRepeat = '';
        if (json[i].isRepeat)
            isRepeat = "是"
        else
            isRepeat = '否'
        var createTime = json[i].createTime;
        h += '  <tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-" title="' + name + '">' + name + '</div></td><td><div class="td-div ellipsis td-" title="' + isRepeat + '">' + isRepeat + '</div></td><td><div class="td-div ellipsis td-" title="' + createTime + '">' + createTime + '</div></td><td><a class="btn btn-primary" href="' + url + '">下载</a></td></tr>'
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