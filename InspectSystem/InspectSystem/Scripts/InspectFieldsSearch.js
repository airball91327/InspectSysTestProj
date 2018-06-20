$(document).ready(function () {

    $(".fieldEditBtn").click(function () {

        var rowNumber = $(this).attr("value");
        var ACID = $("#hideACIDNo" + rowNumber).attr("value");
        var itemID = $("#hideItemIDNo" + rowNumber).attr("value");
        var fieldID = $("#hideFieldIDNo" + rowNumber).attr("value");
        var zoneNo = parseInt(ACID) * 100 + parseInt(itemID);
        var editModalNo = "editModalNo" + zoneNo;
        var editModalContentNo = "editModalContentNo" + zoneNo;

        alert("number:" + rowNumber + "ACID:" + ACID + "itemID:" + itemID + "fieldID:" + fieldID +
            "zoneNo:" + zoneNo + "editModalNo:" + editModalNo +
            "editModalContentNo:" + editModalContentNo);//for debugger

        document.getElementById(editModalNo).style.display = "block";

        $.ajax({
            type: "GET",
            url: "/InspectFields/Edit",
            data: { acid: ACID, itemid: itemID, fieldid: fieldID },
            success: function (result) {
                console.log(result); //For debugger
                $("#" + editModalContentNo).html(result);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });

    $(".fieldCreateBtn").click(function () {

        var ACID = fieldCreateACID;
        var itemID = fieldCreateItemID
        var zoneNo = parseInt(ACID) * 100 + parseInt(itemID);
        var createModalNo = "createModalNo" + zoneNo;
        var createModalContentNo = "createModalContentNo" + zoneNo;

        alert("ACID:" + ACID + "itemID:" + itemID + "zoneNo:" + zoneNo +
            "createModalNo:" + createModalNo + "createModalContentNo:" + createModalContentNo);//for debugger

        document.getElementById(createModalNo).style.display = "block";

        $.ajax({
            type: "GET",
            url: "/InspectFields/Create",
            data: { acid: ACID, itemid: itemID },
            success: function (result) {
                console.log(result); //For debugger
                $("#" + createModalContentNo).html(result);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });

});