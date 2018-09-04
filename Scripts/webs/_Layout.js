var pattern = new RegExp("[~'!@#$%^&*()-+_=:]");
var orderStr = '';
var desc = 0;
var loadingElement;
var leTurnPage;


function loading() {
    loadingElement = layer.load(0, { shade: false });
}

function loading2() {
    leTurnPage = layer.load(0, { shade: false });
}

$(document).ready(function () {
    $('.btnSearch').click(function () {
        var e = $('#div-search')
        if (IsCharRight(e))
            changePage(1);
        else
            layer.msg("查询字符非法");
    })

    $('body').delegate('td select', 'change', function () {
        $(this).addClass('changed');
    })
    $('body').delegate('td input', 'change', function () {
        $(this).addClass('changed');
    })

    $('#div-search input').keydown(function (e) {
        if (e.keyCode == 13)
            $('.btnSearch').click();
    })
    loading2();

    $('.icon-exit').click(function () {
        layer.confirm('确认退出吗？', {
            btn: ['确定', '取消']
        }, function () {
            $.ajax({
                type: 'post',
                url: '/platform/LoginOut',
                cache: false,
                success: function (res) {
                    location.reload();
                },
                error: function (res) {

                }
            })
            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })
    })


    $('.btn-back').click(function () {
        history.back();
    })
    //var ids = new Array();
    //ids.push(1);
    //ids.push(2);
    //var data = {
    //    ids: ids
    //}
    //$.ajax({
    //    type: 'post',
    //    data: JSON.stringify(data),
    //    url: '/user/test',
    //    contentType: "application/json; charset=utf-8",
    //    cache: false,
    //    success: function (res) {

    //    },
    //    error: function (res) {

    //    }
    //})


})

//—————输入限制—————

//input是不是空
function IsInputEmpty(e) {
    var r = false;
    e.find('.inputNotEmpty').each(function () {
        var v = $(this).val();
        if (v == "")
            r = true;
    })
    return r;

    //var v = e.find('.inputNotEmpty').val();
    //if (v == '')
    //    return true;
    //return false;
}
//是否是正数
function IsPositive(e) {
    var flag = true;
    e.find('.onlyDecimal').each(function () {
        var v = $(this).val();
        var v2 = parseFloat(v);

        if (v != 0 && v2 == 0)
            flag = false;
        layer.msg(v2);
        if (v2 < 0)
            flag = false;
    })

    var v3 = e.find('.onlyInt').val();
    var v4 = parseInt(v3);
    if (v4 < 0)
        flag = false;
    return flag;

}
//判断有没有非法字符
function IsCharRight(e) {
    var flag = true;
    e.find('input').each(function () {
        var str = $(this).val().trim();
        if (pattern.test(str))
            flag = false;
    })
    return flag;

    //if (search != "" && search != null) {
    //    if (pattern.test(search.trim())) {
    //        alert("非法字符！");
    //        return false;
    //    }
    //}
}

//———————————

function GetDateTool(e) {
    $(e).datetimepicker({
        format: 'yyyy-mm-dd hh:ii:ss',
        language: 'ch',
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        forceParse: 0,
        showMeridian: 1
    });
}

function InputGetStyle() {
    $('input').iCheck({
        checkboxClass: 'icheckbox_minimal',
        radioClass: 'iradio_minimal',
        increaseArea: '20%', // optional
    });
}

function getPrePage() {
    $('.pagination li').each(function () {
        if ($(this).attr('class') == 'active') {
            var a = $(this).children('a').text();
            var pre = parseInt(a) - 1;
            changePage(pre);
            return;
        }
    })
}

