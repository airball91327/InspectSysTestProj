$(document).ready(function () {

    /* When a edit button click */
    $(".editBtn").click(function () {
        /* hide edit button */
        $(this).hide();
        /* When Editing, disable other edit buttons */
        $(".editBtn").attr("disabled", true);
        /* Change the selected row to be edit */
        $(".displayRow" + $(this).attr("id")).hide();
        $(".editRow" + + $(this).attr("id")).show();
    });
    
    $(".fieldBtn").click(function () {

        var number = $(this).attr("value");
        var ACID = $("#hideACIDNo" + number).attr("value");
        var itemID = $("#hideItemIDNo" + number).attr("value");

        /* When fieldBtn click, silde the field table */
        $(".fieldPanelNo" + number).slideToggle("slow");
        /*Get the search result of fields*/
        $.ajax({
            type: "GET",
            url: "/InspectFields/Search",
            data: { acid: ACID, itemid: itemID },
            success: function (result) {
                console.log(result); //For debug
                $("#fieldDivNo" + number).html(result);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });
    
});