$(document).ready(function () {

    /* InspectFields/Search */
    $(".fieldEditBtn").click(function () {

        var rowNumber = $(this).attr("value");
        var ACID = $("#hideACIDNo" + rowNumber).attr("value");
        var itemID = $("#hideItemIDNo" + rowNumber).attr("value");
        var fieldID = $("#hideFieldIDNo" + rowNumber).attr("value");
        var zoneNo = parseInt(ACID) * 100 + parseInt(itemID);
        var editModalNo = "editModalNo" + zoneNo;
        var editModalContentNo = "editModalContentNo" + zoneNo;

        document.getElementById(editModalNo).style.display = "block";

        $.ajax({
            type: "GET",
            url: "/InspectFields/Edit",
            data: { acid: ACID, itemid: itemID, fieldid: fieldID },
            success: function (result) {
                console.log(result); //For debug
                $(".createModalContent").html("<p></p>");
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

        document.getElementById(createModalNo).style.display = "block";

        $.ajax({
            type: "GET",
            url: "/InspectFields/Create",
            data: { acid: ACID, itemid: itemID },
            success: function (result) {
                console.log(result); //For debug
                $(".editModalContent").html("<p></p>");
                var test = $(".editModalContent").html();
                console.log("...............");
                console.log(test);
                $("#" + createModalContentNo).html(result);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });

});