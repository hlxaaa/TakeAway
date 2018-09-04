var rsId;//riderStockId 全局变量

$(document).ready(function () {
    $('#li-rider').addClass('li-active');

    changePage(1)

    $('body').delegate('.btn-edit', 'click', function () {
        loading();
        var e = $(this);
        e.addClass('btn-hide');
        e.next().removeClass('btn-hide');
        var foodId = e.parent().find('.foodId').val();
        var foodName = e.parent().parent().find('.td-name').html();
        var data = {
            riderId: riderId,
            areaId: areaId
        }

        jQuery.axpost('../foodAjax/getRiderStockOther', JSON.stringify(data), function (data) {
            var res = data.data;
            var json = JSON.parse(res);
            if (json.data == "") {

            }

            var h1 = '<select class="form-control inputNotEmpty">'
            h1 += '<option value="' + foodId + '">' + foodName + '</option>'
            if (json.data != "") {
                for (var i = 0; i < json.length; i++) {
                    h1 += '<option value="' + json[i].id + '">' + json[i].foodName + '</option>'
                }
            }
            h1 += '</select>'
            e.parent().parent().find('.td-name').html(h1);

            var amount = e.parent().parent().find('.td-amount').html();
            var h2 = '<input class="form-control onlyInt inputNotEmpty" value="' + amount + '"/>';
            e.parent().parent().find('.td-amount').html(h2);
            addOnlyInt();
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    url: '/food/GetRiderStockOther',
        //    cache: false,
        //    success: function (res) {

        //        //}
        //    },
        //    error: function (res) {
        //        alert('error');
        //    }
        //})
        layer.close(loadingElement);
    })

    $('body').delegate('.btn-save', 'click', function () {
        loading();
        var riderStockId = $(this).parent().find('.riderStockId').val();
        var tr = $(this).parent().parent();
        var selector = tr.find('.td-name select');
        var input = tr.find('.td-amount input');

        var foodId = selector.val();
        var amount = input.val();
        var data = {
            id: riderStockId,
            foodId: foodId,
            amount: amount
        }

        jQuery.axpost('../riderStockAjax/update', JSON.stringify(data), function (data) {
            var res = data.data;
            layer.msg('更新成功');
            changePage(1);
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    url: '/riderstock/update',
        //    cache: false,
        //    success: function (res) {


        //    },
        //    error: function (res) {
        //        layer.msg('error');
        //    }
        //})
        layer.close(loadingElement);
    })

    $('.btn-add').click(function () {
        loading();
        var data = {
            riderId: riderId,
            areaId: areaId
        }
        jQuery.axpost('../foodAjax/getriderstockother', JSON.stringify(data), function (data) {
            var res2 = data.data;
            var res = JSON.parse(res2)
            if (res.data == "")
                layer.msg('没有其他菜品了')
            else {
                //debugger;
                var json = res;

                var h1 = '<tr><td></td><td class="td-name"><select class="form-control inputNotEmpty">'
                h1 += '<option value=""></option>'
                for (var i = 0; i < json.length; i++) {
                    h1 += '<option value="' + json[i].id + '">' + json[i].foodName + '</option>'
                }
                h1 += '</select></td><td></td><td class="td-amount"><input class="form-control onlyInt inputNotEmpty" value=""></td><td><input class="riderStockId" type="hidden" value=""><input class="foodId" type="hidden" value=""><button class="btn-save-add btn btn-success">保存</button><button class="btn-del-add btn btn-primary">删除</button></td></tr>'
                $('#main-table tbody').prepend(h1);
            }
        })

        $.ajax({
            type: 'post',
            data: data,
            dataType: 'json',
            url: '/food/GetRiderStockOther',
            cache: false,
            success: function (res) {

                //InputGetStyle();
            },
            error: function (res) {
                alert('error');
            }
        })
        layer.close(loadingElement);
    })

    //-txy
    $('body').delegate('.btn-save-add', 'click', function () {

        var tr = $(this).parent().parent();
        var foodId = tr.find('.td-name select').val();
        var amount = tr.find('.td-amount input').val();
        if (IsInputEmpty(tr)) {
            layer.msg('输入不能为空')
            return;
        }
        if (!IsPositive(tr)) {
            layer.msg('数字格式不正确')
            return;
        }
        loading();
        var data = {
            riderId: riderId,
            foodId: foodId,
            amount: amount
        }

        jQuery.axpost('../riderStockAjax/add', JSON.stringify(data), function (data) {
            var res = data.data;
            layer.msg('添加成功');
            changePage(1);
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    url: '/riderstock/add',
        //    cache: false,
        //    success: function (res) {


        //    },
        //    error: function (res) {
        //        layer.msg('error');
        //    }
        //})
        layer.close(loadingElement);
    })

    $('body').delegate('.btn-del-add', 'click', function () {
        $(this).parent().parent().remove();
    })

    $('.btn-batchDel').click(function () {
        var ids = new Array();
        $('#main-table tbody div[class="icheckbox_minimal checked"]').each(function () {
            ids.push($(this).parent().parent().find('.riderStockId').val());
        })
        //alert(ids);
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

            jQuery.axpost('../riderStockAjax/batchdel', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功')
                changePage(1)
            })

            layer.closeAll('dialog');
        },
            function () {
                layer.closeAll('dialog');
            })
        layer.close(loadingElement);
    })

    $('body').delegate('.btn-del', 'click', function () {
        loading();
        var rsId = $(this).parent().find('.riderStockId').val();
        var data = {
            rsId: rsId
        }
        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {

            jQuery.axpost('../riderStockAjax/delete', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功')
                changePage(1)
            })

            layer.closeAll('dialog');
        },
            function () {
                layer.closeAll('dialog');
            })
        layer.close(loadingElement);
    })

    $('.btnSearch1').click(function () {
        changePage(1);
    })

    $('#div-search input').keydown(function (e) {
        if (e.keyCode == 13)
            $('.btnSearch1').click();
    })
})


function getTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        alert('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="10%"><input type="checkbox"></th><th width="15%">名称</th><th width="15%">主/配菜</th><th width="15%">数量</th><th width="30%">操作</th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {

        //h += '<tr><td><input type="checkbox"></td><td><div class="ellipsis td-foodName">' + json[i].foodName + '</div></td><td>' + json[i].foodPrice + '</td><td>' + json[i].foodTagName + '</td><td>' + isMain + '</td><td>' + json[i].foodText + '</td>  <td><input class="foodId" type="hidden" value="' + json[i].id + '"><button class="btn-edit btn btn-primary">编辑</button><button class="btn-del btn btn-primary">删除</button></td></tr>'

        h += '<tr><td><input type="checkbox"></td><td class="td-name">' + json[i].foodName + '</td><td>' + json[i].mainType + '</td><td class="td-amount">' + json[i].amount + '</td><td><input class="riderStockId" type="hidden" value="' + json[i].id + '"><input class="foodId" type="hidden" value="' + json[i].foodId + '"><button class="btn-edit btn btn-primary">编辑</button><button class="btn-hide btn-save btn btn-success">保存</button><button class="btn-del btn btn-primary">删除</button></td></tr>'
    }
    h += '</tbody></table>';
    $('#main-table').remove();
    $('#table-wapper').append(h);

    var h = getPageHtml(parseInt(pages), parseInt(index))
    $('#content nav').remove();
    $('#content-white').after(h);
    InputGetStyle();
    allselectToggle1();
}

