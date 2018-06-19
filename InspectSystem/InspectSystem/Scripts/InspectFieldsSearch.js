$(document).ready(function () {

    $(".fieldEditBtn").click(function () {

        var rowNumber = $(this).attr("value");
        var ACID = $("#hideACIDNo" + rowNumber).attr("value");
        var itemID = $("#hideItemIDNo" + rowNumber).attr("value");
        var fieldID = $("#hideFieldIDNo" + rowNumber).attr("value");

        document.getElementById("fieldModal01").style.display = "block";
        alert("number:" + rowNumber + "ACID:" + ACID + "itemID:" + itemID + "fieldID:" + fieldID);//for test

        $.ajax({
            type: "GET",
            url: "/InspectFields/Edit",
            data: { acid: ACID, itemid: itemID, fieldid: fieldID },
            success: function (result) {
                console.log(result); //For test
                $("#fieldModalContent01").html(result);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });
});