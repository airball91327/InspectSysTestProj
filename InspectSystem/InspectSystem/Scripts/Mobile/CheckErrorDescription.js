$(document).ready(function () {

    $("input:radio").change(function () {
        var objName = this.name.toString();
        var cutName = objName.split(".", 1);
        var targetId = cutName + ".ErrorDescription"
        var value = this.nodeValue;
        document.getElementById(targetId).setAttribute("required", "required");
        alert(objName + ":" + cutName + ":" + targetId + ":" + value);
    });

});