$(document).ready(function () {
    $('#li-area').addClass('li-active');
    changePage(1);
    $('.btn-add').click(function () {
        location.href = '/area/detail';
    })
    $('body').delegate('.btn-edit', 'click', function () {
        var id = $(this).prev().val();
        window.location.href = '/area/detail?areaId=' + id;
    })

    $('body').delegate('.btn-edit2', 'click', function () {
        var id = $(this).parent().find('.areaId').val();
        window.location.href = '/stockrider/index?areaId=' + id;
    })


    $('body').delegate('.btn-delete', 'click', function () {
        loading();
        var id = $(this).parent().find('.areaId').val();

        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {

            var data = {
                areaId: id
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

    $('.btn-batchDel').click(function () {
        var ids = new Array(); $('tbody div[class="icheckbox_minimal checked"]').each(function () {
            ids.push($(this).parent().parent().find('.areaId').val());
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

            jQuery.axpost('../areaajax/batchdel', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功');
                changePage(1)
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/area/BatchDel',
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

function changePage(page) {
    loading();
    var data = {
        search: $('#div-search input').val(),
        index: page,
    }

    jQuery.axpost('../areaajax/get', JSON.stringify(data), function (data) {
        var res = data.data;
        getTable(res);
        layer.close(loadingElement);
    })

    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    url: '/area/Get',
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
        alert('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="2%"><input type="checkbox"></th><th width="15%">区域名称</th><th width="30%">操作</th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        h += '<tr><td><input type="checkbox"></td><td>' + json[i].areaName + '</td><td><input class="areaId" type="hidden" value="' + json[i].id + '"><button class="btn-edit btn btn-primary">编辑区域</button><button class="btn-edit2 btn btn-primary">骑手</button><button class="btn-delete btn btn-primary">删除</button></td></tr>'
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