function changePage(page) {
    loading();
    var data = {
        search: $('#div-search input').val(),
        index: page,
        riderId: riderId
    }

    jQuery.axpost('../riderStockAjax/get', JSON.stringify(data), function (data) {
        var res = data.data;
        getTable(res);
    })

    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    url: '/riderStock/get',
    //    cache: false,
    //    success: function (res) {

    //        layer.close(loadingElement);
    //    },
    //    error: function (res) {
    //        layer.close(loadingElement);
    //    }
    //})

}

function allselectToggle1() {
    $('#main-table thead .iCheck-helper').click(function () {
        $('#main-table tbody div[class="icheckbox_minimal checked"] .iCheck-helper').click();
        $('#main-table tbody .iCheck-helper').click();

        $('#main-table thead .iCheck-helper').click(function () {
            if ($(this).parent().attr('class') == 'icheckbox_minimal hover checked') {
                $('#main-table tbody .icheckbox_minimal .iCheck-helper').click();
            }
            else {
                $('#main-table tbody div[class="icheckbox_minimal checked"] .iCheck-helper').click();
            }
        })
    })
}




//理解错了，下面的不需要了
function allselectToggle2() {
    $('#content-white2 thead th .iCheck-helper').click(function () {

        $('#content-white2 tbody div[class="icheckbox_minimal checked"] .iCheck-helper').click();
        $('#content-white2 tbody .iCheck-helper').click();

        $('#content-white2 thead .iCheck-helper').click(function () {
            if ($(this).parent().attr('class') == 'icheckbox_minimal hover checked') {
                $('#content-white2 tbody .icheckbox_minimal .iCheck-helper').click();
            }
            else {
                $('#content-white2 tbody div[class="icheckbox_minimal checked"] .iCheck-helper').click();
            }
        })
    })
}


