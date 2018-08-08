$(document).ready(function () {

    /* Save inspect data to database. */
    $("#sendToChecker").click(function () {
        document.getElementById("detailsForm").setAttribute("action", "/InspectDocEdit/SaveBeforeSend");
        document.getElementById("detailsForm").submit();
    });

    /* Check the min and max value, when user inputs. */
    $(".inputValue").change(function () {

        var id = $(this).attr("id");
        var value = $(this).val();
        var areaID = document.getElementById("InspectDocDetails[" + id + "].AreaID").value;
        var classID = document.getElementById("InspectDocDetails[" + id + "].ClassID").value;
        var itemID = document.getElementById("InspectDocDetails[" + id + "].ItemID").value;
        var fieldID = document.getElementById("InspectDocDetails[" + id + "].FieldID").value;

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
