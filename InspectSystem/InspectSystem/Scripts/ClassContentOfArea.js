$(document).ready(function () {

    $("#saveToDBbtn").click(function () {
        document.getElementById("detailsForm").setAttribute("action", "/InspectDocDetails/SaveToDataBase");
        document.getElementById("detailsForm").setAttribute("id", "5");
        alert("Test");
    });
})
