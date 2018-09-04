$(document).ready(function () {
    $('#loginPanel').keydown(function (e) {
        if (e.keyCode == 13)
            $('.btn-login').click();
    })

    $('.btn-login').click(function () {
        var account = $('#account').val();
        if (account == '') {
            layer.msg('请输入账号');
            return;
        }
        var pwd = $('#password').val();
        if (pwd == '') {
            layer.msg('请输入密码');
            return;
        }
        var data = {
            pAccount: account,
            pPwd: pwd
        }
        $.ajax({
            type: 'post',
            data: data,
            dataType: 'json',
            url: '/platform/login',
            cache: false,
            success: function (res) {
                if (res.status)
                    location.href = '/foodtag'
                else
                    layer.msg(res.message);
            },
            error: function (res) {

            }
        })
    })
})