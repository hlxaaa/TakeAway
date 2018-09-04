var isAdd = false;
var type = 2;
var status = 2;
$(document).ready(function () {


    $('#li-article').addClass('li-active');

    $('.btn-add').click(function () {
        isAdd = true;
        $('#modal1').modal();
        $('#m-title').val('')
        $('#m-select').val(1);
        $('#divEditor').html('')
        $('#select-status').val(0);
    })

    $('.btn-save').click(function () {
        var e = $(this).parent().parent();
        if (IsInputEmpty(e)) {
            layer.msg('标题不能为空');
            return;
        }

        var title = $('#m-title').val()
        var isArticle = false;
        if ($('#m-select').val() == '1')
            isArticle = true;

        var status = $('#select-status').val();

        var imgs = $('#divEditor img');
        if (imgs.length < 1) {
            layer.msg('至少选择一张图片');
            return;
        }
        var imgNames = new Array();
        loading();
        $('#divEditor iframe').css('width', '100%');
        for (var i = 0; i < imgs.length; i++) {
            var ele = $('#divEditor img:eq(' + i + ')');
            var src = ele.attr('src');
            ele.attr("width", "100%");
            var imgName = src.substring(src.lastIndexOf('/') + 1)
            imgNames.push(imgName);
            ele.attr("src", "/img/article/" + imgName);
        }
        //$('#divEditor img').each(function () {
        //    var src = $(this).attr('src');
        //    imgNames.push(src.substring(src.lastIndexOf('/') + 1));
        //    $(this).attr("width", "100%");
        //})
        var content = $('#divEditor').html();
        content = encode(content);
        if (isAdd) {
            var data = {
                content: content,
                isArticle: isArticle,
                articleName: title,
                imgNames: imgNames,
                status: status
            }

            jQuery.axpost('../articleAjax/add', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('添加成功')
                changePage(1);
                $('#modal1').modal('hide')
            })

            //$.ajax({
            //    type: "post",
            //    data: data,
            //    url: "/article/add",
            //    cache: false,
            //    success: function (data) {

            //    },
            //    error: function (err) {
            //        alert(err);
            //    }
            //});
        } else {
            var id = $('#m-articleId').val();
            var data = {
                content: content,
                isArticle: isArticle,
                articleName: title,
                imgNames: imgNames,
                articleId: id,
                status: status
            }

            jQuery.axpost('../articleAjax/update', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('更新成功')
                changePage(1);
                $('#modal1').modal('hide')
            })

            //$.ajax({
            //    type: "post",
            //    data: data,
            //    url: "/article/update",
            //    cache: false,
            //    success: function (data) {

            //    },
            //    error: function (err) {
            //        alert(err);
            //    }
            //});
        }
        layer.close(loadingElement);
    })

    changePage(1);

    $('body').delegate('.btn-delete', 'click', function () {
        loading();
        var id = $(this).parent().find('.articleId').val();
        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                articleId: id
            }

            jQuery.axpost('../articleAjax/delete', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功');
                changePage(1)
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/article/Delete',
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
            ids.push($(this).parent().parent().find('.articleId').val());
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

            jQuery.axpost('../articleAjax/batchdel', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功')
                changePage(1)
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/article/BatchDel',
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

    $('body').delegate('.btn-edit', 'click', function () {
        loading();
        isAdd = false;
        $('#modal1').modal();
        var title = $(this).parent().parent().find('.td-name').html();
        var type = $(this).parent().parent().find('.td-type').html();
        var status = $(this).parent().parent().find('.td-status').html();
        if (status == '未发布')
            $('#select-status').val(0)
        else
            $('#select-status').val(1)
        $('#m-title').val(title)
        $('#m-select').val(0);
        if (type == '文章')
            $('#m-select').val(1);
        var id = $(this).parent().find('.articleId').val();
        $('#m-articleId').val(id);
        var data = {
            articleId: id
        }
        jQuery.axpost('../articleAjax/getcontentbyid', JSON.stringify(data), function (data) {
            var res = data.data;
            $('#divEditor').html(res);
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    url: '/article/GetContentById',
        //    cache: false,
        //    success: function (res) {

        //    },
        //    error: function (res) { alert(res) }
        //})
        layer.close(loadingElement);
    })

    $('.s-type span').click(function () {
        $('.s-type span').removeClass('label-info').addClass('label-default');
        type = $(this).find('input').val();
        $(this).removeClass('label-default').addClass('label-info');
        //layer.msg(type);

        changePage(1);
    })

    $('.s-status span').click(function () {
        $('.s-status span').removeClass('label-info').addClass('label-default');
        status = $(this).find('input').val();
        $(this).removeClass('label-default').addClass('label-info');
        //layer.msg(status);

        changePage(1);
    })

    $('body').delegate('.btn-up', 'click', function () {
        loading();
        var articleId = $(this).parent().find('.articleId').val();
        var data = {
            articleId: articleId
        }

        jQuery.axpost('../articleAjax/upArticle', JSON.stringify(data), function (data) {
            var res = data.data;
            layer.msg('已发布')
            changePage(1);
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    dataType: 'json',
        //    url: '/article/UpArticle',
        //    cache: false,
        //    success: function (res) {

        //    },
        //    error: function (res) { }
        //})
        //layer.close(loadingElement);
    })

    $('body').delegate('.btn-down', 'click', function () {
        loading();
        var articleId = $(this).parent().find('.articleId').val();
        var data = {
            articleId: articleId
        }

        jQuery.axpost('../articleAjax/downArticle', JSON.stringify(data), function (data) {
            var res = data.data;
            layer.msg('已撤下')
            changePage(1);
        })

        //$.ajax({
        //    type: 'post',
        //    data: data,
        //    dataType: 'json',
        //    url: '/article/downArticle',
        //    cache: false,
        //    success: function (res) {

        //    },
        //    error: function (res) { }
        //})
        layer.close(loadingElement);
    })

    changeWangEditor();

    var div = document.getElementById('divEditor');
    var editor = new wangEditor(div);
    editor.config.uploadImgUrl = '/article/upImg';
    editor.config.hideLinkImg = true;
    editor.create();

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
    var h = '<table id="main-table"><thead><tr><th width="2%"><input type="checkbox"></th><th width="15%">标题</th><th width="15%">文章/公告</th><th width="15%">大致内容</th><th width="30%">操作</th></tr></thead><tbody>';
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="标题">标题</div></th><th width="4%"><div class="td-div ellipsis td-" title="文章/公告">文章/公告</div></th><th width="4%"><div class="td-div ellipsis td-" title="大致内容">大致内容</div></th><th width="4%"><div class="td-div ellipsis td-" title="发布状态">发布状态</div></th><th width="4%"><div class="td-div ellipsis td-" title="最近操作时间">最近操作时间</div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>'
    for (var i = 0; i < json.length; i++) {
        var title = json[i].articleName;
        var isArticle = json[i].articleType
        var content = json[i].content;
        var target = "全体用户";
        var status;
        if (json[i].status)
            status = '已发布'
        else
            status = '未发布'
        var lastTime = json[i].lastTime;
        h += ' <tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-name" title="' + title + '">' + title + '</div></td><td><div class="td-div ellipsis td-type" title="' + isArticle + '">' + isArticle + '</div></td><td><div class="td-div ellipsis td-" title="' + content + '">' + content + '</div></td><td><div class="td-div ellipsis td-status" title="' + status + '">' + status + '</div></td><td><div class="td-div ellipsis td-status" title="' + lastTime + '">' + lastTime + '</div></td><td><input class="articleId" type="hidden" value="' + json[i].id + '"><button class="btn-edit btn btn-primary">编辑</button><button class="btn btn-primary btn-delete">删除</button>'
        if (json[i].status)
            h += '<button class="btn btn-danger btn-down">撤下</button>';
        else
            h += '<button class="btn btn-success btn-up">发布</button>';
        h += '</td></tr>'
    }
    //<td><div class="td-div ellipsis td-" title="' + target + '">' + target + '</div></td>
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
        articleType: type,
        status: status
    }

    jQuery.axpost('../articleAjax/get', JSON.stringify(data), function (data) {
        var res = data.data;
        getTable(res);
        layer.close(loadingElement);
    })

    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    url: '/article/get',
    //    cache: false,
    //    success: function (res) {

    //    },
    //    error: function (res) {
    //        layer.close(loadingElement);
    //    }
    //})

}





//var div = document.getElementById('divEditor');
//var editor = new wangEditor(div);
//editor.config.uploadImgUrl = '/article/upImg';
//editor.config.hideLinkImg = true;
//editor.create();

//取消编辑器上的按钮
function changeWangEditor() {
    $('.menu-group:last').remove();
    $('.menu-group:first').remove();
    $('.menu-group:first .menu-item:eq(4)').remove();
    $('.menu-group:eq(1) .menu-item:eq(0)').remove();
    $('.menu-group:eq(1) .menu-item:eq(0)').remove();
    $('.menu-group:eq(1) .menu-item:eq(2)').remove();
    $('.menu-group:eq(1) .menu-item:eq(2)').remove();
    $('.menu-group:eq(2) .menu-item:eq(1)').remove();
    $('.menu-group:eq(2) .menu-item:eq(2)').remove();
    //$('.menu-group:eq(3) .menu-item:eq(1)').remove();
    $('.menu-group:eq(3) .menu-item:eq(2)').remove();
    $('.menu-group:eq(3) .menu-item:eq(2)').remove();
}

function encode(str) {
    str = str.trim();
    str = str.replace(/</g, "*lt;");
    str = str.replace(/>/g, "*gt;");
    str = str.replace(/&/g, "*amp");
    //str = str.replace(/"/g, "*dquo");
    //str = str.replace(/ /g, "*nbsp;");
    //str = str.replace(/　/g, "*emsp;");
    //str = str.replace(/\//g, "*quot");
    return str;
}