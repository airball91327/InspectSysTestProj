$(document).ready(function () {

    /* Save inspect data to database. */
    $("#sendToChecker").click(function () {
        document.getElementById("detailsForm").setAttribute("action", "/InspectDocDetails/SaveBeforeSend");
        document.getElementById("detailsForm").submit();
    });

    /* Check the min and max value, when user inputs. */
    $(".inputValue").change(function () {

        var id = $(this).attr("id");
        var value = $(this).val();
        var areaID = document.getElementById("InspectDocDetailsTemporary[" + id + "].AreaID").value;
        var classID = document.getElementById("InspectDocDetailsTemporary[" + id + "].ClassID").value;
        var itemID = document.getElementById("InspectDocDetailsTemporary[" + id + "].ItemID").value;
        var fieldID = document.getElementById("InspectDocDetailsTemporary[" + id + "].FieldID").value;

        $.ajax({
            type: "GET",
            url: "/InspectDocDetails/CheckValue",
            data: { AreaID: areaID, ClassID: classID, ItemID: itemID, FieldID: fieldID, Value: value },
            success: function (result) {
                console.log(result); //For debug
                $("." + id).html(result);
            },
            error: function (msg) {
                alert("讀取錯誤");
            }
        });

    });
});