function getPageHtml2(pages, thePage) {
    if (pages <= 5) {
        h = '<nav2 style="text-align:center;display:block">'
            + '<ul class="pagination">'
        if (thePage == 1)
            h += '<li class="disabled"><a >&laquo;</a></li>'
        else
            h += '<li onclick="getPrePage2()"><a >&laquo;</a></li>'


        for (var i = 1; i < pages + 1; i++) {
            if (i == thePage) {
                h += '<li class="active"><a >' + i + '</a></li>'
                //alert(i);
            }
            else
                h += '<li onclick="getPage2(this)"><a >' + i + '</a></li>'
        }
        if (pages == thePage)
            h += '<li class="disabled" ><a >&raquo;</a></li>'
        else
            h += '<li onclick="getNextPage2()" ><a>&raquo;</a></li>'
        h += '</ul>'
            + '</nav>'
    } else {
        if (thePage <= 2) {
            h = '<nav style="text-align:center;display:block"><ul class="pagination">'
            if (thePage == 1)
                h += '<li class="disabled"><a >&laquo;</a></li>'
            else
                h += '<li onclick="getPrePage2()"><a >&laquo;</a></li>'

            for (var i = 1; i < pages + 1; i++) {
                if (i == pages) {
                    h += '<li><a>...</a></li>'
                }
                if (i <= thePage + 2 || i == pages) {
                    if (i == thePage)
                        h += '<li class="active"><a >' + i + '</a></li>'
                    else
                        h += '<li onclick="getPage2(this)"><a >' + i + '</a></li>'
                }
            }

            if (pages == thePage) {

                h += '<li class="disabled" ><a >&raquo;</a></li>'
            }
            else {

                h += '<li onclick="getNextPage2()" ><a>&raquo;</a></li>'
            }
            h += '</ul></nav>'
        } else {
            if (thePage < 4) {
                h = '<nav style="text-align:center;display:block"><ul class="pagination">'
                if (thePage == 1)
                    h += '<li class="disabled"><a >&laquo;</a></li>'
                else
                    h += '<li onclick="getPrePage2()"><a >&laquo;</a></li>'

                for (var i = 1; i < pages + 1; i++) {
                    if (i == pages && thePage < pages - 3) {
                        h += '<li><a>...</a></li>'
                    }
                    if (i <= thePage + 2 && i >= thePage - 2 || i == pages) {
                        if (i == thePage)
                            h += '<li class="active"><a >' + i + '</a></li>'
                        else
                            h += '<li onclick="getPage2(this)"><a >' + i + '</a></li>'
                    }
                }

                if (pages == thePage) {

                    h += '<li class="disabled" ><a >&raquo;</a></li>'
                }
                else {

                    h += '<li onclick="getNextPage2()" ><a>&raquo;</a></li>'
                }
                h += '</ul></nav>'
            } else {
                h = '<nav style="text-align:center;display:block"><ul class="pagination">'
                if (thePage == 1)
                    h += '<li class="disabled"><a >&laquo;</a></li>'
                else
                    h += '<li onclick="getPrePage2()"><a >&laquo;</a></li>'

                for (var i = 1; i < pages + 1; i++) {
                    if (i == pages && thePage < pages - 3) {
                        h += '<li><a>...</a></li>'
                    }
                    if (i <= thePage + 2 && i >= thePage - 2 || i == pages || i == 1) {
                        if (i == thePage)
                            h += '<li class="active"><a >' + i + '</a></li>'
                        else
                            h += '<li onclick="getPage2(this)"><a >' + i + '</a></li>'
                    }
                    if (i == 1 && thePage > 4) {
                        h += '<li><a>...</a></li>'
                    }
                }

                if (pages == thePage) {

                    h += '<li class="disabled" ><a >&raquo;</a></li>'
                }
                else {

                    h += '<li onclick="getNextPage2()" ><a>&raquo;</a></li>'
                }
                h += '</ul></nav>'
            }
        }
    }

    if (pages < 1) {
        return "";
    }
    return h;
}

function getTable2(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        alert('没有结果');
        return;
    }
    var h = '<table id="main-table2"><thead><tr><th width="10%"><input type="checkbox"></th><th width="15%">附加品名称</th><th width="15%">数量</th><th width="15%">操作</th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {

        //h += '<tr><td><input type="checkbox"></td><td>' + json[i].foodName + '</td><td>' + json[i].amount + '</td><td><input class="stockId" type="hidden" value="' + json[i].id + '"><button class="btn-edit btn btn-primary">编辑</button><button class="btn-del btn btn-primary">删除</button></td></tr>'

        h += '<tr><td><input type="checkbox"></td><td class="td-exName">' + json[i].foodName + '</td><td class="td-exAmount">' + json[i].amount + '</td><td><input class="foodId" type="hidden" value="' + json[i].foodId + '"><input class="reId" type="hidden" value="' + json[i].id + '"><button class="ex-edit btn btn-primary">编辑</button><button class="btn-hide ex-save btn btn-success">保存</button><button class="ex-del btn btn-primary">删除</button></td></tr>'
    }
    h += '</tbody></table>';
    $('#main-table2').remove();
    $('#table-wapper2').append(h);

    var h = getPageHtml2(parseInt(pages), parseInt(index))
    $('#content-white2').parent().find('nav2').remove();
    $('#content-white2').after(h);

    $('#main-table2 input').iCheck({
        checkboxClass: 'icheckbox_minimal',
        radioClass: 'iradio_minimal',
        increaseArea: '20%', // optional
    });

    allselectToggle2();
}