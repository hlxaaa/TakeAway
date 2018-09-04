//最后执行的js文件

$(document).ready(function () {
    addOnlyDecimal();
    addOnlyInt();
    layer.close(leTurnPage);
})

function addOnlyDecimal() {
    $('.onlyDecimal').attr('onkeyup', "this.value=this.value.replace(/[^\\d.]/g,'')");
    $('.onlyDecimal').attr('onafterpaste', "this.value=this.value.replace(/[^\\d.]/g,'')");
}

function addOnlyInt() {
    $('.onlyInt').attr("onkeyup", "this.value=this.value.replace(/\\D/g,'')");
    $('.onlyInt').attr("onafterpaste", "this.value=this.value.replace(/\\D/g,'')");
}