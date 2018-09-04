$(document).ready(function () {
    $('#li-area').addClass('li-active');
    //changePage(1)
    changePage2(1)

    $('#select-area').change(function () {
        var id = $(this).val();
        location.href = '/stockrider?areaId=' + id;
    })

    $('#content-white2 thead .iCheck-helper').click(function () {
        alert(1);
    })

    $('body').delegate('.stock-edit', 'click', function () {
        var a = $(this).parent().parent().find('.td-alive')
        var amount = a.html();
        a.html('<input class="form-control" value=' + amount + ' />')
        $(this).addClass('btn-hide');
        $(this).next().removeClass('btn-hide');
    })

    $('body').delegate('.rider-edit', 'click', function () {
        loading();
        var a = $(this).parent().parent().find('.td-alive2')
        var sourceId = $(this).parent().find('.riderId').val();
        $(this).addClass('btn-hide');
        $(this).next().removeClass('btn-hide');
        var data = {
            areaId: areaId
        }

        jQuery.axpost('../riderajax/getotherrider', JSON.stringify(data), function (data) {
            var res = data.data;
            var json = JSON.parse(res);
            var h = '<select class="form-control">'
            h += '<option value="' + sourceId + '">' + a.html() + '</option>';
            for (var i = 0; i < json.length; i++) {
                h += '<option value="' + json[i].id + '">' + json[i].name + '</option>'
            }
            h += '</select>';
            a.html(h);
        })

    })

    $('body').delegate('.stock-save', 'click', function () {
        loading();
        var a = $(this).parent().parent().find('.td-alive')
        var amount = a.find('input').val();

        var stockId = $(this).parent().find('.stockId').val();
        var data = {
            stockId: stockId,
            amount: amount
        }
        var ele = $(this);

        jQuery.axpost('../stockriderajax/update', JSON.stringify(data), function (data) {
            var res = data.data;
            a.html(amount);
            ele.addClass('btn-hide');
            ele.prev().removeClass('btn-hide');
        })

    })

    $('body').delegate('.rider-save', 'click', function () {
        loading();
        var a = $(this).parent().parent().find('.td-alive2')
        var riderNo = a.find('select').find("option:selected").text();
        var ele = $(this);
        var sourceId = $(this).parent().find('.riderId').val();
        var newId = $(this).parent().parent().find('.td-alive2 select').val();
        if (sourceId == newId) {
            a.html(riderNo);
            $(this).addClass('btn-hide');
            $(this).prev().removeClass('btn-hide');
        } else {
            var data = {
                sourceId: sourceId,
                riderId: newId,
                areaId: areaId
            }
            jQuery.axpost('../riderajax/change', JSON.stringify(data), function (data) {
                var res = data.data;
                a.html(riderNo);
                ele.addClass('btn-hide');
                ele.prev().removeClass('btn-hide');
            })
        }

    })

    $('.rider-add').click(function () {
        loading();
        var data = {
            areaId: areaId
        }

        jQuery.axpost('../riderajax/getOtherRider', JSON.stringify(data), function (data) {
            var res = data.data;
            var json = JSON.parse(res);
            if (json.length < 1)
                layer.msg('没有其他骑手了');
            else {
                var h = '<tr><td></td><td class="td-alive2"><select class="form-control" >'
                for (var i = 0; i < json.length; i++) {
                    h += '<option value="' + json[i].id + '">' + json[i].name + '</option>'
                }
                h += '</select></td><td><input class="riderId" type="hidden" value=""><button class=" rider-save-add btn btn-success">保存</button><button class="btn-del btn btn-primary">移除</button></td></tr>'
                $('#main-table2 tbody').prepend(h);
            }
        })


    })

    $('body').delegate('.rider-save-add', 'click', function () {
        loading();
        var riderId = $(this).parent().parent().find('select').val();
        var data = {
            riderId: riderId,
            areaId: areaId
        }

        jQuery.axpost('../riderajax/changearea', JSON.stringify(data), function (data) {
            var res = data.data;
            changePage2(1)
        })
    })

    //删除新添加的tr
    $('body').delegate('.btn-del', 'click', function () {
        $(this).parent().parent().remove();
    })

    //删除一条库存记录
    $('body').delegate('.stock-del', 'click', function () {
        loading();
        var stockId = $(this).parent().find('.stockId').val();

        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                stockId: stockId
            }
            jQuery.axpost('../stockriderajax/delete', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功')
                changePage(1);
            })


            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })
        layer.close(loadingElement);
    })

    $('body').delegate('.rider-del', 'click', function () {
        loading();
        var riderId = $(this).parent().find('.riderId').val();
        layer.confirm('确认移除吗？', {
            btn: ['确定', '取消']
        }, function () {

            var data = {
                riderId: riderId
            }

            jQuery.axpost('../riderajax/removeArea', JSON.stringify(data), function (data) {
                var res = data.data;
                changePage2(1);
            })


            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })

    })

    $('.stock-batchDel').click(function () {
        var ids = new Array(); $('#main-table tbody div[class="icheckbox_minimal checked"]').each(function () {
            ids.push($(this).parent().parent().find('.stockId').val());
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
            jQuery.axpost('../stockriderajax/batchdel', JSON.stringify(data), function (data) {
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

    //批量移除区域中的骑手
    $('.rider-batchDel').click(function () {
        var ids = new Array(); $('#main-table2 tbody div[class="icheckbox_minimal checked"]').each(function () {
            ids.push($(this).parent().parent().find('.riderId').val());
        })
        if (ids.length < 1) {
            layer.msg('请先选择')
            return;
        }

        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                ids: ids
            }

            jQuery.axpost('../riderajax/batchRemoveArea', JSON.stringify(data), function (data) {
                var res = data.data;
                changePage2(1)
            })


            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })

    })

    $('.btnSearch1').click(function () {
        changePage(1);
    })

    //不用了
    $('.btnSearch2').click(function () {
        changePage2(1);
    })

    $('body').delegate('.rider-stock', 'click', function () {
        var id = $(this).parent().find('.riderId').val();
        location.href = '/riderstock?riderId=' + id;
    })
})

function getTable2(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        alert('没有结果');
        return;
    }
    var h = '<table id="main-table2"><thead><tr><th width="10%"><input type="checkbox"></th><th width="15%">骑手名称</th><th width="15%">操作</th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        h += '<tr><td><input type="checkbox"></td><td class="td-alive2">' + json[i].name + '</td><td><input class="riderId" type="hidden" value="' + json[i].id + '"><button class="rider-edit btn btn-primary">编辑</button><button class="btn-hide rider-save btn btn-success">保存</button><button class="rider-stock btn btn-primary">库存</button><button class="rider-del btn btn-primary">移除</button></td></tr>'
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

function changePage2(page) {
    loading();
    var data = {
        search: $('#div-search2 input').val(),
        index: page,
        areaId: areaId
    }
    jQuery.axpost('../riderajax/getByAreaId', JSON.stringify(data), function (data) {
        var res = data.data;
        getTable2(res);
        layer.close(loadingElement);
    })
    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    url: '/rider/getByAreaId',
    //    cache: false,
    //    success: function (res) {
    //        getTable2(res);
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
    return h;
}