$(document).ready(function () {

    $("#saveToDBbtn").click(function () {
        document.getElementById("detailsForm").setAttribute("action", "/InspectDocDetails/SaveToDataBase");
        document.getElementById("detailsForm").submit();
    });

});