function getMidPage(node) {
    var prev = node.previousSibling.innerText
    var next = node.nextSibling.innerText
    var thePage = (parseInt(prev) + parseInt(next)) / 2;
    changePage(parseInt(thePage));
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

function getPage(node) {
    var thePage = node.innerText;
    changePage(thePage);
}

function getPageHtml(pages, thePage) {
    if (pages <= 5) {
        h = '<nav style="text-align:center;display:block">'
            + '<ul class="pagination">'
        if (thePage == 1)
            h += '<li class="disabled"><a >&laquo;</a></li>'
        else
            h += '<li onclick="getPrePage()"><a >&laquo;</a></li>'

        for (var i = 1; i < pages + 1; i++) {
            if (i == thePage) {
                h += '<li class="active"><a >' + i + '</a></li>'
            }
            else
                h += '<li onclick="getPage(this)"><a >' + i + '</a></li>'
        }
        if (pages == thePage)
            h += '<li class="disabled" ><a >&raquo;</a></li>'
        else
            h += '<li onclick="getNextPage()" ><a>&raquo;</a></li>'
        h += '</ul>'
            + '</nav>'
    } else {
        if (thePage <= 2) {
            h = '<nav style="text-align:center;display:block"><ul class="pagination">'
            if (thePage == 1)
                h += '<li class="disabled"><a >&laquo;</a></li>'
            else
                h += '<li onclick="getPrePage()"><a >&laquo;</a></li>'

            for (var i = 1; i < pages + 1; i++) {
                if (i == pages) {
                    h += '<li onclick="getMidPage(this)"><a>...</a></li>'
                }
                if (i <= thePage + 2 || i == pages) {
                    if (i == thePage)
                        h += '<li class="active"><a >' + i + '</a></li>'
                    else
                        h += '<li onclick="getPage(this)"><a >' + i + '</a></li>'
                }
            }

            if (pages == thePage) {

                h += '<li class="disabled" ><a >&raquo;</a></li>'
            }
            else {

                h += '<li onclick="getNextPage()" ><a>&raquo;</a></li>'
            }
            h += '</ul></nav>'
        } else {
            if (thePage < 4) {
                h = '<nav style="text-align:center;display:block"><ul class="pagination">'
                if (thePage == 1)
                    h += '<li class="disabled"><a>&laquo;</a></li>'
                else
                    h += '<li onclick="getPrePage()"><a >&laquo;</a></li>'

                for (var i = 1; i < pages + 1; i++) {
                    if (i == pages && thePage < pages - 3) {
                        h += '<li onclick="getMidPage(this)"><a>...</a></li>'
                    }
                    if (i <= thePage + 2 && i >= thePage - 2 || i == pages) {
                        if (i == thePage)
                            h += '<li class="active"><a >' + i + '</a></li>'
                        else
                            h += '<li onclick="getPage(this)"><a >' + i + '</a></li>'
                    }
                }

                if (pages == thePage) {

                    h += '<li class="disabled" ><a >&raquo;</a></li>'
                }
                else {

                    h += '<li onclick="getNextPage()" ><a>&raquo;</a></li>'
                }
                h += '</ul></nav>'
            } else {
                h = '<nav style="text-align:center;display:block"><ul class="pagination">'
                if (thePage == 1)
                    h += '<li class="disabled"><a >&laquo;</a></li>'
                else
                    h += '<li onclick="getPrePage()"><a >&laquo;</a></li>'

                for (var i = 1; i < pages + 1; i++) {
                    if (i == pages && thePage < pages - 3) {
                        h += '<li onclick="getMidPage(this)"><a>...</a></li>'
                    }
                    if (i <= thePage + 2 && i >= thePage - 2 || i == pages || i == 1) {
                        if (i == thePage)
                            h += '<li class="active"><a >' + i + '</a></li>'
                        else
                            h += '<li onclick="getPage(this)"><a >' + i + '</a></li>'
                    }
                    if (i == 1 && thePage > 4) {
                        h += '<li onclick="getMidPage(this)"><a>...</a></li>'
                    }
                }

                if (pages == thePage) {

                    h += '<li class="disabled" ><a >&raquo;</a></li>'
                }
                else {

                    h += '<li onclick="getNextPage()" ><a>&raquo;</a></li>'
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

function allselectToggle() {
    $('thead .iCheck-helper').click(function () {
        $('tbody div[class="icheckbox_minimal checked"] .iCheck-helper').click();
        $('tbody .iCheck-helper').click();

        $('thead .iCheck-helper').click(function () {
            if ($(this).parent().attr('class') == 'icheckbox_minimal hover checked') {
                $('tbody .icheckbox_minimal .iCheck-helper').click();
            }
            else {
                $('tbody div[class="icheckbox_minimal checked"] .iCheck-helper').click();
            }
        })

    })
}

function getPrePage2() {
    $('.search1 .pagination li').each(function () {
        if ($(this).attr('class') == 'active') {
            var a = $(this).children('a').text();
            var pre = parseInt(a) - 1;
            changePage(pre);
            return false;
        }
    })
}

function getNextPage2() {
    $('.search2 .pagination li').each(function () {
        if ($(this).attr('class') == 'active') {
            var a = $(this).children('a').text();
            var next = parseInt(a) + 1;
            changePage(next);
            return false;
        }
    })
}

function getPage2(node) {
    var thePage = node.innerText;
    changePage2(thePage);
}

function getTableStr(ths, tds) {
    var str = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th>'
    var str2 = '<tr><td><input type="checkbox"></td>'
    for (var i = 0; i < ths.length; i++) {
        str += '<th width="4%"><div class="td-div ellipsis td-" title="' + ths[i] + '">' + ths[i] + '</div></th>'
        str2 += '<td><div class="td-div ellipsis td-" title="' + tds[i] + '">' + tds[i] + '</div></td>'
    }
    str += '<th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>'
    str2 += '<td><input type="hidden" value=""/><button class="btn btn-primary">编辑</button><button class="btn btn-primary">删除</button></td></tr>'
    str += str2;
    str += '</tbody></table>'
    return str;
}

function tool() {
    layer.confirm(str, {
        btn: ['确定', '取消'] //按钮
    }, function () {
        a
    }, function () {
        b
    });
}

function OrderJs(str) {
    var h = '<span class="glyphicon glyphicon-sort"></span>';
    str.append(h);
    return;

    $('.th-div').each(function () {
        //layer.msg($(this).class);
        //layer.msg(str.class);
        if ($(this).attr('class') != str.attr('class'))
            $(this).find('span').remove();
    })
    var e = str.find('span');
    var l = e.length;
    if (l < 1) {
        var h = '<span class="glyphicon glyphicon-chevron-down"></span>';
        $(str).append(h);
    } else {
        if (e.hasClass('glyphicon-chevron-down')) {
            e.removeClass('glyphicon-chevron-down')
            e.addClass('glyphicon-chevron-up');
            desc = 1;
        } else {
            e.removeClass('glyphicon-chevron-up');
            e.addClass('glyphicon-chevron-down')
            desc = 0;
        }
    }
    var i = str.attr('class').lastIndexOf('td-')
    var r = str.attr('class').substring(i + 3);
    orderStr = r;
}


function test() {
    var data = {
        str: 1,
        str2: 2
    };
    $.ajax({
        type: 'post',
        data: data,
        dataType: 'json',
        url: '/user/test7',
        cache: false,
        success: function (res) {
            //alert(1)
            alert(res)
            alert(res.message)
        },
        error: function (res) {
            alert(res);
        }
    })
}

function test3() {
    var name = Array("aaa", "bbb");
    alert(name[0]);
}


function getPageHtml333333(pages, thePage) {
    var h = '';
    if (pages <= 5) {
        if (thePage == 1)
            h += ` <li class="paginate_button disabled">
                                <a href=""><i class="fa fa-angle-left"></i></a>
                            </li>`
        else
            h += ` <li class="paginate_button " onclick="getPrePage()">
                                <a href=""><i class="fa fa-angle-left"></i></a>
                            </li>`

        for (var i = 1; i < pages + 1; i++) {
            if (i == thePage) {
                h += `<li class="paginate_button active"><a>${i}</a></li>`
            }
            else
                h += `<li class="paginate_button" onclick="getPage(this)"><a>${i}</a></li>`
        }
        if (pages == thePage)
            h += `    <li class="paginate_button disabled">
                                <a><i class="fa fa-angle-right"></i></a>
                            </li>`
        else
            h += `    <li class="paginate_button " onclick="getNextPage()">
                                <a><i class="fa fa-angle-right"></i></a>
                            </li>`
    } else {
        if (thePage <= 2) {
            //h = '<nav style="text-align:center;display:block"><ul class="pagination">'
            if (thePage == 1)
                h += ` <li class="paginate_button disabled">
                                <a href=""><i class="fa fa-angle-left"></i></a>
                            </li>`
            else
                h += ` <li class="paginate_button " onclick="getPrePage()">
                                <a href=""><i class="fa fa-angle-left"></i></a>
                            </li>`

            for (var i = 1; i < pages + 1; i++) {
                if (i == pages) {
                    //h += '<li onclick="getMidPage(this)"><a>...</a></li>'
                    h += `<li class="paginate_button" onclick="getMidPage(this)"><a>...</a></li>`
                }
                if (i <= thePage + 2 || i == pages) {
                    if (i == thePage)
                        //h += '<li class="active"><a >' + i + '</a></li>'
                        h += `<li class="paginate_button active"><a>${i}</a></li>`
                    else
                        //h += '<li onclick="getPage(this)"><a >' + i + '</a></li>'
                        h += `<li class="paginate_button" onclick="getPage(this)"><a>${i}</a></li>`
                }
            }

            if (pages == thePage)
                h += `    <li class="paginate_button disabled">
                                <a><i class="fa fa-angle-right"></i></a>
                            </li>`
            else
                h += `    <li class="paginate_button " onclick="getNextPage()">
                                <a><i class="fa fa-angle-right"></i></a>
                            </li>`
            //h += '</ul></nav>'
        } else {
            if (thePage < 4) {
                //h = '<nav style="text-align:center;display:block"><ul class="pagination">'
                //if (thePage == 1)
                //    h += '<li class="disabled"><a>&laquo;</a></li>'
                //else
                //    h += '<li onclick="getPrePage()"><a >&laquo;</a></li>'
                if (thePage == 1)
                    h += ` <li class="paginate_button disabled">
                                <a href=""><i class="fa fa-angle-left"></i></a>
                            </li>`
                else
                    h += ` <li class="paginate_button " onclick="getPrePage()">
                                <a href=""><i class="fa fa-angle-left"></i></a>
                            </li>`

                for (var i = 1; i < pages + 1; i++) {
                    if (i == pages && thePage < pages - 3) {
                        //h += '<li onclick="getMidPage(this)"><a>...</a></li>'
                        h += `<li class="paginate_button" onclick="getMidPage(this)"><a>...</a></li>`
                    }
                    if (i <= thePage + 2 && i >= thePage - 2 || i == pages) {
                        //if (i == thePage)
                        //    h += '<li class="active"><a >' + i + '</a></li>'
                        //else
                        //    h += '<li onclick="getPage(this)"><a >' + i + '</a></li>'
                        if (i == thePage) {
                            h += `<li class="paginate_button active"><a>${i}</a></li>`
                        }
                        else
                            h += `<li class="paginate_button" onclick="getPage(this)"><a>${i}</a></li>`
                    }
                }
                if (pages == thePage)
                    h += `    <li class="paginate_button disabled">
                                <a><i class="fa fa-angle-right"></i></a>
                            </li>`
                else
                    h += `    <li class="paginate_button " onclick="getNextPage()">
                                <a><i class="fa fa-angle-right"></i></a>
                            </li>`
                //if (pages == thePage) {

                //    h += '<li class="disabled" ><a >&raquo;</a></li>'
                //}
                //else {

                //    h += '<li onclick="getNextPage()" ><a>&raquo;</a></li>'
                //}
                //h += '</ul></nav>'
            } else {
                //h = '<nav style="text-align:center;display:block"><ul class="pagination">'
                if (thePage == 1)
                    h += ` <li class="paginate_button disabled">
                                <a href=""><i class="fa fa-angle-left"></i></a>
                            </li>`
                else
                    h += ` <li class="paginate_button " onclick="getPrePage()">
                                <a href=""><i class="fa fa-angle-left"></i></a>
                            </li>`

                for (var i = 1; i < pages + 1; i++) {
                    if (i == pages && thePage < pages - 3) {
                        h += `<li class="paginate_button" onclick="getMidPage(this)"><a>...</a></li>`
                    }
                    if (i <= thePage + 2 && i >= thePage - 2 || i == pages || i == 1) {
                        if (i == thePage) {
                            h += `<li class="paginate_button active"><a>${i}</a></li>`
                        }
                        else
                            h += `<li class="paginate_button" onclick="getPage(this)"><a>${i}</a></li>`
                    }
                    if (i == 1 && thePage > 4) {
                        h += `<li class="paginate_button" onclick="getMidPage(this)"><a>...</a></li>`
                    }
                }

                if (pages == thePage)
                    h += `    <li class="paginate_button disabled">
                                <a><i class="fa fa-angle-right"></i></a>
                            </li>`
                else
                    h += `    <li class="paginate_button " onclick="getNextPage()">
                                <a><i class="fa fa-angle-right"></i></a>
                            </li>`

                //if (pages == thePage) {

                //    h += '<li class="disabled" ><a >&raquo;</a></li>'
                //}
                //else {

                //    h += '<li onclick="getNextPage()" ><a>&raquo;</a></li>'
                //}
                //h += '</ul></nav>'
            }
        }
    }
    if (pages < 1) {
        return "";
    }
    return ` <ul class="pagination">${h}</ul>`;
}