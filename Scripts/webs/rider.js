var isAdd = false;
var type = 3;
var areaType = 0;
$(document).ready(function () {
    $('#li-rider').addClass('li-active');
    changePage(1);
    $('.btn-add').click(function () {
        location.href = '/rider/detail';

    })

    $('body').delegate('.btn-edit', 'click', function () {
        var id = $(this).parent().find('.riderId').val();
        location.href = '/rider/detail?riderId=' + id;
    })

    $('.s-type span').click(function () {
        $('.s-type span').removeClass('label-info').addClass('label-default');
        type = $(this).find('input').val();
        $(this).removeClass('label-default').addClass('label-info');

        changePage(1);
    })

    //不用了
    $('.btn-save').click(function () {
        var account = $('.m-account input').val()
        var pwd = $('.m-pwd input').val()
        var name = $('.m-name input').val();
        var sex = $('.m-sex select').val();
        var type = $('.m-type select').val()
        var status = $('.m-status select').val()
        var areaId = $('.m-area select').val()
        var no = $('.m-no input').val();
        var stars = $('.m-stars input').val();
        var starCount = $('.m-starCount input').val();
        var sendCount = $('.m-sendCount input').val();



        if (isAdd) {
            var data = {
                riderAccount: account,
                riderPwd: pwd,
                riderType: type,
                riderStatus: status,
                riderAreaId: areaId,
                name: name,
                sexType: sex,
                riderNo: no,
                stars: stars,
                starCount: starCount,
                sendCount: sendCount
            }

            jQuery.axpost('../riderajax/add', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('添加成功')
                changePage(1);
                $('#modal1').modal('hide')
            })
            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/rider/Add',
            //    cache: false,
            //    success: function (res) {
            //        if (res == 'have') {
            //            layer.msg('该账号已存在')
            //        } else {

            //        }
            //    },
            //    error: function (res) { }
            //})
        } else {
            var id = $('#m-riderId').val();

            var data = {
                riderAccount: account,
                riderPwd: pwd,
                riderType: type,
                riderStatus: status,
                riderAreaId: areaId,
                riderId: id,
                name: name,
                sexType: sex,
                riderNo: no,
                stars: stars,
                starCount: starCount,
                sendCount: sendCount
            }

            jQuery.axpost('../riderajax/update', JSON.stringify(data), function (data) {
                var res = data.data;
                //if (res == 'have') {
                //    layer.msg('已存在该账号')
                //} else {
                layer.msg('更新成功')
                changePage(1);
                $('#modal1').modal('hide')
                //}
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/rider/update',
            //    cache: false,
            //    success: function (res) {

            //    },
            //    error: function (res) { }
            //})
        }
    })

    $('body').delegate('.rider-stock', 'click', function () {
        var id = $(this).parent().find('.riderId').val();
        location.href = '/riderstock?riderId=' + id;
    })

    $('body').delegate('.btn-del', 'click', function () {
        loading();
        var e = $(this);
        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var riderId = e.parent().find('.riderId').val();
            var data = {
                riderId: riderId
            }

            jQuery.axpost('../riderajax/delete', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功');
                changePage(1);
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/rider/delete',
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
        var ids = new Array();
        $('tbody div[class="icheckbox_minimal checked"]').each(function () {
            ids.push($(this).parent().parent().find('.riderId').val());
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

            jQuery.axpost('../riderajax/batchdel', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功')
                changePage(1)
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/rider/BatchDel',
            //    cache: false,
            //    success: function (res) {

            //    },
            //    error: function (res) {
            //        layer.msg('error')
            //    }
            //})
            layer.closeAll('dialog');
        },
            function () {
                layer.closeAll('dialog');
            })
        layer.close(loadingElement);
    })

    $('.s-area select').change(function () {
        var areaId = $(this).val();
        areaType = areaId;
        changePage(1);
    })
})


function getTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        layer.msg('没有结果')
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="4%"><input type="checkbox"></th><th width="4%">账号</th><th width="4%">密码</th><th width="4%">姓名</th><th width="4%">性别</th><th width="4%">编号</th><th width="4%">车类</th><th width="4%">状态</th><th width="4%">所属区域</th><th width="4%">星级</th><th width="8%">评价次数</th><th width="8%">配送次数</th><th width="30%">操作</th></tr></thead><tbody>';

    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="账号">账号</div></th><th width="4%"><div class="td-div ellipsis td-" title="密码">密码</div></th><th width="4%"><div class="td-div ellipsis td-" title="名称">名称</div></th><th width="4%"><div class="td-div ellipsis td-" title="类型">类型</div></th><th width="4%"><div class="td-div ellipsis td-" title="状态">状态</div></th><th width="4%"><div class="td-div ellipsis td-" title="区域">区域</div></th><th width="4%"><div class="td-div ellipsis td-" title="星级">星级</div></th><th width="4%"><div class="td-div ellipsis td-" title="手机号码">手机号码</div></th><th width="4%"><div class="td-div ellipsis td-" title="配送次数">配送次数</div></th><th width="4%"><div class="td-div ellipsis td-" title="经纬度">经纬度</div></th><th width="4%"><div class="td-div ellipsis td-" title="详细地址">详细地址</div></th><th width="15%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>'
    for (var i = 0; i < json.length; i++) {
        var temp = json[i];
        if (json[i].riderType == 1)
            riderType = '汽车'
        if (json[i].riderType == 0)
            riderType = '电瓶车'
        if (json[i].riderType == 2)
            riderType = '自提点'

        var sex;
        //if (json[i].sexType)
        //    sex = '男'
        //else
        //    sex='女'
        var account = temp.riderAccount;
        var pwd = temp.riderPwd;
        var type;
        if (temp.riderType == 1)
            type = '汽车'
        else if (temp.riderType == 0)
            type = '电瓶车'
        else
            type = '自提点'

        var status = '下线';
        if (temp.riderStatus)
            status = '在线';
        var areaName = temp.areaName == null ? "" : temp.areaName;
        var stars = temp.avgStars;
        var sendCount = temp.sendCount;
        var starCount = temp.phone;
        var latLng = '';
        if (temp.lat != null && temp.lng != null)
            latLng = temp.lat + ',' + temp.lng;
        var mapAddress = temp.mapAddress == null ? '' : temp.mapAddress
        var name = temp.name;
        //h += '<tr><td><input type="checkbox"></td>'
        //    + '<td><div class="td-div ellipsis td-account" title="' + json[i].riderAccount + '">' + json[i].riderAccount + '</div></td>'
        //    + '<td><div class="td-div ellipsis td-pwd" title="' + json[i].riderPwd + '">' + json[i].riderPwd + '</div></td>'
        //    + '<td><div class="td-div ellipsis td-name" title="' + json[i].name + '">' + json[i].name + '</div></td>'
        //    + '<td><div class="td-div ellipsis td-sex" title="' + sex + '">' + sex + '</div></td>'
        //    + '<td><div class="td-div ellipsis td-no" title="' + json[i].riderNo + '">' + json[i].riderNo + '</div></td>'
        //    + '<td><div class="td-div ellipsis td-type" title="' + riderType + '">' + riderType + '</div></td>'
        //    + '<td><div class="td-div ellipsis td-status" title="' + status + '">' + status + '</div></td>'
        //    + '<td><div class="td-div ellipsis td-" title="' + json[i].areaName + '">' + json[i].areaName + '<input class="td-areaId" type="hidden" value="' + json[i].riderAreaId + '"></div></td>'
        //    + '<td><div class="td-div ellipsis td-stars" title="' + json[i].avgStars + '">' + json[i].avgStars + '</div></td>'
        //    + '<td><div class="td-div ellipsis td-starCount" title="' + json[i].starCount + '">' + json[i].starCount + '</div></td>'
        //    + '<td><div class="td-div ellipsis td-sendCount" title="' + json[i].sendCount + '">' + json[i].sendCount + '</div></td>'
        //+ '<td><input class="riderId" type="hidden" value="' + json[i].id + '"><button class="btn-edit btn btn-primary">编辑</button><button class="rider-stock btn btn-primary">库存</button><button class="btn-del btn btn-primary">删除</button></td></tr>'


        h += ' <tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-account" title="' + account + '">' + account + '</div></td><td><div class="td-div ellipsis td-pwd" title="' + pwd + '">' + pwd + '</div></td><td><div class="td-div ellipsis td-name" title="' + name + '">' + name + '</div></td><td><div class="td-div ellipsis td-type" title="' + type + '">' + type + '</div></td><td><div class="td-div ellipsis td-status" title="' + status + '">' + status + '</div></td><td><div class="td-div ellipsis td-" title="' + areaName + '">' + areaName + '<input class="td-areaId" type="hidden" value="' + json[i].riderAreaId + '"></div></td><td><div class="td-div ellipsis td-stars" title="' + stars + '">' + stars + '</div></td><td><div class="td-div ellipsis td-starCount" title="' + starCount + '">' + starCount + '</div></td><td><div class="td-div ellipsis td-sendCount" title="' + sendCount + '">' + sendCount + '</div></td><td><div class="td-div ellipsis td-" title="' + latLng + '">' + latLng + '</div></td><td><div class="td-div ellipsis td-" title="' + mapAddress + '">' + mapAddress + '</div></td><td><input class="riderId" type="hidden" value="' + json[i].id + '"><button class="btn-edit btn btn-primary">编辑</button><button class="rider-stock btn btn-primary">库存</button><button class="btn-del btn btn-primary">删除</button></td></tr>'
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
        areaId: areaType,
        riderType: type
    }

    jQuery.axpost('../riderajax/get', JSON.stringify(data), function (data) {
        var res = data.data;
        getTable(res);
    })

    //$.ajax({
    //    type: 'post',
    //    data: data,
    //    url: '/rider/get',
    //    cache: false,
    //    success: function (res) {

    //        layer.close(loadingElement);
    //    },
    //    error: function (res) {
    //        layer.close(loadingElement);
    //    }
    //})

}