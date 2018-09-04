var isWeek = 2;//0 实时，1周菜品，2全部
var isMain = 2;
var isDeleted = 2;
var weekTag = 0;
var isDeposit = 2;
var isOn = 2;

$(document).ready(function () {
    $('#li-food').addClass('li-active');
    changePage(1);
    $('body').delegate('.btn-edit', 'click', function () {
        var id = $(this).prev().val();
        window.location.href = '/food/detail?FoodId=' + id;
    })
    $('body').delegate('.btn-add', 'click', function () {
        var id = $(this).prev().val();
        window.location.href = '/food/detail';
    })
    $('body').delegate('.btn-addWeek', 'click', function () {
        var id = $(this).prev().val();
        window.location.href = '/food/detail?isweek=1';
    })

    $('body').delegate('.btn-up', 'click', function () {
        loading();
        var id = $(this).parent().find('input').val();
        layer.confirm('确认上架吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                foodId: id
            }

            jQuery.axpost('../foodajax/up', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('上架成功');
                changePage(1)
            })


            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/food/up',
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

    $('body').delegate('.btn-del', 'click', function () {
        loading();
        var id = $(this).parent().find('input').val();
        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                foodId: id
            }

            jQuery.axpost('../foodajax/delete', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('下架成功');
                changePage(1)
            })


            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/food/Delete',
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

    $('body').on('thead .iCheck-helper', 'click', function () {
        //alert(1);
    })

    $('body').delegate('.btn-down', 'click', function () {
        loading();
        var id = $(this).parent().find('input').val();
        var data = {
            foodId: id
        }

        jQuery.axpost('../foodajax/downfood', JSON.stringify(data), function (data) {
            var res = data.data;
            layer.msg('下架成功')
            changePage(1);
        })


        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    url: '/food/downFood',
        //    cache: false,
        //    success: function (res) {

        //    },
        //    error: function () { }
        //})
        layer.close(loadingElement);
    })

    $('body').delegate('.btn-on', 'click', function () {

        loading();
        var id = $(this).parent().find('input').val();
        var data = {
            foodId: id
        }

        jQuery.axpost('../foodajax/upfood', JSON.stringify(data), function (data) {
            var res = data.data;
            layer.msg('上架成功')
            changePage(1);
        })


        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    url: '/food/upFood',
        //    cache: false,
        //    success: function (res) {

        //    },
        //    error: function () { }
        //})
        layer.close(loadingElement);

    })

    $('.btn-batchDel').click(function () {
        var ids = new Array(); $('tbody div[class="icheckbox_minimal checked"]').each(function () {
            ids.push($(this).parent().parent().find('.foodId').val());
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

            jQuery.axpost('../foodajax/batchdel', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg("删除成功");
                changePage(1)
            })


            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/food/BatchDel',
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

    $('.s-main span').click(function () {

        $('.s-main span').removeClass('label-info').addClass('label-default');
        isMain = $(this).find('input').val();
        $(this).removeClass('label-default').addClass('label-info');
        //var page = $('.pagination li[class="active"] a').text();
        //if (page == '')
        //    page = 1;
        changePage(1);

    })

    $('.s-week span').click(function () {
        orderStr = "";
        desc = 0;
        $('.s-week span').removeClass('label-info').addClass('label-default');
        isWeek = $(this).find('input').val();
        //if (isWeek == 1)
        //    //$('.t-isOn').css('display', 'block')
        //    $('.t-isOn span').attr('disabled', 'true')
        //else
        //    $('.t-isOn span').attr('disabled', 'false')
        //    //$('.t-isOn').css('display', 'none')
        $(this).removeClass('label-default').addClass('label-info');
        changePage(1);
    })

    $('.s-deposit span').click(function () {
        $('.s-deposit span').removeClass('label-info').addClass('label-default');
        isDeposit = $(this).find('input').val();
        $(this).removeClass('label-default').addClass('label-info');
        changePage(1);
    })

    $('.s-isOn span').click(function () {
        //if (isWeek == 1) {
        if (isWeek == 1) {
            $('.s-isOn span').removeClass('label-info').addClass('label-default');
            isOn = $(this).find('input').val();
            $(this).removeClass('label-default').addClass('label-info');

            changePage(1);
        } else
            layer.msg('周菜品才支持上架')
    })

    $('#span-all2').click(function () {
        $('.s-isOn span').removeClass('label-info').addClass('label-default');
        $('#span-all3').removeClass('label-default').addClass('label-info');

    })

    $('#select-tag').change(function () {
        weekTag = $(this).val();
        if (isWeek != 0) {
            changePage(1);
        }
    })

    $('body').delegate('.th-div', 'click', function () {
        var str = $(this);
        var l = str.find('span').length
        if (l > 0) {
            var i = str.attr('class').lastIndexOf('td-')
            var r = str.attr('class').substring(i + 3);
            var descTemp;
            if (desc == 1)
                descTemp = 0
            else
                descTemp = 1;
            desc = orderStr == r ? descTemp : 0;
            orderStr = r;
            changePage(1);
        }
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
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div th-div ellipsis td-foodName" title="名称">名称<span class="glyphicon glyphicon-sort"></div></th><th width="4%"><div class="td-div th-div ellipsis td-foodPrice" title="价格">价格<span class="glyphicon glyphicon-sort"></div></th><th width="4%"><div class="td-div th-div ellipsis td-foodTagName" title="种类">种类</div></th><th width="4%"><div class="td-div th-div ellipsis td-ismain" title="主配菜">主配菜<span class="glyphicon glyphicon-sort"></div></th>'


    h += '<th width="4%"><div class="td-div th-div ellipsis td-deposit" title="押金">押金<span class="glyphicon glyphicon-sort"></div></th><th width="4%"><div class="td-div th-div ellipsis td-foodtext" title="内容">内容</div></th><th width="4%"><div class="td-div th-div ellipsis td-star" title="星级">星级<span class="glyphicon glyphicon-sort"></div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>';

    for (var i = 0; i < json.length; i++) {
        var isMain;
        if (json[i].isMain)
            isMain = '主菜'
        else
            isMain = '配菜'

        var foodName = json[i].foodName;
        var foodPrice = json[i].foodPrice;
        var foodType = json[i].foodTagName;
        var deposit = json[i].deposit
        var content = json[i].foodText;
        var stars = json[i].star;
        var isCycle;
        if (deposit > 0)
            isCycle = '可回收'
        else
            isCycle = '否'
        var isDeleted = json[i].isDeleted;
        h += ' <tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-" title="' + foodName + '">' + foodName + '</div></td><td><div class="td-div ellipsis td-" title="' + foodPrice + '">' + foodPrice + '</div></td><td><div class="td-div ellipsis td-" title="' + foodType + '">' + foodType + '</div></td><td><div class="td-div ellipsis td-" title="' + isMain + '">' + isMain + '</div></td>'

        //< td > <div class="td-div ellipsis td-" title="' + isCycle + '">' + isCycle + '</div></td >

        h += '<td><div class="td-div ellipsis td-" title="' + deposit + '">' + deposit + '</div></td> <td><div class="td-div ellipsis td-" title="' + content + '">' + content + '</div></td> <td><div class="td-div ellipsis td-" title="' + stars + '">' + stars + '</div></td> <td><input class="foodId" type="hidden" value="' + json[i].id + '"><button class="btn-edit btn btn-primary">编辑</button>'

        //if (isDeleted)
        //    h += '<button class="btn btn-primary btn-up">上架</button></td>';
        //else
        if (json[i].isWeek) {
            if (json[i].isOn)
                h += '<button class="btn btn-danger btn-down">下架</button>';
            else
                h += '<button class="btn btn-primary btn-on">上架</button>';
        }
        else {
            h += '<button class="btn btn-primary btn-notOn">上架</button>';
        }

        h += '<button class="btn btn-primary btn-del">删除</button></td>';

        h += '</tr>';
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

function getTableForWeek(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="名称">名称</div></th><th width="4%"><div class="td-div ellipsis td-" title="价格">价格</div></th><th width="4%"><div class="td-div ellipsis td-" title="种类">种类</div></th><th width="4%"><div class="td-div ellipsis td-" title="二级标签">二级标签</div></th><th width="4%"><div class="td-div ellipsis td-" title="主配菜">主配菜</div></th>'

    //< th width= "4%" > <div class="td-div ellipsis td-" title="可回收">可回收</div></th>

    h += '<th width="4%"><div class="td-div ellipsis td-" title="押金">押金</div></th> <th width="4%"><div class="td-div ellipsis td-" title="内容">内容</div></th> <th width="4%"><div class="td-div ellipsis td-" title="星级">星级</div></th> <th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        var isMain;
        if (json[i].isMain)
            isMain = '主菜'
        else
            isMain = '配菜'

        var foodName = json[i].foodName;
        var foodPrice = json[i].foodPrice;
        var foodType = json[i].foodTagName;
        var deposit = json[i].deposit
        var content = json[i].foodText;
        var stars = json[i].star;
        var secondTag = json[i].secondTagName;
        var isCycle;
        var isDeleted = json[i].isDeleted;
        if (deposit > 0)
            isCycle = '可回收'
        else
            isCycle = '否'
        h += ' <tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-" title="' + foodName + '">' + foodName + '</div></td><td><div class="td-div ellipsis td-" title="' + foodPrice + '">' + foodPrice + '</div></td><td><div class="td-div ellipsis td-" title="' + foodType + '">' + foodType + '</div></td><td><div class="td-div ellipsis td-" title="' + secondTag + '">' + secondTag + '</div></td><td><div class="td-div ellipsis td-" title="' + isMain + '">' + isMain + '</div></td>'

        //< td > <div class="td-div ellipsis td-" title="' + isCycle + '">' + isCycle + '</div></td >

        h += '<td><div class="td-div ellipsis td-" title="' + deposit + '">' + deposit + '</div></td> <td><div class="td-div ellipsis td-" title="' + content + '">' + content + '</div></td> <td><div class="td-div ellipsis td-" title="' + stars + '">' + stars + '</div></td> <td><input class="foodId" type="hidden" value="' + json[i].id + '"><button class="btn-edit btn btn-primary">编辑</button>'

        //if (isDeleted)
        //    h += '<button class="btn btn-primary btn-up">上架</button></td>';
        //else
        if (json[i].isOn)
            h += '<button class="btn btn-danger btn-down">下架</button>';
        else
            h += '<button class="btn btn-primary btn-on">上架</button>';
        h += '<button class="btn btn-primary btn-del">删除</button></td>';

        h += '</tr>';
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

    var isMainTemp = null;
    if (isMain == 1)
        isMainTemp = true;
    else if (isMain == 0)
        isMainTemp = false;

    var data = {
        search: $('#div-search input').val(),
        index: page,
        isMain: isMainTemp,
        isWeek: isWeek,
        isOn: isOn,
        weekTag: weekTag,
        isDeposit: isDeposit,
        orderStr: orderStr,
        desc: desc
    }

    jQuery.axpost('../foodajax/get', JSON.stringify(data), function (data) {
        //layer.msg(1);
        var res = data.data;
        //debugger;
        if (isWeek != 1)
            getTable(res);
        else
            getTableForWeek(res);
    })


    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    dataType: 'json',
    //    url: '/food/get',
    //    cache: false,
    //    success: function (res) {

    //        layer.close(loadingElement);
    //    },
    //    error: function (res) {
    //        layer.close(loadingElement);
    //    }
    //})
}
