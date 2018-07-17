$(document).ready(function () {

    $("#saveToDBbtn").click(function () {
        document.getElementById("detailsForm").setAttribute("action", "/InspectDocDetails/SaveToDataBase");
        document.getElementById("detailsForm").submit();
    });

    /* 還未實作，大小值的ajax驗證
    $(".inputValue").change(function () {
        var id = $(this).attr("id");
        var value = $(this).val();

        $.ajax({
            type: "GET",
            url: "/InspectDocDetails/CheckValue",
            data: { ACID: acid, DocID: docID },
        });
        $("." + id).html("測試" + value);
    });
    */
});
