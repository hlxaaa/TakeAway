var isAdd = false;
var pageType = 0;
var time1 = '2009-10-01 03:05:00';
var time2 = '2099-10-01 03:05:00';
$(document).ready(function () {
    $('.m-birth input').datetimepicker({
        format: 'yyyy-mm-dd',
        language: 'ch',
        autoclose: true,
        minView: (0, 'month'),
    });

    GetDateTool('.time1');
    GetDateTool('.time2');

    $('#li-user').addClass('li-active');
    changePage(1);

    $('.m-pwd input').change(function () {
        $(this).addClass('changed');
    })

    $('body').delegate('.btn-edit', 'click', function () {
        isAdd = false;
        $('#modal1').modal();
        var tr = $(this).parent().parent();
        var name = tr.find('.td-name').html();
        var balance = tr.find('.td-balance').html();
        var userPhone = tr.find('.td-userPhone').html();
        var pwd = tr.find('.td-pwd').html();
        var birth = tr.find('.td-birth').html();
        var coupon = tr.find('.td-coupon').html();
        var img = tr.find('.headImg').val();
        //img = img == "" ?"/img/icon/appicon.png"
        var userId = $(this).parent().parent().find('.userId').val();
        $('.m-pwd input').removeClass('changed');

        $('.m-name input').val(name);
        $('.m-balance input').val(balance);
        $('.m-phone input').val(userPhone);
        $('.m-pwd input').val(pwd);
        $('#m-userId').val(userId);
        $('.m-birth input').val(birth);
        $('.m-coupon input').val(coupon);
        $('.modal-body img').attr('src', img)
    })

    $('.btn-add').click(function () {
        isAdd = true;
        $('#modal1').modal();
        $('.modal-body input').val('');
        $('#coupon').val(0)
        $('#balance').val(0)
        $('.m-pwd input').removeClass('changed');
    })

    $('body').delegate('.btn-save', 'click', function () {

        var e = $(this).parent();
        if (IsInputEmpty(e)) {
            layer.msg('输入不能为空')
            return;
        }
        if (!IsPositive(e)) {
            layer.msg('数字格式不正确')
            return;
        }

        var name = $('.m-name input').val();
        var balance = $('.m-balance input').val();

        var phone = $('.m-phone input').val();
        if ($('.m-pwd input').hasClass('changed'))
            var pwd = $('.m-pwd input').val();
        var birth = $('.m-birth input').val();
        var coupon = $('.m-coupon input').val();

        loading();
        if (isAdd) {//添加
            var data = {
                userName: name,
                userBalance: balance,
                userPwd: pwd,
                userPhone: phone,
                birthday: birth,
                coupon: coupon,
            }

            jQuery.axpost('../userajax/add', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('添加成功')
                $('#modal1').modal('hide')
                changePage(1);
            })


        } else {//更新

            var userId = $('#m-userId').val();
            //alert(userId);
            var data = {
                userName: name,
                userBalance: balance,
                userPwd: pwd,
                userPhone: phone,
                userId: userId,
                birthday: birth,
                coupon: coupon,
            }

            jQuery.axpost('../userajax/update', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg("更新成功");
                $('#modal1').modal('hide')
                changePage(1);
            })
        }
    })

    $('body').delegate('.btn-delete', 'click', function () {
        loading();
        var userId = $(this).parent().find('.userId').val();
        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                userId: userId
            }

            jQuery.axpost('../userajax/delete', JSON.stringify(data), function (data) {
                var res = data.data;
                layer.msg('删除成功')
                changePage(1)
            })

            //$.ajax({
            //    type: 'post',
            //    data: data,
            //    url: '/user/Delete',
            //    cache: false,
            //    success: function (res) {
            //        layer.msg('删除成功')
            //        changePage(1)
            //    },
            //    error: function (res) {

            //    }
            //})
            layer.closeAll('dialog');
        }, function () {
            layer.closeAll('dialog');
        })
        //layer.close(loadingElement);
    })

    $('body').delegate('.btn-delSug', 'click', function () {
        loading();
        var sugId = $(this).parent().find('.sugId').val();
        layer.confirm('确认删除吗？', {
            btn: ['确定', '取消']
        }, function () {
            var data = {
                id: sugId
            }

            jQuery.axpost('../userajax/DelSuggestion', JSON.stringify(data), function (data) {
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

    $('.btn-batchDel').click(function () {
        if (pageType != 1) {
            var ids = new Array();

            $('tbody div[class="icheckbox_minimal checked"]').each(function () {
                if (pageType == 0)
                    ids.push($(this).parent().parent().find('.userId').val());
                else
                    ids.push($(this).parent().parent().find('.sugId').val());
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
                var url = '';
                if (pageType == 0)
                    url = '../userajax/BatchDel'
                else
                    url = '../userajax/BatchDelSug'

                jQuery.axpost(url, JSON.stringify(data), function (data) {
                    var res = data.data;
                    layer.msg('删除成功');
                    changePage(1)
                })

                //$.ajax({
                //    type: 'post',
                //    data: data,
                //    url: url,
                //    cache: false,
                //    success: function (res) {
                //        layer.msg('删除成功');
                //        changePage(1)
                //    },
                //    error: function (res) {

                //    }
                //})
                layer.closeAll('dialog');
            }, function () {
                layer.closeAll('dialog');
            })

            layer.close(loadingElement);
        }
    })

    $('.s-pageType span').click(function () {
        pageType = $(this).find('input').val();
        //layer.msg(pageType)
        if (pageType == 1)
            $('.btn-batchDel').css("display", "none")
        else
            $('.btn-batchDel').css("display", "inline-block")

        if (pageType == 0)
            $('#timeArea').css("display", "none");
        else
            $('#timeArea').css("display", "table");
        if ($(this).hasClass('label-default')) {
            $('.s-pageType .label-info').removeClass('label-info').addClass('label-default');
            $(this).removeClass('label-default').addClass('label-info');
            changePage(1);
        }
    })

    $('.time1').change(function () {
        timeAreaChange();
    })
    $('.time2').change(function () {
        timeAreaChange();
    })
})

function timeAreaChange() {
    time1 = $('.time1').val();
    time2 = $('.time2').val();
    var t1 = new Date($('.time1').val());
    var t2 = new Date($('.time2').val());
    if (t1 > t2) {
        layer.msg('起始时间要小于终止时间');
        return;
    }
    if (pageType != 0)
        changePage(1)
}

function changePage(page) {
    loading();
    var data = {
        search: $('#div-search input').val(),
        index: page,
        pageType: pageType,
        time1: time1,
        time2: time2
    }
    jQuery.axpost('../userajax/get', JSON.stringify(data), function (data) {
        var res = data.data;
        if (pageType == 0)
            getTable(res);
        else if (pageType == 1)
            getUserOpenInfoTable(res);
        else if (pageType == 2)
            getTableForSuggestion(res);
        layer.close(loadingElement);
    })

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
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="用户ID">用户ID</div></th><th width="4%"><div class="td-div ellipsis td-" title="用户名">用户名</div></th><th width="4%"><div class="td-div ellipsis td-" title="手机号">手机号</div></th><th width="4%"><div class="td-div ellipsis td-" title="社交账号">社交账号</div></th><th width="4%"><div class="td-div ellipsis td-" title="生日">生日</div></th><th width="4%"><div class="td-div ellipsis td-" title="余额">余额</div></th><th width="4%"><div class="td-div ellipsis td-" title="优惠券">优惠券</div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        var sex;
        if (json[i].sexType)
            sex = '男'
        else
            sex = '女'

        var userName = json[i].userName;
        var phone = json[i].phone;
        var password = json[i].password
        var socialAccount = '';
        //layer.msg(typeof (json[i].qq));
        //return;
        if (json[i].qq != '')
            socialAccount = 'QQ '
        if (json[i].wechat != '')
            socialAccount += '微信'
        var birthday = json[i].birthday
        var balance = json[i].userBalance;
        var coupon = json[i].coupon;
        var userId = json[i].userId;
        var img = json[i].headImg;
        h += ' <tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-id" title="' + userId + '">' + userId + '</div></td><td><div class="td-div ellipsis td-name" title="' + userName + '">' + userName + '</div></td><td><div class="td-div ellipsis td-userPhone" title="' + phone + '">' + phone + '</div></td><td><div class="td-div ellipsis td-sAccount" title="' + socialAccount + '">' + socialAccount + '</div></td><td><div class="td-div ellipsis td-birth" title="' + birthday + '">' + birthday + '</div></td><td><div class="td-div ellipsis td-balance" title="' + balance + '">' + balance + '</div></td><td><div class="td-div ellipsis td-coupon" title="' + coupon + '">' + coupon + '</div></td><td><input class="userId" type="hidden" value="' + userId + '"><input class="headImg" type="hidden" value="' + img + '"><button class="btn btn-primary btn-edit">编辑</button><button class="btn btn-primary btn-delete">删除</button></td></tr>'
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

function getTableForSuggestion(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        alert('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="4%"><div class="td-div ellipsis td-" title="手机号">手机号</div></th><th width="4%"><div class="td-div ellipsis td-" title="用户">用户</div></th><th width="4%"><div class="td-div ellipsis td-" title="内容">内容</div></th><th width="4%"><div class="td-div ellipsis td-" title="建议时间">建议时间</div></th><th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th></tr></thead><tbody>';
    for (var i = 0; i < json.length; i++) {
        var userName = json[i].userName;
        var content = json[i].content;
        var createTime = json[i].sTime;
        var phone = json[i].phone;
        h += ' <tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-" title="' + phone + '">' + phone + '</div></td><td><div class="td-div ellipsis td-" title="' + userName + '">' + userName + '</div></td><td><div class="td-div ellipsis td-" title="' + content + '">' + content + '</div></td><td><div class="td-div ellipsis td-" title="' + createTime + '">' + createTime + '</div></td><td><input type="hidden" class="sugId" value="' + json[i].id + '"/><button class="btn btn-primary">编辑</button><button class="btn-delSug btn btn-primary">删除</button></td></tr>'
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

function getUserOpenInfoTable(jsonStr) {
    var jsonAll = JSON.parse(jsonStr);
    var json = jsonAll.data;
    var pages = jsonAll.pages;
    var index = jsonAll.index;
    if (json == null) {
        alert('没有结果');
        return;
    }
    var h = '<table id="main-table"><thead><tr><th width="1%"><input type="checkbox"></th><th width="2%"><div class="td-div ellipsis td-" title="用户名">用户名</div></th><th width="4%"><div class="td-div ellipsis td-" title="地址">地址</div></th><th width="2%"><div class="td-div ellipsis td-" title="纬度">纬度</div></th><th width="2%"><div class="td-div ellipsis td-" title="经度">经度</div></th><th width="4%"><div class="td-div ellipsis td-" title="打开时间">打开时间</div></th><th width="4%"><div class="td-div ellipsis td-" title="关闭时间">关闭时间</div></th><th width="4%"><div class="td-div ellipsis td-" title="使用时长">使用时长</div></th></tr></thead><tbody>';
    //<th width="10%"><div class="td-div ellipsis td-" title="操作">操作</div></th>
    for (var i = 0; i < json.length; i++) {
        var username = json[i].userName;
        var lat = json[i].lat;
        var lng = json[i].lng;
        var opentime = json[i].openTime;
        var closetime = json[i].closeTime == null ? "" : json[i].closeTime;
        var usetime = getNYR(json[i].useTime);
        var address = json[i].address;
        h += '<tr><td><input type="checkbox"></td><td><div class="td-div ellipsis td-" title="' + username + '">' + username + '</div></td><td><div class="td-div ellipsis td-" title="' + address + '">' + address + '</div></td><td><div class="td-div ellipsis td-" title="' + lat + '">' + lat + '</div></td><td><div class="td-div ellipsis td-" title="' + lng + '">' + lng + '</div></td><td><div class="td-div ellipsis td-" title="' + opentime + '">' + opentime + '</div></td><td><div class="td-div ellipsis td-" title="' + closetime + '">' + closetime + '</div></td><td><div class="td-div ellipsis td-" title="' + usetime + '">' + usetime + '</div></td></tr>'
        //<td><input type="hidden" value=""/><button class="btn btn-primary">编辑</button><button class="btn btn-primary">删除</button></td>
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

function getNYR(seconds) {
    if (seconds == null)
        return '';
    if (seconds < 60)
        return seconds + "秒";
    else if (seconds <= 3600)
        return parseInt(seconds / 60) + "分" + seconds % 60 + "秒";
    else if (seconds <= 86400)
        return parseInt(seconds / 3600) + "小时" + getNYR(seconds % 3600);
    else
        return parseInt(seconds / 86400) + "天" + getNYR(seconds % 86400);
}