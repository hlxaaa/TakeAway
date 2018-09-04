var isAdd = false;
var tagType = 2;
$(document).ready(function () {
    $('#li-foodtag').addClass('li-active');

    changePage(1);

    $('.btn-add').click(function () {
        isAdd = true;
        $('#modal1').modal();
        $('#tagName').val('')
        var temp = tagType == 2 ? 0 : tagType;
        $('#isWeek').val(temp);
    })

    $('body').delegate('.btn-edit', 'click', function () {
        isAdd = false;
        $('#modal1').modal();
        var name = $(this).parent().parent().find('.typeName').html();
        var id = $(this).parent().find('.typeId').val();
        var tagType = $(this).parent().parent().find('.isWeek').html();
        //layer.msg(tagType);
        if (tagType == '普通种类')
            $('#isWeek').val(0);
        else
            $('#isWeek').val(1);
        $('#tagName').val(name);

        $('#m-typeId').val(id);
    })

    $('.btn-save').click(function () {

        if (IsInputEmpty($(this).parent())) {
            layer.msg('输入不能为空');
            return;
        }
        loading();
        var name = $('#tagName').val();
        var isWeek = $('#isWeek').val();
        if (isAdd) {
            var data = {
                foodTagName: name,
                isWeek: isWeek
            }

            jQuery.axpost('../foodtagAjax/add', JSON.stringify(data), function (data) {
                var res = data.data;

                layer.msg('添加成功')
                changePage(1);
                $('#modal1').modal('hide')

            })

        } else {
            var id = $('#m-typeId').val();
            var data = {
                foodTagId: id,
                foodTagName: name,
                isWeek: isWeek
            }

            jQuery.axpost('../foodtagajax/update', JSON.stringify(data), function (data) {
                var res = data.data;

                layer.msg('更新成功')
                changePage(1);
                $('#modal1').modal('hide')

            })
        }
    })

    $('body').delegate('.btn-delete', 'click', function () {
        loading();
        var id = $(this).parent().find('.typeId').val();

        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                foodTagId: id
            }

            jQuery.axpost('../foodtagajax/delete', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功');
                changePage(1);
            })


            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })
        layer.close(loadingElement);
    })

    $('.btn-batchDel').click(function () {
        var ids = new Array();
        $('tbody div[class="icheckbox_minimal checked"]').each(function () {
            ids.push($(this).parent().parent().find('.typeId').val());
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

            jQuery.axpost('../foodtagajax/batchdel', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功')
                changePage(1)
            })


            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })
        layer.close(loadingElement);
    })

    $('.s-tagType span').click(function () {
        var e = $(this).html().toString()
        if ($(this).hasClass('label-default')) {
            $('.s-tagType .label-info').removeClass('label-info').addClass('label-default');
            $(this).removeClass('label-default').addClass('label-info');
            if (e == '周标签')
                tagType = 1
            else if (e == '普通标签')
                tagType = 0
            else
                tagType = 2;
            changePage(1);

        }

    })

    $('.modal-dialog').keypress(function (e) {
        if (e.keyCode == 13)
            $('.btn-save').click();
    })
})

function changePage(page) {
    loading();
    var data = {
        search: $('#div-search input').val(),
        index: page,
        tagType: tagType
    }

    jQuery.axpost('../foodTagAjax/get', JSON.stringify(data), function (data) {
        var res = data.data;
        getTable(res);
    })
}

function getTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;

    var ll = json.length;

    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="名称">名称</div></th><th width="4%"><div class="td-div ellipsis td-" title="种类属性">种类属性</div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        var isWeek;
        if (json[i].isWeek)
            isWeek = '周菜单类'
        else
            isWeek = '普通种类'
        var tagName = json[i].foodTagName;
        h += ' <tr><td><input type="checkbox"></td><td><div class="td-div ellipsis typeName" title="' + tagName + '">' + tagName + '</div></td><td><div class="td-div ellipsis isWeek" title="' + isWeek + '">' + isWeek + '</div></td><td><input class="typeId" type="hidden" value="' + json[i].id + '"><button class="btn-edit btn btn-primary">编辑</button><button class="btn btn-primary btn-delete">删除</button></td></tr>'
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