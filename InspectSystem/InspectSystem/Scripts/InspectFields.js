$(document).ready(function () {

    /* InspectFields/Edit and InspectFields/Create */
    ChangeAttrByRaioBtn();

    /* Change Attr to disabled when user selected "string" type. */
    $("input[name='DataType']").click(function () {
        ChangeAttrByRaioBtn();
    });
});

function ChangeAttrByRaioBtn() {

    var dataType = document.getElementsByName("DataType");
    for (i = 0; i < dataType.length; i++) {
        if (dataType[i].checked) {
            checkValue = dataType[i].value;
            console.log(checkValue); // For debug
        }
    }

    if (checkValue == "string" || checkValue == "boolean") {
        $("#UnitOfData").attr("disabled", true); /* If a element set disabled, it won't return values.*/
        $("#MinValue").attr("disabled", true);
        $("#MaxValue").attr("disabled", true);
    }
    else {
        $("#UnitOfData").attr("disabled", false);
        $("#MinValue").attr("disabled", false);
        $("#MaxValue").attr("disabled", false);
    }
}